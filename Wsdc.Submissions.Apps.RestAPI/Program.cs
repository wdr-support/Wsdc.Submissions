using System.Threading.RateLimiting;
using FluentValidation;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.OpenApi.Models;
using Serilog;
using Swashbuckle.AspNetCore.Filters;
using Wsdc.Submissions.Apps.RestAPI.Middleware;
using Wsdc.Submissions.Apps.RestAPI.Swagger;
using Wsdc.Submissions.Repositories;
using Wsdc.Submissions.Repositories.Abstract;
using Wsdc.Submissions.Services;
using Wsdc.Submissions.Services.Abstract;
using Wsdc.Submissions.Services.Validators;

/////////////////////////////////////////////
// SERILOG CONFIGURATION
/////////////////////////////////////////////

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/wsdc-submissions-.txt", rollingInterval: RollingInterval.Day)
    .CreateBootstrapLogger();

Log.Information("Starting WSDC Submissions API");

try
{
    var builder = WebApplication.CreateBuilder(args);

    /////////////////////////////////////////////
    // CONFIGURE SERILOG
    /////////////////////////////////////////////

    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext());

    /////////////////////////////////////////////
    // ADD CONTROLLERS
    /////////////////////////////////////////////

    builder.Services.AddControllers()
        .AddXmlSerializerFormatters()
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
            options.JsonSerializerOptions.WriteIndented = true;
            // Use lenient enum converter that allows invalid values to be deserialized
            // FluentValidation will handle validation of enum values
            options.JsonSerializerOptions.Converters.Add(new Wsdc.Submissions.Core.Converters.LenientEnumConverterFactory());
        })
        .ConfigureApiBehaviorOptions(options =>
        {
            // Disable automatic 400 responses so we can handle validation ourselves
            options.SuppressModelStateInvalidFilter = true;
        });

    /////////////////////////////////////////////
    // VALIDATION
    /////////////////////////////////////////////

    builder.Services.AddValidatorsFromAssemblyContaining<EventSubmissionRequestValidator>();

    /////////////////////////////////////////////
    // HEALTH CHECKS
    /////////////////////////////////////////////

    builder.Services.AddHealthChecks();

    /////////////////////////////////////////////
    // RATE LIMITING
    /////////////////////////////////////////////

    var rateLimitConfig = builder.Configuration.GetSection("RateLimiting");
    var permitLimit = rateLimitConfig.GetValue<int>("PermitLimit", 100);
    var windowSeconds = rateLimitConfig.GetValue<int>("WindowSeconds", 60);
    var queueLimit = rateLimitConfig.GetValue<int>("QueueLimit", 10);

    builder.Services.AddRateLimiter(options =>
    {
        options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

        options.AddPolicy("sliding", httpContext =>
            RateLimitPartition.GetSlidingWindowLimiter(
                partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                factory: _ => new SlidingWindowRateLimiterOptions
                {
                    PermitLimit = permitLimit,
                    Window = TimeSpan.FromSeconds(windowSeconds),
                    SegmentsPerWindow = 4,
                    QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                    QueueLimit = queueLimit
                }));

        options.OnRejected = async (context, cancellationToken) =>
        {
            context.HttpContext.Response.ContentType = "application/json";
            await context.HttpContext.Response.WriteAsJsonAsync(new
            {
                success = false,
                errors = new[]
                {
                    new
                    {
                        code = "RATE_LIMIT_EXCEEDED",
                        message = "Too many requests. Please try again later.",
                        severity = "Warning"
                    }
                }
            }, cancellationToken);
        };
    });

    /////////////////////////////////////////////
    // SWAGGER/OPENAPI
    /////////////////////////////////////////////

    //builder.Services.AddControllers()
    //.AddXmlSerializerFormatters();

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "WSDC Submissions API",
            Version = "1.0",
            Description = "API for validating and submitting WSDC competition results"
        });

        // Add Bearer token security definition
        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "API Key",
            Description = "Enter your API key in the field below. The 'Bearer' prefix will be added automatically."
        });

        // Apply Bearer security requirement globally
        options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                Array.Empty<string>()
            }
        });

        // Provide proper schema for Judge.Score (string or integer)
        options.SchemaFilter<Wsdc.Submissions.Apps.RestAPI.Schema.JudgeScoreSchemaFilter>();

        // Enable example filters for request/response examples
        options.ExampleFilters();
    });

    // Register example providers from this assembly
    builder.Services.AddSwaggerExamplesFromAssemblyOf<EventSubmissionRequestExample>();

    /////////////////////////////////////////////
    // DEPENDENCY INJECTION - COMPOSITION ROOT
    /////////////////////////////////////////////

    // Register repositories
    builder.Services.AddSingleton<IAccountRepository, InMemoryAccountRepository>();

    // Register services as scoped
    builder.Services.AddScoped<ISubmissionResultBuilder, SubmissionResultBuilder>();
    builder.Services.AddScoped<IWsdcSubmissionsService, WsdcSubmissionsService>();
    builder.Services.AddScoped<IWsdcSubmissionsValidationService, WsdcSubmissionsValidationService>();

    /////////////////////////////////////////////
    // CORS POLICY
    /////////////////////////////////////////////

    builder.Services.AddCors(options =>
    {
        options.AddPolicy("ExcelAddInPolicy", policy =>
        {
            policy.WithOrigins("https://localhost:3000")
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials();
        });
    });

    /////////////////////////////////////////////
    // BUILD APPLICATION
    /////////////////////////////////////////////

    var app = builder.Build();

    /////////////////////////////////////////////
    // MIDDLEWARE PIPELINE
    /////////////////////////////////////////////

    // Global exception handler (must be first)
    app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

    // Serilog request logging
    app.UseSerilogRequestLogging();

    // Swagger
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        // Configure single endpoint explicitly
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "WSDC Submissions API");

        // Hide the "Schemas" section at the bottom of the page
        options.DefaultModelsExpandDepth(-1);

        // Hide the definition selector dropdown (only one version)
        options.InjectStylesheet("/swagger-ui/hide-selector.css");
    });

    // HTTPS redirection
    app.UseHttpsRedirection();

    // CORS policy (must be before authentication/authorization)
    app.UseCors("ExcelAddInPolicy");

    // Default files (serve index.html at root) and static files
    app.UseDefaultFiles();
    app.UseStaticFiles();

    // Rate limiting (before authentication to protect all endpoints)
    app.UseRateLimiter();

    // API Key authentication
    app.UseApiKeyAuthentication();

    // Authorization
    app.UseAuthorization();

    // Map health check endpoint (no auth required)
    app.MapHealthChecks("/health").AllowAnonymous();

    // Map controllers with rate limiting policy
    app.MapControllers().RequireRateLimiting("sliding");

    /////////////////////////////////////////////
    // LOG STARTUP INFORMATION
    /////////////////////////////////////////////

    var urls = builder.Configuration["ASPNETCORE_URLS"]
        ?? builder.Configuration.GetSection("Kestrel:Endpoints")
            .GetChildren()
            .Select(endpoint => endpoint["Url"])
            .FirstOrDefault()
        ?? app.Configuration["urls"]
        ?? "http://localhost:5000";

    var urlList = urls.Split(';', StringSplitOptions.RemoveEmptyEntries);

    Log.Information("===========================================");
    Log.Information("WSDC Submissions API started successfully");
    Log.Information("===========================================");
    Log.Information("Environment: {Environment}", app.Environment.EnvironmentName);
    Log.Information("Listening on:");
    foreach (var url in urlList)
    {
        Log.Information("  → {Url}", url.Trim());
    }
    Log.Information("Swagger UI: {SwaggerUrl}", $"{urlList[0].Trim()}/swagger");
    Log.Information("===========================================");

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}

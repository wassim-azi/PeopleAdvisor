using System.IO.Compression;
using System.Reflection;
using AspNetCoreRateLimit;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.OpenApi.Models;
using Upcome.Api.Common;
using Upcome.Api.Middlewares;

const string CORS_POLICY = "CorsPolicy";

var builder = WebApplication.CreateBuilder(args);

#region services.Add

// ---------------------------------------------------
// --------- Add services to the container -----------
// ---------------------------------------------------

// needed to load configuration from appsettings.json
builder.Services.AddOptions();

// store rate limit counters and ip rules
builder.Services.AddMemoryCache();

// load general configuration from appsettings.json
// inject counter, rules stores, resolvers and counter key builders
builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
builder.Services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
builder.Services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
builder.Services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();
builder.Services.AddInMemoryRateLimiting();

// configure MVC services for controllers
builder.Services.AddControllers();

// Register CurrentUserService
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

// adding health check services to container
builder.Services.AddHealthChecks();
builder.Services.AddHealthChecksUI()
                .AddInMemoryStorage();

// compression algorithms
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<BrotliCompressionProvider>();
    options.Providers.Add<GzipCompressionProvider>();
});
builder.Services.Configure<BrotliCompressionProviderOptions>(options => options.Level = CompressionLevel.Fastest);
builder.Services.Configure<GzipCompressionProviderOptions>(options => options.Level = CompressionLevel.Optimal);

builder.Services.AddControllers();

// https://learn.microsoft.com/en-us/aspnet/core/host-and-deploy/linux-nginx?view=aspnetcore-6.0#configure-nginx
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

// configure swagger
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Upcome",
        Version = "v1",
        Description = "Upcome to HirePeople"
    });

    // Set the comments path for the Swagger JSON and UI.
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
});

// Configure HTTP Strict Transport Security Protocol (HSTS)
// Enforce HTTPS in production
// => Require HTTPS for all requests
// => Redirect all HTTP requests to HTTPS
if (builder.Environment.IsDevelopment() == false)
{
    builder.Services.AddHsts(options =>
    {
        options.Preload = true;
        options.IncludeSubDomains = true;
        options.MaxAge = TimeSpan.FromDays(14);
    });

    builder.Services.AddHttpsRedirection(options => options.HttpsPort = 443);
}

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: CORS_POLICY, policy => policy.AllowAnyMethod().AllowAnyOrigin().AllowAnyHeader());
});

// reduces the number of requests a client or proxy makes to a web server
builder.Services.AddResponseCaching(options =>
{
    options.MaximumBodySize = 10 * 1024 * 1024; // The default value is 64 * 1024 * 1024 (64 MB)
    options.SizeLimit = 18 * 1024 * 1024;       // The default value is 100 * 1024 * 1024 (100 MB)
    options.UseCaseSensitivePaths = true;       // The default value is false
});

#endregion

#region app.Use

// -------------------------------------------------------
// -------- Configure the HTTP request pipeline ----------
// -------------------------------------------------------
var app = builder.Build();

app.UseIpRateLimiting();
app.UseClientRateLimiting();

app.UseForwardedHeaders();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error");

    // Enable HTTP Strict Transport Security Protocol (HSTS)
    // The default HSTS value is 30 days. see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
    
    app.UseHttpsRedirection();
}

app.UseStaticFiles();

app.UseRouting();

// allows restricted resources on the api to be requested
app.UseCors(CORS_POLICY);

app.UseAuthentication();

app.UseAuthorization();

// use only if unable to access server-based compression (https://docs.nginx.com/nginx/admin-guide/web-server/compression/)
app.UseResponseCompression();

// determine when responses are cacheable, stores responses, and serves responses from cache.
app.UseResponseCaching();

// global error handler
app.UseMiddleware<ErrorLoggingMiddleware>();

app.UseHealthChecks("/health");
app.UseEndpoints(endpoints =>
{
    endpoints.MapDefaultControllerRoute();

    endpoints.MapHealthChecks("/health");
    endpoints.MapHealthChecksUI();
});

// Enable middleware to serve generated Swagger as a JSON endpoint.
app.UseSwagger();

// Enable middleware to serve swagger-ui (HTML, JS, CSS) & specifying the Swagger JSON endpoint.
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    options.RoutePrefix = string.Empty;
    options.DisplayRequestDuration();
});

#endregion

app.Run();
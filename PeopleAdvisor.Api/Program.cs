using System.IO.Compression;
using System.Net;
using System.Reflection;
using AspNetCoreRateLimit;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.OpenApi.Models;
using PeopleAdvisor.Api.Common;
using PeopleAdvisor.Api.Middlewares;
using PeopleAdvisor.Infrastructure;
using PeopleAdvisor.Infrastructure.Data;

const string CORS_POLICY = "CorsPolicy";
var assemblyName = Assembly.GetExecutingAssembly().GetName().Name;

var builder = WebApplication.CreateBuilder(args);

#region services.Add

// ---------------------------------------------------
// --------- Add services to the container -----------
// ---------------------------------------------------

// load configuration from appsettings.json
builder.Services.AddOptions();

// store rate limit counters and ip rules
builder.Services.AddMemoryCache();

// inject counter, rules stores, resolvers and counter key builders
builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
builder.Services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
builder.Services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
builder.Services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();
builder.Services.AddInMemoryRateLimiting();

// https://learn.microsoft.com/en-us/aspnet/core/host-and-deploy/linux-nginx?view=aspnetcore-6.0#configure-nginx
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
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

    builder.Services.AddHttpsRedirection(options =>
    {
        options.RedirectStatusCode = (int)HttpStatusCode.PermanentRedirect;
        options.HttpsPort = 443;
    });
}

// Configure CORS, see more https://learn.microsoft.com/en-us/aspnet/core/security/cors?view=aspnetcore-6.0
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: CORS_POLICY, policy => policy.AllowAnyMethod().AllowAnyOrigin().AllowAnyHeader());
});

// compression algorithms
builder.Services.AddResponseCompression(options =>
{
    // disabled by default because of the security risk
    // https://learn.microsoft.com/en-us/aspnet/core/performance/response-compression?view=aspnetcore-6.0#risk
    options.EnableForHttps = true;

    options.Providers.Add<BrotliCompressionProvider>();
    options.Providers.Add<GzipCompressionProvider>();
    options.MimeTypes = new[] { "application/pdf", "application/json", "application/xml", "text/*" };

});
builder.Services.Configure<BrotliCompressionProviderOptions>(options => options.Level = CompressionLevel.Fastest);
builder.Services.Configure<GzipCompressionProviderOptions>(options => options.Level = CompressionLevel.Optimal);

// reduces the number of requests a client or proxy makes to a web server
builder.Services.AddResponseCaching(options =>
{
    options.MaximumBodySize = 10 * 1024 * 1024; // The default value is 64 * 1024 * 1024 (64 MB)
    options.SizeLimit = 18 * 1024 * 1024;       // The default value is 100 * 1024 * 1024 (100 MB)
    options.UseCaseSensitivePaths = true;       // The default value is false
});

// Add SECO HttpClient
builder.Services.AddHttpClient("seco", client =>
{
    client.BaseAddress = new Uri("https://entsendung.admin.ch/Lohnrechner/api");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

// adding health check services
builder.Services.AddHealthChecks();
builder.Services.AddHealthChecksUI()
                .AddInMemoryStorage();

// configure MVC services for controllers
builder.Services.AddControllers();

// Register CurrentUserService
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

// configure swagger
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "PeopleAdvisor",
        Version = "v1",
        Description = "PeopleAdvisor by HirePeople"
    });

    // Set the comments path for the Swagger JSON and UI.
    var xmlFile = $"{assemblyName}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
});

builder.Services.AddDbContext<ApplicationContext>(options =>
    options.UseNpgsql(builder.Configuration["ConnectionString"], optionsAction =>
        optionsAction.MigrationsAssembly(assemblyName)));

builder.Services.AddInfrastructure(builder.Configuration);

#endregion

#region app.Use

// -------------------------------------------------------
// -------- Configure the HTTP request pipeline ----------
// -------------------------------------------------------
var app = builder.Build();

// should be registered before any other components
app.UseIpRateLimiting();

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

    // HTTPS redirection
    app.UseHttpsRedirection();
}

// profiles resumes folders
var resumesFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "resumes");

// create resumes folder if it does not exist
Directory.CreateDirectory(resumesFolder);

// use static files for storing them physically on the hard drive
app.UseStaticFiles();
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(resumesFolder),
    RequestPath = "/resumes"
});

// it's very important that UsePathBase() is placed before UseRouting()
app.UseRouting();

// allows restricted resources on the api to be requested
app.UseCors(CORS_POLICY);

// After UseRouting(), so that route information is available for authentication decisions.
// Before UseEndpoints(), so that users are authenticated before accessing the endpoints.
app.UseAuthentication();

// In between the Routing and UseEndpoints middleware
// Authorisation will check whether the browser is authorised to access the information it is requesting.
app.UseAuthorization();

// use only if unable to access server-based compression https://docs.nginx.com/nginx/admin-guide/web-server/compression
app.UseResponseCompression();

// determine when responses are cacheable, stores responses, and serves responses from cache.
app.UseResponseCaching();

// global error handler
app.UseMiddleware<ErrorLoggingMiddleware>();

app.UseHealthChecks("/health");
app.UseHealthChecksUI(config => config.UIPath = "/health-ui");

app.UseEndpoints(endpoints => endpoints.MapDefaultControllerRoute());

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
using System.Data;
using System.Text;
using Asp.Versioning;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using AutoMapper;
using Findly.Api;
using Findly.Api.Filters;
using Findly.Api.Middleware;
using Findly.Api.OpenApi;
using Findly.Application;
using Findly.Contracts.Listing.Requests;
using Findly.Infrastructure;
using Findly.Infrastructure.Security;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

var jwtOptions = builder.Configuration.GetSection("Jwt").Get<JwtOptions>()
    ?? throw new InvalidOperationException("Missing 'Jwt' configuration section.");

// Autofac as DI container
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
builder.Host.ConfigureContainer<ContainerBuilder>(container =>
{
    container.RegisterModule<WebModule>();
    container.RegisterModule<ApplicationModule>();
    container.RegisterModule<InfrastructureModule>();

    container.RegisterInstance(jwtOptions);

    // IDbConnection — one per request (InstancePerLifetimeScope)
    container.Register(_ =>
    {
        var conn = new SqlConnection(builder.Configuration.GetConnectionString("DefaultConnection"));
        conn.Open();
        return (IDbConnection)conn;
    }).InstancePerLifetimeScope();

    // AutoMapper — register all profiles from Application + Infrastructure
    container.Register(_ => new MapperConfiguration(cfg =>
    {
        cfg.AddMaps(typeof(ApplicationModule).Assembly);
        cfg.AddMaps(typeof(InfrastructureModule).Assembly);
    }, NullLoggerFactory.Instance)).SingleInstance();

    container.Register(c => c.Resolve<MapperConfiguration>().CreateMapper())
        .As<IMapper>()
        .SingleInstance();
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddCors(options => options.AddPolicy("frontend", policy => policy
    .WithOrigins("http://localhost:5173")
    .AllowAnyHeader()
    .AllowAnyMethod()));
builder.Services.AddOpenApi(options =>
    options.AddDocumentTransformer<BearerSecuritySchemeTransformer>());
builder.Services.AddControllers(options =>
    options.Filters.Add<FluentValidationFilter>());
builder.Services.AddValidatorsFromAssemblyContaining<CreateListingRequest>();
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion                   = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions                   = true;
}).AddApiExplorer(options =>
{
    options.GroupNameFormat           = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.MapInboundClaims = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer           = true,
            ValidIssuer              = jwtOptions.Issuer,
            ValidateAudience         = true,
            ValidAudience            = jwtOptions.Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey         = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SigningKey)),
            RoleClaimType            = "role",
            NameClaimType            = "name",
            ClockSkew                = TimeSpan.FromMinutes(1)
        };
    });
builder.Services.AddAuthorization();

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
        options.SwaggerEndpoint("/openapi/v1.json", "Findly API v1"));
}

if (!app.Environment.IsDevelopment())
    app.UseHttpsRedirection();

app.UseCors("frontend");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();

// Exposes the implicit Program class to WebApplicationFactory in integration tests.
public partial class Program { }

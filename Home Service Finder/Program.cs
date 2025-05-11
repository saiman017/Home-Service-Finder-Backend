using System.Net;
using System.Text;
using System.Text.Json.Serialization;
using Home_Service_Finder;
using Home_Service_Finder.Authentication;
using Home_Service_Finder.Authentication.Contracts;
using Home_Service_Finder.Configurations;
using Home_Service_Finder.Data;
using Home_Service_Finder.Data.Contracts;
using Home_Service_Finder.Email;
using Home_Service_Finder.Email.Contracts;
using Home_Service_Finder.Locations;
using Home_Service_Finder.Locations.Contracts;
using Home_Service_Finder.Ratings;
using Home_Service_Finder.Ratings.Contracts;
using Home_Service_Finder.RequestServices;
using Home_Service_Finder.RequestServices.ServiceOffers;
using Home_Service_Finder.RequestServices.ServiceOffers.Contracts;
using Home_Service_Finder.RequestServices.ServiceRequest;
using Home_Service_Finder.RequestServices.ServiceRequest.Contracts;
using Home_Service_Finder.Roles;
using Home_Service_Finder.Roles.Contracts;
using Home_Service_Finder.ServiceCategories;
using Home_Service_Finder.ServiceCategories.Contracts;
using Home_Service_Finder.ServicesList;
using Home_Service_Finder.ServicesList.Contracts;
using Home_Service_Finder.Users;
using Home_Service_Finder.Users.Contracts;
using Home_Service_Finder.Users.ServiceProvider;
using Home_Service_Finder.Users.ServiceProvider.Contracts;
using Home_Service_Finder.Users.UserDetails;
using Home_Service_Finder.Users.UserDetails.Contracts;
using Home_Service_Finder.Users.Users;
using Home_Service_Finder.Users.Users.Contracts;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);


var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .Build();

builder.Configuration.AddConfiguration(configuration);

builder.Services.AddControllers();

builder.Services.AddSignalR(options =>
{
    options.KeepAliveInterval = TimeSpan.FromSeconds(10);
    options.ClientTimeoutInterval = TimeSpan.FromSeconds(60);
}).AddJsonProtocol(options =>
{
    options.PayloadSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});


builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserDetailRepository, UserDetailRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IServiceProviderRepository, ServiceProviderRepository>();
builder.Services.AddScoped<IServiceProviderService, ServiceProviderService>();
builder.Services.AddScoped<IServiceCategoryRepository, ServiceCategoryRepository>();
builder.Services.AddScoped<IServiceCategoryService, ServiceCategoryService>();
builder.Services.AddScoped<IEmailSender, EmailSender>();
builder.Services.AddScoped<IEmailOTPRepository, EmailOTPRepository>();
builder.Services.AddScoped<IEmailOTPService, EmailOTPService>();
builder.Services.AddScoped<IServiceListRepository, ServiceListRepository>();
builder.Services.AddScoped<IServiceListService, ServiceListService>();
builder.Services.AddScoped<ILocationRepository, LocationRepository>();
builder.Services.AddScoped<ILocationService, LocationService>();
builder.Services.AddScoped<IServiceRequestRepositpry, ServiceRequestRepository>();
builder.Services.AddScoped<IServiceRequestService, ServiceRequestService>();
builder.Services.AddScoped<IServiceRequestServiceListRepsoitory, ServiceRequestServiceListRepsoitory>();
builder.Services.AddHostedService<ServiceRequestExpirationService>();
builder.Services.AddHostedService<ServiceOfferExpirationService>();
builder.Services.AddScoped<IServiceOfferRepository, ServiceOfferRepository>();
builder.Services.AddScoped<IServiceOfferService, ServiceOfferService>();
builder.Services.AddScoped<IRatingRepository, RatingRepository>();
builder.Services.AddScoped<IRatingService, RatingService>();
builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));


// Configure JWT
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("ApiSettings:JwtOptions"));

// Add JWT Authentication
var jwtOptions = builder.Configuration.GetSection("ApiSettings:JwtOptions").Get<JwtOptions>();
var key = Encoding.ASCII.GetBytes(jwtOptions.Secret);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = true,
            ValidIssuer = jwtOptions.Issuer,
            ValidateAudience = true,
            ValidAudience = jwtOptions.Audience,
            ValidateLifetime = true,
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];

                var path = context.HttpContext.Request.Path;
                if (!string.IsNullOrEmpty(accessToken) &&
                    (path.StartsWithSegments("/requestHub") || path.StartsWithSegments("/serviceOfferHub")))
                {
                    context.Token = accessToken;
                }
                return Task.CompletedTask;
            }
        };
    }
});

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
});


// Custom Configurations
builder.Services.ConfigureServices(configuration);
builder.Services.AddOpenApi();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Home Service Finder.API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Description = "Authorization",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT"
    });
    c.AddSecurityRequirement(
        new OpenApiSecurityRequirement
        {
            { new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type=ReferenceType.SecurityScheme,
                        Id="Bearer"
                    }
                },
            new string[] { }
        }
    });
});
// CORS Configuration
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseHttpsRedirection();
}

app.UseStaticFiles(); 

app.UseRouting();
app.UseCors(builder => builder
     .WithOrigins("http://192.168.18.112:8081", "http://localhost:5173")
    .AllowAnyMethod()
    .AllowAnyHeader()
    .AllowCredentials());

app.UseAuthentication();
app.UseAuthorization();



app.MapControllers();
app.MapHub<ServiceRequestHub>("/requestHub");
app.MapHub<ServiceOfferHub>("/serviceOfferHub");



app.Run();
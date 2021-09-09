using System;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using FluentValidation;
using FluentValidation.AspNetCore;
using Hangfire;
using Hangfire.Dashboard;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Web.Domain;
using Web.Infrastructure;
using Web.Infrastructure.Api;
using Web.Infrastructure.Middleware;
using Web.Infrastructure.Model;
using Web.Messaging.Sender;
using Web.Services.User;
using Web.Services.Weather;

namespace Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<WeatherApiSetting>(Configuration.GetSection(nameof(WeatherApiSetting)));
            services.Configure<JwtSetting>(Configuration.GetSection(nameof(JwtSetting)));
            services.Configure<RabbitMqSetting>(Configuration.GetSection(nameof(RabbitMqSetting)));

            services.AddHttpClient(Configuration["WeatherApiSetting:ClientName"],
                config => { config.BaseAddress = new Uri(Configuration["WeatherApiSetting:Url"]); });

            services.AddControllers(config => { config.Filters.Add(typeof(ApiResultFilterAttribute)); })
                .AddFluentValidation(config =>
                {
                    config.RegisterValidatorsFromAssemblyContaining<Startup>();
                    config.RunDefaultMvcValidationAfterFluentValidationExecutes = false;
                });

            services.AddDbContext<RaimunDbContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("RaimunDb"));
            });

            services.AddScoped<IHttpClientServices, HttpClientServices>();
            services.AddScoped<IUserServices, UserServices>();
            services.AddScoped<IWeatherServices, WeatherServices>();
            services.AddScoped<IJwtHandler, JwtHandler>();
            services.AddScoped<IWeatherSender, WeatherSender>();

            services.AddValidatorsFromAssemblies(new List<Assembly>
            {
                Assembly.GetExecutingAssembly(),
                typeof(JwtHandler).Assembly
            });


            #region Swagger

            services.AddSwaggerGen(swagger =>
            {
                //This is to generate the Default UI of Swagger Documentation  
                swagger.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Raimun Test Task Api Documentation",
                });
                // To Enable authorization using Swagger (JWT)  
                swagger.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description =
                        "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\"",
                });
                swagger.AddSecurityRequirement(new OpenApiSecurityRequirement
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
                        new string[] { }
                    }
                });
            });

            #endregion


            #region Hangfire

            // Add Hangfire services.
            services.AddHangfire(configuration => configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage(Configuration.GetConnectionString("RaimunDb"), new SqlServerStorageOptions()
                {
                    CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                    SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                    QueuePollInterval = TimeSpan.Zero,
                    UseRecommendedIsolationLevel = true,
                    DisableGlobalLocks = true
                }));

            // Add the processing server as IHostedService
            services.AddHangfireServer();

            #endregion


            #region Authentication

            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;


                    options.Events = new JwtBearerEvents
                    {
                        //check security stamp in Token with security stamp in DB
                        OnTokenValidated = async context =>
                        {
                            var ctx = (RaimunDbContext)
                                context.HttpContext.RequestServices.GetRequiredService(typeof(RaimunDbContext));
                            var claims = context.Principal?.Identity as ClaimsIdentity;
                            var securityStamp = claims?.FindFirst("SecurityStamp")?.Value;
                            var userId = claims?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                            var user = await ctx.Users.FindAsync(Convert.ToInt32(userId));
                            if (user == null)
                                context.Fail("user not found");
                            if (user?.StampCode != Guid.Parse(securityStamp ?? string.Empty))
                                context.Fail("user Security Stamp Not Valid");
                        }
                    };


                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ClockSkew = TimeSpan.Zero,

                        RequireExpirationTime = true,
                        ValidateLifetime = true,

                        ValidateIssuer = true,
                        ValidIssuer = Configuration["JwtSetting:Issuer"],

                        ValidateAudience = true,
                        ValidAudience = Configuration["JwtSetting:Audience"],

                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey =
                            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JwtSetting:SecurityKey"])),
                    };
                });

            #endregion
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseAuthMiddleware();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Web v1"));
            }

            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                Authorization = new[] { new HangfireDashboardAuthorization() },
                IsReadOnlyFunc = (DashboardContext context) => true
            });

            app.UseHttpsRedirection();

            app.UseRouting();


            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}
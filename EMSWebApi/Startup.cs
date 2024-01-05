using EMS.Business.DataService;
using EMS.Business.Interfaces;
using EMS.Data;
using EMS.Data.Interfaces;
using EMSWebApi.Middlewares;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EMSWebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }



        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.AddConsole();
            });
            // services.AddScoped<TokenExpirationCheckFilter>();

            var config = Configuration.GetSection("JwtSettings");
            var secretKey = config["JwtSecretKey"];
            var key = Encoding.ASCII.GetBytes(secretKey!);
            var issue = config["JwtIssuer"];
            var audience = config["JwtAudience"];


            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = issue,
                    ValidateAudience = true,
                    ValidAudience = audience,
                    ValidateLifetime = true
                };

                //congifuring jwt events
                options.Events = new JwtBearerEvents
                {

                    OnTokenValidated = context =>
                    {
                        var usernameClaim = context.Principal!.FindFirst(ClaimTypes.Name);
                        var userTypeClaim = context.Principal.FindFirst(ClaimTypes.Role);
                        var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Startup>>();

                        if (context.SecurityToken is JwtSecurityToken jwtSecurityToken) //type checking and assingning 
                        {
                            if (userTypeClaim?.Value == "Administrator" || userTypeClaim?.Value == "Organizer")
                            {
                                logger.LogInformation($"User '{usernameClaim?.Value}' with role '{userTypeClaim?.Value}' authenticated successfully");

                                // checking for expired token
                                if (jwtSecurityToken.ValidTo < DateTime.UtcNow)
                                {
                                    logger.LogWarning($"User '{usernameClaim?.Value}' with role '{userTypeClaim?.Value}' authenticated with an expired token");
                                }
                            }
                            else
                            {
                                logger.LogWarning($"User '{usernameClaim?.Value}' attempted to log in with a restricted role");
                            }
                        }
                        else
                        {
                            logger.LogError("Invalid token type. Expected JWT token.");
                            context.Fail("Token is expired. Please provide a valid JWT token.");
                        }
                        return Task.CompletedTask;
                    },
                    OnChallenge = context =>
                    {
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        context.HandleResponse();
                        context.Response.Headers.Add("WWW-Authenticate", "Bearer Error");
                        var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Startup>>();
                        logger.LogError("Authentication challenge occurred.");
                        var responseMessage = new
                        {
                            Message = "Unauthorized: You need to provide a valid JWT token"
                        };
                        logger.LogError("Response Message: {Message}", responseMessage);
                        return Task.CompletedTask;
                    },
                    OnAuthenticationFailed = context =>
                    {
                        var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Startup>>();
                        logger.LogWarning(context.Exception.Message, " OnAuthenticationFailed");
                        context.Response.Headers.Add("message", "Authentication Failed");
                        return Task.CompletedTask;
                    },
                    OnForbidden = context =>
                    {
                        var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Startup>>();
                        logger.LogWarning($"A User attempted to access a forbidden resource");

                        // Forbid access and return a 403 
                        context.Response.StatusCode = StatusCodes.Status403Forbidden;
                        context.Response.ContentType = "application/json";

                        var responseMessage = new
                        {
                            Message = "Forbidden: You do not have permission to access this resource."
                        };

                        return context.Response.WriteAsJsonAsync(responseMessage);
                    }
                };
            });

            services.AddDbContext<EMSDbContext>(options =>
            {
                options.UseSqlServer(
                    Configuration.GetConnectionString("EMS-DataBase"),
                    sqlServerOptionsAction: sqlOptions =>
                    {
                        sqlOptions.MigrationsAssembly(typeof(EMSDbContext).Assembly.FullName);
                    }
                );
            });

            services.AddAutoMapper(typeof(BusinessMappingProfile));
            services.AddControllers();
            services.AddTransient<ConsoleLoggerMiddleware>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IEventsService, EventService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ITicketService, TicketService>();
            services.AddScoped<IReviewService, ReviewService>();
            services.AddScoped<IVendorService, VendorService>();


            //Configure Swagger
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "EMS-Web-Api", Version = "v1" });

                //security definition for JWT

                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = " JWT Authorization header using the Bearer scheme",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey
                });

                //requirement for JWT Authorization
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
                        new string[]{}
                    }
                });
            });

            services.AddScoped<TokenExpirationCheckFilter>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();

            //enabling CORS
            app.UseCors(builder =>
            {
                builder.WithOrigins("http://localhost:3000")
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials();
            });

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API");
                c.RoutePrefix = string.Empty;
            });
            app.UseLoggingMiddleware();
            app.UseMiddleware<ConsoleLoggerMiddleware>();


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });


        }
    }
}

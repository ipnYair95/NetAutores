using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using WebApp.Filters;
using WebApp.Middlewares;
using WebApp.Services;
using WebApp.Utils;

[assembly: ApiConventionType(typeof(DefaultApiConventions))]
namespace WebApp
{
    public class StartUp
    {



        public StartUp(IConfiguration configuration)
        {
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            var serverVersion = new MySqlServerVersion(new Version(8, 0, 21));

            services
                 .AddControllers(opciones =>
                 {
                     opciones.Filters.Add(typeof(FiltroException));
                     opciones.Conventions.Add(new SwaggerAgrupaPorVersion());
                 })
                .AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles)
                .AddNewtonsoftJson();

            services.AddDbContext<ApplicationDbContext>(options =>
               options.UseMySql(Configuration.GetConnectionString("defaultConnection"), serverVersion)
                .LogTo(Console.WriteLine, LogLevel.Information)
                .EnableSensitiveDataLogging()
                .EnableDetailedErrors()
            );


            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(opt => opt.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["secret"])),
                    ClockSkew = TimeSpan.Zero
                });

            services.AddEndpointsApiExplorer();

            services.AddSwaggerGen(c =>
            {

                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "WebApp",
                    Version = "v1",
                    Description = "Este es un api para trabjar con autores y libros",
                    Contact = new OpenApiContact
                    {
                        Email = "correo@mail.com",
                        Name = "John Doe",
                        Url = new Uri("http://google.com")
                    },
                    License = new OpenApiLicense
                    {
                        Name = "MIT"
                    }
                }); 
                c.SwaggerDoc("v2", new OpenApiInfo { Title = "WebApp", Version = "v2" });

                c.OperationFilter<AgregarParametroHATEOAS>();
                c.OperationFilter<AgregarParametroXVersion>();

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement {
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

                var archivoXML = $"{ Assembly.GetExecutingAssembly().GetName().Name }.xml";
                var rutaXML = Path.Combine( AppContext.BaseDirectory, archivoXML );

                c.IncludeXmlComments(rutaXML);

            });

            services.AddAutoMapper(typeof(StartUp));

            services.AddIdentity<IdentityUser, IdentityRole>()
                    .AddEntityFrameworkStores<ApplicationDbContext>()
                    .AddDefaultTokenProviders();

            services.AddAuthorization(opts =>
            {
                opts.AddPolicy("EsAdmin", pol => pol.RequireClaim("esAdmin"));
                //opts.AddPolicy("EsVendedor", pol => pol.RequireClaim("esVendedor"));
            });

            services.AddCors(opts =>
            {
                opts.AddDefaultPolicy(builder =>
                {
                    builder.WithOrigins("").AllowAnyMethod().AllowAnyHeader().WithHeaders( new string[] { "cantidadTotalRegistros" } );
                });
            });

            services.AddDataProtection();

            services.AddTransient<HashService>();

            services.AddTransient<GeneradorEnlace>();
            services.AddTransient<HATEOASAutorFilterAttribute>();
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();


        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<StartUp> logger)
        {

            //app.UseMiddleware<LoguearRespuestaMIddleware>();
            app.UseLoguearRespuestaHTTP();



            // Configure the HTTP request pipeline.
            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebApp v1");
                    c.SwaggerEndpoint("/swagger/v2/swagger.json", "WebApp v2");
                });
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors();

            app.UseResponseCaching();

            app.UseAuthorization();

            app.UseEndpoints(resp => resp.MapControllers());


        }

    }
}

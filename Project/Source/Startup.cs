using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using Api213.V2.Dal;
using Api213.V2.Exception;
using Api213.V2.Interface;
using Api213.V2.Manager;
using Api213.V2.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Api213
{
    /// <summary>
    /// Startup
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Startup"/> class.
        /// 
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="logger"></param>
        /// <param name="hostingEnvironment"></param>
        public Startup(IConfiguration configuration, ILogger<Startup> logger, IHostingEnvironment hostingEnvironment)
        {
            Configuration = configuration;
            Logger = logger;
            HostingEnvironment = hostingEnvironment;

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        public IHostingEnvironment HostingEnvironment { get; }

        /// <summary>
        /// 
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// Gets 
        /// </summary>
        public ILogger Logger { get; }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddDbContext<DataContext>(opt =>
                opt.UseInMemoryDatabase("DataContextList"));

            services.AddHttpContextAccessor();
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
                options.SuppressConsumesConstraintForFormFileParameters = true;
            });

            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = actionContext =>
                {
                    var errors = actionContext.ModelState
                        .Where(e => e.Value.Errors.Count > 0)
                        .Select(e => new
                        {
                            Name = e.Key,
                            Message = e.Value.Errors.First().ErrorMessage
                        }).ToArray();
                    
                    var invalidResponseFactory = new InvalidResponseFactory(actionContext.HttpContext);
                    var errorreturn = new BadRequestObjectResult(errors);
                    return invalidResponseFactory.ResponseGenericResult(errorreturn.StatusCode, errorreturn.Value.ToString(), "Invalid Model State");
                };
            });

            services.AddScoped<DbContext, DataContext>(f => f.GetService<DataContext>());

            services.AddMvcCore().AddVersionedApiExplorer(
                options =>
                {
                    options.GroupNameFormat = "'v'VVV";

                    // note: this option is only necessary when versioning by url segment. the SubstitutionFormat
                    // can also be used to control the format of the API version in route templates
                    options.SubstituteApiVersionInUrl = true;
                });

            services.AddApiVersioning(
                o =>
                {
                    o.AssumeDefaultVersionWhenUnspecified = true;
                    o.DefaultApiVersion = new ApiVersion(2, 0);
                    o.ReportApiVersions = true;
                    o.ErrorResponses = new InvalidResponseFactory();
                });

            #region AddSwaggerGen

           SwaggerDefaultValues.AddSwaggerGenforService(services);

            #endregion

            services.AddTransient(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddTransient(typeof(IGenericRepository<PetEntity>), typeof(GenericRepository<PetEntity>));
          
            services.AddTransient<IPetsManager, PetsManager>();
            services.AddTransient<IInvalidResponseFactory, InvalidResponseFactory>();
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="loggerFactory"></param>
        /// <param name="provider"></param>
        public void Configure(
            IApplicationBuilder app, 
            ILoggerFactory loggerFactory,
            IApiVersionDescriptionProvider provider)
        {
            if (HostingEnvironment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                Logger.LogWarning(" IsDevelopment" + HostingEnvironment.ContentRootPath);
            }
            else
            {
              app.UseHsts();
            }

#pragma warning disable S125 // Sections of code should not be "commented out"

            // app.UseHttpsRedirection();
#pragma warning restore S125 // Sections of code should not be "commented out"

            app.UseCors(builder => builder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials());

            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseMvc();
            
            app.UseSwagger();
          
            app.UseSwaggerUI(
                options => { SwaggerDefaultValues.SwaggerOptionUi(provider, options); });
        }
    }
}

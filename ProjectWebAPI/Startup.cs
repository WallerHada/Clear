using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using ProjectWebAPI.ActionFilter;
using ProjectWebAPI.Middleware;
using System;
using System.IO;
using System.Reflection;

namespace ProjectWebAPI
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
            #region controllers
            services.AddControllers(c =>
            {
                c.Filters.Add(typeof(CustomExceptionFilter));
                c.Filters.Add(typeof(MySampleActionFilter));
            });
            #endregion

            // services.AddDbContext<MyContext>(opt => opt.UseSqlServer(Configuration["ConnectionStrings:MyContext"]));

            #region IP
            services.AddScoped(container =>
            {
                var loggerFactory = container.GetRequiredService<ILoggerFactory>();
                var logger = loggerFactory.CreateLogger<ClientIpCheckActionFilter>();

                return new ClientIpCheckActionFilter(
                    Configuration["AdminSafeList"], logger);
            });
            #endregion

            #region Swagger
            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "ProjectWebAPI", Version = "v1" });

                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });
            #endregion

            #region Globalization and localization
            services.AddLocalization(o => o.ResourcesPath = "Resources");
            services.AddMvc()
                    .AddDataAnnotationsLocalization(options =>
                    {
                        options.DataAnnotationLocalizerProvider = (type, factory) =>
                            factory.Create(typeof(SharedResource));
                    });
            #endregion

            services.AddAuthentication().AddIdentityServerJwt();
            services.Configure<JwtBearerOptions>
                (IdentityServerJwtConstants.IdentityServerJwtBearerScheme,
                options => 
                {
                    var onTokenValidated = options.Events.OnTokenValidated;

                    options.Events.OnTokenValidated = async context =>
                    {
                        await onTokenValidated(context);
                    };
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

#if false
            The following Startup.Configure method adds middleware components for common app scenarios:
            1.Exception/error handling
                Developer Exception Page Middleware (UseDeveloperExceptionPage) reports app runtime errors.
                Exception Handler Middleware (UseExceptionHandler) catches exceptions thrown in the following middlewares.
            2.HTTPS Redirection Middleware (UseHttpsRedirection) redirects HTTP requests to HTTPS.
            3.Static File Middleware (UseStaticFiles) returns static files and short-circuits further request processing.
            4.Cookie Policy Middleware (UseCookiePolicy) conforms the app to the EU General Data Protection Regulation (GDPR) regulations.
            5.Routing Middleware (UseRouting) to route requests.
            6.Authentication Middleware (UseAuthentication) attempts to authenticate the user before they're allowed access to secure resources.
            7.Authorization Middleware (UseAuthorization) authorizes a user to access secure resources.
            8.Session Middleware (UseSession) establishes and maintains session state. If the app uses session state, call Session Middleware after Cookie Policy Middleware and before MVC Middleware.
            9.Endpoint Routing Middleware (UseEndpoints with MapRazorPages) to add Razor Pages endpoints to the request pipeline.
#endif
            app.UseRecordAccessLogMiddleware();

            app.UseIPLogMiddleware();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler();
                app.UseHsts();
            }

            #region Swagger
            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();
            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.)
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Project v1");
                c.RoutePrefix = string.Empty;
            });
            #endregion

            app.UseHttpsRedirection();

            #region Provides static file access
            var provider = new FileExtensionContentTypeProvider();
            provider.Mappings[".apk"] = "application/vnd.android.package-archive";
            provider.Mappings[".wgt"] = "application/wgt";
            provider.Mappings[".mp4"] = "application/iphone";
            app.UseStaticFiles(new StaticFileOptions
            {
                // this is you file-name
                FileProvider = new PhysicalFileProvider(Path.Combine(env.ContentRootPath, "MyStaticFiles")),
                // this is url path-name
                RequestPath = "/StaticFiles",
                ContentTypeProvider = provider
            });
            #endregion

            app.UseRouting();

            #region Globalization and localization
            // Globalization is the process of designing apps that support different cultures.
            // Globalization adds support for input, display, and output of a defined set of language scripts that relate to specific geographic areas.
            // Localization is the process of adapting a globalized app, which you have already processed for localizability, to a particular culture/locale. 
            var supportedCultures = new[] { "en-US", "zh-CN" };
            var localizationOptions = new RequestLocalizationOptions()
                .SetDefaultCulture(supportedCultures[0])
                // The CultureInfo object for SupportedCultures determines the results of culture-dependent functions, such as date, time, number, and currency formatting.
                // SupportedCultures also determines the sorting order of text, casing conventions, and string comparisons. 
                .AddSupportedCultures(supportedCultures)
                //  The SupportedUICultures determines which translated strings (from .resx files) are looked up by the ResourceManager. 
                .AddSupportedUICultures(supportedCultures);

            app.UseRequestLocalization(localizationOptions);
            #endregion

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}

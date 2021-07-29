using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using CommonServiceLocator;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Narik.Common.Infrastructure.Interfaces;
using Narik.Common.Infrastructure.Startup;
using Narik.Common.Services.Core;
using Narik.Common.Shared.Models;
using Narik.Common.Web.Infrastructure.Authorization;
using Narik.Common.Web.Infrastructure.Authorization.ResourceBased;
using Narik.Common.Web.Infrastructure.Authorization.RoleBased;
using Narik.Common.Web.Infrastructure.Filters;
using Narik.Common.Web.Infrastructure.Interfaces;
using Unity;
using Microsoft.AspNetCore.OData;
using Microsoft.OData.ModelBuilder;


namespace Narik.Common.Web.Infrastructure
{
    public enum AuthenticationModes
    {
        Cookie,
        Jwt
    }
    public class NarikStarupOptions
    {
        public Dictionary<string, Action<AuthorizationPolicyBuilder>> AuthorizationPolicies { get; set; }
        public Func<HttpContext, Task<bool>> NotFoundHandler { set; get; }

    }
    public class NarikStartup
    {

        private NarikModulesConfig _config;
        private readonly IUnityContainer _unityContainer;
        private NarikStarupOptions _options;
        private readonly IConfiguration _configuration;


        public NarikStartup(IConfiguration configuration,
            IUnityContainer unityContainer,
            NarikStarupOptions options)
        {
            _configuration = configuration;
            _unityContainer = unityContainer;
            _options = options;
        }

        public NarikStartup(IConfiguration configuration,
            IUnityContainer unityContainer) : this(configuration, unityContainer, null)
        {
        }


        private void Init(IServiceCollection services)
        {
            var moduleConfig = _configuration.GetSection("NarikModulesConfig");
            services.Configure<NarikModulesConfig>(moduleConfig);
            _config = moduleConfig.Get<NarikModulesConfig>();
            _unityContainer.RegisterInstance(typeof(NarikModulesConfig), _config);
            if (_options == null)
                _options = new NarikStarupOptions
                {

                };
        }

        public void ConfigureServices(IServiceCollection services)
        {
            Init(services);

            var key = Encoding.ASCII.GetBytes(_config.Secret);

            services.AddIdentityCore<ApplicationUser>(o =>
            {

            });
            services.AddHttpContextAccessor();
            var authenticationMode =
                _config.AuthenticationMode == null ? AuthenticationModes.Jwt :
                    Enum.Parse(typeof(AuthenticationModes), _config.AuthenticationMode, true) as AuthenticationModes?;
            if (authenticationMode == AuthenticationModes.Cookie)
            {
                services.AddAuthentication(
                    o =>
                    {
                        o.DefaultAuthenticateScheme = IdentityConstants.ApplicationScheme;
                        o.DefaultChallengeScheme = IdentityConstants.ApplicationScheme;
                        o.DefaultForbidScheme = IdentityConstants.ApplicationScheme;
                        o.DefaultSignInScheme = IdentityConstants.ApplicationScheme;
                        o.DefaultSignOutScheme = IdentityConstants.ApplicationScheme;
                    }).AddCookie(IdentityConstants.ApplicationScheme,
                    o =>
                    {
                        o.Events = new CookieAuthenticationEvents
                        {
                            OnValidatePrincipal = SecurityStampValidator.ValidatePrincipalAsync
                        };
                    });
            }
            else
            {
                services.AddAuthentication(
                    o =>
                    {
                        o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                        o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    }).AddJwtBearer(x =>
                {
                    x.RequireHttpsMetadata = false;
                    x.SaveToken = true;
                    x.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                });
            }


            services.AddSingleton<IAuthorizationPolicyProvider, NarikAuthorizationPolicyProvider>();
            services.AddSingleton<IAuthorizationHandler, NarikRoleAuthorizationHandler>();
            services.AddTransient<IAuthorizationHandler, NarikResourceAuthorizationHandler>();

            ConfigureContainer(_unityContainer);
            _unityContainer.Resolve<IEnvironment>().MainAssembly = Assembly.GetExecutingAssembly();
            var moduleService = _unityContainer.Resolve<IModuleService>();
            moduleService.InitModules();
            AutoMapperConfig.Configure(_unityContainer,services, moduleService);

           
            services.AddMvc(mvcOption =>
                {
                    //TODO:CORE_3
                    //o.AllowCombiningAuthorizeFilters = false;

                    mvcOption.EnableEndpointRouting = false;
                    if (_config.AddDefaultAuthenticationPolicy == null || _config.AddDefaultAuthenticationPolicy.Value)
                    {
                        var policy = new AuthorizationPolicyBuilder()
                            .RequireAuthenticatedUser()
                            .Build();
                        mvcOption.Filters.Add(new AuthorizeFilter(policy));
                    }
                    mvcOption.Filters.Add(typeof(NarikExceptionFilter));
                }).SetCompatibilityVersion(CompatibilityVersion.Latest)
                .ConfigureApplicationPartManager(apm =>
                {
                    var modules = _unityContainer.Resolve<IModuleService>().ModuleAssemblies;
                    foreach (var pluginAssembly in modules)
                    {
                        apm.ApplicationParts.Add(new AssemblyPart(pluginAssembly.Value));
                    }
                })
                .AddJsonOptions(opt =>
                {
                    if (_config.UseCamelCase == null || _config.UseCamelCase.Value)
                        opt.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                });

            services.AddControllers().AddOData(opt =>
            {
                opt.Select().Expand().Filter().OrderBy().SetMaxTop(_config.OdataMaxTop ?? 1000).Count();

                var narikModules = _unityContainer.ResolveAll<INarikModule>().ToList();
                foreach (var module in narikModules.OfType<INarikWebModule>())
                {
                    var builder = new ODataConventionModelBuilder();
                    if (_config.UseCamelCase == null || _config.UseCamelCase.Value)
                        builder.EnableLowerCamelCase();
                    module.RegisterOdataController(builder);
                    opt.AddRouteComponents("odata/" + module.Key, builder.GetEdmModel());
                }
            });

            
            services.AddSignalR();

            services.AddAuthorization(x =>
            {
                if (_options?.AuthorizationPolicies != null)
                    foreach (var optionsAuthorizationPolicy in _options.AuthorizationPolicies)
                        x.AddPolicy(optionsAuthorizationPolicy.Key, optionsAuthorizationPolicy.Value);

            });



        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            string[] apiPaths = { };
            string[] appDirectories = { };
            if (!string.IsNullOrEmpty(_config.ApiPrefixes))
            {
                apiPaths = _config.ApiPrefixes.Split(",", StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => $"/{x}/").ToArray();
            }
            if (!string.IsNullOrEmpty(_config.AppDirectories))
            {
                appDirectories = _config.AppDirectories.Split(",", StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => $"/{x}/").ToArray();
            }
            var container = ServiceLocator.Current.GetInstance<IUnityContainer>();
            var modules = container.ResolveAll<INarikModule>().ToList();
            var logService = container.Resolve<ILoggingService>();

            logService.Log("Configure:" + modules.Count);
            app.UseRouting();
            app.UseAuthentication();
            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseMvc(b =>
            {
                b.MapRoute("DefaultApi", "api/{controller}/{action}");
            });

            app.UseEndpoints(endPoints =>
            {
                endPoints.MapControllers();
                foreach (var module in modules.OfType<INarikWebModule>())
                    module.RegisterSignalRHubs(endPoints);
            });

            app.UseExceptionHandler(x =>
            {
                x.Run(async context =>
                {
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    context.Response.ContentType = "application/json";
                    var ex = context.Features.Get<IExceptionHandlerFeature>();
                    if (ex != null)
                    {
                        await context.Response.WriteAsync(ex.Error.Message
                        ).ConfigureAwait(false);
                    }
                });
            });
            app.Run(async (context) =>
            {
                var handled = false;
                if (_options?.NotFoundHandler != null)
                    handled = await _options.NotFoundHandler(context);
                if (!handled)
                {
                    if (!apiPaths.Any(x => context.Request.Path.Value.ToLower().StartsWith(x)))
                    {
                        if (string.IsNullOrEmpty(Path.GetExtension(context.Request.Path.Value)))
                        {
                            context.Response.ContentType = "text/html";
                            var matchAppDirectory = appDirectories.FirstOrDefault(x =>
                                context.Request.Path.Value.ToLower().Contains(x));
                            if (matchAppDirectory != null)
                                await context.Response.SendFileAsync(Path.Combine(env.WebRootPath,
                                    $"{matchAppDirectory.Substring(1)}/index.html"));
                            else
                                await context.Response.SendFileAsync(Path.Combine(env.WebRootPath, "index.html"));
                        }

                    }
                    else
                    {
                        context.Response.StatusCode = 404;
                    }
                }
            });
        }

        private void ConfigureContainer(IUnityContainer container)
        {
            UnityConfig.RegisterComponents(container);
            container.RegisterInstance(typeof(NarikModulesConfig), _config);
        }
    }
}

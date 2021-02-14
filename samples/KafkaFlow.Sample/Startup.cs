using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Reflection;
using System.Text;
using MessagePipeline;
using MessagePipeline.Producers;
using MessagePipeline.Sample;
using MessagePipeline.TypedHandler;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using NLog;
using Volte.Data.VolteDi;
using Volte.Utils;

namespace Zero.Boot.Launcher
{
    public class Startup
    {
        //private readonly IHostingEnvironment _env;
        private readonly IConfiguration _config;
        //////private readonly ILoggerFactory _loggerFactory;
        private string ZERO_ENABLE_SERVICE = "";
        private string sAPI_PATH_PREFIX = "";
        public Startup(IConfiguration config)
        {
            _config = config.ReplacePlaceholders();
            ZERO_ENABLE_SERVICE = _config["ZERO_ENABLE_SERVICE"] ?? string.Empty;
            if (string.IsNullOrEmpty(ZERO_ENABLE_SERVICE))
            {
                ZERO_ENABLE_SERVICE = "ZERO_API";
            }
        }

        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddJobHosting()
            services.AddOptions();
            if (_config["ZERO_BETA"]!=null)
            {
                NLogger.Info("nlog-dev.config ZERO_BETA=true");
                LogManager.LoadConfiguration($@"nlog-dev.config");
            }
            else
            {
                NLogger.Info("nlog-prd.config ZERO_BETA=false");
                LogManager.LoadConfiguration($@"nlog-prd.config");
            }

            //services.AddSingleton<WorkflowHostService>();

            string ZERO_MAX_CONCURRENT_REQUESTS = _config["ZERO_MAX_CONCURRENT_REQUESTS"] ?? "2";
            string ZERO_REQUEST_QUEUE_LIMIT = _config["ZERO_REQUEST_QUEUE_LIMIT"] ?? "10";

            NLogger.Info("MaxConcurrentRequests(ZERO_MAX_CONCURRENT_REQUESTS) = [" + ZERO_MAX_CONCURRENT_REQUESTS + "]");
            NLogger.Info("RequestQueueLimit(ZERO_REQUEST_QUEUE_LIMIT) = [" + ZERO_REQUEST_QUEUE_LIMIT + "]");

            //if (Util.ToInt(ZERO_MAX_CONCURRENT_REQUESTS) > 1)
            //{
            //    NLogger.Info("QueuePolicy: Enabled");

            //    services.AddQueuePolicy(options =>
            //    {
            //        //最大并发请求数
            //        options.MaxConcurrentRequests = Util.ToInt(ZERO_MAX_CONCURRENT_REQUESTS);
            //        //请求队列长度限制
            //        options.RequestQueueLimit = Util.ToInt(ZERO_REQUEST_QUEUE_LIMIT);
            //    });
            //}
            //else
            //{
            //    NLogger.Info("QueuePolicy: Disabled");
            //}
        
            string ZERO_REDIS = _config["ZERO_REDIS"] ?? string.Empty;
            string ZERO_REDIS_DB = _config["ZERO_REDIS_DB"] ?? string.Empty;
            NLogger.Info("RedisClient (ZERO_REDIS) = [" + ZERO_REDIS+"]");

            //if (!string.IsNullOrEmpty(ZERO_REDIS))
            //{
            //    NLogger.Info("Using CSRedisClient - " + ZERO_REDIS);

            //    services.AddScoped<ICacheService, RedisCache>();
            //    var csredis = new CSRedis.CSRedisClient(ZERO_REDIS);
            //    RedisHelper.Initialization(csredis);//初始化
            //}
            VolteDiOptions _opt = new VolteDiOptions();
            List<string> aDir = new List<string>();
            string ZERO_ADDONS_DIR = _config["ZERO_ADDONS_DIR"] ?? string.Empty;

            NLogger.Info("Plugin Location(ZERO_ADDONS_DIR) = [" + ZERO_ADDONS_DIR + "]");

            if (!string.IsNullOrEmpty(ZERO_ADDONS_DIR))
            {
                ZERO_ADDONS_DIR = ZERO_ADDONS_DIR + ";";

                foreach (string dd2 in ZERO_ADDONS_DIR.Split(new char[1] { ';' }))
                {
                    if (!string.IsNullOrEmpty(dd2) && Directory.Exists(dd2))
                    {
                        aDir.Add(dd2);
                    }
                }
            }

            string sAddonsDirectory = Directory.GetParent(Directory.GetCurrentDirectory()) + Path.DirectorySeparatorChar.ToString() + "addons";
            NLogger.Info("AddonsDirectory = " + sAddonsDirectory);

            if (Directory.Exists(sAddonsDirectory) && aDir.IndexOf(sAddonsDirectory)<0)
            {
                aDir.Add(sAddonsDirectory);
            }

            string sBinDirectory = Directory.GetCurrentDirectory();
            if (Directory.Exists(sBinDirectory))
                {
                aDir.Add(sBinDirectory);
            }

            if (aDir.Count > 0)
            {
                StringBuilder sDir = new StringBuilder();
                _opt.AssemblyPaths = aDir.ToArray();
                sDir.AppendLine("\nAddons Directory:");
                foreach(string d in aDir)
                {
                    sDir.AppendLine("   "+d);
                }
                NLogger.Info(sDir.ToString());
            }
            _opt.AssemblyNames = new string[] { "*" };

            NLogger.Info("Scan classes ...");
             IList<Assembly> _Assembly = AssemblyLoader.LoadAssembly(_opt).ToList();

            //VolteDiLoader _VolteDiLoader = new VolteDiLoader();

            //IList<Assembly> _Assembly = _VolteDiLoader.LoadAssembly(_opt).Where(u => (u.FullName.Contains(".CAP") || u.FullName.Contains("Zero.") || u.FullName.Contains("Volte.") || u.FullName.Contains("Zero.Common") || u.FullName.Contains("ADM0") || u.FullName.Contains("1084") || u.FullName.Contains("1074") || u.FullName.Contains("Kafka.MySql"))).ToList();

            StringBuilder UseAssembly = new StringBuilder();
            UseAssembly.AppendLine("\nUse Assembly:");
            foreach (var v in _Assembly)
            {
                if (string.IsNullOrEmpty(v.Location))
                {
                    UseAssembly.AppendLine("  Assembly ---- " + v.GetAssemblyName());
                }
                else
                {
                    UseAssembly.AppendLine("  Assembly File " + v.Location);
                }
            }
            NLogger.Debug(UseAssembly);
            StringBuilder ScanClass = new StringBuilder();
            ScanClass.AppendLine();

            services.UseKafkaFlow();

            services.LoadInjection(_Assembly.ToArray<Assembly>());

            //string ZERO_ELSA = _config["ZERO_ELSA"] ?? string.Empty;
           
            VolteDiServiceProvider.Registered(services);

            sAPI_PATH_PREFIX = _config["ZERO_API_PATH_PREFIX"];
            if (string.IsNullOrEmpty(sAPI_PATH_PREFIX))
            {
                sAPI_PATH_PREFIX = "";
            }
            sAPI_PATH_PREFIX = "/" + sAPI_PATH_PREFIX.TrimStart('/');
            NLogger.Info("PathMatch (ZERO_API_PATH_PREFIX)=" + sAPI_PATH_PREFIX);

            if (ZERO_ENABLE_SERVICE.IndexOf("ZERO_API") >= 0)
            {
                NLogger.Info("ZERO_API [Enabled]");
            
                string sAPP_PATH = _config["ZERO_API_APP_PATH"];
                if (string.IsNullOrEmpty(sAPI_PATH_PREFIX))
                {
                    sAPI_PATH_PREFIX = "/activity";
                }
                if (string.IsNullOrEmpty(sAPP_PATH))
                {
                    sAPP_PATH = "";
                }
                NLogger.Info("PathMatch =["+ sAPI_PATH_PREFIX + "]");
                NLogger.Info("AppPath =["+ sAPP_PATH + "]");
            }

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IMessageHandlerFactory, DefaultMessageHandlerFactory>();

            services.AddMvc(options => options.EnableEndpointRouting = true);

            string ZERO_HEALTH_CHECKS = _config["ZERO_HEALTH_CHECKS"];
            NLogger.Info("Health Checks (ZERO_HEALTH_CHECKS)=" + ZERO_HEALTH_CHECKS);

            if (Util.ToBoolean(ZERO_HEALTH_CHECKS))
            {
                NLogger.Info("HealthChecks =[Enabled]");

                var AddHealthChecks = services.AddHealthChecks();
                //AddHealthChecks.AddCheck<MySqlDbHealthCheck>("database");
                AddHealthChecks.AddCheck("self", () => HealthCheckResult.Healthy());
            }
            else
            {
                NLogger.Info("HealthChecks =[Disabled]");
            }
            
            new VolteServiceCollection(services);

        }

        public void Configure(IApplicationBuilder app)
        {
            var provider = app.ApplicationServices;

            var bus=provider.GetRequiredService<IKafkaBus>();
            bus.StartAsync();

            app.Use((context, next) =>
            {
                for (int i = 0; i < 10000; i++)
                {
                    const string producerName = "master";

                    var producers = provider.GetRequiredService<IProducerAccessor>();

                    var msg = new TestMessage { Text = $"Message:{i}-{Guid.NewGuid()}" };
                    producers[producerName].Produce(Guid.NewGuid().ToString(), msg);

                }
                NLogger.Info("producers 10000 message");

                return next();
            });

            //设置CultureInfo
            var zh = new CultureInfo("zh-CN");
            zh.DateTimeFormat.FullDateTimePattern = "yyyy-MM-dd HH:mm:ss";
            zh.DateTimeFormat.LongDatePattern = "yyyy-MM-dd";
            zh.DateTimeFormat.LongTimePattern = "HH:mm:ss";
            zh.DateTimeFormat.ShortDatePattern = "yyyy-MM-dd";
            zh.DateTimeFormat.ShortTimePattern = "HH:mm:ss";
            IList<CultureInfo> supportedCultures = new List<CultureInfo>
            {
                zh,
            };
            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture("zh-CN"),
                SupportedCultures = supportedCultures,
                SupportedUICultures = supportedCultures
            });

            //app.MystiqueRoute();
            //app.UseHangfireServer(new BackgroundJobServerOptions
            //{
            //    Queues = new[] { "critical", "default" },
            //    TaskScheduler = null
            //});

            if (ZERO_ENABLE_SERVICE.IndexOf("ZERO_EVENTBUS") >= 0)
            {
                NLogger.Info("EventBus API Setting");

                if (ZERO_ENABLE_SERVICE.IndexOf("ZERO_DASHBOARD") >= 0)
                {
                    NLogger.Info("Configure AddApi.....");
                }
            }

            if (ZERO_ENABLE_SERVICE.IndexOf("ZERO_API") >= 0)
            {
                NLogger.Info("Web API Setting");

                //if (provider.GetService<ActivityOptions>() != null)
                //{
                //    app.UseMiddleware<ActivityMiddleware>();
                //    app.UseMiddleware<FileUpMiddleware>();
                //}
                //else
                //{
                //    NLogger.Info("Web API ActivityOptions Is Null");
                //}
            }
            else
            {
                NLogger.Info("Web API disabled");
            }

            string sDirectory = _config["ZERO_WEB_ROOT"];

            if (string.IsNullOrEmpty(sDirectory))
            {
                sDirectory = Directory.GetParent(Directory.GetCurrentDirectory()) + Path.DirectorySeparatorChar.ToString() + "www";
            }
            string sRequestPath = sAPI_PATH_PREFIX;
            if (string.IsNullOrEmpty(sRequestPath))
            {
                sRequestPath = Path.GetDirectoryName(sDirectory);
            }
            sRequestPath = "/" + sRequestPath.TrimStart('/');

            if (Directory.Exists(sDirectory))
            {
                NLogger.Info("Static Web [Enabled]");
                NLogger.Info("Web Root (ZERO_WEB_ROOT) = " + sDirectory);

                NLogger.Debug("Directory   = " + Path.GetFileName(sDirectory));
                NLogger.Debug("RequestPath = " + sRequestPath);
                var staticfile = new StaticFileOptions();
                staticfile.FileProvider = new PhysicalFileProvider(sDirectory);
                staticfile.RequestPath = sAPI_PATH_PREFIX;

                app.UseDefaultFiles();
                app.UseStaticFiles(staticfile);
            }
            else
            {
                NLogger.Info(sDirectory + " Not Found,Static Web disabled!");
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                //endpoints.MapHealthChecks("/health");
                //endpoints.MapDynamicControllerRoute<TranslationTransformer>("{area}/{controller=Home}/{action=Index}/{id?}");
            });

            string ZERO_HEALTH_CHECKS = _config["ZERO_HEALTH_CHECKS"];
            NLogger.Info("Health Checks (ZERO_HEALTH_CHECKS)=" + ZERO_HEALTH_CHECKS);

            if (Util.ToBoolean(ZERO_HEALTH_CHECKS))
            {
                app.UseHealthChecks("/healthz", new HealthCheckOptions
                {
                    Predicate = _ => true
                });

                app.UseHealthChecks(sRequestPath + "/health", new HealthCheckOptions
                {
                    Predicate = _ => true,
                    ResponseWriter = async (context, report) =>
                    {
                        var result = JsonConvert.SerializeObject(
                            new
                            {
                                status = report.Status.ToString(),
                                entries = report.Entries.Select(e => new { key = e.Key, value = Enum.GetName(typeof(HealthStatus), e.Value.Status) })
                            });
                        context.Response.ContentType = MediaTypeNames.Application.Json;
                        await context.Response.WriteAsync(result);
                    }
                });

            }
        }
    }
}
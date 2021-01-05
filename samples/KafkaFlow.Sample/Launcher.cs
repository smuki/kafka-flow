using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Volte.Data.Json;
using Volte.Utils;

namespace Zero.Boot.Launcher
{
    public class Launcher
    {
        public static void Main(string[] args)
        {

            var _Config = new ConfigurationBuilder()
                  .SetBasePath(Directory.GetCurrentDirectory())
                  .AddCommandLine(args)
                  .AddEnvironmentVariables()
                  .Build();

            StringBuilder confInfo = new StringBuilder();

            string sConfigurationFile = _Config["ZERO_CONF_FILE"];

            if (string.IsNullOrEmpty(sConfigurationFile) )
            {
                confInfo.AppendLine($"\nConfiguration file EnvironmentVariable(ZERO_CONF_FILE): Undefined");
                sConfigurationFile = Path.DirectorySeparatorChar.ToString() + "conf" + Path.DirectorySeparatorChar.ToString() + @"appsettings.json";
            }
            else if (!File.Exists(sConfigurationFile))
            {
                confInfo.AppendLine($"Configuration file: {sConfigurationFile} (No)");

                sConfigurationFile = Path.DirectorySeparatorChar.ToString() + "conf" + Path.DirectorySeparatorChar.ToString() + @"appsettings.json";
            }
            else
            {
                confInfo.AppendLine($"Configuration file: {sConfigurationFile} (Yes)");
            }
            if (!File.Exists(sConfigurationFile))
            {
                confInfo.AppendLine($"Configuration file: {sConfigurationFile} (No)");

                sConfigurationFile = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar.ToString() + @"appsettings.json";
            }
            string Version = "5.75.0";

            if (File.Exists("version.json"))
            {
                JSONObject oVersion = new JSONObject(File.ReadAllText("version.json"));
                _Config["ZERO_BUILD_VERSION"] = oVersion.GetValue("version");
                Version = oVersion.GetValue("version");
            }
            if (File.Exists(sConfigurationFile))
            {
                string sConfiguration = File.ReadAllText(sConfigurationFile);
                JSONObject vVariable = new JSONObject(sConfiguration).GetJSONObject("Variable");
                JSONObject vMaster = new JSONObject(sConfiguration).GetJSONObject("master");
                foreach (string name in vVariable.Names)
                {
                    if (!string.IsNullOrEmpty(name))
                    {
                        _Config[name] = vVariable.GetValue(name);
                    }
                }

                foreach (string name in vMaster.Names)
                {
                    if (!string.IsNullOrEmpty(name))
                    {
                        _Config["master_"+name] = vMaster.GetValue(name);
                    }
                }
            }
           

            var builder = new HostBuilder();
            builder.UseContentRoot(Directory.GetCurrentDirectory());
            builder.ConfigureHostConfiguration(config =>
            {
                config.AddEnvironmentVariables(prefix: "ZERO_");
                config.AddInMemoryCollection(
                new Dictionary<string, string> {
                    {"Timezone", "+1"}
                });

                if (args != null)
                {
                    config.AddCommandLine(args);
                }
            });

            var ZERO_PORT = _Config["ZERO_PORT"];
            if (string.IsNullOrEmpty(ZERO_PORT))
            {
                ZERO_PORT = "80";
            }

            string sUseUrls = "http://*:" + ZERO_PORT;

            StringBuilder art = new StringBuilder();
            art.AppendLine(@"");
            art.AppendLine(@"                                 ");
            art.AppendLine(@"                                 ");
            art.AppendLine(@"                   (    (        ");
            art.AppendLine(@"               (   )\   )\       ");
            art.AppendLine(@"      (       ))\ ((_) ((_)      ");
            art.AppendLine(@"      )\ )   /((_)  _    _       ");
            art.AppendLine(@"     _(_/(  (_))(  | |  | |      ");
            art.AppendLine(@"    | ' \)) | || | | |  | |      ");
            art.AppendLine(@"    |_||_|   \_,_| |_|  |_|      ");
            art.AppendLine(@"                                 ");
            art.AppendLine(@"    =========================    ");
            art.AppendLine(@"                                 ");
            art.AppendLine(@"                                 ");
            art.AppendLine($"Environment Variables:");
            foreach (var c in _Config.AsEnumerable())
            {
                if (c.Key.IndexOf("ZERO_") == 0)
                {
                    art.AppendLine($"   " + c.Key + " = [" + c.Value + "]");
                }
            }
            art.AppendLine($"Listening On:[{sUseUrls}]");
            art.AppendLine(@"                                 ");

            string RandomId = Util.GenerateRandomId(12);
            string ClientId = Dns.GetHostName();
            TimeZoneInfo zone = TimeZoneInfo.Local;
            string ClusterId = "-";
            art.AppendLine($"Current timezone is {zone.Id} ({zone.BaseUtcOffset.ToString()})");
            art.AppendLine($"Program version is {Version}");
            art.AppendLine($"Cluster id is {ClusterId}");
            art.AppendLine($"Client id is {ClientId}");
            art.AppendLine($"Random id is {RandomId}");
            art.AppendLine("Public address is / " + Util.InterfaceAddress(""));
            art.AppendLine("Starting NULL ...");

            if (File.Exists(sConfigurationFile))
            {
                confInfo.AppendLine($"Load external configuration file: {sConfigurationFile}");
                builder.ConfigureAppConfiguration((hostingContext, config) =>
                 {
                     config.SetBasePath(Directory.GetCurrentDirectory());
                     if (File.Exists(sConfigurationFile))
                     {
                         config.AddJsonFile(sConfigurationFile, optional: true, reloadOnChange: true);
                     }
                 });
            }
            else
            {
                confInfo.AppendLine("Configuration File " + sConfigurationFile + " (No)");
            }
            confInfo.AppendLine(confInfo.ToString());
            Console.WriteLine(art.ToString());

            string nlogFile = _Config["ZERO_NLOG_FILE"];
            if (nlogFile==null || !File.Exists(nlogFile))
            {
                NLogger.Info("NLog Config(ZERO_BETA)=[" + _Config["ZERO_BETA"] + "]");
                if (Util.ToBoolean(_Config["ZERO_BETA"]))
                {
                    nlogFile = "nlog-dev.config";
                }
                else
                {
                    nlogFile = "nlog-prd.config";
                }
            }
            NLogger.Info("NLog Config(ZERO_NLOG_FILE)=[" + nlogFile+"]");
            builder.ConfigureWebHostDefaults(webBuilder =>
            {
                 webBuilder.UseConfiguration(_Config);
                 webBuilder.ConfigureLogging((hostingContext, logging) =>
                 {
                     logging.ClearProviders();
                     //logging.AddNLog();
                     if (File.Exists(nlogFile))
                     {
                         NLog.LogManager.LoadConfiguration(nlogFile);
                     }
                 });
                 webBuilder.UseKestrel((context, options) =>
                 {
                     options.AllowSynchronousIO = true;
                     // Set properties and call methods on options
                 })
                 .UseUrls(sUseUrls)
                 .UseStartup<Startup>();

             });
            builder.Build().Run();
            Console.WriteLine("Server shutdown.");
        }
    }
}
using Serilog;
using Serilog.Core;
using System;
using System.Configuration;


namespace TCPOS.InsertCustomers.Utils
{
    class LogUtil
    {
        public static void InitLogger()
        {
            //// read path from app.config 
            var path2 = ConfigurationManager.AppSettings["logFile"];
            string currentDirectory = Environment.CurrentDirectory;
            string path = currentDirectory + path2;

            var levelSwitch = new LoggingLevelSwitch();
            levelSwitch.MinimumLevel = Serilog.Events.LogEventLevel.Verbose;

            var logger = new LoggerConfiguration()
                .WriteTo.Logger(subLogger => subLogger
                    .WriteTo.File($"{path}DebugLogs-{DateTime.Today:yyyyMMdd}.Log",
                                  restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Verbose))
                    .MinimumLevel.ControlledBy(levelSwitch)
                .WriteTo.Logger(subLogger => subLogger
                    .WriteTo.File($"{path}Journal-{DateTime.Today:yyyyMMdd}.Log",
                                  restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information))
                .CreateLogger();

            Log.Logger = logger;
        }
    }
}
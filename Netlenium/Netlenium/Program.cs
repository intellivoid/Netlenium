using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using Mono.Options;
using Netlenium.Logging;
using ApplicationPaths = Netlenium.Driver.ApplicationPaths;

namespace Netlenium
{

    /// <summary>
    /// Main Program Class
    /// </summary>
    internal static class Program
    {
        /// <summary>
        /// Main Execution Pointer
        /// </summary>
        /// <param name="args"></param>
        private static void Main(string[] args)
        {
            Console.Title = ProgramText.ProgramTitle;
            Console.WriteLine(ProgramText.ProgramVersion, Assembly.GetExecutingAssembly().GetName().Version);
            Console.WriteLine(ProgramText.ProgramCopyright, Environment.NewLine);

            // Parse the command-line arguments
            CommandLineParameters.Help = false;
            CommandLineParameters.UpdateDrivers = false;
            CommandLineParameters.ClearCache = false;
            CommandLineParameters.DisabledStdout = false;
            CommandLineParameters.DisableFileLogging = false;
            CommandLineParameters.DriverLoggingLevel = 1;
            CommandLineParameters.ServerLoggingLevel = 1;
            CommandLineParameters.Port = 6410;
            CommandLineParameters.ServerName = ProgramText.ProgramTitle;
            CommandLineParameters.MaxSessions = 100;
            CommandLineParameters.SessionInactivityLimit = 10;
            CommandLineParameters.DefaultDriver = "chrome";
            CommandLineParameters.DisableHeadlessMode = false;
            CommandLineParameters.DisableChromeDriver = false;
            CommandLineParameters.DisableFirefoxDriver = false;
            CommandLineParameters.DisableOperaDriver = false;
            CommandLineParameters.AuthPassword = string.Empty;
            CommandLineParameters.AdministratorPassword = string.Empty;
            CommandLineParameters.AllowRemoteRequests = false;
            CommandLineParameters.DebuggingMode = false;

            var p = new OptionSet
            {

                { "h|help",  "Displays the help menu and exit",
                  v => CommandLineParameters.Help = v != null },

                 { "update",  "Fetches missing driver resources and or updates outdated resources then exits",
                  v => CommandLineParameters.UpdateDrivers = v != null },

                 { "clear-cache",  "Clears unused cache files created by Netlenium",
                  v => CommandLineParameters.ClearCache = v != null },

                { "disable-stdout",  "Disables standard output",
                  v => CommandLineParameters.DisabledStdout = v != null },

                { "disable-file-logging",  "Disables logging to files",
                  v => CommandLineParameters.DisableFileLogging = v != null },

                { "driver-logging-level=", "Logging level for Driver Sessions",
                  v => { if (v != null) CommandLineParameters.DriverLoggingLevel = Convert.ToInt32(v); } },

                { "server-logging-level=", "Logging level for the Web Service",
                  v => { if (v != null) CommandLineParameters.ServerLoggingLevel = Convert.ToInt32(v); } },
                
                { "p|port=", "The port to run the Web Service on",
                  v => { if (v != null) CommandLineParameters.Port = Convert.ToInt32(v); } },

                { "allow-remote-requests",  "Disables loopback only connections",
                  v => CommandLineParameters.AllowRemoteRequests = v != null },

                { "debugging-mode",  "Sets server and driver logging level to 3 and disables headless mode",
                  v => CommandLineParameters.DebuggingMode = v != null },

                { "server-name=", "The port to run the Web Service on",
                  v => { if (v != null) CommandLineParameters.ServerName = v; } },

                { "max-sessions=", "The maximum number of sessions that can be created",
                  v => { if (v != null) CommandLineParameters.MaxSessions = Convert.ToInt32(v); } },

                { "session-inactivity-limit=", "The amount of minutes that a session is allowed to be inactive for",
                  v => { if (v != null) CommandLineParameters.SessionInactivityLimit = Convert.ToInt32(v); } },

                { "default-driver=", "The default driver to start when no target browser is provided",
                  v => { if (v != null) CommandLineParameters.DefaultDriver = v; } },
                
                { "disable-headless-mode", "Prevents browsers from starting in headless mode",
                  v => CommandLineParameters.DisableHeadlessMode = v != null },

                { "disable-chrome-driver",  "Disables the ability to start Chrome Drivers",
                  v => CommandLineParameters.DisableChromeDriver = v != null },

                { "disable-firefox-driver",  "Disables the ability to start Firefox Drivers",
                  v => CommandLineParameters.DisableFirefoxDriver = v != null },

                 { "disable-opera-driver",  "Disables the ability to start Opera Drivers",
                  v => CommandLineParameters.DisableOperaDriver = v != null },

                { "auth-password=", "Authentication Password for Web Service access",
                  v => { if (v != null) CommandLineParameters.AuthPassword = v; } },

                { "admin-password=", "Authentication Password for Administrator access",
                  v => { if (v != null) CommandLineParameters.AdministratorPassword = v; } }

            };
            p.Parse(args);

            if(CommandLineParameters.Help)
            {
                ShowHelp();
            }
            
            VerifyValues();

            WebService.Logging.Name = "Netlenium.Server";

            switch (CommandLineParameters.ServerLoggingLevel)
            {
                case 0:
                    WebService.Logging.InformationEntriesEnabled = false;
                    WebService.Logging.WarningEntriesEnabled = false;
                    WebService.Logging.ErrorEntriesEnabled = false;
                    WebService.Logging.VerboseEntriesEnabled = false;
                    WebService.Logging.DebuggingEntriesEnabled = false;
                    break;

                case 1:
                    WebService.Logging.InformationEntriesEnabled = true;
                    WebService.Logging.WarningEntriesEnabled = true;
                    WebService.Logging.ErrorEntriesEnabled = true;
                    WebService.Logging.VerboseEntriesEnabled = false;
                    WebService.Logging.DebuggingEntriesEnabled = false;
                    break;

                case 2:
                    WebService.Logging.InformationEntriesEnabled = true;
                    WebService.Logging.WarningEntriesEnabled = true;
                    WebService.Logging.ErrorEntriesEnabled = true;
                    WebService.Logging.VerboseEntriesEnabled = true;
                    WebService.Logging.DebuggingEntriesEnabled = false;
                    break;

                case 3:
                    WebService.Logging.InformationEntriesEnabled = true;
                    WebService.Logging.WarningEntriesEnabled = true;
                    WebService.Logging.ErrorEntriesEnabled = true;
                    WebService.Logging.VerboseEntriesEnabled = true;
                    WebService.Logging.DebuggingEntriesEnabled = true;
                    break;

                default:
                    Console.WriteLine(ProgramText.ServerLoggingLevelInvalidOption);
                    Environment.Exit(1);
                    break;
            }

            if (CommandLineParameters.DebuggingMode == true)
            {
                CommandLineParameters.DriverLoggingLevel = 3;
                CommandLineParameters.ServerLoggingLevel = 3;
                CommandLineParameters.DisableHeadlessMode = true;
            }
            else
            {
                if (CommandLineParameters.DriverLoggingLevel < 0)
                {
                    Console.WriteLine(ProgramText.DriverLoggingLevelInvalidOption);
                    Environment.Exit(1);
                }

                if (CommandLineParameters.DriverLoggingLevel > 3)
                {
                    Console.WriteLine(ProgramText.DriverLoggingLevelInvalidOption);
                    Environment.Exit(1);
                }
            }
            
            if(CommandLineParameters.UpdateDrivers)
            {
                UpdateDrivers();
                Environment.Exit(0);
            }

            if (CommandLineParameters.ClearCache)
            {
                ClearCache();
                Environment.Exit(0);
            }

            Console.Title = ProgramText.ProgramServerTitle;
            Console.CancelKeyPress += CommandCancelEventHandler;

            try
            {
                var endpoint = WebService.Start(CommandLineParameters.Port);
                Console.Title = string.Format(ProgramText.ProgramServerEndpointTitle, ProgramText.ProgramServerTitle, endpoint);
            }
            catch(Exception)
            {
                Environment.Exit(1);
            }

            while (true) { Console.ReadKey(true); }

            // ReSharper disable once FunctionNeverReturns
        }

        /// <summary>
        /// Verifies the values in the given arguments
        /// </summary>
        private static void VerifyValues()
        {
            if (CommandLineParameters.MaxSessions < 1)
            {
                Console.WriteLine(ProgramText.MaxSessionsInvalidOption_Less);
                Environment.Exit(64);
            }

            if (CommandLineParameters.MaxSessions > 99999)
            {
                Console.WriteLine(ProgramText.MaxSessionsInvalidOption_Greater);
                Environment.Exit(64);
            }

            if(CommandLineParameters.SessionInactivityLimit < 0)
            {
                Console.WriteLine(ProgramText.SessionInactivityLimitInvalidValue_Less);
                Environment.Exit(64);
            }

            if(CommandLineParameters.ServerName.Length < 1)
            {
                Console.WriteLine(ProgramText.ServerNameInvalidOption_Empty);
                Environment.Exit(64);
            }

            if (CommandLineParameters.SessionInactivityLimit > 99999)
            {
                Console.WriteLine(ProgramText.SessionInactivityLimitInvalidValue_Greater);
                Environment.Exit(64);
            }
            
            if(CommandLineParameters.AuthPassword != string.Empty)
            {
                if(CommandLineParameters.AuthPassword.Length < 6)
                {
                    Console.WriteLine(ProgramText.InavlidAuthPasswordOption);
                    Environment.Exit(64);
                }
            }

            if (CommandLineParameters.AdministratorPassword != string.Empty)
            {
                if (CommandLineParameters.AdministratorPassword.Length < 6)
                {
                    Console.WriteLine(ProgramText.ProgramInvalidOptionForAdminPassword);
                    Environment.Exit(64);
                }
            }

            if (CommandLineParameters.DefaultDriver != string.Empty)
            {
                switch(CommandLineParameters.DefaultDriver.ToLower())
                {
                    case "chrome": break;
                    case "opera": break;
                    case "firefox": break;

                    default:
                        Console.WriteLine(ProgramText.ProgramInvalidOptionForDefaultDriver);
                        Environment.Exit(64);
                        break;
                }
            }

            WebService.Logging.CommandLineLoggingEnabled = !CommandLineParameters.DisabledStdout;
            WebService.Logging.FileLoggingEnabled = !CommandLineParameters.DisableFileLogging;
        }

        /// <summary>
        /// Fetches missing driver resources and or updates existing resources that are outdated
        /// </summary>
        private static void UpdateDrivers()
        {
            var chromeDriver = new Driver.Chrome.Driver();
            var firefoxDriver = new Driver.Firefox.Driver();
            var operaDriver = new Driver.Opera.Driver();

            Utilities.ApplyOptionsToDriver(chromeDriver);
            Utilities.ApplyOptionsToDriver(firefoxDriver);
            Utilities.ApplyOptionsToDriver(operaDriver);

            chromeDriver.DriverManager.Initialize();
            firefoxDriver.DriverManager.Initialize();
            operaDriver.DriverManager.Initialize();
        }

        /// <summary>
        /// Clears unused cache files created by Netlenium
        /// </summary>
        private static void ClearCache()
        {
            foreach(var file in Directory.GetFiles(ApplicationPaths.TemporaryDirectory))
            {
                try
                {
                    Console.WriteLine(ProgramText.ProgramDeletingFile, file);
                    File.Delete(file);
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ProgramText.ErrorCannotDeleteFile, file, ex.Message);
                }
            }

            foreach (var directory in Directory.GetDirectories(ApplicationPaths.TemporaryDirectory))
            {
                try
                {
                    Console.WriteLine(ProgramText.ProgramDeletingDirectory, directory);
                    Directory.Delete(directory, true);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ProgramText.ErrorCannotDeleteDirectory, directory, ex.Message);
                }
            }

            Console.WriteLine(ProgramText.ProgramOperationCompleted);
        }

        /// <summary>
        /// Displays the Help Menu
        /// </summary>
        private static void ShowHelp()
        {
            Console.WriteLine(ProgramText.HelpMenu);
            Environment.Exit(0);
        }

        /// <summary>
        /// Handles the Cancel Command event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void CommandCancelEventHandler(object sender, ConsoleCancelEventArgs e)
        {
            GracefullyShutdown();
        }

        /// <summary>
        /// Gracefully shuts down the server and closes the process
        /// </summary>
        private static void GracefullyShutdown()
        {
            WebService.Logging.WriteEntry(MessageType.Information, "Application", "Shutting down server");
            WebService.Stop();

            if (SessionManager.ActiveSessions != null)
            {
                WebService.Logging.WriteEntry(MessageType.Information, "Application", "Closing active sessions");
                
                while(true)
                {
                    var currentActiveSessions = new List<string>();
                    foreach (var session in SessionManager.ActiveSessions.Keys)
                    {
                        currentActiveSessions.Add(session);
                    }

                    try
                    {
                        foreach (var session in currentActiveSessions)
                        {
                            try
                            {
                                SessionManager.StopSession(session);
                            }
                            catch (Exception ex)
                            {
                                WebService.Logging.WriteEntry(MessageType.Warning, "Application", $"Cannot close session '{session}', {ex.Message}");
                            }
                        }

                        break;
                    }
                    catch(Exception)
                    {
                        WebService.Logging.WriteEntry(MessageType.Error, "Application", "SessionManager is busy, trying again in 2 seconds");
                        Thread.Sleep(2000);
                    }
                }
                
            }

            WebService.Logging.WriteEntry(MessageType.Information, "Application", "Closing process");
            Environment.Exit(0);
        }
    }
}

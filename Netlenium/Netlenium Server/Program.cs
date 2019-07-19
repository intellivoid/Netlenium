using System;
using System.Collections.Generic;
using System.Threading;
using Mono.Options;

namespace NetleniumServer
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
            // Parse the command-line arguments
            CommandLineParameters.Help = false;
            CommandLineParameters.DisabledStdout = false;
            CommandLineParameters.DisableFileLogging = false;
            CommandLineParameters.DriverLoggingLevel = 1;
            CommandLineParameters.ServerLoggingLevel = 1;
            CommandLineParameters.Port = 6410;
            CommandLineParameters.ServerName = ProgramText.ProgramTitle;
            CommandLineParameters.MaxSessions = 100;
            CommandLineParameters.SessionInactivityLimit = 10;
            CommandLineParameters.DisableChromeDriver = false;
            CommandLineParameters.DisableFirefoxDriver = false;
            CommandLineParameters.AuthPassword = string.Empty;
            

            var p = new OptionSet
            {

                { "h|help",  "Displays the help menu and exit",
                  v => CommandLineParameters.Help = v != null },

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

                { "server-name=", "The port to run the Web Service on",
                  v => { if (v != null) CommandLineParameters.ServerName = v; } },

                { "max-sessions=", "The maximum amount of sessions that can be created",
                  v => { if (v != null) CommandLineParameters.MaxSessions = Convert.ToInt32(v); } },

                { "session-inactivity-limit=", "The amount of minutes that a session is allowed to be inactive",
                  v => { if (v != null) CommandLineParameters.SessionInactivityLimit = Convert.ToInt32(v); } },

                { "disable-chrome-driver",  "Disables the ability to start Chrome Drivers",
                  v => CommandLineParameters.DisableChromeDriver = v != null },

                { "disable-firefox-driver",  "Disables the ability to start Firefox Drivers",
                  v => CommandLineParameters.DisableFirefoxDriver = v != null },

                { "auth-password=", "Authentication Password for Web Service access",
                  v => { if (v != null) CommandLineParameters.AuthPassword = v; } },

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

            if(CommandLineParameters.DriverLoggingLevel < 0)
            {
                Console.WriteLine(ProgramText.DriverLoggingLevelInvalidOption);
                Environment.Exit(1);
            }

            if(CommandLineParameters.DriverLoggingLevel > 3)
            {
                Console.WriteLine(ProgramText.DriverLoggingLevelInvalidOption);
                Environment.Exit(1);
            }

            Console.Title = ProgramText.ProgramTitle;
            Console.CancelKeyPress += CommandCancelEventHandler;

            try
            {
                WebService.Start(CommandLineParameters.Port);
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

            WebService.Logging.CommandLineLoggingEnabled = !CommandLineParameters.DisabledStdout;
            WebService.Logging.FileLoggingEnabled = !CommandLineParameters.DisableFileLogging;
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
            WebService.Logging.WriteEntry(Netlenium.Logging.MessageType.Information, "Application", "Shutting down server");
            WebService.Stop();

            if (SessionManager.activeSessions != null)
            {
                WebService.Logging.WriteEntry(Netlenium.Logging.MessageType.Information, "Application", "Closing active sessions");
                
                while(true)
                {
                    var currentActiveSessions = new List<string>();
                    foreach (var session in SessionManager.activeSessions.Keys)
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
                                WebService.Logging.WriteEntry(Netlenium.Logging.MessageType.Warning, "Application", $"Cannot close session '{session}', {ex.Message}");
                            }
                        }

                        break;
                    }
                    catch(Exception)
                    {
                        WebService.Logging.WriteEntry(Netlenium.Logging.MessageType.Error, "Application", "SessionManager is busy, trying again in 2 seconds");
                        Thread.Sleep(2000);
                    }
                }
                
            }

            WebService.Logging.WriteEntry(Netlenium.Logging.MessageType.Information, "Application", "Closing process");
            Environment.Exit(0);
        }
    }
}

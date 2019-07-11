using Netlenium.Driver;
using System;
using System.Collections.Generic;

namespace NetleniumServer
{
    /// <summary>
    /// Manages driver sessions to mantain resources
    /// </summary>
    public static class SessionManager
    {
        /// <summary>
        /// The current list of active sessions
        /// </summary>
        public static Dictionary<string, Session> activeSessions;

        /// <summary>
        /// Generates a new Session ID
        /// </summary>
        /// <returns></returns>
        private static string GeneratedSessionId()
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var stringChars = new char[32];
            var random = new Random();

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            return new String(stringChars);
        }

        /// <summary>
        /// Creates a new Driver Session
        /// </summary>
        /// <param name="targetBrowser"></param>
        /// <returns></returns>
        public static Session CreateSession(Browser targetBrowser)
        {
            WebService.logging.WriteEntry(Netlenium.Logging.MessageType.Information, "SessionManager", $"Creating new session for '{targetBrowser.ToString()}'");

            var SessionObject = new Session(targetBrowser)
            {
                ID = GeneratedSessionId()
            };

            if (activeSessions == null)
            {
                activeSessions = new Dictionary<string, Session>();
            }

            WebService.logging.WriteEntry(Netlenium.Logging.MessageType.Information, "SessionManager", $"Session '{SessionObject.ID}' created, starting driver");
            SessionObject.Driver.Start();

            WebService.logging.WriteEntry(Netlenium.Logging.MessageType.Information, "SessionManager", $"Session ready");
            activeSessions.Add(SessionObject.ID, SessionObject);

            return SessionObject;
        }

        /// <summary>
        /// Determines if the given Session ID exists
        /// </summary>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        public static bool SessionExists(string sessionId)
        {
            if (activeSessions == null)
            {
                return false;
            }

            if (activeSessions.ContainsKey(sessionId))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Stops an existing session
        /// </summary>
        /// <param name="sessionID"></param>
        public static void StopSession(string sessionId)
        {
            WebService.logging.WriteEntry(Netlenium.Logging.MessageType.Information, "SessionManager", $"Closing session '{sessionId}'");

            if(SessionExists(sessionId) == false)
            {
                WebService.logging.WriteEntry(Netlenium.Logging.MessageType.Error, "SessionManager", $"Cannot close session '{sessionId}', it does not exist or it has already been closed");
                throw new SessionNotFoundException();
            }

            try
            {

                try
                {
                    activeSessions[sessionId].Driver.Stop();
                }
                catch(DriverNotRunningException)
                {
                    WebService.logging.WriteEntry(Netlenium.Logging.MessageType.Warning, "SessionManager", $"The driver for '{sessionId}' cannot be stopped because it is not running");
                }

                activeSessions.Remove(sessionId);
                WebService.logging.WriteEntry(Netlenium.Logging.MessageType.Information, "SessionManager", $"The session '{sessionId}' has been closed");
            }
            catch (Exception exception)
            {
                WebService.logging.WriteEntry(Netlenium.Logging.MessageType.Error, "SessionManager", $"Cannot close session '{sessionId}', {exception.Message}");
                throw;
            }
        }
    }
}

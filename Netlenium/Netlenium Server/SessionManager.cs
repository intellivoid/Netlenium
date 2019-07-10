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

    }
}

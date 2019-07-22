using System;
using System.Collections.Generic;
using Netlenium.Driver;
using Netlenium.Logging;

namespace Netlenium
{
    /// <summary>
    /// Manages driver sessions to mantain resources
    /// </summary>
    public static class SessionManager
    {
        /// <summary>
        /// The current list of active sessions
        /// </summary>
        public static Dictionary<string, Session> ActiveSessions;

        /// <summary>
        /// Current list of expired sessions, this list will be cleared over time
        /// </summary>
        public static IDictionary<string, DateTime> ExpiredSessions;

        /// <summary>
        /// Returns the total amount of sessions that are currently active
        /// </summary>
        public static int TotalActiveSessions
        {
            get
            {
                if(ActiveSessions == null)
                {
                    return 0;
                }

                return ActiveSessions.Count;
            }
        }

        /// <summary>
        /// Generates a new Session ID
        /// </summary>
        /// <returns></returns>
        private static string GeneratedSessionId()
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var stringChars = new char[32];
            var random = new Random();

            for (var i = 0; i < stringChars.Length; i++)
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
            WebService.Logging.WriteEntry(MessageType.Information, "SessionManager", $"Creating new session for '{targetBrowser.ToString()}'");

            var sessionObject = new Session(targetBrowser)
            {
                Id = GeneratedSessionId()
            };

            if (ActiveSessions == null)
            {
                ActiveSessions = new Dictionary<string, Session>();
            }

            WebService.Logging.WriteEntry(MessageType.Information, "SessionManager", $"Session '{sessionObject.Id}' created, starting driver");
            sessionObject.Driver.Start();

            WebService.Logging.WriteEntry(MessageType.Information, "SessionManager", "Session ready");
            ActiveSessions.Add(sessionObject.Id, sessionObject);

            return sessionObject;
        }

        /// <summary>
        /// Determines if the given Session ID exists
        /// </summary>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        public static bool SessionExists(string sessionId)
        {
            if (ActiveSessions == null)
            {
                return false;
            }

            if (ActiveSessions.ContainsKey(sessionId))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Determines if the given session has expired
        /// </summary>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        public static bool SessionExpired(string sessionId)
        {
            if (ExpiredSessions == null)
            {
                return false;
            }

            if (ExpiredSessions.ContainsKey(sessionId))
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
            WebService.Logging.WriteEntry(MessageType.Information, "SessionManager", $"Closing session '{sessionId}'");

            if(SessionExists(sessionId) == false)
            {
                WebService.Logging.WriteEntry(MessageType.Error, "SessionManager", $"Cannot close session '{sessionId}', it does not exist or it has already been closed");
                throw new SessionNotFoundException();
            }

            try
            {

                try
                {
                    ActiveSessions[sessionId].Driver.Stop();
                }
                catch(DriverNotRunningException)
                {
                    WebService.Logging.WriteEntry(MessageType.Warning, "SessionManager", $"The driver for '{sessionId}' cannot be stopped because it is not running");
                }

                ActiveSessions.Remove(sessionId);
                WebService.Logging.WriteEntry(MessageType.Information, "SessionManager", $"The session '{sessionId}' has been closed");
            }
            catch (Exception exception)
            {
                WebService.Logging.WriteEntry(MessageType.Error, "SessionManager", $"Cannot close session '{sessionId}', {exception.Message}");
                throw;
            }
        }

        /// <summary>
        /// Syncs the current active sessions and manages the expired ones
        /// </summary>
        public static void Sync()
        {
            var currentTime = DateTime.Now;

            if (ActiveSessions != null)
            {
                foreach (var activeSession in ActiveSessions)
                {
                    var sessionId = activeSession.Key;
                    var totalInactivity = Convert.ToInt32((currentTime - activeSession.Value.LastActivity).TotalMinutes);
                    if (totalInactivity > CommandLineParameters.SessionInactivityLimit)
                    {
                        WebService.Logging.WriteEntry(MessageType.Warning, "SessionManager", $"The session '{sessionId}' has expired due to {totalInactivity} minute(s) of inactivity");
                        StopSession(sessionId);
                        if(ExpiredSessions == null)
                        {
                            ExpiredSessions = new Dictionary<string, DateTime>();
                        }
                        ExpiredSessions.Add(sessionId, DateTime.Now);
                    }
                }
            }

            if (ExpiredSessions != null)
            {
                foreach (var expiredSession in ExpiredSessions)
                {
                    var sessionId = expiredSession.Key;
                    if (Convert.ToInt32((currentTime - expiredSession.Value).TotalMinutes) > 10)
                    {
                        ExpiredSessions.Remove(sessionId);
                    }
                }
            }
            
        }
        
    }
}

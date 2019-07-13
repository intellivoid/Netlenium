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
        /// Current list of expired sessions, this list will be cleared over time
        /// </summary>
        public static IDictionary<string, DateTime> expiredSessions;

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
            SessionObject.Driver.Headless = false;
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
        /// Determines if the given session has expired
        /// </summary>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        public static bool SessionExpired(string sessionId)
        {
            if (expiredSessions == null)
            {
                return false;
            }

            if (expiredSessions.ContainsKey(sessionId))
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

        /// <summary>
        /// Syncs the current active sessions and manages the expired ones
        /// </summary>
        public static void Sync()
        {
            var current_time = DateTime.Now;

            if (activeSessions != null)
            {
                foreach (KeyValuePair<string, Session> active_session in activeSessions)
                {
                    var session_id = active_session.Key;
                    var total_inactivity = Convert.ToInt32((current_time - active_session.Value.LastActivity).TotalMinutes);
                    if (total_inactivity > CommandLineParameters.SessionInactivityLimit)
                    {
                        WebService.logging.WriteEntry(Netlenium.Logging.MessageType.Warning, "SessionManager", $"The session '{session_id}' has expired due to {total_inactivity} minute(s) of inactivity");
                        StopSession(session_id);
                        if(expiredSessions == null)
                        {
                            expiredSessions = new Dictionary<string, DateTime>();
                        }
                        expiredSessions.Add(session_id, DateTime.Now);
                    }
                }
            }

            if (expiredSessions != null)
            {
                foreach (KeyValuePair<string, DateTime> expired_session in expiredSessions)
                {
                    var session_id = expired_session.Key;
                    if (Convert.ToInt32((current_time - expired_session.Value).TotalMinutes) > 10)
                    {
                        expiredSessions.Remove(session_id);
                    }
                }
            }
            
        }
        
    }
}

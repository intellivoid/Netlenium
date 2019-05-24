namespace Netlenium.Driver.WebDriver.Remote
{
    /// <summary>
    /// Provides a mechanism for maintaining a session for a test
    /// </summary>
    public class SessionId
    {
        private string sessionOpaqueKey;

        /// <summary>
        /// Initializes a new instance of the <see cref="SessionId"/> class
        /// </summary>
        /// <param name="opaqueKey">Key for the session in use</param>
        public SessionId(string opaqueKey)
        {
            sessionOpaqueKey = opaqueKey;
        }

        /// <summary>
        /// Get the value of the key
        /// </summary>
        /// <returns>The key in use</returns>
        public override string ToString()
        {
            return sessionOpaqueKey;
        }

        /// <summary>
        /// Get the hash code of the key
        /// </summary>
        /// <returns>The hash code of the key</returns>
        public override int GetHashCode()
        {
            return sessionOpaqueKey.GetHashCode();
        }

        /// <summary>
        /// Compares two Sessions
        /// </summary>
        /// <param name="obj">Session to compare</param>
        /// <returns>True if they are equal or False if they are not</returns>
        public override bool Equals(object obj)
        {
            var objectsAreEqual = false;
            var other = obj as SessionId;
            if (other != null)
            {
                objectsAreEqual = sessionOpaqueKey.Equals(other.sessionOpaqueKey);
            }

            return objectsAreEqual;
        }
    }
}

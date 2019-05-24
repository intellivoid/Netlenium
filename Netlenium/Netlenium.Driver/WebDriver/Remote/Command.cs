using System.Collections.Generic;
using Newtonsoft.Json;

namespace Netlenium.Driver.WebDriver.Remote
{
    /// <summary>
    /// Provides a way to send commands to the remote server
    /// </summary>
    public class Command
    {
        private SessionId commandSessionId;
        private string commandName;
        private Dictionary<string, object> commandParameters = new Dictionary<string, object>();

        /// <summary>
        /// Initializes a new instance of the <see cref="Command"/> class using a command name and a JSON-encoded string for the parameters.
        /// </summary>
        /// <param name="name">Name of the command</param>
        /// <param name="jsonParameters">Parameters for the command as a JSON-encoded string.</param>
        public Command(string name, string jsonParameters)
            : this(null, name, ConvertParametersFromJson(jsonParameters))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Command"/> class for a Session
        /// </summary>
        /// <param name="sessionId">Session ID the driver is using</param>
        /// <param name="name">Name of the command</param>
        /// <param name="parameters">Parameters for that command</param>
        public Command(SessionId sessionId, string name, Dictionary<string, object> parameters)
        {
            commandSessionId = sessionId;
            if (parameters != null)
            {
                commandParameters = parameters;
            }

            commandName = name;
        }

        /// <summary>
        /// Gets the SessionID of the command
        /// </summary>
        [JsonProperty("sessionId")]
        public SessionId SessionId
        {
            get { return commandSessionId; }
        }

        /// <summary>
        /// Gets the command name
        /// </summary>
        [JsonProperty("name")]
        public string Name
        {
            get { return commandName; }
        }

        /// <summary>
        /// Gets the parameters of the command
        /// </summary>
        [JsonProperty("parameters")]
        public Dictionary<string, object> Parameters
        {
            get { return commandParameters; }
        }

        /// <summary>
        /// Gets the parameters of the command as a JSON-encoded string.
        /// </summary>
        public string ParametersAsJsonString
        {
            get
            {
                var parametersString = string.Empty;
                if (commandParameters != null && commandParameters.Count > 0)
                {
                    parametersString = JsonConvert.SerializeObject(commandParameters);
                }

                if (string.IsNullOrEmpty(parametersString))
                {
                    parametersString = "{}";
                }

                return parametersString;
            }
        }

        /// <summary>
        /// Returns a string of the Command object
        /// </summary>
        /// <returns>A string representation of the Command Object</returns>
        public override string ToString()
        {
            return string.Concat("[", SessionId, "]: ", Name, " ", Parameters.ToString());
        }

        /// <summary>
        /// Gets the command parameters as a <see cref="Dictionary{K, V}"/>, with a string key, and an object value.
        /// </summary>
        /// <param name="value">The JSON-encoded string representing the command parameters.</param>
        /// <returns>A <see cref="Dictionary{K, V}"/> with a string keys, and an object value. </returns>
        private static Dictionary<string, object> ConvertParametersFromJson(string value)
        {
            var parameters = JsonConvert.DeserializeObject<Dictionary<string, object>>(value, new ResponseValueJsonConverter());
            return parameters;
        }
    }
}

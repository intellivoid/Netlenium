using System;
using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json;

namespace Netlenium.Driver.WebDriver.Remote
{
    /// <summary>
    /// Handles reponses from the browser
    /// </summary>
    public class Response
    {
        private object responseValue;
        private string responseSessionId;
        private WebDriverResult responseStatus;
        private bool isSpecificationCompliant;

        /// <summary>
        /// Initializes a new instance of the <see cref="Response"/> class
        /// </summary>
        public Response()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Response"/> class
        /// </summary>
        /// <param name="sessionId">Session ID in use</param>
        public Response(SessionId sessionId)
        {
            if (sessionId != null)
            {
                responseSessionId = sessionId.ToString();
            }
        }

        private Response(Dictionary<string, object> rawResponse)
        {
            if (rawResponse.ContainsKey("sessionId"))
            {
                if (rawResponse["sessionId"] != null)
                {
                    responseSessionId = rawResponse["sessionId"].ToString();
                }
            }

            if (rawResponse.ContainsKey("value"))
            {
                responseValue = rawResponse["value"];
            }

            if (rawResponse.ContainsKey("status"))
            {
                responseStatus = (WebDriverResult)Convert.ToInt32(rawResponse["status"], CultureInfo.InvariantCulture);
            }
            else
            {
                // If the response does *not* have a "status" property, it
                // is compliant with the specification, which does not put
                // status in its responses.
                isSpecificationCompliant = true;

                // If the returned object does *not* have a "value" property
                // the response value should be the entirety of the response.
                // TODO: Remove this if statement altogether; there should
                // never be a spec-compliant response that does not contain a
                // value property.
                if (!rawResponse.ContainsKey("value") && responseValue == null)
                {
                    // Special-case for the new session command, where the "capabilities"
                    // property of the response is the actual value we're interested in.
                    if (rawResponse.ContainsKey("capabilities"))
                    {
                        responseValue = rawResponse["capabilities"];
                    }
                    else
                    {
                        responseValue = rawResponse;
                    }
                }

                var valueDictionary = responseValue as Dictionary<string, object>;
                if (valueDictionary != null)
                {
                    // Special case code for the new session command. If the response contains
                    // sessionId and capabilities properties, fix up the session ID and value members.
                    if (valueDictionary.ContainsKey("sessionId"))
                    {
                        responseSessionId = valueDictionary["sessionId"].ToString();
                        if (valueDictionary.ContainsKey("capabilities"))
                        {
                            responseValue = valueDictionary["capabilities"];
                        }
                        else
                        {
                            responseValue = valueDictionary["value"];
                        }
                    }
                    else if (valueDictionary.ContainsKey("error"))
                    {
                        responseStatus = WebDriverError.ResultFromError(valueDictionary["error"].ToString());
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the value from JSON.
        /// </summary>
        public object Value
        {
            get { return responseValue; }
            set { responseValue = value; }
        }

        /// <summary>
        /// Gets or sets the session ID.
        /// </summary>
        public string SessionId
        {
            get { return responseSessionId; }
            set { responseSessionId = value; }
        }

        /// <summary>
        /// Gets or sets the status value of the response.
        /// </summary>
        public WebDriverResult Status
        {
            get { return responseStatus; }
            set { responseStatus = value; }
        }

        /// <summary>
        /// Gets a value indicating whether this response is compliant with the WebDriver specification.
        /// </summary>
        public bool IsSpecificationCompliant
        {
            get { return isSpecificationCompliant; }
        }

        /// <summary>
        /// Returns a new <see cref="Response"/> from a JSON-encoded string.
        /// </summary>
        /// <param name="value">The JSON string to deserialize into a <see cref="Response"/>.</param>
        /// <returns>A <see cref="Response"/> object described by the JSON string.</returns>
        public static Response FromJson(string value)
        {
            var deserializedResponse = JsonConvert.DeserializeObject<Dictionary<string, object>>(value, new ResponseValueJsonConverter());
            var response = new Response(deserializedResponse);
            return response;
        }

        /// <summary>
        /// Returns this object as a JSON-encoded string.
        /// </summary>
        /// <returns>A JSON-encoded string representing this <see cref="Response"/> object.</returns>
        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        /// <summary>
        /// Returns the object as a string.
        /// </summary>
        /// <returns>A string with the Session ID, status value, and the value from JSON.</returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "({0} {1}: {2})", SessionId, Status, Value);
        }
    }
}

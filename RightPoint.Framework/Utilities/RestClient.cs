using System;
using System.IO;
using System.Net;

namespace RightPoint.Framework.Utilities
{
    public class RestClient
    {
        #region Enumerations and Declarations
        public enum httpVerb
        {
            GET,
            POST,
            PUT,
            DELETE
        }
        #endregion

        #region Web request Calls
        public string endPointURL { get; set; }
        public httpVerb httpMethod { get; set; }

        public RestClient(string endPointURL, httpVerb httpMethod)
        {
            this.endPointURL = endPointURL;
            this.httpMethod = httpMethod;
        }

        public string Get()
        {
            string responseValue = string.Empty;
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(endPointURL);
            request.Method = httpMethod.ToString();
            // request.Timeout = 1000000;

            // Get Response from the httpWebRequest object
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw new ApplicationException("error coe: " + response.StatusCode.ToString());
                }

                // Process the JSON response stream
                using (Stream responseStream = response.GetResponseStream())
                {
                    if (responseStream != null)
                    {
                        using (StreamReader reader = new StreamReader(responseStream))
                        {
                            responseValue = reader.ReadToEnd();
                        }
                    }
                }
            }

            return responseValue;

        }

        #endregion

    }

}

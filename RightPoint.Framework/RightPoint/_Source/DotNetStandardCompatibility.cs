using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Text;


#if DOTNET_STANDARD
namespace System.Web
{
    public class HttpContext
    {
        public static HttpContext Current = null;

        public System.Collections.IDictionary Items = null;


        public HttpRequest Request = null;

        public HttpServer Server;
    }

    public class HttpServer
    {
        public string MapPath(string path)
        {
            return path;
        }
    } 


    public class HttpRequest
    {
        public Dictionary<string, string> Headers = new Dictionary<string, string>();
        public String HttpMethod;

        public UrlReferrer UrlReferrer;
        public String UserHostName;
        public String UserHostAddress;
        public String RawUrl;
        public NameValueCollection Form = new NameValueCollection();
        public bool IsSecureConnection = false;
        public NameValueCollection QueryString = new NameValueCollection();
        public NameValueCollection ServerVariables = new NameValueCollection();
        public Uri Url;
        public Stream InputStream = new MemoryStream();
    }

    public class UrlReferrer
    {
        public String OriginalString;
    }


    public class HttpException : Exception
    {
    }

}



#endif


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Collections.Specialized;

namespace GrepolisBot2
{
    class HttpHandler : WebClient
    {
        private CookieContainer m_CookieContainer = new CookieContainer();

        protected override WebRequest GetWebRequest(Uri address)
        {
            Settings l_Settings = Settings.Instance;
            WebRequest request = base.GetWebRequest(address);
            if (request is HttpWebRequest)
            {
                (request as HttpWebRequest).CookieContainer = m_CookieContainer;
                //The underlying connection was closed: A connection that was expected to be kept alive was closed by the server.
                (request as HttpWebRequest).KeepAlive = false;
                (request as HttpWebRequest).UserAgent = l_Settings.AdvUserAgent;
            }

            //The underlying connection was closed: The connection was closed unexpectedly.
            //request.ConnectionGroupName = Guid.NewGuid().ToString();
            return request;
        }

        public CookieContainer CookieContainer
        {
            get { return m_CookieContainer; }
            set { m_CookieContainer = value; }
        }

        public void clearCookies()
        {
            m_CookieContainer = new CookieContainer();
        }
    }
}
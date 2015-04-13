using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;

namespace GrepolisBot2
{
    static class CookieHelpers
    {
        [DllImport("wininet.dll", SetLastError = true)]
        public static extern bool InternetGetCookieEx(string url, string cookieName, StringBuilder cookieData, ref int size, Int32  dwFlags, IntPtr  lpReserved);
        private const Int32 InternetCookieHttponly = 0x2000;
        [DllImport("wininet.dll", SetLastError = true)]
        private static extern bool InternetSetOption(IntPtr hInternet, int dwOption, IntPtr lpBuffer, int lpdwBufferLength);
        private const int INTERNET_OPTION_END_BROWSER_SESSION = 42;

       public static CookieContainer GetUriCookieContainer(Uri uri)
       {
            CookieContainer l_Cookies = null;
            // Determine the size of the cookie
            int l_Datasize = 8192 * 16;
            StringBuilder l_CookieData = new StringBuilder(l_Datasize);

            if (!InternetGetCookieEx(uri.ToString(), null, l_CookieData, ref l_Datasize, InternetCookieHttponly, IntPtr.Zero))
            {
                if (l_Datasize < 0)
                    return null;

                // Allocate stringbuilder large enough to hold the cookie
                l_CookieData = new StringBuilder(l_Datasize);

                if (!InternetGetCookieEx(uri.ToString(), null, l_CookieData, ref l_Datasize, InternetCookieHttponly, IntPtr.Zero))
                    return null;
            }

            if (l_CookieData.Length > 0)
            {
                l_Cookies = new CookieContainer();
                l_Cookies.SetCookies(uri, l_CookieData.ToString().Replace(';', ','));
            }
            return l_Cookies;
       }

        public static void ClearCookie()
        {
            InternetSetOption(IntPtr.Zero, INTERNET_OPTION_END_BROWSER_SESSION, IntPtr.Zero, 0); 
        }
    }
}
/* Connection Planet - SRP6a Implementation
 * Copyright (C) 2013  MusicDemon
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 *
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using System.Numerics;

namespace CPlan.SRP.Web
{
    public class Global : System.Web.HttpApplication
    {
        public static List<User> users;
        public static List<UserSession> sessions;
        public static BigInteger N;
        public static BigInteger g;
        public static BigInteger k;

        void Application_Start(object sender, EventArgs e)
        {
            // Code that runs on application startup
            users = new List<User>();
            sessions = new List<UserSession>();
            N = Client.Constants.N1024Bit;
            g = Client.Constants.g1024Bit;
            k = Client.Functional.Calck(g, N);
            Client.Functional.algorithm = System.Security.Cryptography.SHA256.Create();
        }

        public enum bis { N = 0, g = 1, k = 2 }

        public static string Hex(bis n)
        {
            BigInteger x;
            switch (n)
            {
                case bis.N:
                    x = N;
                    break;
                case bis.g:
                    x = g;
                    break;
                case bis.k:
                    x = k;
                    break;
                default:
                    x = 0;
                    break;
            }
            return BitConverter.ToString(x.ToByteArray()).Replace("-", "");
        }

        void Application_End(object sender, EventArgs e)
        {
            //  Code that runs on application shutdown

        }

        void Application_Error(object sender, EventArgs e)
        {
            // Code that runs when an unhandled error occurs

        }

        void Session_Start(object sender, EventArgs e)
        {
            // Code that runs when a new session is started

        }

        void Session_End(object sender, EventArgs e)
        {
            // Code that runs when a session ends. 
            // Note: The Session_End event is raised only when the sessionstate mode
            // is set to InProc in the Web.config file. If session mode is set to StateServer 
            // or SQLServer, the event is not raised.

        }

    }
}

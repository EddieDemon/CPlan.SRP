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
using System.Web.Services;
using System.Numerics;
using CPlan.SRP.Web.Models;

namespace CPlan.SRP.Web
{
    /// <summary>
    /// An athentication service using the SRP6a handshake.
    /// </summary>
    [WebService(Namespace = "http://connectionplanet.nl/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    [System.Web.Script.Services.ScriptService]
    public class Authentication : System.Web.Services.WebService
    {
        [WebMethod(EnableSession = true, MessageName = "SRP/AddAccount")]
        public AuthenticationModel AddUser(string user, string s, string v)
        {
            if (s.Length % 2 == 0)
            {
                UserSession u = new UserSession() { UserName = user, s = s.StringToByteArray(), v = new BigInteger(v.StringToByteArray()) };
                if (Global.users.Exists(usr => u.UserName.Equals(usr.UserName, StringComparison.InvariantCultureIgnoreCase)))
                    return new AuthenticationModel
                    {
                        error = 1
                    };
                Global.users.Add(u);
                return new AuthenticationModel
                {
                    error = 0,
                };
            }
            return new AuthenticationModel
            {
                error = 2
            };
        }

        /// <summary>
        /// Initializes the handshake getting the user name and the public client value.
        /// </summary>
        /// <param name="userName">The user name.</param>
        /// <param name="A">Public client value.</param>
        /// <returns>Returns B.</returns>
        [WebMethod(EnableSession = true, MessageName = "SRP/AuthStep1")]
        public AuthenticationModel Step1(string user, string A)
        {
            if (Global.users.Exists(usr => user.Equals(usr.UserName, StringComparison.InvariantCultureIgnoreCase)))
            {
                User Usr = Global.users.Find(usr => user.Equals(usr.UserName, StringComparison.InvariantCultureIgnoreCase));
                UserSession Usrs = (UserSession)Usr;
                Usrs.SessionId(DateTime.Now.Ticks.ToString());
                Usrs.CalculateEverything(A);
                Global.sessions.Add(Usrs);
                return new AuthenticationModel
                {
                    error = 0,
                    data = new
                    {
                        uniq1 = Usrs.Id,
                        s = BitConverter.ToString(Usr.s).Replace("-", ""),
                        B = BitConverter.ToString(Usrs.B.ToByteArray()).Replace("-", ""),
                        u = BitConverter.ToString(Usrs.u.ToByteArray()).Replace("-", "")
                    }
                };
            }
            return new AuthenticationModel { error = 1 };
        }

        [WebMethod(EnableSession = true, MessageName = "SRP/AuthStep2")]
        public AuthenticationModel Step2(string user, string uniq1, string m1)
        {
            foreach (var usr in Global.sessions)
            {
                var a = m1.StringToByteArray().Compare(usr.M4Net);
                var b = m1.StringToByteArray();
                var c = "";
            }
            IEnumerable<UserSession> Usrs = from usr in Global.sessions
                                            where usr.UserName.Equals(user, StringComparison.InvariantCultureIgnoreCase) &&
                                            usr.Id.Equals(uniq1) /*&&
                                            m1.StringToByteArray().Compare(usr.M)*/
                                            select usr;
            if (Usrs.Count() > 0)
            {
                UserSession Usr = Usrs.First();
                Usr.SessionId(DateTime.Now.Ticks.ToString());
                var x = new AuthenticationModel
                {
                    error = 0,
                    data = new
                    {
                        uniq2 = Usr.Id,
                        m2 = BitConverter.ToString(Usr.M2).Replace("-", "")
                    }
                };
                return x;
            }
            return new AuthenticationModel
            {
                error = 1,
            };
        }
    }
}

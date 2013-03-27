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

using C = CPlan.SRP.Client.Functional;
using H = CPlan.SRP.Host.Functional;

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
        public bool AddUser(string user, string s, string v)
        {
            if (s.Length % 2 == 0)
            {
                User u = new User() { UserName = user, s = StringToByteArray(s), v = new BigInteger(StringToByteArray(v)) };
                if (Global.users.Exists(usr => u.UserName.Equals(usr.UserName, StringComparison.InvariantCultureIgnoreCase))) return false;
                Global.users.Add(u);
            }
            return false;
        }

        /// <summary>
        /// Initializes the handshake getting the user name and the public client value.
        /// </summary>
        /// <param name="userName">The user name.</param>
        /// <param name="A">Public client value.</param>
        /// <returns>Returns B.</returns>
        [WebMethod(EnableSession = true, MessageName = "SRP/AuthStep1")]
        public string Step1(string user, string A)
        {
            if (Global.users.Exists(usr => user.Equals(usr.UserName, StringComparison.InvariantCultureIgnoreCase)))
            {
                User Usr = Global.users.Find(usr => user.Equals(usr.UserName, StringComparison.InvariantCultureIgnoreCase));
                string name = user;
                BigInteger _A = new BigInteger(StringToByteArray(A));
                BigInteger b = H.Getb();
                BigInteger B = H.CalcB(Global.k, Usr.v, b, Global.g, Global.N);
                BigInteger u = C.Calcu(_A, B, Global.N);
                BigInteger S = H.CalcS(_A, Usr.v, u, b, Global.N);
                byte[] K = C.CalcK(S);
                byte[] M = C.M(Usr.UserName, Usr.s, _A, B, K, Global.g, Global.N);
            }
            return "No.";
        }

        public byte[] StringToByteArray(string hex)
        {
            int NumberChars = hex.Length;
            byte[] bytes = new byte[NumberChars / 2];
            for (int i = 0; i < NumberChars; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            return bytes;
        }
    }
}

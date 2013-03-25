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
using System.Threading;
using System.Net;
using System.Text;
using System.Linq;
using System.Net.Sockets;
using System.Numerics;
using H = CPlan.SRP.Host.Functional;
using C = CPlan.SRP.Client.Functional;

namespace CPlan.UI
{
    class TCP_Test
    {
        public static byte[] salt;
        public static BigInteger v;

        public TCP_Test()
        {
            Host h = new Host();
            Client c = new Client("MusicDemon", "123Test123");
            while (h.run || c.run)
            {
                // Suspend the console from continueing until both the server and the client are done.
            }
        }
    }
}

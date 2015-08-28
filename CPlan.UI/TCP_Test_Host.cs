/* Connection Planet - SRP6a Implementation
 * Copyright (C) 2013  MusicDemon (http://www.connectionplanet.nl)
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
    class Host
    {
        private TcpListener l;
        private TcpClient c;
        public volatile bool run;

        static BigInteger N = SRP.Client.Constants.N1024Bit; // We will be using a default 1024-bit prime.
        static BigInteger g = SRP.Client.Constants.g1024Bit;

        static BigInteger v = TCP_Test.v;
        static byte[] salt = TCP_Test.salt;

        string userName = "";
        static BigInteger k = C.Calck6a(g, N);
        static BigInteger b = H.Getb();
        BigInteger A = 0;
        BigInteger B = H.CalcB(k, TCP_Test.v, b, g, N);

        BigInteger S;
        byte[] K = new byte[] { };
        byte[] M = new byte[] { };
        byte[] M2 = new byte[] { };

        public Host()
        {
            l = new TcpListener(new IPEndPoint(IPAddress.Loopback, 4567));
            l.Start();
            run = true;
            ThreadPool.QueueUserWorkItem(CheckClient);
        }

        public void CheckClient(object state)
        {
            while (run)
                if (l.Pending() && (c == null || !c.Connected))
                {
                    c = l.AcceptTcpClient();
                    ThreadPool.QueueUserWorkItem(CheckData);
                }
        }

        public void CheckData(object state)
        {
            int stage = 0; // Used to check which packet is coming in.


            while (run)
            {
                if ((c != null && c.Connected) && c.Available > 0)
                {
                    byte[] data = new byte[c.Available];
                    c.Client.Receive(data);
                    switch (stage)
                    {
                        case 0: // Receive username and A.
                            // We have fixed this.
                            userName = Encoding.UTF8.GetString(data.Take(10).ToArray()); // Receive user name.
                            A = new BigInteger(data.Skip(10).ToArray()); // NOTE: A is not checked for "A % N = 0"!!!
                            c.Client.Send(salt.Concat(B.ToByteArray()).ToArray()); // Send salt and B.
                            S = H.CalcS(A, v, C.Calcu(A, B, N), b, N); // Calculate S.
                            K = C.CalcK(S); // Calculate K.
                            M = C.M(userName, salt, A, B, K, g, N); // Caculate M.
                            stage++;
                            break;
                        case 1: // Receive and calculate M and M2.
                            if (C.CompareArrays(data, M))
                            {
                                // Client M and Server M are equal, 
                                c.Client.Send(C.M2(A, M, K)); // Send calculated M2.
                                Console.WriteLine("From Server: Status OK. Logged in!");
                                stage++;
                            }
                            else
                            {
                                c.Close();
                                c = null;
                                run = false;
                            }
                            break;
                        case 2: // We want to know if the M2 is equal on the clients side. So, we'll wait for another empty packet.
                            Console.WriteLine("From Server: Exiting server...");
                            run = false;
                            break;
                        default:
                            c.Close();
                            c = null;
                            break;
                    }
                }
            }
        }
    }
}

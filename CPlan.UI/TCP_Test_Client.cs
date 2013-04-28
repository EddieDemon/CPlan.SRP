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
using CPlan.SRP.Client;
using C = CPlan.SRP.Client.Functional;

namespace CPlan.UI
{
    class TCPClient
    {
        public string UserName { get; private set; }
        public string Password { get; private set; }
        TcpClient c;
        public volatile bool run;

        BigInteger N = Constants.N1024Bit; // We will be using a default 1024-bit prime.
        BigInteger g = Constants.g1024Bit;

        Carrier carrier = new Carrier("MusicDemon", "123Test123", KeySizes._2048);

        public TCPClient(string userName, string password)
        {
            UserName = userName;
            Password = password;
            c = new TcpClient(IPAddress.Loopback.ToString(), 4567);
            run = true;
            ThreadPool.QueueUserWorkItem(CheckData);
        }

        public void CheckData(object state)
        {
            int stage = 0; // Used to check which packet is coming in.

            if (c.Connected)
            {
                // Send user name and A when connected to the server.
                c.Client.Send(Encoding.UTF8.GetBytes("MusicDemon").Concat(carrier.A.ToByteArray()).ToArray());

                while (run)
                {
                    if ((c != null && c.Connected) && c.Available > 0)
                    {
                        byte[] data = new byte[c.Available];
                        c.Client.Receive(data);

                        switch (stage)
                        {
                            case 0:
                                byte[] salt = data.Take(16).ToArray(); // Extract the salt.
                                BigInteger B = new BigInteger(data.Skip(16).ToArray()); // Extract the B. 
                                carrier.CalculateEverthing(salt, B);
                                c.Client.Send(carrier.M); // Send M.
                                stage++;
                                break;
                            case 1:
                                if (C.CompareArrays(data, carrier.M2))
                                {
                                    // Yea!! M2 is equal to the server M2. 
                                    c.Client.Send(new byte[] { 0 });
                                    Console.WriteLine("From Client: Status OK. Logged in!");
                                    Console.WriteLine("From Client: Exiting client...");
                                    run = false;
                                }
                                else
                                {
                                    c.Close();
                                    c = null;
                                    run = false;
                                }
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
}

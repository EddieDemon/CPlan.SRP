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
using System.Numerics;
using H = CPlan.SRP.Host.Functional;
using C = CPlan.SRP.Client.Functional;

namespace CPlan.UI
{
    class Program
    {
        static void Main(string[] args)
        {
            string userName = "MusicDemon"; // The used user name during the test.
            string password = "123Test123?"; // The used user password during the test.
            byte[] salt = C.GetSalt(); // The used user salt during the test.

            BigInteger N = CPlan.SRP.Client.Constants.N1024Bit; // We will be using a default 1024-bit prime.
            BigInteger g = CPlan.SRP.Client.Constants.g1024Bit;
            BigInteger verifier = C.GetVerifier(salt, userName, password, g, N); // Calculate v for the host.

            // Client side public calculations.
            var a = C.Geta();
            var A = C.CalcA(a, g, N);
            var k = C.Calck(g, N);

            // Host side public calculations.
            var b = H.Getb();
            var B = H.CalcB(k, verifier, b, g, N);

            // Check A and B.
            if ((A % N) == 0) { Console.WriteLine("(A % N) == 0"); return; }
            if ((B % N) == 0) { Console.WriteLine("(B % N) == 0"); return; }

            // Client side internal calculations.
            var u = C.Calcu(A, B, N);
            var x = C.Calcx(salt, userName, password);
            var cS = C.CalcS(a, B, k, x, u, g, N);
            var cK = C.CalcK(cS);
            var cM = C.M(N, g, userName, salt, A, B, cK);
            var cM2 = C.M2(A, cM, cK);

            // Host side internal calculations.            
            var sS = H.CalcS(A, verifier, u, b, N);
            var sK = C.CalcK(sS);
            var sM = C.M(N, g, userName, salt, A, B, sK);
            var sM2 = C.M2(A, cM, sK);

            // Check if any of these are equal.
            bool M = cM.Equals(sM);
            bool M2 = cM2.Equals(sM2);
        }
    }
}

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
 
using System.Numerics;
using System.Security.Cryptography;

namespace CPlan.SRP.Host
{
    /// <summary>
    /// Represents a set of functions for the SRP protocol.
    /// </summary>
    public static class Functional
    {
        /// <summary>
        /// Get randomly generated b.
        /// </summary>
        /// <returns></returns>
        public static BigInteger Getb()
        {
            RandomNumberGenerator r = RandomNumberGenerator.Create();
            // TODO: Probably a and b must have the same length; Verify this.
            byte[] b = new byte[256 / 8]; // http://tools.ietf.org/html/rfc5054 Paragraph: 2.5.4.
            r.GetBytes(b);
            BigInteger _b = new BigInteger(b);
            return _b.Sign != 1 ? BigInteger.Negate(_b) : _b; // Check if a is negative, if so, change it to positve.
        }
        /// <summary>
        /// Calculates the B value.
        /// </summary>
        /// <param name="k">SRP-6a multiplier</param>
        /// <param name="v">The verifier.</param>
        /// <param name="b">Server private value.</param>
        /// <param name="g">The generator.</param>
        /// <param name="N">The large prime.</param>
        /// <returns>k*v + g^b % N</returns>
        public static BigInteger CalcB(BigInteger k, BigInteger v, BigInteger b, BigInteger g, BigInteger N)
        {
            BigInteger B = BigInteger.Add(BigInteger.Multiply(k, v), BigInteger.ModPow(g, b, N));
            if (B.Sign != 1) B = BigInteger.Negate(B);
            return B;
        }
        /// <summary>
        /// Calculates the secret.
        /// </summary>
        /// <param name="A">Client public value.</param>
        /// <param name="v">User verifier.</param>
        /// <param name="u">The u value.</param>
        /// <param name="b">Server private value.</param>
        /// <param name="N">The large prime.</param>
        /// <returns>(A * v^u) ^ b % N</returns>
        public static BigInteger CalcS(BigInteger A, BigInteger v, BigInteger u, BigInteger b, BigInteger N)
        {
            BigInteger S = BigInteger.ModPow((BigInteger.Multiply(A, BigInteger.ModPow(v, u, N))), b, N);
            return S;
        }
    }
}

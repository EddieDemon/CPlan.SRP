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
using System.Collections.Generic;
using System.Numerics;
using F = CPlan.SRP.Client.Functional;

namespace CPlan.SRP.Host
{
    /// <summary>
    /// Represents a simple carrier for the functions.
    /// </summary>
    public class Carrier
    {
        #region Fields
        /// <summary>
        /// Gets or sets the private server value.
        /// </summary>
        private BigInteger b;
        #endregion
        #region Properties
        /// <summary>
        /// Gets the current user name.
        /// </summary>
        public string UserName { get; private set; }
        /// <summary>
        /// Gets the current password.
        /// </summary>
        public string Password { get; private set; }
        /// <summary>
        /// Gets the current N.
        /// </summary>
        public BigInteger N { get; private set; }
        /// <summary>
        /// Gets the current generator.
        /// </summary>
        public BigInteger g { get; private set; }
        /// <summary>
        /// Gets the current public server value.
        /// </summary>
        public BigInteger B { get; private set; }
        /// <summary>
        /// Gets the current salt used with the algorithm.
        /// </summary>
        public byte[] salt { get; private set; }
        /// <summary>
        /// Gets the current password verifier used with the algorithm.
        /// </summary>
        public BigInteger v { get; private set; }
        /// <summary>
        /// Gets the calculated secret.
        /// </summary>
        public BigInteger S { get; private set; }
        /// <summary>
        /// Gets the calculated session key.
        /// </summary>
        public byte[] K { get; private set; }
        /// <summary>
        /// Gets the calculated client proof.
        /// </summary>
        public byte[] M { get; private set; }
        /// <summary>
        /// Gets the calculated server proof.
        /// </summary>
        public byte[] M2 { get; private set; }
        #endregion
        #region .ctor
        /// <summary>
        /// Initializes a new instance of the <see cref="Carrier"/> object with a user name and the public client value.
        /// </summary>
        /// <param name="userName">The user name.</param>
        /// <param name="A">The public client value.</param>
        public Carrier(string userName, BigInteger A, byte[] salt, BigInteger v) { this.v = v; this.salt = salt; b = Functional.Getb(); }
        #endregion
        /// <summary>
        /// Caluclates <see cref="S"/>, <see cref="K"/>, <see cref="M"/> and <see cref="M2"/>.
        /// </summary>
        /// <param name="A">The public client value.</param>
        public void CalculateEverthing(BigInteger A)
        {
            if ((A % N) == BigInteger.Zero) { throw new ArgumentException("A modulo N (A % N) equals 0, this is in invalid value.", "A"); }

            while (B == BigInteger.Zero || ((B % N) == BigInteger.Zero))
            {
                b = Functional.Getb();
                B = Functional.CalcB(F.Calck(g, N), v, b, g, N);
            }
            BigInteger u = F.Calcu(A, B, N);
            S = Functional.CalcS(A, v, u, b, N);
            K = F.CalcK(S);
            M = F.M(UserName, salt, A, B, K, g, N);
            M2 = F.M2(A, M, K);
        }
    }
}

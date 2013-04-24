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

namespace CPlan.SRP.Client
{
    /// <summary>
    /// Represents a simple carrier for the functions.
    /// </summary>
    public class Carrier
    {
        #region Fields
        /// <summary>
        /// Gets or sets the private client value.
        /// </summary>
        private BigInteger a;
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
        /// Gets the current public client value.
        /// </summary>
        public BigInteger A { get; private set; }
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
        /// Initializes a new instance of the <see cref="Carrier"/> object with a user name and password.
        /// </summary>
        /// <param name="userName">The user name.</param>
        /// <param name="password">The password.</param>
        /// <param name="size">The bit-size for the large prime.</param>
        public Carrier(string userName, string password, KeySizes size)
        {
            UserName = userName;
            Password = password;
            BigInteger _N, _g;
            this.UserName = userName;
            Constants.GetNandg(size, out _N, out _g);
            N = _N; g = _g;
            while (A == BigInteger.Zero || ((A % N) == BigInteger.Zero))
            {
                a = Functional.Geta();
                A = Functional.CalcA(a, g, N);
            }
        }
        #endregion
        /// <summary>
        /// Caluclates <see cref="S"/>, <see cref="K"/>, <see cref="M"/> and <see cref="M2"/>.
        /// </summary>
        /// <param name="salt">The salt.</param>
        /// <param name="B">The public server value.</param>
        public void CalculateEverthing(byte[] salt, BigInteger B)
        {
            if ((B % N) == BigInteger.Zero) { throw new ArgumentException("B modulo N (B % N) equals 0, this is in invalid value.", "B"); }
            S = F.CalcS(a, B, F.Calck(g, N), F.Calcx(salt, UserName, Password), F.Calcu(A, B, N), g, N); // Calculate S.
            K = F.CalcK(S); // Calculate K.
            M = F.M(Password, salt, A, B, K, g, N); // Calculate M.
            M2 = F.M2(A, M, K); // Calculate M2.
        }
    }
}

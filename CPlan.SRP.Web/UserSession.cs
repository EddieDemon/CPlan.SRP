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

using C = CPlan.SRP.Client.Functional;
using H = CPlan.SRP.Host.Functional;

namespace CPlan.SRP.Web
{
    /// <summary>
    /// Represents a storage for an SRP authentication. Can also be expanded for higher goals.
    /// </summary>
    public class UserSession : User
    {
        /// <summary>
        /// Gets the u.
        /// </summary>
        public BigInteger u { get; private set; }
        /// <summary>
        /// Gets the server's public value.
        /// </summary>
        public BigInteger B { get; private set; }
        /// <summary>
        /// Gets the key validation 1.
        /// </summary>
        public byte[] M { get; private set; }
        /// <summary>
        /// Gets the server proof of S.
        /// </summary>
        public byte[] M2 { get; private set; }
        /// <summary>
        /// Gets the session Id.
        /// </summary>
        public string Id { get; private set; }
        public UserSession() { }
        public UserSession(string _A) { CalculateEverything(_A); }
        public void CalculateEverything(string _A)
        {
            BigInteger A = new BigInteger(_A.StringToByteArray());
            BigInteger b = H.Getb();
            B = H.CalcB(Global.k, v, b, Global.g, Global.N);
            u = C.Calcu(A, B, Global.N);
            BigInteger S = H.CalcS(A, v, u, b, Global.N);
            byte[] K = C.CalcK(S);
            M = C.M(UserName, s, A, B, K, Global.g, Global.N);
            M2 = C.M2(A, M, K);
        }
        /// <summary>
        /// Check whether or not there's a known session ID. If not the session ID will be saved.
        /// </summary>
        /// <param name="id">The session ID to save.</param>
        /// <exception cref="InvalidOperationException">Thrown when a session ID is already set.</exception>
        public void SessionId(string id)
        {
            if (Id == null || Id.Equals(string.Empty))
                Id = id;
            else
                throw new InvalidOperationException("This user is already logged in.");
        }
    }
}
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
using System.Linq;
using System.Text;
using System.Numerics;
using System.Security.Cryptography;

namespace CPlan.SRP.Client
{
    public static class Functional
    {
        private static SHA1 sha = SHA1.Create();

        // N and g can be different per authentication. Unsafe to make it static on this level.
        ///// <summary>
        ///// The prime N. Default 1024 bits.
        ///// </summary>
        //public static BigInteger N = Constants.N1024Bit;
        ///// <summary>
        ///// The generator.
        ///// </summary>
        //public static BigInteger g = Constants.g1024Bit;
        /// <summary>
        /// Get randomly generated a.
        /// </summary>
        /// <returns></returns>
        public static BigInteger Geta()
        {
            RandomNumberGenerator r = RNGCryptoServiceProvider.Create();
            byte[] a = new byte[256 / 8]; // http://www.ietf.org/rfc/rfc5054.txt Paragraph: 2.5.4.
            r.GetBytes(a);
            BigInteger _a = new BigInteger(a);
            return _a.Sign != 1 ? BigInteger.Negate(_a) : _a; // Check if a is negative, if so, change it to positve.
        }
        /// <summary>
        /// Calculate A from a.
        /// </summary>
        /// <param name="a">Random 256 bits integer.</param>
        /// <param name="g">The generator.</param>
        /// <param name="N">The large prime.</param>
        /// <returns>g ^ a % N</returns>
        /// <exception cref="ArgumentOutOfRangeException">Return value will be (A % N) == 0, get a new <paramref name="a"/>.</exception>
        public static BigInteger CalcA(BigInteger a, BigInteger g, BigInteger N)
        {
            BigInteger A = BigInteger.ModPow(g, a, N);
            if ((A % N) == 0) throw new ArgumentOutOfRangeException("a", "Return value will be (A % N) == 0, get a new a.");
            return A;
        }
        /// <summary>
        /// Calculates the SRP-6 multiplier.
        /// </summary>
        /// <param name="g">The generator.</param>
        /// <param name="N">The large prime.</param>
        /// <returns>SHA1(N | PAD(g))</returns>
        public static BigInteger Calck(BigInteger g, BigInteger N)
        {
            string _N = BitConverter.ToString(N.ToByteArray()).Replace("-", string.Empty);
            byte[] _g = Encoding.ASCII.GetBytes(BitConverter.ToString(g.ToByteArray()).Replace("-", string.Empty).PadLeft(_N.Length, '0'));
            BigInteger k = new BigInteger(sha.ComputeHash(N.ToByteArray().Concat(_g).ToArray()));
            if (k.Sign != 1) k = BigInteger.Negate(k);
            return k;
        }
        /// <summary>
        /// Calculates u using client-side A and server-side B.
        /// </summary>
        /// <param name="A">Client public value.</param>
        /// <param name="B">Server public value.</param>
        /// <param name="N">The large prime.</param>
        /// <returns>SHA1(PAD(A) | PAD(B))</returns>
        public static BigInteger Calcu(BigInteger A, BigInteger B, BigInteger N)
        {
            string _N = BitConverter.ToString(N.ToByteArray()).Replace("-", string.Empty);
            byte[] _A = Encoding.ASCII.GetBytes(BitConverter.ToString(A.ToByteArray()).Replace("-", string.Empty).PadLeft(_N.Length, '0'));
            byte[] _B = Encoding.ASCII.GetBytes(BitConverter.ToString(B.ToByteArray()).Replace("-", string.Empty).PadLeft(_N.Length, '0'));
            BigInteger u = new BigInteger(sha.ComputeHash(_A.Concat(_B).ToArray()));
            if (u.Sign != 1) u = BigInteger.Negate(u);
            return u;
        }
        /// <summary>
        /// Calculates x.
        /// </summary>
        /// <param name="salt">The salt.</param>
        /// <param name="userName">The user name.</param>
        /// <param name="password">The user password.</param>
        /// <returns>SHA1(s | SHA1(I | ":" | P))</returns>
        public static BigInteger Calcx(byte[] salt, string userName, string password)
        {
            BigInteger x = new BigInteger(sha.ComputeHash(salt.Concat(sha.ComputeHash(Encoding.UTF8.GetBytes(userName + "|" + password))).ToArray()));
            if (x.Sign != 1) x = BigInteger.Negate(x);
            return x;
        }
        /// <summary>
        /// Calculates the secret.
        /// </summary>
        /// <param name="a">Client private value.</param>
        /// <param name="B">Server public value.</param>
        /// <param name="k">SRP-6 multiplier.</param>
        /// <param name="x">The x value.</param>
        /// <param name="u">The u value.</param>
        /// <param name="g">The generator.</param>
        /// <param name="N">The large prime.</param>
        /// <returns>(B - (k * g^x)) ^ (a + (u * x)) % N</returns>
        public static BigInteger CalcS(BigInteger a, BigInteger B, BigInteger k, BigInteger x, BigInteger u, BigInteger g, BigInteger N)
        {
            BigInteger S = BigInteger.ModPow((BigInteger.Min(B, (BigInteger.Multiply(k, BigInteger.ModPow(g, x, N))))), (BigInteger.Add(a, (BigInteger.Multiply(u, x)))), N);
            return S;
        }
        /// <summary>
        /// Calculates K.
        /// </summary>
        /// <param name="S">The secret.</param>
        /// <returns>H265(S)</returns>
        /// <remarks>Done differently than described in RFC2945.</remarks>
        public static byte[] CalcK(BigInteger S)
        {
            SHA256 sha256 = SHA256.Create();
            return sha256.ComputeHash(S.ToByteArray());
        }
        /// <summary>
        /// Calculates the key proof.
        /// </summary>      
        /// <param name="userName"></param>
        /// <param name="salt"></param>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <param name="K"></param>
        /// <param name="g"></param>
        /// <param name="N"></param>
        /// <returns></returns>
        public static byte[] M(string userName, byte[] salt, BigInteger A, BigInteger B, byte[] K, BigInteger g, BigInteger N)
        {
            byte[] Ng = XorArrays(sha.ComputeHash(N.ToByteArray()), sha.ComputeHash(g.ToByteArray()));
            byte[] M = sha.ComputeHash(Ng.Concat(sha.ComputeHash(Encoding.UTF8.GetBytes(userName)).Concat(salt).Concat(A.ToByteArray()).Concat(B.ToByteArray()).Concat(K).ToArray()).ToArray());
            return M;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="A"></param>
        /// <param name="M"></param>
        /// <param name="K"></param>
        /// <returns></returns>
        public static byte[] M2(BigInteger A, byte[] M, byte[] K)
        {
            byte[] M2 = sha.ComputeHash(A.ToByteArray().Concat(M.Concat(K)).ToArray());
            return M2;
        }

        #region Indirectly needed
        /// <summary>
        /// Get randomly generated salt.
        /// </summary>
        /// <returns></returns>
        public static byte[] GetSalt()
        {
            byte[] saltBytes = new byte[16];
            RandomNumberGenerator rng = RandomNumberGenerator.Create();
            rng.GetBytes(saltBytes);
            return saltBytes;
        }
        /// <summary>
        /// Calculates the verfier.
        /// </summary>
        /// <param name="salt">The salt.</param>
        /// <param name="userName">The user name.</param>
        /// <param name="password">The user password.</param>
        /// <param name="g">The generator.</param>
        /// <param name="N">The large prime.</param>
        /// <returns>x = SHA(&lt;salt> | SHA(&lt;username> | ":" | &lt;raw password>))
        /// &lt;password verifier> = v = g^x % N</returns>
        public static BigInteger GetVerifier(byte[] salt, string userName, string password, BigInteger g, BigInteger N)
        {
            BigInteger x = new BigInteger(sha.ComputeHash(salt.Concat(sha.ComputeHash(Encoding.UTF8.GetBytes(userName + "|" + password))).ToArray()));
            if (x.Sign != 1) x = BigInteger.Negate(x);
            BigInteger v = BigInteger.ModPow(g, x, N);
            if (v.Sign != 1) v = BigInteger.Negate(v);
            return BigInteger.ModPow(g, x, N);
        }

        /// <summary>
        /// XOR two byte arrays together and returns result.  Both arrays must be same length and neither can be null.
        /// Resulting array will be same size as array1.
        /// </summary>
        /// <param name="array1"></param>
        /// <param name="array2"></param>
        /// <returns>byte[] which is the XOR result of input arrays.</returns>
        private static byte[] XorArrays(byte[] array1, byte[] array2)
        {
            if (array1 == null)
                throw new ArgumentNullException("array1");
            if (array2 == null)
                throw new ArgumentNullException("array2");
            if (array1.Length == 0)
                throw new ArgumentOutOfRangeException("array1 can not be zero length.");
            if (array1.Length != array2.Length)
                throw new ArgumentOutOfRangeException("array1.Length != array2.Length");

            byte[] newArray = new byte[array1.Length];
            for (int i = 0; i < array1.Length; i++)
            {
                newArray[i] = (byte)(array1[i] ^ array2[i]);
            }
            return newArray;
        }

        #endregion
    }
}

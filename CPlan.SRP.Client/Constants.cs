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

namespace CPlan.SRP.Client
{
    // Snippets used from http://channel9.msdn.com/forums/sandbox/secure-remote-password-srp-in-wse.

    public static class Constants
    {
        // Various N's sizes in Base64. These are the supported bit sizes.  We use on 1024 bit for now, but any of the following can be used.
        // Both client and server must agree on same N and g.  As N and g are needed to generate Verifier, N and g are "fixed" after verifier creation
        // on the server (i.e. the client can not just pick a size of N).
        private const string prime1024Bit = "7q8Kua2zjdacM/gK+o/F6GByYYd1/zwLnqIxTJwlZXbWdN90luqB0zg7SBPWksbg4NXY4lC5i+SOSVwdYIna0V3H17RhVNa2zo70rWmxXUmCVZspe88YhcUp9WZmDlfsaO28PAVybMAv1Mv0l26qmv1ROP6DdkNbn8YdL8DrBuM=";
        private const string prime1536Bit = "ne88r7k5J3qx8SqGF6R7u9ulHfSZrEyAvu6pYUsZzE1fT19VbifL3lHGqUvkYHopFViQO6DQ+EOAtlW7miLo3N8CinzsZ/DQgTSxyLl5iRSbYJ4L47q2PUdUg4HbxbH8dk4/S1PdnaEVi/0+K5yM9W7fAZU5NJYn2y/VPSS3xIZldy5DfWx/jORCc0r3zLeug3wmSuOpvrh/ii/puLUpLloCH/9ekUeejOeijCRCxvMVGA+TSZojTc924/7RNfm7";
        private const string prime2048Bit = "rGvbQTJKmpvxZt5eE4lYL69ytmUZh+4H/DGSlD21YFCjcynLtKCZ7YGT4HV3Z6E91SMSq0sDMQ3Nf0ip2gT9UOgIOWntt2ewz2CVF5oWOrNmGgX71fqq6CkYqZYvC5O4Vfl5k+yXXuqoDXQK2/T/dHNZ0EHVwz6nHSgeRGsUdzvKl7Q6I/uAFna9IHpDbGSB8dK5B4cXRhpbnTLmiPh3SFRFI7UksNV9Xqd6J3XS7PoDLPvb9S+zeGFgJ5AE5Xrmr4dOcwPOUymczAQce8MI2CpWmPOo0MOCca41+Onb+7aUtcgD2J965DXeI21SX1R1m2XjcvzWjvIPpxEfnkr/cw==";
        private const string prime3072Bit = "///////////JD9qiIWjCNMTGYouA3BzRKQJOCIpnzHQCC76mOxObIlFKCHmONATd75UZs806QxswKwpt8l8UN0/hNW1tUcJF5IW1dmJefsb0TELppjftawv/XLb0Brft7jhr+1qJn6WunyQRfEsf5kkoZlHs5Fs9wgB8uKFjvwWY2kg2HFXTmmkWP6j9JM9fg2VdI9yjrZYcYvNWIIVSu57VKQdwlpZtZww1Tkq8mATxdGwIyhghfDKQXkYuNs474553LBgOhgObJ4Oi7Aeij7XFXfBvTFLJ3ivL9pVYFxg5lUl86pVq5RXSJhiY+gUQFXKOWoqqxC2tMxcNBFB6M6hVIavfHLpk7PuFBFjb7wqK6nFXXQYMfbOXD4Wm4eTHq/WujNsJM9cejJTgSiVhnc7j0iYa0u5r8S/6BtmKCGTYdgJzPshqZFIfKxgXeyAMu+EXV3phXWx3CYjAutlG4gjiT6B05asxQ9tb/OD9EI5LgtEgqTrSyv//////////";
        private const string prime4096Bit = "///////////JD9qiIWjCNMTGYouA3BzRKQJOCIpnzHQCC76mOxObIlFKCHmONATd75UZs806QxswKwpt8l8UN0/hNW1tUcJF5IW1dmJefsb0TELppjftawv/XLb0Brft7jhr+1qJn6WunyQRfEsf5kkoZlHs5Fs9wgB8uKFjvwWY2kg2HFXTmmkWP6j9JM9fg2VdI9yjrZYcYvNWIIVSu57VKQdwlpZtZww1Tkq8mATxdGwIyhghfDKQXkYuNs474553LBgOhgObJ4Oi7Aeij7XFXfBvTFLJ3ivL9pVYFxg5lUl86pVq5RXSJhiY+gUQFXKOWoqqxC2tMxcNBFB6M6hVIavfHLpk7PuFBFjb7wqK6nFXXQYMfbOXD4Wm4eTHq/WujNsJM9cejJTgSiVhnc7j0iYa0u5r8S/6BtmKCGTYdgJzPshqZFIfKxgXeyAMu+EXV3phXWx3CYjAutlG4gjiT6B05asxQ9tb/OD9EI5LgtEgqSEIARpyPBKnh+bXiHGaEL26WyaZwycYavTiPBqUaDS2FQvaJYPpyirUTOjbu8LbBN6O+S6O/BQfvsqmKHxZR05rwF2ZspZPoJDDoiM7oYZRW+ftH2EpcM7i16+4G912IXBIHNAGkSc=";
        private const string prime6144Bit = "///////////JD9qiIWjCNMTGYouA3BzRKQJOCIpnzHQCC76mOxObIlFKCHmONATd75UZs806QxswKwpt8l8UN0/hNW1tUcJF5IW1dmJefsb0TELppjftawv/XLb0Brft7jhr+1qJn6WunyQRfEsf5kkoZlHs5Fs9wgB8uKFjvwWY2kg2HFXTmmkWP6j9JM9fg2VdI9yjrZYcYvNWIIVSu57VKQdwlpZtZww1Tkq8mATxdGwIyhghfDKQXkYuNs474553LBgOhgObJ4Oi7Aeij7XFXfBvTFLJ3ivL9pVYFxg5lUl86pVq5RXSJhiY+gUQFXKOWoqqxC2tMxcNBFB6M6hVIavfHLpk7PuFBFjb7wqK6nFXXQYMfbOXD4Wm4eTHq/WujNsJM9cejJTgSiVhnc7j0iYa0u5r8S/6BtmKCGTYdgJzPshqZFIfKxgXeyAMu+EXV3phXWx3CYjAutlG4gjiT6B05asxQ9tb/OD9EI5LgtEgqSEIARpyPBKnh+bXiHGaEL26WyaZwycYavTiPBqUaDS2FQvaJYPpyirUTOjbu8LbBN6O+S6O/BQfvsqmKHxZR05rwF2ZspZPoJDDoiM7oYZRW+ftH2EpcM7i16+4G912IXBIHNAGkSfVsFqpk7TqmI2P3cGG/7fckKbAj030Nck0AoSSNsP6tNJ8cCbB1NyyYCZG3sl1HnY9uje9+P+UBq2eUw7l2zgvQTABrrBqU+2QJ9gxF5cnsIZaiRjaPtvrz5sU7UTObLrO1Lsb238UR+bMJUszIFFRK9evQm+49AE3jNK/WYPKAcZLkuzwMuoV0XIdA/SC185udP721V5wL0aYDIK1qEAxkAscnlnnyX++x+jzI6l6fjbMiL4PHUW3/1haxUvUB7IrQVSqzI9tfr9I4dgUzF7SD4A34KeXFe7ym+MoBqHVi7fF2nb1UKo9ih+/8OsZzLGjE9Vc2lbJ7C7yljI4f+jXbjwEaAQ+j2Y/SGDuEr8tWwt0dNbmlPkebcxAJP//////////";
        private const string prime8192Bit = "///////////JD9qiIWjCNMTGYouA3BzRKQJOCIpnzHQCC76mOxObIlFKCHmONATd75UZs806QxswKwpt8l8UN0/hNW1tUcJF5IW1dmJefsb0TELppjftawv/XLb0Brft7jhr+1qJn6WunyQRfEsf5kkoZlHs5Fs9wgB8uKFjvwWY2kg2HFXTmmkWP6j9JM9fg2VdI9yjrZYcYvNWIIVSu57VKQdwlpZtZww1Tkq8mATxdGwIyhghfDKQXkYuNs474553LBgOhgObJ4Oi7Aeij7XFXfBvTFLJ3ivL9pVYFxg5lUl86pVq5RXSJhiY+gUQFXKOWoqqxC2tMxcNBFB6M6hVIavfHLpk7PuFBFjb7wqK6nFXXQYMfbOXD4Wm4eTHq/WujNsJM9cejJTgSiVhnc7j0iYa0u5r8S/6BtmKCGTYdgJzPshqZFIfKxgXeyAMu+EXV3phXWx3CYjAutlG4gjiT6B05asxQ9tb/OD9EI5LgtEgqSEIARpyPBKnh+bXiHGaEL26WyaZwycYavTiPBqUaDS2FQvaJYPpyirUTOjbu8LbBN6O+S6O/BQfvsqmKHxZR05rwF2ZspZPoJDDoiM7oYZRW+ftH2EpcM7i16+4G912IXBIHNAGkSfVsFqpk7TqmI2P3cGG/7fckKbAj030Nck0AoSSNsP6tNJ8cCbB1NyyYCZG3sl1HnY9uje9+P+UBq2eUw7l2zgvQTABrrBqU+2QJ9gxF5cnsIZaiRjaPtvrz5sU7UTObLrO1Lsb238UR+bMJUszIFFRK9evQm+49AE3jNK/WYPKAcZLkuzwMuoV0XIdA/SC185udP721V5wL0aYDIK1qEAxkAscnlnnyX++x+jzI6l6fjbMiL4PHUW3/1haxUvUB7IrQVSqzI9tfr9I4dgUzF7SD4A34KeXFe7ym+MoBqHVi7fF2nb1UKo9ih+/8OsZzLGjE9Vc2lbJ7C7yljI4f+jXbjwEaAQ+j2Y/SGDuEr8tWwt0dNbmlPkebb4RWXSjkm8S/uXkOHd8tqky34zYvsTQc7kxujvIMraNndMAdB+nv4r8R+0ldvaTa6QkZjqrY5xa5PVoNCO0dCvxyXgjjxbL451lLeP9uL78hIrZIiIuBKQDfAcT61eoGiPwxzRz/GRs6jBrS8vIhi+Dhd36nUt/osCH6HloMwPtW906Bis89bOieKZtKhP4P0T4Ld8xDuB0q2o2RZfomaAlXcFk8xzFCEaFHfmrSBld7X6hsdUQvX7nTXP682vDHs+iaDWQRvTrh5+SQAlDi0gcbNeImgAu1e44K8kZDab8Am5HlVjkR1Z36aqeMFDidlaU38gfVuiAuW5xYMmA3Zjt09///////////w==";

        /// <summary>
        /// A large safe 1024 bit prime. All SRP arithmetic is done modulo N.
        /// See: http://www.ietf.org/internet-drafts/draft-ietf-tls-srp-09.txt
        /// </summary>
        public static readonly BigInteger N1024Bit = new BigInteger(Convert.FromBase64String(prime1024Bit));
        /// <summary>
        /// Generator modulo N for 1024 bit N.
        /// </summary>
        public static readonly BigInteger g1024Bit = new BigInteger(2);

        /// <summary>
        /// A large safe 1536 bit prime. All SRP arithmetic is done modulo N.
        /// </summary>
        public static readonly BigInteger N1536Bit = new BigInteger(Convert.FromBase64String(prime1536Bit));
        /// <summary>
        /// Generator modulo N for 1536 bit N.
        /// </summary>
        public static readonly BigInteger g1536Bit = new BigInteger(2);

        /// <summary>
        /// A large safe 2048 bit prime. All SRP arithmetic is done modulo N.
        /// </summary>
        public static readonly BigInteger N2048Bit = new BigInteger(Convert.FromBase64String(prime2048Bit));
        /// <summary>
        /// Generator modulo N for 2048 bit N.
        /// </summary>
        public static readonly BigInteger g2048Bit = new BigInteger(2);

        /// <summary>
        /// A large safe 3072 bit prime. All SRP arithmetic is done modulo N.
        /// </summary>
        public static readonly BigInteger N3072Bit = new BigInteger(Convert.FromBase64String(prime3072Bit));
        /// <summary>
        /// Generator modulo N for 3072 bit N.
        /// </summary>
        public static readonly BigInteger g3072Bit = new BigInteger(5);

        /// <summary>
        /// A large safe 4096 bit prime. All SRP arithmetic is done modulo N.
        /// </summary>
        public static readonly BigInteger N4096Bit = new BigInteger(Convert.FromBase64String(prime4096Bit));
        /// <summary>
        /// Generator modulo N for 4096 bit N.
        /// </summary>
        public static readonly BigInteger g4096Bit = new BigInteger(5);

        /// <summary>
        /// A large safe 6144 bit prime. All SRP arithmetic is done modulo N.
        /// </summary>
        public static readonly BigInteger N6144Bit = new BigInteger(Convert.FromBase64String(prime6144Bit));
        /// <summary>
        /// Generator modulo N for 6144 bit N.
        /// </summary>
        public static readonly BigInteger g6144Bit = new BigInteger(5);

        /// <summary>
        /// A large safe 8192 bit prime. All SRP arithmetic is done modulo N.
        /// </summary>
        public static readonly BigInteger N8192Bit = new BigInteger(Convert.FromBase64String(prime8192Bit));
        /// <summary>
        /// Generator modulo N for 8192 bit N.
        /// </summary>
        public static readonly BigInteger g8192Bit = new BigInteger(19);
    }
}

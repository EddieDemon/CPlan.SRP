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
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CPlan.SRP.Web
{
    /// <summary>
    /// Represents a set of globally used functions.
    /// </summary>
    public static class Utils
    {
        /// <summary>
        /// Converts a hexadecimal-string to a byte-array.
        /// </summary>
        /// <param name="hex">A hexadecimal string.</param>
        /// <returns>A byte-array representation of the hexadecimal-string.</returns>
        public static byte[] StringToByteArray(this string hex)
        {
            int NumberChars = hex.Length;
            byte[] bytes = new byte[(int)Math.Ceiling((decimal)(NumberChars / 2))];
            for (int i = 0; i < NumberChars; i += 2)
                if (hex.Substring(i).Length == 0) { }
                else if (hex.Substring(i).Length == 1)
                    bytes[i / 2] = Convert.ToByte(hex.Substring(i, 1), 16);
                else
                    bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            return bytes;
        }
        /// <summary>
        /// Compares two byte arrays.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool Compare(this byte[] left, byte[] right)
        {
            if (left == null || right == null || left.Length != right.Length) return false;
            for (int i = 0; i < left.Length; i++) if (left[i] != right[i]) return false;
            return true;
        }
    }
}
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

using System.Numerics;

namespace CPlan.SRP.Web
{
    /// <summary>
    /// Represents a user with salt and verifier.
    /// </summary>
    public class User
    {
        /// <summary>
        /// The user name.
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// The verifier of the user.
        /// </summary>
        public BigInteger v { get; set; }
        /// <summary>
        /// The salt of the user.
        /// </summary>
        public byte[] s { get; set; }
    }
}
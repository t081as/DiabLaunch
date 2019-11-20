// DiabLaunch - Diablo 2 full screen launcher
// Copyright (C) 2018-2019  Tobias Koch
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.

using System;
using System.IO;
using System.Runtime.Serialization;

namespace DiabLaunch
{
    /// <summary>
    /// Represents the application configuration.
    /// </summary>
    [DataContract]
    public class AppConfig
    {
        /// <summary>
        /// Gets or sets a value indicating whether the screen shall be streched.
        /// </summary>
        [DataMember(Name = "stretchScreen")]
        public bool StretchScreen { get; set; } = true;

        /// <summary>
        /// Writes the given <paramref name="appConfig"/> to the given <paramref name="stream"/>.
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> that shall be written to.</param>
        /// <param name="appConfig">An <see cref="AppConfig"/> that shall be written.</param>
        /// <exception cref="IOException">Error while writing the configuration.</exception>
        public static void Write(Stream stream, AppConfig appConfig)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Reads an <see cref="AppConfig"/> from a given <see cref="Stream"/>.
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> that shall be read.</param>
        /// <returns>The <see cref="AppConfig"/> that has been read.</returns>
        /// <exception cref="IOException">Error while reading the configuration.</exception>
        public static AppConfig Read(Stream stream)
        {
            throw new NotImplementedException();
        }
    }
}

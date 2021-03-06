// DiabLaunch - Diablo 2 full screen launcher
// Copyright (C) 2018-2020  Tobias Koch
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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace DiabLaunch
{
    /// <summary>
    /// Contains the main entry point for the application.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// <param name="args">The command line parameters given to the application.</param>
        /// <returns>An <see cref="int"/> representing the application return code.</returns>
        [STAThread]
        public static int Main(string[] args)
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (Environment.OSVersion.Platform != PlatformID.Win32NT)
            {
                MessageBox.Show("DiabLaunch requires Microsoft Windows", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 1;
            }

            if (!Environment.Is64BitProcess)
            {
                MessageBox.Show("DiabLaunch requires a x64 processor", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 1;
            }

            AppConfig config = new AppConfig();

            try
            {
                using Stream stream = File.OpenRead(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty, "Configuration.json"));
                config = AppConfig.Read(stream);
            }
            catch
            {
                // Ignore errors and use default configuration
            }

            Diablo2 diabloGame;

            try
            {
                diabloGame = new Diablo2();
            }
            catch (FileNotFoundException)
            {
                MessageBox.Show("DiabLaunch is unable to detect the Diablo 2 directory.\nMake sure that Diablo 2 is installed correctly or copy this application into your Diablo 2 directory.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 1;
            }

            IntPtr gameWindowHandle = Diablo2.DetectRunningInstance();

            if (gameWindowHandle == IntPtr.Zero)
            {
                gameWindowHandle = diabloGame.Launch(PrepareCommandLineParameters(args, config.StretchScreen).ToArray());
            }

            ExternalWindow gameWindow = new ExternalWindow(gameWindowHandle);
            gameWindow.RemoveBorder();
            gameWindow.SetPosition(Screen.PrimaryScreen.Bounds.X, Screen.PrimaryScreen.Bounds.Y, Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);

            return 0;
        }

        /// <summary>
        /// Prepares the command line arguments given to the game by adding the necessary parameters
        /// if not present yet.
        /// </summary>
        /// <param name="args">The command line arguments given to this application.</param>
        /// <param name="stretchScreen"><c>True</c> if the game shall be strechted to the whole screen; otherwise <c>False</c>.</param>
        /// <returns>The processed command line parameters.</returns>
        private static IEnumerable<string> PrepareCommandLineParameters(string[] args, bool stretchScreen = true)
        {
            const string parameterWindow = "-w";
            const string parameterAspect = "-nofixaspect";

            var parameters = new List<string>(args);

            if (!parameters.Any(s => s.ToUpperInvariant() == parameterWindow.ToUpperInvariant()))
            {
                parameters.Add(parameterWindow);
            }

            if (!parameters.Any(s => s.ToUpperInvariant() == parameterAspect.ToUpperInvariant()) && stretchScreen)
            {
                parameters.Add(parameterAspect);
            }

            return parameters;
        }
    }
}
using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace OsuDevServerSwitcher
{
    internal static class Program
    {
        private static readonly string AppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        private static readonly string ProgramFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);

        private static readonly string[] CommonOsuPaths =
        {
            Path.Combine(AppData, "osu!"),
            Path.Combine(ProgramFiles, "osu!")
        };
        
        [STAThread]
        public static void Main()
        {
            var configPath = Path.Combine(AppData, Settings.ServerName);
            var configFile = Path.Combine(configPath, "osuPath.txt");
            
            string osuPath;

            if (!Directory.Exists(configPath))
                Directory.CreateDirectory(configPath);

            readconfig:
            if (!File.Exists(configFile))
            {
                osuPath = GetOsuPath();
                File.WriteAllText(configFile, osuPath);
            }
            else
            {
                osuPath = File.ReadAllText(configFile);

                if (!File.Exists(osuPath))
                {
                    File.Delete(osuPath);
                    goto readconfig;
                }
            }

            if (string.IsNullOrEmpty(osuPath))
                return;

            if (Process.GetProcessesByName("osu!.exe").Length != 0)
            {
                Console.WriteLine("osu! is running. Please, close osu! and rerun this file.");
                Console.ReadKey();
                return;
            }

            Process.Start(osuPath, "-devserver " + Settings.ServerUrl);
        }

        private static string GetOsuPath()
        {
            foreach (var path in CommonOsuPaths)
            {
                if (!Directory.Exists(path)) continue;
                
                var osu = Path.Combine(path, "osu!.exe");
                if (File.Exists(osu)) return osu;
            }

            var dialog = new OpenFileDialog
            {
                Filter = "osu!|osu!.exe"
            };
            
            Console.WriteLine("Please, select osu!.exe.");

            return dialog.ShowDialog() != DialogResult.OK ? string.Empty : dialog.FileName;
        } 
    }
}
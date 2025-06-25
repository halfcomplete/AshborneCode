using System;
using System.IO;
using System.Diagnostics;
using System.Threading.Tasks;

namespace AshborneTooling
{
    internal class InkWatcher
    {
        private static readonly string inklecatePath = "C:\\Tools\\inklecate\\inklecate.exe";
        private static readonly string inkScriptsRoot = "D:\\C# Projects\\AshborneCode\\AshborneGame\\_Core\\Data\\Scripts";

        public static async Task Main(string[] args)
        {
            Console.WriteLine("Ashborne Ink Watcher Started");
            Console.WriteLine($"Watching for changes in: {inkScriptsRoot}");

            using var watcher = new FileSystemWatcher(inkScriptsRoot, "*.ink")
            {
                IncludeSubdirectories = true,
                NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.CreationTime
            };

            watcher.Changed += OnInkFileChanged;
            watcher.Created += OnInkFileChanged;
            watcher.Renamed += OnInkFileChanged;
            watcher.EnableRaisingEvents = true;

            Console.WriteLine("Watching for .ink changes... Press [Enter] to stop.");
            await Task.Run(() => Console.ReadLine());
        }

        private static void OnInkFileChanged(object sender, FileSystemEventArgs e)
        {
            try
            {
                Console.WriteLine($"📝 Change detected: {e.Name}");
                CompileInkFile(e.FullPath);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to compile {e.Name}: {ex.Message}");
            }
        }

        private static void CompileInkFile(string inkFilePath)
        {
            string jsonOutputPath = Path.ChangeExtension(inkFilePath, ".json");

            ProcessStartInfo startInfo = new()
            {
                FileName = inklecatePath,
                Arguments = $"-o \"{jsonOutputPath}\" \"{inkFilePath}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(startInfo);
            process.WaitForExit();

            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();

            if (process.ExitCode == 0)
            {
                Console.WriteLine($"✅ Compiled: {Path.GetFileName(inkFilePath)}");
            }
            else
            {
                Console.WriteLine($"❌ Compilation error in {Path.GetFileName(inkFilePath)}:");
                Console.WriteLine(error);
            }
        }
    }
}

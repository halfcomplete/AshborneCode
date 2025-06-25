using System;
using System.IO;
using System.Diagnostics;
using System.Threading.Tasks;

namespace AshborneTooling
{
    internal class InkWatcher
    {
        private static readonly string inklecatePath = @"D:\Ink\inklecate.exe";
        private static readonly string inkScriptsRoot = @"D:\C# Projects\AshborneDesign\Narrative";
        private static readonly string outputRoot = @"D:\C# Projects\AshborneCode\AshborneGame\_Core\Data\Scripts";

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
                Console.WriteLine($"Change detected: {e.Name} at {DateTime.Now}");
                CompileInkFile(e.FullPath);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to compile {e.Name}: {ex.Message}");
            }
        }

        private static void CompileInkFile(string inkFilePath)
        {
            string fileName = Path.GetFileName(inkFilePath); // e.g. Act1_Scene1_intro.ink

            // Extract act and scene numbers using regex or split
            string[] parts = fileName.Split('_', StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length < 2 || !parts[0].StartsWith("Act") || !parts[1].StartsWith("Scene"))
            {
                Console.WriteLine($"Invalid file name format: {fileName}");
                return;
            }

            string actPart = parts[0];   // e.g. Act1
            string scenePart = parts[1]; // e.g. Scene1

            // Construct output folder path: Scripts\Act1\Scene1\
            string outputFolder = Path.Combine(outputRoot, actPart, scenePart);
            Directory.CreateDirectory(outputFolder); // Ensure the folder exists

            // Set output path inside that folder
            string outputFileName = Path.ChangeExtension(fileName, ".json");
            string jsonOutputPath = Path.Combine(outputFolder, outputFileName);

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

            // Force Visual Studio to refresh
            File.SetLastWriteTimeUtc(jsonOutputPath, DateTime.UtcNow);

            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();

            if (process.ExitCode == 0)
            {
                Console.WriteLine($"Compiled: {fileName} to {actPart}\\{scenePart}\\{outputFileName}");
            }
            else
            {
                Console.WriteLine($"Compilation error in {fileName}:");
                Console.WriteLine(error);
            }
        }

    }
}

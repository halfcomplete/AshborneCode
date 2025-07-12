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
        private static readonly string wasmRoot = @"D:\C# Projects\AshborneCode\AshborneWASM\wwwroot\Scripts";
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
            string[] parts = fileName.Split('_', StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length < 2 || !parts[0].StartsWith("Act") || !parts[1].StartsWith("Scene"))
            {
                Console.WriteLine($"Invalid file name format: {fileName}");
                return;
            }

            string actPart = parts[0];   // Act1
            string scenePart = parts[1]; // Scene1

            string outputFolder = Path.Combine(outputRoot, actPart, scenePart);
            string wasmFolder = Path.Combine(wasmRoot, actPart, scenePart);

            Directory.CreateDirectory(outputFolder);
            Directory.CreateDirectory(wasmFolder);

            string outputFileName = Path.ChangeExtension(fileName, ".json");
            string jsonOutputPath = Path.Combine(outputFolder, outputFileName);
            string jsonWasmPath = Path.Combine(wasmFolder, outputFileName);

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
                // Copy to WASM project as well
                File.Copy(jsonOutputPath, jsonWasmPath, overwrite: true);
                File.SetLastWriteTimeUtc(jsonOutputPath, DateTime.UtcNow);
                File.SetLastWriteTimeUtc(jsonWasmPath, DateTime.UtcNow);

                Console.WriteLine($"Compiled and copied: {fileName} → Game + WASM");
            }
            else
            {
                Console.WriteLine($"Compilation error in {fileName}:\n{error}");
            }
        }
    }
}

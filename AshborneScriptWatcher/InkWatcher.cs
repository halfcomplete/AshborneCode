using System;
using System.IO;
using System.Diagnostics;
using System.Threading.Tasks;
using AshborneGame._Core.Globals.Constants;

namespace AshborneTooling
{
    internal class InkWatcher
    {
        private static readonly string inklecatePath = @"D:\Ink\inklecate.exe";
        private static readonly string inkDialogueRoot = @"D:\C# Projects\AshborneDesign\Narrative";
        private static readonly string outputRoot = @"D:\C# Projects\AshborneCode\AshborneGame\_Core\Data\Dialogue";
        private static readonly string wasmRoot = @"D:\C# Projects\AshborneCode\AshborneWASM\wwwroot\Dialogue";

        public static async Task Main(string[] args)
        {
            Console.WriteLine("Ashborne Ink Watcher Started");
            Console.WriteLine($"Watching for changes in: {inkDialogueRoot}");

            Console.WriteLine("Registered State Keys:");
            foreach (var key in InkStateKeyRegistry.GetAllRegisteredLabelKeys())
            {
                Console.WriteLine($"- {key}");
            }
            foreach (var key in InkStateKeyRegistry.GetAllRegisteredFlagKeys())
            {
                Console.WriteLine($"- {key}");
            }
            foreach (var key in InkStateKeyRegistry.GetAllRegisteredCounterKeys())
            {
                Console.WriteLine($"- {key}");
            }

            using var watcher = new FileSystemWatcher(inkDialogueRoot, "*.ink")
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
                Console.WriteLine("============================================================================");
                Console.WriteLine($"Change detected: {e.Name} at {DateTime.Now}");
                var jsonPath = CompileInkFile(e.FullPath);
                var issues = InkDialogueValidator.ValidateSingleFile(jsonPath);
                Console.WriteLine();
                foreach (var issue in issues)
                {
                    Console.WriteLine($"[ERROR] Validation Issue in {e.Name}: {issue}");
                }
                if (issues.Count == 0)
                {
                    Console.WriteLine($"[SUCCESS] Compile and Validation complete for {e.Name}!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to compile and validate {e.Name}: {ex.Message}");
            }
        }

        private static string CompileInkFile(string inkFilePath)
        {
            string fileName = Path.GetFileName(inkFilePath); // e.g. Act1_Scene1_intro.ink
            string[] parts = fileName.Split('_', StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length < 2 || !parts[0].StartsWith("Act") || !parts[1].StartsWith("Scene"))
            {
                Console.WriteLine($"Invalid file name format: {fileName}");
                return "";
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
            }
            else
            {
                Console.WriteLine($"Compilation error in {fileName}:\n{error}");
                return "";
            }

            return jsonOutputPath;
        }
    }
}

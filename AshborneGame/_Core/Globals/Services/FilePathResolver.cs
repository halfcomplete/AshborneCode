using AshborneGame._Core.Globals.Services;
using AshborneGame._Core.Globals.Enums;
using System;

namespace AshborneGame._Core.Globals.Services
{
    public static class FilePathResolver
    {
        // Used in console/local builds only
        public static string DialogueRootPath { get; } =
            @"D:\C# Projects\AshborneCode\AshborneGame\_Core\Data\Dialogue";

        public static string FromDialogue(string scriptFilename)
        {
            if (string.IsNullOrWhiteSpace(scriptFilename))
                throw new ArgumentException("Dialogue filename cannot be null or empty.");

            // Strip .json extension if present
            string baseName = Path.GetFileNameWithoutExtension(scriptFilename);
            string[] parts = baseName.Split('_');

            if (parts.Length < 2)
                throw new ArgumentException("Dialogue filename must start with Act and Scene, e.g., Act1_Scene1_*");

            string actFolder = parts[0];   // "Act1"
            string sceneFolder = parts[1]; // "Scene1"
            string fullFileName = baseName + ".json";

            // --- Web Context (Blazor WebAssembly / GitHub Pages) ---
            if (OperatingSystem.IsBrowser())
            {
                // Use relative web path (not /absolute!)
                string webPath = $"Dialogue/{actFolder}/{sceneFolder}/{fullFileName}";
                IOService.Output.DisplayDebugMessage($"[WASM] Using web path: {webPath}", ConsoleMessageTypes.INFO);
                return webPath;
            }

            // --- Console Context (Local build/testing) ---
            string fullPath = Path.Combine(DialogueRootPath, actFolder, sceneFolder, fullFileName);

            if (!File.Exists(fullPath))
            {
                IOService.Output.DisplayDebugMessage($"[LOCAL] File not found at: {fullPath}", ConsoleMessageTypes.ERROR);
                throw new FileNotFoundException($"[LOCAL] Dialogue file missing: {fullPath}");
            }

            IOService.Output.DisplayDebugMessage($"[LOCAL] Resolved file path: {fullPath}", ConsoleMessageTypes.INFO);
            return fullPath;
        }
    }
}

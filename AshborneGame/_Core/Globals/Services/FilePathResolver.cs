
using AshborneGame._Core.Globals.Services;
using AshborneGame._Core.Globals.Enums;

namespace AshborneGame._Core.Globals.Services
{
    public static class FilePathResolver
    {
        public static string DialogueRootPath { get; } = @"D:\C# Projects\AshborneCode\AshborneGame\_Core\Data\Dialogue";

        public static string FromDialogue(string scriptFilename)
        {
            if (string.IsNullOrWhiteSpace(scriptFilename))
                throw new ArgumentException("Script filename cannot be null or empty.");

            // Strip .json if already present
            string baseName = Path.GetFileNameWithoutExtension(scriptFilename);

            string[] parts = baseName.Split('_');
            if (parts.Length < 2)
                throw new ArgumentException("Script filename must start with Act and Scene, e.g., Act1_Scene1_*");

            string actFolder = parts[0];   // e.g. "Act1"
            string sceneFolder = parts[1]; // e.g. "Scene1"
            string fullFileName = baseName + ".json";

            // Check if we're in a web context (current directory is root)
            string currentDir = Directory.GetCurrentDirectory();
            IOService.Output.DisplayDebugMessage($"FilePathResolver: Current directory = '{currentDir}'", ConsoleMessageTypes.INFO);
            
            if (currentDir == "/" || currentDir == "\\")
            {
                // Web context - try to find the file in the current directory structure
                // First try the wwwroot path relative to the current directory
                string[] possiblePaths = {
                    Path.Combine("wwwroot", "Dialogue", actFolder, sceneFolder, fullFileName),
                    Path.Combine("Dialogue", actFolder, sceneFolder, fullFileName),
                    Path.Combine("..", "wwwroot", "Dialogue", actFolder, sceneFolder, fullFileName),
                    Path.Combine("..", "..", "wwwroot", "Dialogue", actFolder, sceneFolder, fullFileName),
                    Path.Combine("..", "..", "..", "wwwroot", "Dialogue", actFolder, sceneFolder, fullFileName)
                };
                
                foreach (string path in possiblePaths)
                {
                    IOService.Output.DisplayDebugMessage($"FilePathResolver: Trying path = '{path}'", ConsoleMessageTypes.INFO);
                    if (File.Exists(path))
                    {
                        IOService.Output.DisplayDebugMessage($"FilePathResolver: Found file at = '{path}'", ConsoleMessageTypes.INFO);
                        return path;
                    }
                }
                
                // If no file found, fall back to HTTP path
                string webPath = $"/Dialogue/{actFolder}/{sceneFolder}/{fullFileName}";
                IOService.Output.DisplayDebugMessage($"FilePathResolver: No file found, using HTTP path = '{webPath}'", ConsoleMessageTypes.WARNING);
                return webPath;
            }

            // Console context - use absolute path
            string fullPath = Path.Combine(DialogueRootPath, actFolder, sceneFolder, fullFileName);

            if (!File.Exists(fullPath))
            {
                throw new FileNotFoundException(fullPath);
            }
            
            return fullPath;
        }
    }
}

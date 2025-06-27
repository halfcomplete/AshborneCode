
namespace AshborneGame._Core.Globals.Services
{
    public static class FilePathResolver
    {
        public static string ScriptsRootPath { get; } =
            Path.Combine(AppContext.BaseDirectory, "Data", "Scripts");

        public static string FromScripts(string scriptFilename)
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

            string fullPath = Path.Combine(ScriptsRootPath, actFolder, sceneFolder, fullFileName);

            if (!File.Exists(fullPath))
            {
                throw new FileNotFoundException(fullPath);
            }

            return fullPath;
        }
    }
}

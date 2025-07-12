using AshborneGame._Core.Game;
using AshborneGame._Core.SceneManagement;
using AshborneGame._Core.Globals.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AshborneGame._Core.Globals.Services
{
    public class DialogueService
    {
        private readonly InkRunner _inkRunner;

        public DialogueService(InkRunner inkRunner)
        {
            _inkRunner = inkRunner;
        }

        public bool IsRunning => _inkRunner.IsRunning;

        public void StartDialogue(string inkFilePath)
        {
            StartDialogueAsync(inkFilePath).GetAwaiter().GetResult();
        }

        public async Task StartDialogueAsync(string inkFilePath)
        {
            try
            {
                IOService.Output.DisplayDebugMessage($"Starting dialogue: {inkFilePath}", ConsoleMessageTypes.INFO);
                IOService.Output.DisplayDebugMessage($"Current directory: {Directory.GetCurrentDirectory()}", ConsoleMessageTypes.INFO);
                GameContext.GameEngine.DialogueRunning = true;
                inkFilePath = FilePathResolver.FromScripts(inkFilePath);
                IOService.Output.DisplayDebugMessage($"Loading file: {inkFilePath}", ConsoleMessageTypes.INFO);
                await _inkRunner.LoadFromFileAsync(inkFilePath);
                IOService.Output.DisplayDebugMessage("Running Ink story...", ConsoleMessageTypes.INFO);
                await _inkRunner.RunAsync();
                IOService.Output.DisplayDebugMessage("Dialogue completed", ConsoleMessageTypes.INFO);
            }
            catch (Exception ex)
            {
                IOService.Output.DisplayDebugMessage($"Dialogue error: {ex.Message}", ConsoleMessageTypes.ERROR);
                IOService.Output.DisplayDebugMessage($"Error type: {ex.GetType().Name}", ConsoleMessageTypes.ERROR);
                if (ex is FileNotFoundException fileEx)
                {
                    IOService.Output.DisplayDebugMessage($"File not found: {fileEx.FileName}", ConsoleMessageTypes.ERROR);
                }
                IOService.Output.DisplayDebugMessage($"Stack trace: {ex.StackTrace}", ConsoleMessageTypes.ERROR);
                throw;
            }
            finally
            {
                GameContext.GameEngine.DialogueRunning = false;
            }
        }

        public void JumpTo(string knot)
        {
            _inkRunner.JumpTo(knot);
        }

        public object? GetInkVariable(string key)
        {
            return _inkRunner.GetInkVariable(key);
        }

        public bool HasInkVariable(string key)
        {
            return _inkRunner.HasInkVariable(key);
        }
    }
}

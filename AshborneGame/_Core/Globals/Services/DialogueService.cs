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
        private string? _currentDialogueKey = null;
    private string? _lastDialogueKey = null;

    public string? CurrentDialogueKey => _currentDialogueKey;
    public string? LastDialogueKey => _lastDialogueKey;

        public DialogueService(InkRunner inkRunner)
        {
            _inkRunner = inkRunner;
        }

        public bool IsRunning => _inkRunner.IsRunning;

        public event Action? DialogueComplete;
        public event Action? DialogueStart;

        public async Task StartDialogue(string inkFilePath)
        {
            string originalKey = inkFilePath;
            _currentDialogueKey = originalKey;
            Console.WriteLine($"[DialogueService] StartDialogue invoked with key='{originalKey}' (before path resolution)");
            try
            {
                IOService.Output.DisplayDebugMessage($"Starting dialogue: {inkFilePath}", ConsoleMessageTypes.INFO);
                IOService.Output.DisplayDebugMessage($"Current directory: {Directory.GetCurrentDirectory()}", ConsoleMessageTypes.INFO);
                DialogueStart?.Invoke();
                inkFilePath = FilePathResolver.FromDialogue(inkFilePath);
                Console.WriteLine($"[DialogueService] Resolved ink file path='{inkFilePath}' for key='{originalKey}'");
                IOService.Output.DisplayDebugMessage($"Loading file: {inkFilePath}", ConsoleMessageTypes.INFO);
                await _inkRunner.LoadFromFileAsync(inkFilePath);
                IOService.Output.DisplayDebugMessage("Running Ink story...", ConsoleMessageTypes.INFO);
                await _inkRunner.RunAsync();
                IOService.Output.DisplayDebugMessage("Dialogue completed", ConsoleMessageTypes.INFO);
                Console.WriteLine($"[DialogueService] Dialogue run completed for key='{originalKey}'");
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
                // Only invoke DialogueComplete if this is still the current dialogue
                if (_currentDialogueKey == originalKey)
                {
                    // Set last key BEFORE firing event so listeners can detect it
                    _lastDialogueKey = _currentDialogueKey;
                    Console.WriteLine($"[DialogueService] Setting LastDialogueKey='{_lastDialogueKey}' and invoking DialogueComplete");
                    DialogueComplete?.Invoke();
                    Console.WriteLine($"[DialogueService] DialogueComplete event invoked for key='{_lastDialogueKey}'");
                    _currentDialogueKey = null;
                    Console.WriteLine("[DialogueService] _currentDialogueKey cleared (now null)");
                }
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

        public void SetPlayerInputCallback(Func<bool, Task<string>> callback)
        {
            _inkRunner.SetPlayerInputCallback(callback);
        }
    }
}

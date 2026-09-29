using AshborneGame._Core.Game;
using AshborneGame._Core.Data.BOCS;
using AshborneGame._Core.Data.IDSystem;
using AshborneGame._Core.LocationManagement;
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

        public event Action? DialogueStart;
        public event Action? OnDialogueComplete;

        private string originalKey = string.Empty;

        public async Task StartDialogue(string inkFileName)
        {
            await StartDialogue(inkFileName, null);
        }

        public async Task StartNPCDialogue(string inkFileName, DefinitionID npcDefinitionID)
        {
            BOCSObject? npc = GameContext.InstanceRegistry
                .GetByDefinition(npcDefinitionID)
                .SingleOrDefault();

            if (npc == null)
            {
                throw new InvalidOperationException($"Could not find NPC instance with definition ID '{npcDefinitionID}'.");
            }

            await StartDialogue(inkFileName, npc);
        }

        public async Task StartDialogue(string inkFileName, BOCSObject? dialogueInteractionTarget)
        {
            originalKey = inkFileName;
            _currentDialogueKey = originalKey;
            GameContext.Player.CurrentNPCInteraction = dialogueInteractionTarget;
            _inkRunner.SetDialogueInteractionTarget(dialogueInteractionTarget);
            Console.WriteLine($"[DialogueService] StartDialogue invoked with key='{originalKey}' (before path resolution)");
            try
            {
                await IOService.Output.DisplayDebugMessage($"Starting dialogue: {inkFileName}", ConsoleMessageTypes.INFO);
                await IOService.Output.DisplayDebugMessage($"Current directory: {Directory.GetCurrentDirectory()}", ConsoleMessageTypes.INFO);
                DialogueStart?.Invoke();
                inkFileName = await FilePathResolver.FromDialogue(inkFileName);
                await IOService.Output.DisplayDebugMessage($"[DialogueService] Resolved ink file path='{inkFileName}' for key='{originalKey}'");
                await IOService.Output.DisplayDebugMessage($"Loading file: {inkFileName}", ConsoleMessageTypes.INFO);
                await _inkRunner.LoadFromFileAsync(inkFileName);
                await IOService.Output.DisplayDebugMessage("Running Ink story...", ConsoleMessageTypes.INFO);
                await _inkRunner.RunAsync();
                await IOService.Output.DisplayDebugMessage("RunAsync() completed.", ConsoleMessageTypes.INFO);
                Console.WriteLine($"[DialogueService] Dialogue run completed for key='{originalKey}'");
            }
            catch (Exception ex)
            {
                await IOService.Output.DisplayDebugMessage($"Dialogue error: {ex.Message}", ConsoleMessageTypes.ERROR);
                await IOService.Output.DisplayDebugMessage($"Error type: {ex.GetType().Name}", ConsoleMessageTypes.ERROR);
                if (ex is FileNotFoundException fileEx)
                {
                    await IOService.Output.DisplayDebugMessage($"File not found: {fileEx.FileName}", ConsoleMessageTypes.ERROR);
                }
                await IOService.Output.DisplayDebugMessage($"Stack trace: {ex.StackTrace}", ConsoleMessageTypes.ERROR);
                throw;
            }
        }

        public void DialogueComplete()
        {
            // Only invoke DialogueComplete if this is still the current dialogue
            if (_currentDialogueKey == originalKey)
            {
                // Set last key BEFORE firing event so listeners can detect it
                _lastDialogueKey = _currentDialogueKey;
                Console.WriteLine($"[DialogueService] Setting LastDialogueKey='{_lastDialogueKey}' and invoking DialogueComplete");
                OnDialogueComplete?.Invoke();
                Console.WriteLine($"[DialogueService] DialogueComplete event invoked for key='{_lastDialogueKey}'");
                _currentDialogueKey = null;
                Console.WriteLine("[DialogueService] _currentDialogueKey cleared (now null)");
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

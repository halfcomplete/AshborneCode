﻿using AshborneGame._Core.Globals.Constants;
using AshborneGame._Core.Globals.Enums;
using AshborneGame._Core.Globals.Interfaces;

namespace AshborneGame.WebPort
{
    public class WebOutputHandler : IOutputHandler
    {
        private readonly Func<string, Task> _writeDialogueLineCallback;
        private readonly Func<string, Task> _writeNonDialogueLineCallback;
        private readonly Func<ConsoleMessageTypes, string, Task> _debugCallback;

        public WebOutputHandler(
            Func<string, Task> writeDialogueLineCallback,
            Func<string, Task> writeNonDialogueLineCallback,
            Func<ConsoleMessageTypes, string, Task>? debugCallback = null)
        {
            _writeDialogueLineCallback = writeDialogueLineCallback;
            _writeNonDialogueLineCallback = writeNonDialogueLineCallback;
            _debugCallback = debugCallback ?? ((type, msg) => Task.CompletedTask);
        }

        public async Task Write(string message)
        {
            await _writeDialogueLineCallback(message);
        }

        public async Task Write(string message, int ms)
        {
            await _writeDialogueLineCallback(message);
        }

        public async Task WriteNonDialogueLine(string message)
        {
            await _writeNonDialogueLineCallback(message);
        }

        public async Task WriteNonDialogueLine(string message, int ms)
        {
            await _writeNonDialogueLineCallback(message);
        }

        public async Task WriteDialogueLine(string message)
        {
            await _writeDialogueLineCallback(message);
        }

        public async Task WriteDialogueLine(string message, int ms)
        {
            await _writeDialogueLineCallback($"{OutputConstants.DefaultTypeSpeed * OutputConstants.TypeSpeedMultiplier}{OutputConstants.TypewriterStartMarker}{message}{OutputConstants.TypewriterEndMarker}");
        }

        public async Task DisplayFailMessage(string message)
        {
            await _debugCallback(ConsoleMessageTypes.ERROR, message);
        }

        public async Task DisplayDebugMessage(string message, ConsoleMessageTypes type = ConsoleMessageTypes.INFO)
        {
#if DEBUG
            await _debugCallback(type, message);
#endif
        }
    }
}

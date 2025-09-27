using AshborneGame._Core.Data.BOCS.NPCSystem.NPCBehaviourModules;
using AshborneGame._Core.Globals.Services;

namespace AshborneGame._Core.Data.BOCS.NPCSystem.NPCBehaviours
{
    public class OnAttackedLogMessage : IActOnAttacked
    {
        public string Message { get; private set; }

        public OnAttackedLogMessage(string message)
        {
            Message = message ?? throw new ArgumentNullException(nameof(message), "Log message cannot be null.");
        }

        public void OnAttacked()
        {
            IOService.Output.WriteNonDialogueLine(Message);
        }
    }
}

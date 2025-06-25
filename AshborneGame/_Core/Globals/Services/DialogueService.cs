using AshborneGame._Core.SceneManagement;
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
            _inkRunner.LoadFromFile(inkFilePath);
            _inkRunner.Run();
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

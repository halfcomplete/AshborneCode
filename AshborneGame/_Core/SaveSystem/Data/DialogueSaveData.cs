using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AshborneGame._Core.SaveSystem.Data
{
    public class DialogueSaveData
    {
        public string? StoryJsonFileName { get; set; }
        public string? StoryStateJson { get; set; }
        public bool DialogueRunning { get; set; }
    }
}

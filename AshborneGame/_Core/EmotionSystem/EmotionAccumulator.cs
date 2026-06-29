using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AshborneGame._Core.EmotionSystem
{
    public class EmotionAccumulator
    {
        public double TotalMult { get; set; }= 1.0;
        public double TotalAdd { get; set; }= 0.0;
    }
}
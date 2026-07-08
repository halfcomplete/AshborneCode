using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AshborneGame._Core.Data.BOCS.ItemSystem.ItemCapabilities
{
    public interface IStorable
    {
        int StackLimit { get; }
    }
}

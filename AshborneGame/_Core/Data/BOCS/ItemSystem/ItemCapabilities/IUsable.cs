
using AshborneGame._Core._Player;

namespace AshborneGame._Core.Data.BOCS.ItemSystem.ItemCapabilities
{
    public interface IUsable
    {
       void Use(Player player, string? target = null);
    }
}

using AshborneGame._Core._Player;
using AshborneGame._Core.Data.BOCS.ItemSystem;
using AshborneGame._Core.Globals.Interfaces;
using AshborneGame._Core.Globals.Services;
using System.Numerics;
using System.Text;

namespace AshborneGame._Core.Game.CommandHandling.Commands.BaseCommands
{
	internal abstract class BaseInventoryCommand : ICommand
	{
		protected int ParseQuantity(ref List<string> args)
		{
			string first = args[0].ToLower();

			if (int.TryParse(first, out int parsed) && parsed > 0)
			{
				args.RemoveAt(0);
				return parsed;
			}
			if (first == "all")
			{
				args.RemoveAt(0);
				return -1;
			}

			return 0; // Invalid
		}

		protected void ShowInventorySummary(Player player, Inventory inventory, string header)
		{
			IOService.Output.WriteLine(header);
			var (isEmpty, contents) = inventory.GetInventoryContents(player);
			IOService.Output.WriteLine(isEmpty ? "Nothing." : header);
			if (!isEmpty)
			{
				IOService.Output.WriteLine(contents);
			}
		}

		public abstract List<string> Names { get; }
		public abstract string Description { get; }
		public abstract bool TryExecute(List<string> args, Player player);
	}
}

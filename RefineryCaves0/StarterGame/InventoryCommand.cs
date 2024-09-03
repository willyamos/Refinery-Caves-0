using System;
namespace StarterGame
{
	public class InventoryCommand : Command
	{
        public InventoryCommand() : base()
        {
            this.Name = "inventory";
            this.Description = "\nCheck what is in your inventory.\n\nUsage: \"inventory\"\n";
        }
        
        override
        public bool Execute(Player player)
        {
            if (this.HasExtension())
            {
                player.WarningMessage("\nI cannot do an inventory on " + Extension);
            }
            else
            {
                player.Inventory();
            }
            return false;
        }
    }
	
}


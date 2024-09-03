using System;
namespace StarterGame
{
    public class DropCommand :Command
    {
        public DropCommand() : base()
        {
            this.Name = "drop";
            this.Description = "\nDrop the specified item from your inventory\n\nUsage: \"drop item\"\n";
        }

        override
        public bool Execute(Player player)
        {
            if (this.HasExtension())
            {
                player.Drop(this.Extension);
            }
            else
            {
                player.WarningMessage("\nDrop What?");
            }
            return false;
        }
    }
}


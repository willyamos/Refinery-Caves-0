using System;
namespace StarterGame
{
    public class PickupCommand : Command
    {
        public PickupCommand() : base()
        {
            this.Name = "pickup";
            this.Description = "\nPickup an item from the ground or a specified container\n\nUsage: \"pickup item\"\n";
        }

        override
        public bool Execute(Player player)
        {
            if (this.HasExtension())
            {
                player.Pickup(this.Extension);
            }
            else
            {
                player.WarningMessage("\nPickup What?");
            }
            return false;
        }
    }
}


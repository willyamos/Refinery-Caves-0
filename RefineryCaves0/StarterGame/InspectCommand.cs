using System;
namespace StarterGame
{
	public class InspectCommand : Command
	{
        public InspectCommand() : base()
        {
            this.Name = "inspect";
            this.Description = "\nUsed to inspect a specified item, item in a container, or the room you are in\n\nUsage: \"inspect\" or \"inspect item\" or \"inspect container item\"\n";
        }

        override
        public bool Execute(Player player)
        {
            if (this.HasExtension())
            {
                player.Inspect(this.Extension);
            }
            else
            {
                player.InfoMessage("\n" + player.CurrentRoom.Description()); //If Inspect with no extension return room description
            }
            return false;
        }
    }
}


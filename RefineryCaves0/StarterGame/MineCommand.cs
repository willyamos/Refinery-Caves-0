using System;
namespace StarterGame
{
	public class MineCommand : Command
    {
        public MineCommand() : base()
        {
            this.Name = "mine";
            this.Description = "\nMine deposits of ore\n\nUsage: \"mine ore\"\n";
        }

        override
        public bool Execute(Player player)
        {
            if (this.HasExtension())
            {
                player.Mine(this.Extension);
            }
            else
            {
                player.WarningMessage("\nMine What?");
            }
            return false;
        }
    }
}


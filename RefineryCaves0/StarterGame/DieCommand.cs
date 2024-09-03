using System;
namespace StarterGame
{
    public class DieCommand : Command
    {
        public DieCommand() : base()
        {
            this.Name = "die";
            this.Description = "\nDie\n\nUsage: \"die\"\n";
        }

        override
        public bool Execute(Player player)
        {
            if (this.HasExtension())
            {
                player.WarningMessage("Sorry, I can't die " + Extension);
            }
            else
            {
                player.YouDied("Self-inflicted damage");
            }
            return false;
        }
    }
}


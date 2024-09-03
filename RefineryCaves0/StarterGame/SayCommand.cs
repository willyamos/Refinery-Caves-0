using System;
namespace StarterGame
{
	public class SayCommand : Command
	{
        public SayCommand() : base()
        {
            this.Name = "say";
            this.Description = "\nSay whatever words specified\n\nUsage: \"say words\"\n";
        }

        override
        public bool Execute(Player player)
        {
            if (this.HasExtension())
            {
                player.Say(this.Extension);
            }
            else
            {
                player.WarningMessage("\nSay What?");
            }
            return false;
        }
    }
}


using System;
namespace StarterGame
{
	public class BackCommand : Command
    {

        public BackCommand() : base()
        {
            this.Name = "back";
            this.Description = "\nReturn to the previous room\n\n" +
                "Usage: \"back\"\n";
        }

        override
        public bool Execute(Player player)
        {
            if (this.HasExtension())
            {
                player.WarningMessage("\nI cannot go back to " + Extension);
            }
            else
            {
                player.GoBack();
            }
            return false;
        }
    }
}


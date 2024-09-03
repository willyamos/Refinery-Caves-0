using System.Collections;
using System.Collections.Generic;

namespace StarterGame
{
    /*
     * Spring 2024
     */
    public class GoCommand : Command
    {

        public GoCommand() : base()
        {
            this.Name = "go";
            this.Description = "\nMove to a room through a specified exit\n\nUsage: \"go direction\"\n";
        }

        override
        public bool Execute(Player player)
        {
            if (this.HasExtension())
            {
                player.WaltTo(this.Extension);
            }
            else
            {
                player.WarningMessage("\nGo Where?");
            }
            return false;
        }
    }
}

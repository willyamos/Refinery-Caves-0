using System.Collections;
using System.Collections.Generic;

namespace StarterGame
{
    /*
     * Spring 2024
     */
    public class MenuCommand : Command
    {

        public MenuCommand() : base()
        {
            this.Name = "menu";
            this.Description = "\nGo to the Main Menu\n\nUsage: \"menu\"\n";
        }

        override
        public bool Execute(Player player)
        {
            bool answer = true;
            if (this.HasExtension())
            {
                player.WarningMessage("\nI cannot menu " + this.Extension);
                answer = false;
            }
            return answer;
        }
    }
}

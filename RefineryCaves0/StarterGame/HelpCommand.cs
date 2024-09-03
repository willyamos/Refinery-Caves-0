using System.Collections;
using System.Collections.Generic;

namespace StarterGame
{
    /*
     * Spring 2024
     */
    public class HelpCommand : Command
    {
        private CommandWords _words;

        public HelpCommand() : this(new CommandWords()){}

        // Designated Constructor
        public HelpCommand(CommandWords commands) : base()
        {
            _words = commands;
            this.Name = "help";
            this.Description = "\nUsed to explain aspects of the game\n\nUsage: \"help\" or \"help command\"\n";
        }

        override
        public bool Execute(Player player)
        {
            if (this.HasExtension())
            {
                Command helping = null;
                try
                {
                    helping = _words.Get(this.Extension);
                    player.InfoMessage(helping.Description);
                }
                catch
                {
                    player.ErrorMessage("\nThere is nothing to help with " + this.Extension + "\n");
                }
            }
            else
            {
                player.InfoMessage("\nYour available commands are:" + _words.Description());
            }
            return false;
        }
    }
}

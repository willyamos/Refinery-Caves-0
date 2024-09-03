using System;
namespace StarterGame
{
    public class UnlockCommand : Command
    {
        public UnlockCommand() : base()
        {
            this.Name = "unlock";
            this.Description = "\nCan be used to unlock doors\n\nUsage: \"unlock direction\"\n";
        }

        override
        public bool Execute(Player player)
        {
            if (this.HasExtension())
            {
                player.Unlock(this.Extension);
            }
            else
            {
                player.WarningMessage("\nUnlock What?");
            }
            return false;
        }
    }
}
using System;
namespace StarterGame
{
    public class ZoomCommand : Command
    {

        public ZoomCommand() : base()
        {
            this.Name = "zoom";
            this.Description = "\nPeer into a room through a specified exit\n\nUsage: \"zoom direction\"\n";
        }

        override
        public bool Execute(Player player)
        {
            if (this.HasExtension())
            {
                player.Zoom(this.Extension);
            }
            else
            {
                player.WarningMessage("\nZoom Where?");
            }
            return false;
        }
    }
}



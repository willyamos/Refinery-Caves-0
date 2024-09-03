using System.Collections;
using System.Collections.Generic;
using System;

namespace StarterGame
{

    /*
     * Spring 2024
     */
    public abstract class Command
    {
        private string _name;
        public string Name { get { return _name; } set { _name = value; } }
        private string _extension;
        public string Extension { get { return _extension; } set { _extension = value; } }
        private string _description;
        public string Description { get { return _description; } set { _description = value; } }

        public Command()
        {
            this.Name = "";
            this.Extension = null;
            this.Description = "";
        }

        public bool HasExtension()
        {
            return this.Extension != null;
        }

        public abstract bool Execute(Player player);
    }
}

using System.Collections;
using System.Collections.Generic;
using System;

namespace StarterGame
{
    /*
     * Spring 2024
     */
    public class Parser
    {
        private CommandWords _commands;

        public Parser() : this(new CommandWords()){}

        // Designated Constructor
        public Parser(CommandWords newCommands)
        {
            _commands = newCommands;
        }

        public Command ParseCommand(string commandString)
        {
            Command command = null;
            string[] words = commandString.Split(' ');
            if (words.Length > 0)
            {
                command = _commands.Get(words[0]);
                if (command != null) 
                {
                    if (words.Length > 1) 
                    {
                        
                        if (words.Length > 2) //if there is more than just the command
                        {
                            command.Extension = null; //reset the extension!
                            for (int i = 1; i < words.Length; i++)
                            {
                                command.Extension += words[i] + " "; //combine everything after the command into "extension"
                            }
                            command.Extension.Trim(); //a little bit off the top!

                        }
                        else
                        {
                            command.Extension = words[1]; //if there is only one word after the command
                        }
                    }
                    else
                    {
                        command.Extension = null; //if no more than the command
                    }
                }
                else
                {
                    // This is debug line of code, should remove for regular execution
                    //Console.WriteLine(">>>Did not find the command " + words[0]);
                }
            }
            else
            {
                // This is a debug line of code
                //Console.WriteLine("No words parsed!");
            }
            return command;
        }

        public string Description()
        {
            return _commands.Description();
        }
    }
}

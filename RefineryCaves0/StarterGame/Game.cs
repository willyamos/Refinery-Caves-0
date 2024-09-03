using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;

namespace StarterGame
{
    /*
     * Spring 2024
     */
    public class Game
    {
        private Player _player;
        private Parser _parser;
        private bool _playing;
        private string saveData = @"saveData.txt";

        public Game()
        {
            _playing = false;
            _parser = new Parser(new CommandWords());
            _player = new Player(GameWorld.Instance.Entrance);  //Singleton
        }



        /**
     *  Main play routine.  Loops until end of play.
     */
        public void Play()
        {

            // Enter the main command loop.  Here we repeatedly read commands and
            // execute them until the game is over.
            if (_playing)
            {
                bool finished = false;
                bool menu = false;
                while (!finished)
                {
                    Console.Write("\n>");
                    string stringCommand = Console.ReadLine();
                    Command command = _parser.ParseCommand(stringCommand);
                    if (command == null)
                    {
                        _player.ErrorMessage("I don't understand...");
                    }
                    else
                    {
                        _player.Log(stringCommand);         //Adds command to Log
                        menu = command.Execute(_player);
                        while (menu)
                        {
                            Console.Clear();            //Clears Console        
                            if (Start() == false)
                            {
                                finished = true;        //End game
                            }
                            menu = false;       //Exits while loop
                        }
                    }
                }
            }
        }

        public bool Start()
        {
            //if this is the first occurence of Start()
            if (_playing != true)
            {
                _player.InfoMessage(Welcome());
                _player.SelectMessage("New Game");
            }
            else
            {
                _player.InfoMessage("Paused\n");
            }

            //if a log file exists, display "Continue", set a bool to fileExists
            bool fileExists = File.Exists(saveData);
            if (fileExists)
            {
                _player.SelectMessage("Contine\n");     //Continue option appears
            }
            else
            {
                _player.InfoMessage("");
            }
            if (_playing)
            {
                _player.SelectMessage("Save\nQuit\n");      //If playing and menu is called Save and Quit appear
            }
            string game = Console.ReadLine().TrimEnd();     //Players Input
            bool query = true;
            while (query == true)
            {
                if (game.Equals("New Game"))
                {
                    if (_playing)
                    {
                        _player.ErrorMessage("\nPlease enter \"Contine\", \"Save\" or \"Quit\"\n");     //Continue Save Quit appear if playing
                        game = Console.ReadLine().TrimEnd();
                    }
                    else
                    {

                        FileStream fs = File.Create(saveData);      //Creates First File
                        fs.Close();
                        Tutorial();     // Calls Tutorial!
                        Console.Clear();
                        query = false;
                    }
                    //Exits While Loop
                }
                else if (game.Equals("Continue"))
                {
                    //fileExists = true, if not, errormessage and break DO NOT _playing = true;
                    if (fileExists != true)
                    {
                        _player.ErrorMessage("\nPlease enter \"New Game\"\n");
                        game = Console.ReadLine().TrimEnd();

                    }
                    else
                    {
                        //load log file and for every command run it as console input, Console.Clear() after every loop
                        Load();
                        query = false;
                        Console.Clear();
                    }
                    //Exits While Loop
                }
                else if (game.Equals("Save"))
                {
                    if (_playing != true)
                    {
                        if (fileExists != true)
                        {
                            _player.ErrorMessage("\nPlease enter \"New Game\"\n");
                            game = Console.ReadLine().TrimEnd();

                        }
                        else
                        {
                            _player.ErrorMessage("\nPlease enter \"New Game\" or \"Continue\"\n");
                            game = Console.ReadLine().TrimEnd();
                        }
                        query = false;
                    }
                    else //If playing
                    {
                        if (Save(_player.DisplayLog())) //Save the log
                        {
                            Console.Clear();
                            _player.InfoMessage("\nGame saved!");
                        }
                        else
                        {
                            _player.ErrorMessage("\nError saving game, no log?");
                        }
                        query = false;
                    }
                    //Exits While Loop
                }
                else if (game.Equals("Quit"))
                {
                    if (_playing == true)
                    {
                        query = false;
                        return false;   //Quits Game
                    }
                    else // if not playing
                    {
                        if (fileExists != true)
                        {
                            _player.ErrorMessage("\nPlease enter \"New Game\"\n"); //Deny Save
                            game = Console.ReadLine().TrimEnd();

                        }
                        else
                        {
                            _player.ErrorMessage("\nPlease enter \"New Game\" or \"Continue\"\n"); //Deny save
                            game = Console.ReadLine().TrimEnd();

                        }
                    }
                }
                //if the input is something i dont want
                else
                {
                    //fileExists = true, if not, errormessage is Please enter New Game
                    if (fileExists != true)
                    {
                        _player.ErrorMessage("\nPlease enter \"New Game\"\n");
                        game = Console.ReadLine().TrimEnd();

                    }
                    else if (_playing)
                    {
                        _player.ErrorMessage("\nPlease enter \"Contine\", \"Save\" or \"Quit\"\n");
                        game = Console.ReadLine().TrimEnd();
                    }
                    else //If file exists
                    {
                        _player.ErrorMessage("\nPlease enter \"New Game\" or \"Continue\"\n");
                        game = Console.ReadLine().TrimEnd();
                    }

                }
            }
            //Console.Clear();
            _playing = true;
            _player.InfoMessage("\n" + _player.CurrentRoom.Description() + "\n");
            return true;
        }

        public void End() //Ends Game
        {
            _playing = false;
            _player.InfoMessage(Goodbye());
            Console.ReadLine();
        }

        private bool Save(string log) //Save Game
        {
            try
            {
                StreamWriter sw = new StreamWriter(saveData, false); //Writes to the file, false turns off apped mode
                sw.WriteLine(log);
                sw.Close();
                return true;    //Saved the game
            }
            catch
            {
                return false;   //Failed Save
            }
        }

        private void Load()
        {
            try
            {
                StreamReader sr = new StreamReader(saveData);
                string raw = sr.ReadLine();
                string[] log = raw.Split("~");      // ~ Separates commands
                for (int i = 0; i < log.Length; i++)
                {
                    if (!log[i].Equals("menu") && !log[i].Equals("go teleport") && !log[i].Equals("go shop")) //regular gameplay emulation
                    {
                        Command command = _parser.ParseCommand(log[i]);
                        if (command != null)
                        {
                            command.Execute(_player);       //game emulator, actually executes code
                            _player.Log(log[i]);
                        }
                        Console.Clear();
                    }
                    else if (log[i].Equals("go shop")) //player entered shop GUI, emulate 
                    {
                        _player.SneakTo("shop");
                        _player.Log(log[i]);
                        i++;    //skip
                        bool shopping = true;
                        while (shopping)
                        {
                            if (log[i].Equals("buy"))
                            {
                                i++;    //skip
                                _player.TryBuy(log[i]); //Runs buy in shop
                            }
                            else if (log[i].Equals("sell"))
                            {
                                i++;    //skip
                                _player.TrySell(log[i]); //Runs sell in shop
                            }
                            else if (log[i].Equals("fail"))
                            {
                                //do nothin
                            }
                            else if (log[i].Equals("back"))
                            {
                                _player.GoBack();   //Kicks player out of shop
                                shopping = false;
                            }
                            i++; //Advances the log check
                        }
                    }
                    else if (log[i].Equals("go teleport")) //teleport room emulator
                    {
                        _player.SneakTo("teleport");
                        _player.Log(log[i]);
                        i++;    //skip
                        bool tping = true;
                        while (tping)
                        {
                            if (log[i].Equals("Y"))
                            {
                                i++;    //skip
                                _player.WaltTo(log[i], true);
                                tping = false;
                            }
                            else if (log[i].Equals("N"))
                            {
                                _player.GoBack(true);
                                Console.WriteLine(_player.CurrentRoom.Tag);
                                tping = false;
                            }
                            else
                            {
                                i++;    //skip
                            }

                            Console.WriteLine(log[i]);
                        }

                    }
                }
                sr.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("no " + e);
            }
        }

        public void Tutorial()
        {
            Console.Clear();
            bool tutHelp;
            string tutInput;
            void EnterNext()
            {
                _player.WarningMessage("\nPress Enter to continue..."); //Let's the player read
                Console.ReadLine();
                Console.Clear();
            }

            _player.InfoMessage("Welcome to the tutorial!\n\n" +                //Plays a tutorial
                "In this game, you are a miner exploring two caves:\n\n");
            _player.SelectMessage("Mythril Cave\tAdaman's Pass\n");
            EnterNext();
            //enter
            _player.InfoMessage("\nHere is a pickaxe! You can use the command \"mine\" to mine any ore you find.\n");
            _player.OutputMessage("You have been given Pickaxe\n");
            EnterNext();
            //enter
            _player.InfoMessage("Speaking of commands, why don't I go ahead and show you all of them?" +
                "\n\nFeel free to type the name of any command you'd like to know more about.");
            EnterNext();
            Command tutcommand = _parser.ParseCommand("help");
            tutcommand.Execute(_player);             //Runs the help command
            _player.InfoMessage("\nOr, to continue, type \"continue\"\n");
            tutHelp = true;
            while (tutHelp)
            {
                tutInput = Console.ReadLine();
                while (!tutInput.Equals("continue"))
                {
                    tutcommand = _parser.ParseCommand("help " + tutInput);
                    tutcommand.Execute(_player);        //Forces tutorial input
                    tutInput = Console.ReadLine();
                }
                tutHelp = false;
                Console.Clear();
            }
            _player.InfoMessage("If you need help at any point during the game, just type help!\nNeed help with a specific command? " +
                "Include the command you need help with after help.\n\nExample: \"help help\"");
            EnterNext();
            //enter
            _player.InfoMessage("You're almost ready to play! One last thing:" +
                "\n\nMythril Cave is free, but Adaman's pass is not. " +
                "\nTo get money in this game, sell the ores you mine to the shopkeep at homebase!" +
                "\n\nI also heard he can give you some helpful info...");
            EnterNext();
            //enter
            _player.InfoMessage("Alright! Enough yapping. Have fun!" +
                "\n\nAnd if you ever need to save or quit, type \"menu\"!");
            EnterNext();
        }

        public string Welcome()
        {
            return "Welcome to Refinery Caves 0!\n\n";
        }

        public string Goodbye()
        {
            return "\nThank you for playing, Goodbye. \n";
        }

    }
}

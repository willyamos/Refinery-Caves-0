using System.Collections;
using System.Collections.Generic;
using System;

namespace StarterGame
{
    /*
     * Spring 2024
     */

    public class Room
    {
        private Dictionary<string, Room> _exits;   
        private Dictionary<string, string> _exitLocks;  //exit names , keys
        private string _tag;
        public string Tag
        {
            get
            {
                return _roomDelegate == null ? _tag : _roomDelegate.OnTag(_tag);
            }
            set
            {
                _tag = value;
            }
        }
        private IRoomDelegate _roomDelegate;
        public IRoomDelegate RoomDelegate
        {
            get { return _roomDelegate; }
            set
            {
                if (_roomDelegate != null) 
                {
                    _roomDelegate.ContainingRoom = null;
                }
                if (value != null)
                {
                    if (value.ContainingRoom != null)
                    {
                        value.ContainingRoom.RoomDelegate = null;
                    }
                    value.ContainingRoom = this;


                }
                _roomDelegate = value;
            }
        }

        private IItemContainer _itemsOnFloor;

        public Room() : this("No Tag") { }

        // Designated Constructor
        public Room(string tag)
        {
            _itemsOnFloor = new ItemContainer("floor", 0f);
            _roomDelegate = null;
            _exits = new Dictionary<string, Room>();
            _exitLocks = new Dictionary<string, string>();
            this.Tag = tag;
        }

        //Was made to return correct statements regarding locked doors
        public char GetLockedStatus(string exitName)
        {
            if (_exits.ContainsKey(exitName))
            {
                if (_exitLocks[exitName] != null && !_exitLocks[exitName].Equals("bomb")) //the door is locked key is not bomb
                {
                    return 'T';
                }
                else if (_exitLocks[exitName] != null && _exitLocks[exitName].Equals("bomb")) //the door is locked the key is bomb
                {
                    return 'B';
                }
                else
                {
                    return 'F'; // success the door is unlocked
                }
            }
            return 'N'; // the door was never locked
        }

        // Locks exit
        public void LockExit(string exitName, string key)
        {
            if (_exits.ContainsKey(exitName))
            {
                _exitLocks[exitName] = key; //Locks exit
            }
        }

        //Unlocks the exit
        public bool UnlockExit(string exitName, string key)
        {
            if (_exits.ContainsKey(exitName)) //if door exists
            {
                if (_exitLocks.ContainsKey(exitName)) //if door is locked
                {
                    if (_exitLocks[exitName] != null)
                    {
                        string requiredKey = _exitLocks[exitName]; 
                        if (requiredKey == key)
                        {
                            _exitLocks[exitName] = null; // Remove the required key, effectively unlocking the exit
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        //Drops items to room
        public IItem Drop(IItem item)
        {
            IItem oldItem = _itemsOnFloor.Remove(item.Name); //Prevents duplicates
            _itemsOnFloor.Insert(item);
            return oldItem;
        }

        //Removes items from floor
        public IItem Pickup(string itemName)
        {
            return _itemsOnFloor.Remove(itemName);
        }

        //Sets exits
        public void SetExit(string exitName, Room room, string key)
        {
            _exits[exitName] = room;
            _exitLocks[exitName] = key; //Sets keys
        }

        public Room GetExit(string exitName)
        {
            Room room = null;
            if (_exits.TryGetValue(exitName, out room))
            {
                if (_exitLocks.TryGetValue(exitName, out string requiredKey)) // Check if exit is locked and get required key
                {
                    if (string.IsNullOrEmpty(requiredKey)) // If no key required (empty or null), exit is unlocked so continue
                    {
                        if (_roomDelegate != null)
                        {
                            room = _roomDelegate.OnGetExit(room);
                        }
                        return room;
                    }
                }
            }
            return null; //cannot access
        }

        public string GetExits()
        {
            string exitNames = "Exits: ";
            foreach (var exitPair in _exits)
            {
                string exitName = exitPair.Key;
                if (_exitLocks.TryGetValue(exitName, out string requiredKey)) // Check if exit is locked and get required key
                {
                    if (string.IsNullOrEmpty(requiredKey)) // If no key required (empty or null), exit is unlocked
                    {
                        exitNames += " " + exitName;
                    }
                    else
                    {
                        exitNames += " [" + exitName + " (Locked)]"; //Adds (Locked) onto exit
                    }
                }
            }
            if (_roomDelegate != null)
            {
                return _roomDelegate.OnGetExits(exitNames);
            }

            return exitNames;
        }


        // Normal Player Room Description
        public string Description()
        {
            return "You are " + this.Tag + ".\n *** " + this.GetExits() + "\nItem: " + _itemsOnFloor.Description;
        }

        // Description altered for zoom command
        public string Description(bool zoom)
        {
            return "You zoomed " + this.Tag + ".\n *** " + this.GetExits() + "\nItem: " + _itemsOnFloor.Description;
        }
    }

    //The Shop Room GUI
    public class ShopRoom : IRoomDelegate
    {
        private Room _containingRoom;

        public Room ContainingRoom
        {
            set
            {
                _containingRoom = value;
            }
            get
            {
                return _containingRoom;
            }
        }

        private IItemContainer shopSelling; //Shop Inventory

        public ShopRoom(IItemContainer shopSelling)
        {
            NotificationCenter.Instance.AddObserver("PlayerDidEnterRoom", PlayerDidEnterRoom);
            this.shopSelling = shopSelling;
        }

        public Room OnGetExit(Room room)
        {
            return room;
        }

        public string OnGetExits(string exits)
        {
            return "back";
        }

        public string OnTag(string tag)
        {
            return tag;
        }

        //Activates when Notification is sent that Player has entered a room
        public void PlayerDidEnterRoom(Notification notification)
        {
            Player player = (Player)notification.Object;
            if (player != null)
            {
                if (player.CurrentRoom == ContainingRoom)
                {
                    player.SpokenMessage("\nWelcome to the shop! How can I help you?\n\n"); //Opening Message
                    bool finished = false;
                    while (!finished)
                    {
                        player.SelectMessage("buy\nsell\ninfo\nback\n");
                        string answer = Console.ReadLine();
                        //logs base level shop commands
                        player.Log(answer);
                        if (answer.Equals("buy"))
                        {

                            if (!shopSelling.ItemNamesAndCosts.Equals("")) //if shop not empty
                            {
                                player.SpokenMessage("\nTake a look at what I've got:\n");
                                player.InfoMessage(shopSelling.ItemNamesAndCosts + "\n\n"); //Displays items + costs
                                player.SpokenMessage("Anything interest you?\n");
                                bool buying = true;
                                while (buying)
                                {
                                    string item = Console.ReadLine();
                                    if (player.TryBuy(item))
                                    {
                                        //successful interraction log
                                        player.Log(item);
                                        player.SpokenMessage("\nThank you for your business! " +
                                            "\n\nAs a bonus, I've gone ahead and upgraded your bag for ya. Thank me later!\n\n" +
                                            " Anything else I can do for you?\n");
                                        player.UpdateVolumeLimit(20);
                                        player.UpdateWeightLimit(20f);
                                        buying = false;
                                    }
                                    else
                                    {
                                        //failed interraction log
                                        player.Log("fail");
                                        player.SpokenMessage("Sorry!\nAnything else I can do for you?\n");
                                        buying = false;
                                    }
                                }
                            }
                            else
                            {
                                player.SpokenMessage("\nSorry, I'm fresh out of stock! Anything else I can help you with?\n"); //shop is empty
                            }

                        }
                        else if (answer.Equals("sell"))
                        {
                            player.SpokenMessage("Let's see what you've got! Go ahead and open your pack for me.\n");
                            player.SellInventory();
                            bool selling = true;
                            while (selling)
                            {
                                player.SpokenMessage("\nSo, what'll ya give me?\n");
                                string item = Console.ReadLine();
                                if (player.TrySell(item))
                                {
                                    //successful interraction log
                                    player.Log(item);
                                    player.SpokenMessage("\nPleasure doing business with you." +
                                        "Anything else I can do for you?\n");
                                        
                                    selling = false;
                                }
                                else
                                {
                                    //failed interraction log
                                    player.Log("fail");
                                    player.SpokenMessage("Sorry!\nAnything else I can do for you?");
                                    selling = false;
                                }
                            }
                        }
                        else if (answer.Equals("info"))
                        {
                            Random rand = new Random();
                            int randomNumber = rand.Next(1, 10000000); //grabs a random number for more "accurate" randomization
                            if (randomNumber < 2500000)
                            {
                                player.SpokenMessage("\nI've heard there's a miner stuck somewhere in Mythril Cave...\n");
                            }
                            else if (randomNumber > 2500000 && randomNumber < 5000000)
                            {
                                player.SpokenMessage("\nDid you know there is a bomb production facility deep in Adaman's pass?\n\n...I know right? Doesn't seem too safe.");
                            }
                            else if (randomNumber > 5000000 && randomNumber < 7500000)
                            {
                                player.SpokenMessage("\nI used to sell more pickaxes, but another miner came through and bought them all. Sorry!\n");
                            }
                            else //If <750000
                            {
                                player.SpokenMessage("\nEver wonder why there isn't more than one ore of each type in the mines?\n\n...stop thinking about it.\n");
                            }
                            player.SpokenMessage("\nAnything else I can help you with?\n");
                        }
                        else if (answer.Equals("back"))
                        {
                            player.SpokenMessage("\nCome back soon!\n");
                            player.GoBack(true); //activates players back command quietly
                            finished = true;
                        }
                        else
                        {
                            player.SpokenMessage("\nHuh? You're gonna have to speak up.\n"); //you said something the shop keeper didnt like
                        }
                    }
                }
            }
        }
    }

    // Teleporter Room GUI
    public class TeleportRoom : IRoomDelegate
    {
        private Room _containingRoom;

        public Room ContainingRoom
        {
            set
            {
                _containingRoom = value;
            }
            get
            {
                return _containingRoom;
            }
        }

        public TeleportRoom()
        {
            NotificationCenter.Instance.AddObserver("PlayerDidEnterRoom", PlayerDidEnterRoom);
        }

        public Room OnGetExit(Room room)
        {
            return room;
        }

        public string OnGetExits(string exits)
        {
            return "back";
        }

        public string OnTag(string tag)
        {
            return tag;
        }

        public void PlayerDidEnterRoom(Notification notification)
        {
            Player player = (Player)notification.Object;
            if (player != null)
            {
                if (player.CurrentRoom == ContainingRoom)
                {
                    bool finished = false;
                    while (!finished) //Traps you in a confirmation loop
                    {
                        player.InfoMessage("\nYou are in the Random Teleporter. Would you like to be teleported? Y/N\n");
                        string answer = Console.ReadLine();
                        player.Log(answer);
                        if (answer.Equals("Y"))
                        {
                            Random rand = new Random();
                            int randomNumber = rand.Next(1, 12);    //randomizes 1-11 and brings you to that exit
                            string randomRoom = randomNumber.ToString();
                            player.InfoMessage("\nYou have been teleported!");
                            player.Log(randomRoom);
                            player.WaltTo(randomRoom, true); //Walks to room quietly and clears the back log so you cannot return
                            finished = true;
                        }
                        else if (answer.Equals("N"))
                        {
                            player.InfoMessage("Returning to previous room\n");
                            player.GoBack(true);
                            finished = true;
                        }
                        else
                        {
                            player.ErrorMessage("Please enter \"Y\" or \"N\"\n");
                        }
                    }
                }
            }
        }
    }

    //WinRoom GUI 
    public class WinRoom : IRoomDelegate
    {
        private Room _containingRoom;

        public Room ContainingRoom
        {
            set
            {
                _containingRoom = value;
            }
            get
            {
                return _containingRoom;
            }
        }

        int count = 0; //Here to validate that the player has seen the cutscene already and you cannot return to the room

        public WinRoom()
        {
            NotificationCenter.Instance.AddObserver("PlayerDidEnterRoom", PlayerDidEnterRoom);
        }

        public Room OnGetExit(Room room)
        {
            return room;
        }

        public string OnGetExits(string exits)
        {
            return "back";
        }

        public string OnTag(string tag)
        {
            return tag;
        }

        public void PlayerDidEnterRoom(Notification notification)
        {
            
            Player player = (Player)notification.Object;
            if (player.CurrentRoom == ContainingRoom)
            {
                if (player != null && count == 0) //If its your first time
                {
                    player.InfoMessage("\nAs your eyes and ears recover from the blast, you hear a faint voice in the distance...");
                    System.Threading.Thread.Sleep(1500);
                    player.SpokenMessage("\n...");
                    System.Threading.Thread.Sleep(500);
                    player.SpokenMessage("\nHello?.. Is anyone there?");
                    System.Threading.Thread.Sleep(1500);
                    player.InfoMessage("\n\nYou see a miner covered head to toe in soot walk into your torch-light");
                    System.Threading.Thread.Sleep(1500);
                    player.SpokenMessage("\nOh my gyatt... I haven't seen light in ages!");
                    System.Threading.Thread.Sleep(1000);
                    player.SpokenMessage("\nThank you so much for saving me! I'm not sure how I'd ever repay you, but...");
                    System.Threading.Thread.Sleep(1500);
                    player.SpokenMessage("\nI'd say you're a real\n");
                    player.SelectMessage("WINNER\n");
                    player.SpokenMessage("in my book!\n");

                    player.WarningMessage("Press Enter to continue...");
                    Console.ReadLine();


                    player.SelectMessage("\n\nYou won the game!");
                    player.InfoMessage("\nYou earned " + player.Currency + " coins during your exploration");

                    if (player.Currency < 400) //if player hasnt sold almost all items in the game
                    {
                        player.InfoMessage("\n\nThere's still a bit more out there.. See if you can find it all!");
                    }

                    count++;
                    player.GoBack(true); //quiet go back command
                }
                else //if count++ then youve already been here and it kicks you out
                {
                    player.InfoMessage("\nAll that's left here is rubble and darkness.");
                    player.InfoMessage("\nYou have gone back a room!");
                    player.GoBack(true); //quiet go back command
                }
            }
        }
    }
}

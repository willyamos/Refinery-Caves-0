using System.Collections;
using System.Collections.Generic;
using System;

namespace StarterGame
{
    /*
     * Spring 2024
     */
    public class Player
    {
        private Stack<Room> _back;
        private Room _currentRoom = null;
        public Room CurrentRoom { get { return _currentRoom; } set { _currentRoom = value; } }
        private Room _checkpoint = null;
        public Room Checkpoint { get { return _checkpoint; } set { _checkpoint = value; } }
        private IItemContainer _backpack;
        private float WeightLimit = 20f;
        private int VolumeLimit = 10;
        private List<string> _log;
        private int _currency;
        public int Currency { get { return _currency; } set { _currency = value; } }
        private IRoomProxy _roomProxy;

        //Constructor 
        public Player(Room room)
        {
            _currentRoom = room;
            _backpack = new ItemContainer("backPack", 0f, 0);
            _back = new Stack<Room>();
            _checkpoint = room;
            _log = new List<string>();
            _currency = 0;
            _roomProxy = new RoomProxy(room);
        }

        //Check the next room without visiting it
        public void Zoom(string direction)
        {
            Room nextRoom = _roomProxy.Zoom(direction);
            if (nextRoom != null)
            {
                NormalMessage(nextRoom.Description(true));
            }
            else if (CurrentRoom.GetLockedStatus(direction) == 'T') //if room is locked!
            {
                ErrorMessage("\nThe door to " + direction + " is locked");
            }
            else if (CurrentRoom.GetLockedStatus(direction) == 'B') //if room is caved-in!
            {
                ErrorMessage("\nThat direction is covered in rubble!");
            }
            else //if there is no door
            {
                ErrorMessage("\nThere is no door on " + direction);
            }
        }

        //Takes the last command and adds on to the running player log for save file
        public void Log(string command)
        {
            _log.Add(command);
        }

        //Used for debugging loading save files
        public string DisplayLog()
        {
            string logList = "";
            foreach (string command in _log)
            {
                logList += command + "~";
            }
            return logList;
        }

        //Effectively "kills" the player, respawning at _checkpoint and dropping all items upon death
        public void YouDied(string reason)
        {
            string[] items = _backpack.ItemNames.TrimStart().Split(" ");
            if (items[0].Length > 1)
            {
                foreach (string item in items)
                {
                    Drop(item, true);
                }
                InfoMessage("\nYou have dropped your items on the floor in " + CurrentRoom.Tag);
            }
            WarningMessage("\nYou died to " + reason + "\n");
            //Sets the players current room location back to the specified checkpoint.
            if (!reason.Equals("an explosion"))
            {
                CurrentRoom = _checkpoint;

                //Clears backlog. It would be cheap to just say back to go back where you died!
                _back.Clear();

                InfoMessage("\nYou respawned at your checkpoint, " + CurrentRoom.Tag);
            }
            else
            {
                ErrorMessage("\nYou lose. Quit.");
                CurrentRoom = _checkpoint;
            }
        }

        //Try to visit the room via direction
        public void WaltTo(string direction)
        {
            Room nextRoom = this.CurrentRoom.GetExit(direction);
            if (nextRoom != null)
            {
                _back.Push(CurrentRoom); //add current room to _back stack for back command call
                CurrentRoom = nextRoom;
                _roomProxy = new RoomProxy(nextRoom);
                Notification notification = new Notification("PlayerDidEnterRoom", this);
                NotificationCenter.Instance.PostNotification(notification);
                NormalMessage("\n" + this.CurrentRoom.Description());

            }
            else if (CurrentRoom.GetLockedStatus(direction) == 'T') //if door is locked
            {
                ErrorMessage("\nThe door to " + direction + " is locked");
            }
            else if (CurrentRoom.GetLockedStatus(direction) == 'B') //if door is caved-in
            {
                ErrorMessage("\nThat direction is covered in rubble!");
            }
            else //if no door
            {
                ErrorMessage("\nThere is no door on " + direction);
            }
        }

        //"quiet" overload of WaltTo. Used for teleporting.
        public void WaltTo(string direction, bool quiet)
        {
            Room nextRoom = this.CurrentRoom.GetExit(direction);
            if (nextRoom != null)
            {
                CurrentRoom = nextRoom;
                _roomProxy = new RoomProxy(nextRoom);
                Notification notification = new Notification("PlayerDidEnterRoom", this);
                NotificationCenter.Instance.PostNotification(notification);

                //If quiet, I am assuming the player is teleporting. Therefore, I am clearing backlog. If you use
                //this constructor anywhere else and you can't use the back command anymore, this is why.
                _back.Clear();

            }
            else
            {
                ErrorMessage("\nThere is no door on " + direction);
            }
        }

        //An even "quieter" overload of WaltTo- used for getting in rooms that have Notification overrides
        public void SneakTo(string direction)
        {
            Room nextRoom = this.CurrentRoom.GetExit(direction);
            _back.Push(CurrentRoom);
            CurrentRoom = nextRoom;
            _roomProxy = new RoomProxy(nextRoom);
        }

        //Used for the back command
        public void GoBack()
        {

            if (_back.Count > 0)
            {
                Room lastRoom = _back.Peek(); //checks if last room is not null (should never occur)
                if (lastRoom != null)
                {
                    _back.Pop(); //Takes last room off of top of stack and pushes the player there
                    CurrentRoom = lastRoom;
                    _roomProxy = new RoomProxy(lastRoom);
                    Notification notification = new Notification("PlayerDidEnterRoom", this);
                    NotificationCenter.Instance.PostNotification(notification);
                    InfoMessage("\nYou have gone back a room!");
                    NormalMessage("\n" + this.CurrentRoom.Description());
                }

            }
            else //If the stack is empty, you can't go back anymore
            {
                ErrorMessage("\nYou can no longer go back");
            }
        }

        //A "quiet" overload of GoBack. Used for forcing player out of rooms without spamming the console
        public void GoBack(bool quiet)
        {

            if (_back.Count > 0)
            {
                Room lastRoom = _back.Peek(); //checks if last room is not null (should never occur)
                if (lastRoom != null)
                {
                    _back.Pop(); //Takes last room off of top of stack and pushes the player there
                    CurrentRoom = lastRoom;
                    _roomProxy = new RoomProxy(lastRoom);
                    Notification notification = new Notification("PlayerDidEnterRoom", this);
                    NotificationCenter.Instance.PostNotification(notification); //Sends notification, but not Message
                }

            }
            else //If the stack is empty, you can't go back anymore
            {
                ErrorMessage("\nYou can no longer go back");
            }
        }

        //Used to make the player "say" anything. Planned for use, plan was scrapped
        public void Say(string word) 
        {

            NormalMessage(word);
            Notification notification = new Notification("PlayerDidSayAWord", this);
            Dictionary<string, object> userInfo = new Dictionary<string, object>();
            userInfo["word"] = word;
            notification.UserInfo = userInfo;
            NotificationCenter.Instance.PostNotification(notification);
        }

        //Inspect is now up to a 3 word command
        public void Inspect(string itemName)
        {
            string[] words = itemName.Split(' ');
            IItem pickedUpItem = CurrentRoom.Pickup(words[0]);
            if (pickedUpItem != null)
            {
                if (words.Length > 3) // If over 3 words this item does not exist. All items are 1 word long.
                {
                    ErrorMessage("\nThere is no item named " + itemName.Substring(words[0].Length).TrimStart());
                }
                else if (words.Length > 1 && pickedUpItem.IsContainer) //Checks if first word is an item or container
                {
                    IItemContainer pickedUpContainer = (IItemContainer)pickedUpItem; //pickupUpItem turns into a pickedUpContainer
                    try
                    {
                        IItem extraItem = pickedUpContainer.Remove(words[1]); // Removes item from container
                        InfoMessage(extraItem.Description);
                        pickedUpContainer.Insert(extraItem);
                        CurrentRoom.Drop(pickedUpItem); //Places everything back
                    }
                    catch
                    {
                        ErrorMessage("\nThere is no " + words[1] + " inside of " + words[0]); //if the item not in container
                        CurrentRoom.Drop(pickedUpItem);
                    }
                }
                else if (words.Length > 1 && !pickedUpItem.IsContainer) // Not container
                {
                    ErrorMessage("\n" + words[0] + " is not a container");
                }
                else
                {
                    InfoMessage("\nItem info: " + pickedUpItem.Description); //Success gives item info
                    CurrentRoom.Drop(pickedUpItem);
                }
            }
            else
            {
                ErrorMessage("\nThere is no item named " + itemName + " on the floor");
            }
        }

        // Returns the players inventory
        public void Inventory()
        {
            NormalMessage(_backpack.Description);
            NormalMessage("Maximum Weight: " + WeightLimit);
            NormalMessage("Maximum Volume: " + VolumeLimit);
            NormalMessage("\nCoins: " + _currency);
        }

        //Gets names and costs of all inventory items, used by shopkeep
        public void SellInventory()
        {
            NormalMessage(_backpack.ItemNamesAndCosts);
            NormalMessage("\nCoins: " + _currency);
        }

        // inserts item to inventory
        public void Give(IItem item)
        {
            _backpack.Insert(item);
        }

        // removes item from inventory
        public IItem Take(string itemName)
        {
            return _backpack.Remove(itemName);
        }

        // Attempts to buy item from shop
        public bool TryBuy(string itemName)
        {
            IItemContainer shopInventory = (IItemContainer)CurrentRoom.Pickup("Shopkeep's Inventory"); //Grabs shop inventory
            try
            {
                IItem buying = shopInventory.Remove(itemName);
                bool tooHeavy = _backpack.Weight + buying.Weight > WeightLimit;
                bool tooBig = _backpack.Volume + buying.Volume > VolumeLimit;
                bool tooCostly = _currency < buying.Price;
                if ((tooHeavy == false) && (tooBig == false) && (tooCostly == false)) //You can buy item
                {
                    Give(buying);
                    NormalMessage("\nYou bought " + buying.Name + "for " + buying.Price + " coins\n");
                    CurrentRoom.Drop(shopInventory);
                    _currency = _currency - buying.Price;
                    return true;
                }
                else
                {
                    if (tooHeavy) //You will weigh too much
                    {
                        WarningMessage("\n" + itemName + " is too heavy!\n");
                    }
                    if (tooBig) //You don't have enough room
                    {
                        WarningMessage("\nYou do not have enough space for " + itemName + "\n");
                    }
                    if (tooCostly) //Youre broke
                    {
                        WarningMessage("\nYou do not have enough coins for " + itemName + "\n");
                    }
                    shopInventory.Insert(buying); //Gives the item back since you can't buy it
                    CurrentRoom.Drop(shopInventory); //drops the shop inv back
                }
            }
            catch
            {
                ErrorMessage("\n" + itemName + " is not beind sold\n"); //Item doesn't exist
                CurrentRoom.Drop(shopInventory);
                
            }
            return false;
        }

        //Tries to sell items
        public bool TrySell(string itemName)
        {
            IItem item = Take(itemName); //Takes item from players inventory
            if (item != null)
            {
                Currency += item.Price; //Increases player currency
                return true;
            }
            ErrorMessage("\n" + itemName + " is not in your inventory\n");
            return false;
        }
        
        // Update Weight Limit of backpack
        public void UpdateWeightLimit(float weight)
        {
            WeightLimit += weight;
        }

        // Update Volume Limit of backpack
        public void UpdateVolumeLimit(int volume)
        {
            VolumeLimit += volume;
        }

        // Used on mineable containers to break them and drop the contained item on the floor
        public void Mine(string itemName)
        {
            IItem minedItem = CurrentRoom.Pickup(itemName); 
            if(minedItem != null)
            {
                if (minedItem.IsContainer && minedItem.IsMineable) //If it is an ore
                {
                    IItemContainer minedContainer = (IItemContainer)minedItem; //force container type
                    string[] items = minedContainer.ItemNames.TrimStart().Split(" ");        
                    if (items[0].Length > 1) //if item isnt empty
                    {
                        foreach (string item in items) //Remove everything from ore
                        {
                            IItem floorItem = minedContainer.Remove(item);
                            CurrentRoom.Drop(floorItem);
                            NormalMessage("\nYou have mined the " + minedContainer.Name + "!\n");
                        }
                    }
                    else 
                    {
                        NormalMessage("\nThis item has already been mined\n");
                    }                   
                }
                else if(!minedItem.IsContainer && minedItem.IsMineable) //If bomb
                {
                    ErrorMessage("\nYou shouldn't have done that...");

                    YouDied("an explosion"); //die and be softlocked

                }
                else
                {
                    NormalMessage("\nThis item cannot be mined\n"); // You can't mine chests or items
                    CurrentRoom.Drop(minedItem);
                }
            }
        }

        //Pickups items
        public void Pickup(string itemName)
        {
            string[] words = itemName.Split(' '); //takes the extension of the command and chops it up
            IItem pickedUpItem = CurrentRoom.Pickup(words[0]);
            if (pickedUpItem != null)
            {
                bool tooHeavy = _backpack.Weight + pickedUpItem.Weight > WeightLimit;
                bool tooBig = _backpack.Volume + pickedUpItem.Volume > VolumeLimit;
                if (words.Length > 3) // No item will be 2 words long so this command does not exist
                {
                    ErrorMessage("\nThere is no item named " + itemName.Substring(words[0].Length).TrimStart());
                }
                else if (words.Length > 1 && pickedUpItem.IsContainer) // If multi word commmand and item is container
                {
                    IItemContainer pickedUpContainer = (IItemContainer)pickedUpItem; //the item changes into container class
                    try
                    {
                        IItem extraItem = pickedUpContainer.Remove(words[1]); //removes item from container
                        tooHeavy = _backpack.Weight + extraItem.Weight > WeightLimit;
                        tooBig = _backpack.Volume + extraItem.Volume > VolumeLimit;
                        if ((tooHeavy == false) && (tooBig == false) && !pickedUpContainer.IsMineable) //checks if you have enough space and if not ore deposit
                        {
                            Give(extraItem);
                            NormalMessage("\nYou picked up " + words[1] + " from " + words[0]);
                            CurrentRoom.Drop(pickedUpContainer); //drop container on ground
                        }
                        else if ( pickedUpContainer.IsMineable)
                        {
                            ErrorMessage("\nYou cannot pick up out of an ore deposit");
                            pickedUpContainer.Insert(extraItem); //puts item back
                            CurrentRoom.Drop(pickedUpContainer);
                        }
                        else
                        {
                            
                            ErrorMessage("\nYou cannot pick up " + pickedUpContainer.Name); //you can't pickup containers
                            CurrentRoom.Drop(pickedUpContainer);
                        }
                    }
                    catch
                    {
                        ErrorMessage("\nThere is no " + words[1] + " inside of " + words[0]); //No item inside the container
                        CurrentRoom.Drop(pickedUpItem);
                    }
                    words[0] = words[1];
                }
                else if (words.Length == 1 && pickedUpItem.IsContainer) //Pickup container denied
                {
                    ErrorMessage("\nYou may not pick up " + words[0] + "\n");
                    CurrentRoom.Drop(pickedUpItem);
                }

                else if (words.Length > 1 && !pickedUpItem.IsContainer) //First word is not a container so it can't hold items
                {
                    ErrorMessage("\n" + words[0] + " is not a container");
                }
                else
                {
                    if ((tooHeavy == false) && (tooBig == false)) //Pickup pass no container
                    {
                        Give(pickedUpItem);
                        NormalMessage("\nYou picked up " + words[0]);
                    }
                    else
                    {
                        CurrentRoom.Drop(pickedUpItem);           //Pickup fail
                    }
                }
                if (tooHeavy)
                {
                    WarningMessage("\n" + words[0] + " is too heavy!\n");
                }

                if (tooBig)
                {
                    WarningMessage("\nYou do not have enough space for " + words[0] + "\n");
                }
            }
            else
            {
                ErrorMessage("\nThere is no item named " + itemName.TrimEnd() + " on the floor\n"); //The item is not on the floor
            }
        }

        // Used to unlock locked doors
        public void Unlock(string exit)
        {
            int count = 0; //Counter
            bool bombCheck = (CurrentRoom.GetLockedStatus(exit) == 'B'); //checks if key is bomb
            if (CurrentRoom != null && (CurrentRoom.GetLockedStatus(exit) == 'T' || CurrentRoom.GetLockedStatus(exit) == 'B')) //checks if exit is locked
            {
                string[] items = _backpack.ItemNames.TrimStart().Split(" "); //gets all items from bag
                if (items[0].Length > 1)
                {
                    foreach (string item in items)
                    {
                        if (CurrentRoom.UnlockExit(exit, item)) //checks all items in for loop if true pass
                        {
                            count++;
                        }
                    }
                    switch (count)
                    {
                        case 0:
                            InfoMessage("\nYou do not have the item needed to unlock this door."); //if 0 you dont have the key
                            break;
                        case 1:
                            if (bombCheck)
                            {
                                InfoMessage("\nYou \"unlocked\" the rubble the old fashioned way... Brute force!"); //if bomb the rubble explodes
                                break;
                            }
                            else
                            {
                                InfoMessage("\nThe door has been unlocked!"); //if 1 and item isnt bomb the door is unlocked
                                break;
                            }
                        default:
                            break;
                    }
                    count = 0; 
                }
                else
                {
                    NormalMessage("\nYou have nothing in your inventory."); //cant unlock without inventory
                }
            }
            else if (CurrentRoom.GetLockedStatus(exit) == 'F')
            {
                ErrorMessage("\nThis door is not locked");
            }
            else if (CurrentRoom.GetLockedStatus(exit) == 'N')
            {
                ErrorMessage("\nThere is no exit at " + exit);
            }
            else if (CurrentRoom.GetLockedStatus(exit) == 'B')
            {
                ErrorMessage("\nYou're gonna need something with some blast power to \"unlock\" this direction...");
            }
        }

        //Drops items onto the floor
        public void Drop(string itemName)
        {
            IItem item = Take(itemName); //Takes item from backpack
            if (item != null)
            {
                CurrentRoom.Drop(item); //Drops in current room
                NormalMessage("\nYou dropped " + itemName);
            }
            else
            {
                ErrorMessage("\nThere is no item named " + itemName + " in the backpack");
            }
        }

        //Drops items on the floor quietly, this is used when we don't want the player to see every item drop at a time in YouDied
        public void Drop(string itemName, bool quiet)
        {
            IItem item = Take(itemName);
            if (item != null)
            {
                CurrentRoom.Drop(item);
            }
            else
            {
                ErrorMessage("\nThere is no item named " + itemName + " in the backpack");
            }
        }

        public void OutputMessage(string message)
        {
            Console.WriteLine(message);
        }

        public void ColoredMessage(string message, ConsoleColor newColor)
        {
            ConsoleColor oldColor = Console.ForegroundColor;
            Console.ForegroundColor = newColor;
            OutputMessage(message);
            Console.ForegroundColor = oldColor;
        }

        public void NormalMessage(string message)
        {
            ColoredMessage(message, ConsoleColor.Gray);
        }

        public void InfoMessage(string message)
        {
            ColoredMessage(message, ConsoleColor.Blue);
        }

        public void WarningMessage(string message)
        {
            ColoredMessage(message, ConsoleColor.DarkYellow);
        }

        public void ErrorMessage(string message)
        {
            ColoredMessage(message, ConsoleColor.Red);
        }

        public void SelectMessage(string message)
        {
            ColoredMessage(message, ConsoleColor.Green);
        }

        public void SpokenMessage(string message)
        {
            ColoredMessage(message, ConsoleColor.DarkCyan);
        }
        
    }

}

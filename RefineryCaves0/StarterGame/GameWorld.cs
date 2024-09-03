using System.Collections;
using System.Collections.Generic;
using System;

namespace StarterGame
{
    public class GameWorld
    {


        private static GameWorld _instance;
        public static GameWorld Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new GameWorld();
                }
                return _instance;
            }
        }

        private Room _entrance;
        public Room Entrance { get { return _entrance; } }
        //private Room _triggerRoom;
        //private Room _worldOut;
        //private Room _worldInAnnex;
        private Dictionary<Room, WorldEvent> worldEvents;

        public GameWorld()
        {
            worldEvents = new Dictionary<Room, WorldEvent>();
            CreateWorld();
            NotificationCenter.Instance.AddObserver("PlayerDidEnterRoom", PlayerDidEnterRoom);
        }

        public void PlayerDidEnterRoom(Notification notification)
        {
            Player player = (Player)notification.Object;
            if (player != null)
            {
                WorldEvent we = null;
                worldEvents.TryGetValue(player.CurrentRoom, out we);
                if (we != null)
                {
                    we.ExecuteEvent();
                    player.InfoMessage("\nUpon entering this room, your eyes are immediately drawn to a lever in the center of it.\n");
                    System.Threading.Thread.Sleep(1500);
                    player.WarningMessage("\nOf course, you immediately flick the lever.\n\n");
                    System.Threading.Thread.Sleep(1500);
                    player.InfoMessage("You can feel machinery moving underneath your feet...\n");
                    System.Threading.Thread.Sleep(1500);
                    worldEvents[we.Trigger] = null;
                }
                //player.WarningMessage("\n***Player is " + player.CurrentRoom.Tag);
            }
        }

        private class WorldEvent
        {

            private Room _trigger;
            public Room Trigger { get { return _trigger; } }
            private Room _worldOut;
            private Room _worldInAnnex;
            private string _directionFromWorld;
            private string _directionToWorld;

            public WorldEvent(Room trigger, Room worldOut, Room worldInAnnex, string directionFromWorld, string directionToWorld)
            {
                _trigger = trigger;
                _worldOut = worldOut;
                _worldInAnnex = worldInAnnex;
                _directionFromWorld = directionFromWorld;
                _directionToWorld = directionToWorld;
            }



            public void ExecuteEvent()
            {
                _worldOut.SetExit(_directionFromWorld, _worldInAnnex, null);
                _worldInAnnex.SetExit(_directionToWorld, _worldOut, null);
            }
        }

        public void CreateWorld()
        {
            //Homebase rooms
            Room homebase = new Room("at home base");
            Room shop = new Room("at the shop");
            Room teleport = new Room("at the teleport room");
            Room mine1 = new Room("at the entrance of Mythril Cave");
            Room mine2 = new Room("at the entrance of Adaman's Pass");

            //Mythril Cave rooms

            Room mine1a = new Room("in the Mythril Cave");
            Room mine1b = new Room("in the Mythril Cave");
            Room mine1c = new Room("in the Mythril Cave");
            Room rubbleRoom = new Room("in the Mythril Cave"); //Rubble Room
            Room mine1e = new Room("in the Mythril Cave");
            Room mine1f = new Room("in the Mythril Cave");
            Room mine1g = new Room("in the Mythril Cave");
            Room ladderRoom1 = new Room("in the Mythril Cave, there is a ladder going down");
            Room ladderRoom2 = new Room("in the Mythril Cave, there is a ladder going up");
            Room mine1j = new Room("in the Mythril Cave");
            Room mine1k = new Room("in the Mythril Cave");

            //Adaman's Pass rooms

            Room mine2a = new Room("in Adaman's Pass");
            Room mine2b = new Room("in Adaman's Pass");
            Room mine2c = new Room("in Adaman's Pass");
            Room mine2d = new Room("in Adaman's Pass");
            Room ladderRoom3 = new Room("in Adaman's Pass, there is a ladder going down");
            Room mine2f = new Room("in Adaman's Pass");
            Room mine2g = new Room("in Adaman's Pass");
            Room mine2h = new Room("in Adaman's Pass");
            Room mine2i = new Room("in Adaman's Pass");
            Room mine2j = new Room("in Adaman's Pass");
            Room mine2k = new Room("in Adaman's Pass");
            Room mine2l = new Room("in Adaman's Pass");
            Room mine2m = new Room("in Adaman's Pass, there is a lever in this room");
            Room ladderRoom4 = new Room("in Adaman's Pass, there is a ladder going up");
            Room mine2o = new Room("in Adaman's Pass");
            Room mine2p = new Room("in Adaman's Pass");
            Room mine2q = new Room("in Adaman's Pass");
            Room mine2r = new Room("in Adaman's Pass, chains and wiring span from the roof of the cave to the bottom"); //hint room
            Room mine2s = new Room("in Adaman's Pass");
            Room mine2t = new Room("in Adaman's Pass");
            Room ladderRoom5 = new Room("in Adaman's Pass, is a ladder going down");
            Room ladderRoom6 = new Room("in Adaman's Pass, is a ladder going up");
            Room mine2w = new Room("in Adaman's Pass");
            Room mine2x = new Room("in Adaman's Pass");
            Room mine2y = new Room("in Adaman's Pass");
            Room mine2z = new Room("in Adaman's Pass");
            Room mine2aa = new Room("in Adaman's Pass");
            Room mine2ab = new Room("in Adaman's Pass, there is a metal door in this room");
            Room bombRoom = new Room("in the bomb production facility in Adaman's Pass"); //Bomb Room

            //setup lever WorldEvent

            WorldEvent we = new WorldEvent(mine2m, mine2ab, bombRoom, "west", "east");
            worldEvents[we.Trigger] = we;

            //set homebase exits & locks

            homebase.SetExit("shop", shop, null);
            homebase.SetExit("teleport", teleport, null);
            homebase.SetExit("MythrilCave", mine1, null);
            homebase.SetExit("AdamansPass", mine2, null);

            homebase.LockExit("teleport", "TeleportKey");   //Exit Lock Teleport Room
            homebase.LockExit("AdamansPass", "AdamanKey"); //Exit Lock Adaman's Pass

            //teleportation random rooms

            teleport.SetExit("1", mine1c, null);        //Teleportation
            teleport.SetExit("2", mine1g, null);
            teleport.SetExit("3", ladderRoom2, null);
            teleport.SetExit("4", mine1e, null);
            teleport.SetExit("5", mine1, null);
            teleport.SetExit("6", mine2, null);
            teleport.SetExit("7", mine2g, null);
            teleport.SetExit("8", mine2d, null);
            teleport.SetExit("9", mine2j, null);
            teleport.SetExit("10", mine2p, null);
            teleport.SetExit("11", mine2w, null);

            //Mythril Cave exits

            mine1.SetExit("homebase", homebase, null);
            mine1.SetExit("east", mine1a, null);

            mine1a.SetExit("west", mine1, null);
            mine1a.SetExit("east", mine1b, null);

            mine1b.SetExit("west", mine1a, null);
            mine1b.SetExit("east", mine1c, null);
            mine1b.SetExit("south", mine1e, null);

            mine1c.SetExit("west", mine1b, null);
            mine1c.SetExit("rubble", rubbleRoom, null);

            mine1c.LockExit("rubble", "bomb");                //Exit Lock Rubble Room

            //Rubble Room no exit

            mine1e.SetExit("north", mine1b, null);
            mine1e.SetExit("west", mine1g, null);
            mine1e.SetExit("east", mine1f, null);

            mine1f.SetExit("west", mine1e, null);

            mine1g.SetExit("east", mine1e, null);
            mine1g.SetExit("south", ladderRoom1, null);

            ladderRoom1.SetExit("north", mine1g, null);
            ladderRoom1.SetExit("down", ladderRoom2, null);

            ladderRoom2.SetExit("up", ladderRoom1, null);
            ladderRoom2.SetExit("west", mine1j, null);

            mine1j.SetExit("east", ladderRoom2, null);
            mine1j.SetExit("north", mine1k, null);

            mine1k.SetExit("south", mine1j, null);

            //Adaman's Pass exits

            mine2.SetExit("homebase", homebase, null);
            mine2.SetExit("south", mine2a, null);

            mine2a.SetExit("north", mine2, null);
            mine2a.SetExit("west", mine2h, null);
            mine2a.SetExit("east", mine2b, null);

            mine2b.SetExit("west", mine2a, null);
            mine2b.SetExit("south", mine2c, null);

            mine2c.SetExit("north", mine2b, null);
            mine2c.SetExit("south", mine2d, null);

            mine2d.SetExit("north", mine2c, null);
            mine2d.SetExit("west", ladderRoom3, null);

            ladderRoom3.SetExit("east", mine2d, null);
            ladderRoom3.SetExit("down", ladderRoom4, null);
            ladderRoom3.SetExit("west", mine2f, null);

            mine2f.SetExit("east", ladderRoom3, null);
            mine2f.SetExit("north", mine2g, null);

            mine2g.SetExit("south", mine2f, null);
            mine2g.SetExit("north", mine2h, null);
            mine2g.SetExit("west", mine2i, null);

            mine2h.SetExit("south", mine2g, null);
            mine2h.SetExit("east", mine2a, null);

            mine2i.SetExit("east", mine2g, null);
            mine2i.SetExit("west", mine2j, null);

            mine2j.SetExit("east", mine2i, null);
            mine2j.SetExit("north", mine2k, null);
            mine2j.SetExit("west", mine2l, null);

            mine2k.SetExit("south", mine2j, null);

            mine2l.SetExit("east", mine2j, null);
            mine2l.SetExit("south", mine2m, null);

            mine2m.SetExit("north", mine2l, null); //Holds lever to bomb room

            ladderRoom4.SetExit("up", ladderRoom3, null);
            ladderRoom4.SetExit("west", mine2o, null);

            mine2o.SetExit("east", ladderRoom4, null);
            mine2o.SetExit("west", mine2p, null);

            mine2p.SetExit("east", mine2o, null);
            mine2p.SetExit("west", mine2q, null);
            mine2p.SetExit("south", mine2s, null);

            mine2q.SetExit("east", mine2p, null);
            mine2q.SetExit("west", mine2r, null);

            mine2r.SetExit("east", mine2q, null);

            mine2s.SetExit("north", mine2p, null);
            mine2s.SetExit("south", mine2t, null);

            mine2t.SetExit("north", mine2s, null);
            mine2t.SetExit("east", ladderRoom5, null);

            ladderRoom5.SetExit("east", mine2t, null);
            ladderRoom5.SetExit("down", ladderRoom6, null);

            ladderRoom6.SetExit("up", ladderRoom5, null);
            ladderRoom6.SetExit("east", mine2w, null);
            ladderRoom6.SetExit("west", mine2x, null);

            mine2w.SetExit("west", ladderRoom6, null);

            mine2x.SetExit("east", ladderRoom6, null);
            mine2x.SetExit("west", mine2y, null);

            mine2y.SetExit("east", mine2x, null);
            mine2y.SetExit("south", mine2z, null);
            mine2y.SetExit("north", mine2aa, null);

            mine2z.SetExit("north", mine2y, null);

            mine2aa.SetExit("south", mine2y, null);
            mine2aa.SetExit("west", mine2ab, null);

            mine2ab.SetExit("east", mine2aa, null);


            _entrance = homebase;                       //Entrance



            //Room Delegates

            //shop setup
            IItemContainer shopSelling = new ItemContainer("Shopkeep's Inventory", 0f, 1);  //Sets up Shop
            IItem item = new ItemClass("AdamanKey", 1f, 1, 50);                            
            shopSelling.Insert(item);
            item = new ItemClass("TeleportKey", 1f, 1, 250);
            shopSelling.Insert(item);
            ShopRoom shp = new ShopRoom(shopSelling);   
            shop.Drop(shopSelling);
            shop.RoomDelegate = shp;

            //teleport room setup

            TeleportRoom tele = new TeleportRoom();     
            teleport.RoomDelegate = tele;

            //winning room!

            WinRoom win = new WinRoom();
            rubbleRoom.RoomDelegate = win;



            //Chests

            IItemContainer chest = new ItemContainer("chest", 0f, 1);
            item = new ItemClass("bomb", 1f, 1, 0, true);
            chest.Insert(item);
            bombRoom.Drop(chest);                         //Key for Rubble

            //MINE 1 ORES
            IItemContainer ore1 = new ItemContainer("ore", 0f, 1, 0, true);
            mine1f.Drop(ore1);
            item = new ItemClass("coal", 2f, 3, 15);     
            ore1.Insert(item);

            IItemContainer ore2 = new ItemContainer("ore", 0f, 1, 0, true);
            mine1g.Drop(ore2);
            item = new ItemClass("gold", 15f, 5, 25);     
            ore2.Insert(item);

            IItemContainer ore3 = new ItemContainer("ore", 0f, 1, 0, true);
            mine1k.Drop(ore3);
            item = new ItemClass("iron", 6f, 8, 30);     
            ore3.Insert(item);

            //MINE 2 ORES
            IItemContainer ore4 = new ItemContainer("ore", 0f, 1, 0, true);
            mine2c.Drop(ore4);
            item = new ItemClass("silver", 5f, 5, 12);     
            ore4.Insert(item);

            IItemContainer ore5 = new ItemContainer("ore", 0f, 1, 0, true);
            mine2k.Drop(ore5);
            item = new ItemClass("crystal", 2.5f, 10, 15);     
            ore5.Insert(item);

            IItemContainer ore6 = new ItemContainer("ore", 0f, 1, 0, true);
            mine2f.Drop(ore6);
            item = new ItemClass("tungsten", 7.3f, 2, 10);     
            ore6.Insert(item);

            IItemContainer ore7 = new ItemContainer("ore", 0f, 1, 0, true);
            mine2l.Drop(ore7);
            item = new ItemClass("emerald", 10.5f, 5, 25);    
            ore7.Insert(item);

            IItemContainer ore8 = new ItemContainer("ore", 0f, 1, 0, true);
            mine2q.Drop(ore8);
            item = new ItemClass("ruby", 5.5f, 10, 25);   
            ore8.Insert(item);

            IItemContainer ore9 = new ItemContainer("ore", 0f, 1, 0, true);
            mine2s.Drop(ore9);
            item = new ItemClass("platinum", 40f, 12, 8);   
            ore9.Insert(item);

            IItemContainer ore10 = new ItemContainer("ore", 0f, 1, 0, true);
            mine2w.Drop(ore10);
            item = new ItemClass("cobalt", 8.4f, 15, 40);    
            ore10.Insert(item);

            IItemContainer ore11 = new ItemContainer("ore", 0f, 1, 0, true);
            mine2z.Drop(ore11);
            item = new ItemClass("titanium", 20f, 10, 50);    
            ore11.Insert(item);

            IItemContainer ore12 = new ItemContainer("ore", 0f, 1, 0, true);
            mine2aa.Drop(ore12);
            item = new ItemClass("diamond", 7.9f, 20, 65);     
            ore12.Insert(item);


            //Random items - Mine 1

            item = new ItemClass("rock", 1.2f, 2, 2);
            mine1c.Drop(item);

            item = new ItemClass("stick", 0.3f, 1, 1);
            IItem decorator = new ItemClass("broken handle", 0.01f, 4, 2);
            item.Decorate(decorator);
            mine1a.Drop(item);

            item = new ItemClass("fossil", 1.4f, 2, 10);
            mine1j.Drop(item);

            //Random items - Mine 2

            item = new ItemClass("bucket", 4f, 3, 5);
            decorator = new ItemClass("sand", 0.5f, 1, 1);
            item.Decorate(decorator);
            mine2h.Drop(item);

            item = new ItemClass("lunchbox", 5f, 5, 3);
            decorator = new ItemClass("tinfoil", 0.2f, 0, 6);
            item.Decorate(decorator);
            ladderRoom4.Drop(item);

            item = new ItemClass("copper", 0.5f, 1, 8);
            mine2r.Drop(item);

            item = new ItemClass("gear", 4f, 2, 10);
            mine2ab.Drop(item);

            item = new ItemClass("obsidian", 2f, 1, 12);
            ladderRoom6.Drop(item);

        }
    }
}
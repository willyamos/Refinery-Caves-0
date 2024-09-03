using System;
namespace StarterGame
{
    public class RoomProxy : IRoomProxy // "Proxy" Object-Oriented Design Pattern
    {
        private Room _currentRoom;

        public RoomProxy(Room currentRoom)
        {
            _currentRoom = currentRoom;
        }

        // Sends substitute in place of player to gather info and return
        public Room Zoom(string direction)
        {
            Room nextRoom = _currentRoom.GetExit(direction);
            return nextRoom;
        }
    }

}


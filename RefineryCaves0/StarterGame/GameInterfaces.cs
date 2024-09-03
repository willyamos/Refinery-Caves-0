using System;
namespace StarterGame
{
    public interface IRoomDelegate
    {
        public Room OnGetExit(Room room);

        public string OnGetExits(string room);

        public string OnTag(string room);

        public Room ContainingRoom { set; get; }

    }

    public interface IRoomProxy
    {
        public Room Zoom(string direction);
    }

    public interface IItem
    {
        public string Name { get; set; }

        public float Weight { get; }

        public int Volume { get; }

        public int Price { get; }

        public bool IsMineable { get; }

        public string Description { get; }

        public void Decorate(IItem decorator);

        public bool IsContainer { get; }

    }

    public interface IItemContainer : IItem
    {
        public bool Insert(IItem item);

        public IItem Remove(string itemName);

        public string ItemNames { get; }

        public string ItemNamesAndCosts { get;  }
    }
}


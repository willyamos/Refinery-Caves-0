using System;
using System.Collections.Generic;
using System.Text;

namespace StarterGame
{
    public class ItemClass : IItem
    {
        private string _name;

        public string Name { get { return _name; } set { _name = value; } }

        private float _weight;

        public float Weight { get { return _weight + (_decorator == null ? 0 : _decorator.Weight); } }

        private int _volume;

        public int Volume { get { return _volume; } }

        private int _price;

        public int Price { get { return _price; } }

        private bool _ismineable;

        public bool IsMineable { get { return _ismineable; } }

        public string Description { get { return Name + ", weight = " + Weight + ", volume = " + Volume; } }
        private IItem _decorator;
        public bool IsContainer { get { return false; } }

        public ItemClass() : this("Nameless") { }

        public ItemClass(string name) : this(name, 1f, 0) { }

        public ItemClass(string name, float weight) : this(name, weight, 0) { }

        public ItemClass(string name, float weight, int volume) : this(name, weight, volume, 0) { }

        public ItemClass(string name, float weight, int volume, int price) : this(name, weight, volume, price, false) { }

        //Designated constructor
        public ItemClass(string name, float weight, int volume, int price, bool ismineable)
        {
            Name = name;
            _weight = weight;
            _volume = volume;
            _decorator = null;
            _price = price;
            _ismineable = ismineable;
        }

        public void Decorate(IItem decorator)
        {
            if (_decorator == null)
            {
                _decorator = decorator;
            }
            else
            {
                _decorator.Decorate(decorator);
            }
        }
    }

    public class ItemContainer : ItemClass, IItemContainer
    {
        private Dictionary<string, IItem> _items;

        public new bool IsContainer { get { return true; } }

        public new float Weight
        {
            get
            {
                float myWeight = base.Weight;
                foreach (IItem item in _items.Values)
                {
                    myWeight += item.Weight; //Combines item weight on top of container weight
                }
                return myWeight;
            }
        }

        public new int Volume
        {
            get
            {
                int myVolume = base.Volume;
                foreach (IItem item in _items.Values)
                {
                    myVolume += item.Volume; //Combines item volume on top of container volume
                }
                return myVolume;
            }
        }

        public new string Description
        {
            get
            {
                string itemsNames = "";
                foreach (string name in _items.Keys)
                {
                    itemsNames += " " + name; //Item names are put in a list
                }
                return Name + ", weight = " + Weight + ", volume = " + Volume + "\n" + itemsNames; //Returns Description
            }
        }

        // Here for item names alone
        public string ItemNames
        {
            get
            {
                string itemsNames = "";
                foreach (string name in _items.Keys)
                {
                    itemsNames += " " + name;
                }
                return itemsNames; //Returns only names
            }
        }

        public string ItemNamesAndCosts
        {
            get
            {
                string itemsNamesAndCosts = "";
                foreach (IItem item in _items.Values)
                {
                    itemsNamesAndCosts += "\n" + item.Name + " " + item.Price + " coins";
                }
                return itemsNamesAndCosts; //Returns Items + Prices
            }
        }

        public ItemContainer() : this("nameless") { }

        public ItemContainer(string name) : this(name, 1f) { }

        public ItemContainer(string name, float weight) : this(name, weight, 0) { }

        public ItemContainer(string name, float weight, int volume) : this(name, weight, volume, 0) { }

        public ItemContainer(string name, float weight, int volume, int price) : this(name, weight, volume, 0, false) { }
        //Designated Constructor
        public ItemContainer(string name, float weight, int volume, int price, bool ismineable) : base(name, weight, volume, price, ismineable)
        {
            _items = new Dictionary<string, IItem>();
        }

        public bool Insert(IItem item)
        {
            _items[item.Name] = item;
            return true;
        }

        public IItem Remove(string itemName)
        {
            IItem itemToRemove = null;
            _items.TryGetValue(itemName, out itemToRemove);
            if (itemToRemove != null)
            {
                _items.Remove(itemName);
            }
            return itemToRemove;
        }
    }
}


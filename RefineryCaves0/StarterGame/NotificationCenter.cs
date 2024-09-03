using System;
using System.Collections.Generic;

namespace StarterGame;

public class NotificationCenter
{
    private Dictionary<String, EventContainer> _observers;
    private static NotificationCenter _instance;
    public static NotificationCenter Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new NotificationCenter();
            }
            return _instance;
        }
    }
    public NotificationCenter()
    {
        _observers = new Dictionary<string, EventContainer>();
    }
    private class EventContainer
    {
        private event Action<Notification> Observer;
        public EventContainer() { }

        public void AddObserver(Action<Notification> observer)
        {
            Observer += observer;
        }
        public void RemoveObserver(Action<Notification> observer)
        {
            Observer -= observer;
        }
        public void SendNotification(Notification notification)
        {
            Observer(notification);
        }
        public bool IsEmpty()
        {
            return Observer == null;
        }
    }
    public void AddObserver(String notificationName, Action<Notification> observer)
    {
        if (!_observers.ContainsKey(notificationName))
        {
            _observers[notificationName] = new EventContainer();
        }
        _observers[notificationName].AddObserver(observer);
    }
    public void RemoveObserver(String notificationName, Action<Notification> observer)
    {
        if (_observers.ContainsKey(notificationName))
        {
            _observers[notificationName].RemoveObserver(observer);
            if (_observers[notificationName].IsEmpty())
            {
                _observers.Remove(notificationName);
            }
        }
    }
    public void PostNotification(Notification notification)
    {
        if (_observers.ContainsKey(notification.Name))
        {
            _observers[notification.Name].SendNotification(notification);
        }
    }
}
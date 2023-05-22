using Assets.DesignPatterns.ObserverPattern;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts {
    public class PlanetEntity : MonoBehaviour, Observable<PlanetEntity> {

        private readonly List<Observer<PlanetEntity>> observers = new List<Observer<PlanetEntity>>();

        private void OnMouseDown()
        {
            NotifyObservers();
        }

        public void SubscribeObserver(Observer<PlanetEntity> observer)
        {
            observers.Add(observer);
        }

        public void UnsubscribeObserver(Observer<PlanetEntity> observer)
        {
            observers.Remove(observer);
        }

        public void NotifyObservers()
        {
            foreach(var observer in observers) {
                observer.SendUpdate(this);
            }
        }

    }
}
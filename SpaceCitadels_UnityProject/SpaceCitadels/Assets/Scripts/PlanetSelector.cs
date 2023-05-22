using Assets.DesignPatterns.ObserverPattern;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Assets.Scripts {
    public class PlanetSelector : MonoBehaviour, Observer<PlanetEntity> {
        
        public static PlanetSelector instance;

        private static PlanetEntity currentlySelectedPlanet;

        public static PlanetEntity CurrentlySelectedPlanet
        {
            get { return currentlySelectedPlanet; }
        }

        private void Awake()
        {
            if(instance != null) {
                Debug.LogError("Multiple instances of PlanetSelector found!");
                return;
            }
            instance = this;
        }

        private void Start()
        {
            List<PlanetEntity> planetsList = MapGeneration_0_2.MapGenerator_0_2.instance.planetsList;
            foreach(var planetEntity in planetsList) {
                planetEntity.SubscribeObserver(this);
            }
        }

        public void SendUpdate(PlanetEntity notifyObject)
        {
            OnPlanetSelectionUpdate(notifyObject);
        }

        private void OnPlanetSelectionUpdate(PlanetEntity notifyObject)
        {
            currentlySelectedPlanet = notifyObject;

            PlayerMovement_0_2.instance.OnMoveCalled(notifyObject.transform.position);
        }
    }
}
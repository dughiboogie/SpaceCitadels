using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.MapGeneration_0_2 {
    public class MapGenerator_0_2 : MonoBehaviour {

        // Distance between planets
        private const float PATH_MAGNITUDE = 10f;
        
        // Degree that separates nearby planets
        private const int ANGLE_DEGREE_STEP = 60;

        // Planet spawning chance max threshold
        private const int PLANET_SPAWN_MAX_CHANCE = 100;
        // Spawning chance of the nearby planets
        private const int PLANET_SPAWN_CHANCE = 60;

        // List of spawned planets
        public readonly List<PlanetEntity> planetsList = new List<PlanetEntity>();

        // Position of the first planet to be spawned
        public Vector3 origin = Vector3.zero;

        public static MapGenerator_0_2 instance;

        [SerializeField]
        private PlanetEntity planetEntityPrefab;

        private void Awake()
        {
            if(instance != null) {
                Debug.LogError("Multiple instances of MapGenerator_0_2 found!");
                return;
            }
            instance = this;

            PlanetEntity originPlanet = MakePlanet(origin);
            // originPlanet.name = originPlanet.name + "_0";

            SpawnNearbyPlanets();
        }

        private void SpawnNearbyPlanets()
        {
            int vertexCount = 360 / ANGLE_DEGREE_STEP;

            for(int directionIndex = 0; directionIndex < vertexCount; directionIndex++) {

                int planetSpawnRoll = UnityEngine.Random.Range(0, PLANET_SPAWN_MAX_CHANCE);
                if(planetSpawnRoll < PLANET_SPAWN_CHANCE) {
                    Vector3 planetPosition = CalculatePlanetPosition(directionIndex);
                    PlanetEntity planet = MakePlanet(planetPosition);
                    planet.name = planet.name + '_' + (directionIndex + 1);
                }
            }
        }

        private Vector3 CalculatePlanetPosition(int directionIndex)
        {
            var angleDeg = ANGLE_DEGREE_STEP * directionIndex;
            var angleRad = Math.PI / 180 * angleDeg;
            float xCoord = (float)(origin.x + PATH_MAGNITUDE * Math.Cos(angleRad));
            float zCoord = (float)(origin.z + PATH_MAGNITUDE * Math.Sin(angleRad));
            return new Vector3(xCoord, 0, zCoord);
        }

        private PlanetEntity MakePlanet(Vector3 position)
        {
            PlanetEntity planet = Instantiate(planetEntityPrefab, position, Quaternion.identity);
            // planet.name = "Planet";

            // Add spawned planet to list of planets
            planetsList.Add(planet);

            return planet;
        }

    }
}
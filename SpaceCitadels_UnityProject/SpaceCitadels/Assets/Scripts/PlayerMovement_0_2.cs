using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts {
    public class PlayerMovement_0_2 : MonoBehaviour {

        public static PlayerMovement_0_2 instance;

        public readonly float MOVEMENT_SPEED = 0.1f;

        private Vector3 currentDestination;
        private float movementInterpolationValue = 0f;

        private void Awake()
        {
            if(instance != null) {
                Debug.LogError("Multiple instances of PlayerMovement_0_2 found!");
                return;
            }
            instance = this;
        }

        private void Start()
        {
            List<PlanetEntity> planetsList = MapGeneration_0_2.MapGenerator_0_2.instance.planetsList;
            currentDestination = transform.position;
        }

        private void Update()
        {
            if(transform.position != currentDestination) {
                MovePosition();
            }
        }

        public void OnMoveCalled(Vector3 planetPosition)
        {
            currentDestination = new Vector3(planetPosition.x, planetPosition.y + transform.position.y, planetPosition.z);
        }

        private void MovePosition()
        {
            transform.position = Vector3.Lerp(transform.position, currentDestination, movementInterpolationValue);

            movementInterpolationValue += MOVEMENT_SPEED * Time.deltaTime;

            if(transform.position == currentDestination) {
                movementInterpolationValue = 0f;
            }
        }
    }
}
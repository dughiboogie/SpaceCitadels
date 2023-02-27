using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridGenerator : MonoBehaviour
{
    private float pathSize = 1.0f;

    // Special nodes
    private MapNode firstMapNode;
    
    // Map with ALL the nodes generated in the current map (empty and full)
    private Dictionary<MapNode, System.Object> mapElements = new Dictionary<MapNode, System.Object>();

    private void Awake()
    {
        GenerateStartingPlanet();
        GenerateMapNodesNextToCurrent(firstMapNode);    
    }

    private void GenerateStartingPlanet()
    {
        GameObject planet = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        planet.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);

        firstMapNode = new MapNode(0, 0, 0, 0);
        planet.transform.position = firstMapNode.UnityPosition;

        mapElements.Add(firstMapNode, planet);
    }

    private void GenerateMapNodesNextToCurrent(MapNode currentMapNode)
    {
        // Generate nodes starting from the right one (x+1,x) clockwise
        for(int directionIndex = 0; directionIndex < 6; directionIndex++) {
            var nextMapNodeCoordinates = CalculateNextMapNodeCoordinates(currentMapNode, directionIndex);

            var angleDeg = 60 * directionIndex;
            var angleRad = Math.PI / 180 * angleDeg;
            float xUnityCoord = (float)(currentMapNode.XCoordinate + pathSize * Math.Cos(angleRad));
            float zUnityCoord = (float)(currentMapNode.ZCoordinate + pathSize * Math.Sin(angleRad));

            mapElements.Add(new MapNode(nextMapNodeCoordinates.Item1, nextMapNodeCoordinates.Item2, xUnityCoord, zUnityCoord), null);
        }

        // Draw grid lines for 5 minutes, just for visual reference while prototyping
        foreach(var mapElement in mapElements) {
            if(!mapElement.Key.Equals(firstMapNode)) {
                Debug.DrawLine(firstMapNode.UnityPosition, mapElement.Key.UnityPosition, Color.yellow, 300);
            }
        }
    }

    private (int, int) CalculateNextMapNodeCoordinates(MapNode currentMapNode, int directionIndex)
    {
        int xNodeCoordinate = 0;
        int zNodeCoordinate = 0;

        switch(directionIndex) {
            case 0:
                xNodeCoordinate = currentMapNode.XCoordinate + 1;
                zNodeCoordinate = currentMapNode.ZCoordinate;
                break;
            case 1:
                xNodeCoordinate = currentMapNode.XCoordinate + 1;
                zNodeCoordinate = currentMapNode.ZCoordinate - 1;
                break;
            case 2:
                xNodeCoordinate = currentMapNode.XCoordinate - 1;
                zNodeCoordinate = currentMapNode.ZCoordinate - 1;
                break;
            case 3:
                xNodeCoordinate = currentMapNode.XCoordinate -1;
                zNodeCoordinate = currentMapNode.ZCoordinate;
                break;
            case 4:
                xNodeCoordinate = currentMapNode.XCoordinate - 1;
                zNodeCoordinate = currentMapNode.ZCoordinate + 1;
                break;
            case 5:
                xNodeCoordinate = currentMapNode.XCoordinate + 1;
                zNodeCoordinate = currentMapNode.ZCoordinate + 1;
                break;
        }
        return (xNodeCoordinate, zNodeCoordinate);
    }

    /*
    [SerializeField]
    private float hexSize = 1.0f;

    private List<Vector3> hexVertices = new List<Vector3>();

    private void Awake()
    {
        GenerateGrid();
        PlacePlanets();
    }
    */

    /*
     * Generate the corners of the hex grid, and store them in hexVertices list
     *
    private void GenerateGrid()
    {
        for(int i = 0; i < 3; i++) {
            var angleDeg = 60 * i;
            var angleRad = Math.PI / 180 * angleDeg;
            float xCoord = (float)(gridCenter.x + hexSize * Math.Cos(angleRad));
            float zCoord = (float)(gridCenter.z + hexSize * Math.Sin(angleRad));
            hexVertices.Add(new Vector3(xCoord, 0, zCoord));
        }

        // Draw grid lines for 5 minutes, just for visual reference while prototyping
        for(int i = 0; i < hexVertices.Count - 1; i++) {
            Debug.DrawLine(hexVertices[i], hexVertices[i + 1], Color.yellow, 300);
        }
        Debug.DrawLine(hexVertices.Last(), hexVertices[0], Color.yellow, 300);
    }
    */

    /*
     * Place "planets" on the corners of the grid just generated
     *
    private void PlacePlanets()
    {
        foreach(var vertex in hexVertices) {
            GameObject planet = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            planet.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            planet.transform.position = vertex;
        }
    }
    */
}

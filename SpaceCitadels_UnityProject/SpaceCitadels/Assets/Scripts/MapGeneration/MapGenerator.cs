using System;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    private MapModel mapModel;

    private float pathSize = 10.0f;

    private MapNode currentElaboratingNode;

    private List<GameObject> possibleStartingNodes = new List<GameObject>();

    [SerializeField]
    private Transform mapElementsParent;
    [SerializeField]
    private MapNodeView mapNodePrefab;
    [SerializeField]
    private int mapGenerationIterations = 3;
    [SerializeField]
    private int maxDistanceFromBoss = 2;
    [SerializeField]
    private int startingDistanceFromBoss = 1;

    private void Awake()
    {
        mapModel = new MapModel();

        // Start generating map from boss node
        currentElaboratingNode = GenerateMapNode(0, 0, new Vector3(), 0, NodeTypes.BOSS_ROOM);

        for(int nodeIndex = 0; nodeIndex < mapGenerationIterations; nodeIndex++) {
            if(IsMapNodeAtMaxDistanceFromBoss()) {
                MoveToPreviousVisitedNode();
            }
            else {
                GenerateNearbyMapNodes();
                MakeRandomNearbyNodesReachable();

                currentElaboratingNode.HasAlreadyBeenElaborated = true;

                MoveToNextNodeToElaborate();
            }
        }

        foreach(var mapNode in mapModel.GetMapNodes()) {

            GameObject mapNodeGameObject = Instantiate(mapNodePrefab.gameObject, mapElementsParent, true);
            mapNodeGameObject.transform.position = mapNode.UnityPosition;

            mapNodeGameObject.GetComponent<MapNodeView>().MapNodeData = mapNode;

            /*
            GameObject reachableNodePlaceholder = GameObject.CreatePrimitive(PrimitiveType.Cube);
            reachableNodePlaceholder.transform.position = mapNode.UnityPosition;
            reachableNodePlaceholder.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            */

            foreach(var reachableMapNode in mapNode.reachableNodes) {
                Debug.DrawLine(mapNode.UnityPosition, reachableMapNode.UnityPosition, Color.yellow, 300);
            }

            if(mapNode.NodeType == NodeTypes.BOSS_ROOM) {
                mapNodeGameObject.GetComponent<Renderer>().material.SetColor("_Color", Color.red);
            }

            if(mapNode.NodeType == NodeTypes.UNREACHABLE) {
                mapNodeGameObject.GetComponent<Renderer>().material.SetColor("_Color", Color.black);
            }

            if(IsMapNodePossibleStartingPosition(mapNode)) {
                possibleStartingNodes.Add(mapNodeGameObject);
            }
        }

        var startingNodeIndex = UnityEngine.Random.Range(0, possibleStartingNodes.Count);
        possibleStartingNodes[startingNodeIndex].GetComponent<Renderer>().material.SetColor("_Color", Color.green);

    }

    private MapNode GenerateMapNode(int xCoordinate, int zCoordinate, Vector3 unityPosition, int distanceFromBoss, NodeTypes nodeType = NodeTypes.UNREACHABLE)
    {
        MapNode newMapNode = new MapNode(xCoordinate, zCoordinate, unityPosition);
        newMapNode.DistanceFromBoss = distanceFromBoss;
        newMapNode.SetNodeType(nodeType);

        mapModel.AddMapNode(newMapNode);
        return newMapNode;
    }

    private void GenerateNearbyMapNodes()
    {
        // Generate nodes starting from the right one (x+1,x) clockwise
        for(int directionIndex = 0; directionIndex < 6; directionIndex++) {
            MapNode adjacentMapNode = null;
            var nextMapNodeCoordinates = CalculateNextMapNodeCoordinates(currentElaboratingNode, directionIndex);

            // If the coordinates for the map node to generate are already present in the map model, use the MapNode stored there
            foreach(var alreadyGeneratedMapNode in mapModel.GetMapNodes()) {
                if(alreadyGeneratedMapNode.XCoordinate == nextMapNodeCoordinates.Item1 && alreadyGeneratedMapNode.ZCoordinate == nextMapNodeCoordinates.Item2) {
                    adjacentMapNode = alreadyGeneratedMapNode;
                    break;
                }
            }

            if(adjacentMapNode == null) {
                var angleDeg = 60 * directionIndex;
                var angleRad = Math.PI / 180 * angleDeg;
                float xUnityCoord = (float)(currentElaboratingNode.UnityPosition.x + pathSize * Math.Cos(angleRad));
                float zUnityCoord = (float)(currentElaboratingNode.UnityPosition.z + pathSize * Math.Sin(angleRad));

                adjacentMapNode = GenerateMapNode(nextMapNodeCoordinates.Item1, nextMapNodeCoordinates.Item2, new Vector3(xUnityCoord, 0, zUnityCoord),
                currentElaboratingNode.DistanceFromBoss + 1);
            }

            currentElaboratingNode.surroundingNodes.Add(adjacentMapNode);
        }
    }

    private void MakeRandomNearbyNodesReachable()
    {
        // If the current node is the boss one, set ONLY ONE other node to reachable
        if(currentElaboratingNode.NodeType == NodeTypes.BOSS_ROOM) {
            var randomNodeRoll = UnityEngine.Random.Range(0, currentElaboratingNode.surroundingNodes.Count);
            MapNode onlyReachableNode = currentElaboratingNode.surroundingNodes[randomNodeRoll];
            onlyReachableNode.SetNodeType(NodeTypes.REACHABLE);
            currentElaboratingNode.reachableNodes.Add(onlyReachableNode);
        }
        // If not, set nodes to reachable based on a random roll
        else {
            for(int directionIndex = 0; directionIndex < currentElaboratingNode.surroundingNodes.Count; directionIndex++) {

                // Don't roll for a node that was already elaborated previously
                if(currentElaboratingNode.surroundingNodes[directionIndex].NodeType == NodeTypes.UNREACHABLE) {
                    // 40% chance
                    var isNodeReachable = UnityEngine.Random.Range(0, 10) > 6 ? true : false;

                    if(isNodeReachable) {
                        MapNode nextReachableNode = currentElaboratingNode.surroundingNodes[directionIndex];
                        nextReachableNode.SetNodeType(NodeTypes.REACHABLE);
                        currentElaboratingNode.reachableNodes.Add(nextReachableNode);
                    }
                }
                else {
                    if(currentElaboratingNode.surroundingNodes[directionIndex].NodeType != NodeTypes.BOSS_ROOM) {
                        currentElaboratingNode.reachableNodes.Add(currentElaboratingNode.surroundingNodes[directionIndex]);
                    }
                }
            }
        }
    }

    private void MoveToNextNodeToElaborate()
    {
        // No reachable nodes near current one
        if(currentElaboratingNode.reachableNodes.Count == 0) {
            return;
        }
       
        var randomNodeRoll = UnityEngine.Random.Range(0, currentElaboratingNode.reachableNodes.Count);

        // Don't move to not reachable nodes or previously visited ones
        /*
         * || currentElaboratingNode.reachableNodes[randomNodeRoll] == currentElaboratingNode.PreviouslyVisitedNode
         */
        if(currentElaboratingNode.reachableNodes[randomNodeRoll].NodeType != NodeTypes.REACHABLE) {
            MoveToNextNodeToElaborate();
        }
        else {
            // make current node previous one
            MapNode temp = currentElaboratingNode;

            // make next node current elaborating one
            currentElaboratingNode = currentElaboratingNode.reachableNodes[randomNodeRoll];
            currentElaboratingNode.PreviouslyVisitedNode = temp;
        }

        // Keep searching for a node that hasn't been elaborated yet
        /*
        if(currentElaboratingNode.HasAlreadyBeenElaborated) {
            MoveToNextNodeToElaborate();
        }
        */
    }

    private void MoveToPreviousVisitedNode()
    {
        currentElaboratingNode = currentElaboratingNode.PreviouslyVisitedNode;
    }

    private bool IsMapNodePossibleStartingPosition(MapNode mapNode)
    {
        if(mapNode.DistanceFromBoss == startingDistanceFromBoss) {
            return true;
        }
        return false;
    }

    private bool IsMapNodeAtMaxDistanceFromBoss()
    {
        if(currentElaboratingNode.DistanceFromBoss == maxDistanceFromBoss) {
            return true;
        }
        return false;
    }



    /*

    private MapNode ChooseRandomReachableNode()
    {
        MapNode chosenNode = null;
        var randomNodeRoll = UnityEngine.Random.Range(0, reachableNodes.Count);

        int currentNodeIndex = 0;

        foreach(MapNode current in reachableNodes.Keys) {
            if(currentNodeIndex == randomNodeRoll) {
                chosenNode = current;

                // Just for debugging
                reachableNodes[chosenNode].GetComponent<Renderer>().material.SetColor("_Color", Color.red);
            }
            
            currentNodeIndex++;
        }

        return chosenNode;
    }

    private void SpawnReachableNodes()
    {
        Dictionary<MapNode, GameObject> mapWithNodePlaceholder = new Dictionary<MapNode, GameObject>();

        foreach(MapNode current in reachableNodes.Keys) {
            if(reachableNodes[current] == null) {
                GameObject reachableNodePlaceholder = GameObject.CreatePrimitive(PrimitiveType.Cube);
                reachableNodePlaceholder.transform.position = current.UnityPosition;
                reachableNodePlaceholder.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);

                mapWithNodePlaceholder.Add(current, reachableNodePlaceholder);
            }
        }

        reachableNodes = mapWithNodePlaceholder;
    }

    private void RollForNodeIsReachable()
    {
        Dictionary<MapNode, GameObject> actualReachableNodes = new Dictionary<MapNode, GameObject>();

        foreach(MapNode current in mapNodes) {
            var reachabilityChance = UnityEngine.Random.Range(0, 5);

            // 40% chance
            if(reachabilityChance > 1) {
                actualReachableNodes.Add(current, null);
            }
        }

        reachableNodes = actualReachableNodes;
    }

    private void GenerateStartingPlanet()
    {
        firstMapNode = new MapNode(0, 0, 0, 0);
        mapNodes.Add(firstMapNode);
        reachableNodes.Add(firstMapNode, null);
        previousNode = firstMapNode;
        currentWorkingNode = firstMapNode;
        
        SpawnReachableNodes();
        reachableNodes.Clear();
    }

    private GameObject SpawnPlanet(Vector3 planetCenter)
    {
        GameObject planet = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        planet.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        planet.transform.position = planetCenter;

        return planet;
    }

    private void GenerateMapNodesNextToCurrent(MapNode currentMapNode)
    {
        reachableNodes.Clear();

        // Generate nodes starting from the right one (x+1,x) clockwise
        for(int directionIndex = 0; directionIndex < 6; directionIndex++) {
            var nextMapNodeCoordinates = CalculateNextMapNodeCoordinates(currentMapNode, directionIndex);

            var angleDeg = 60 * directionIndex;
            var angleRad = Math.PI / 180 * angleDeg;
            float xUnityCoord = (float)(currentMapNode.XCoordinate + pathSize * Math.Cos(angleRad));
            float zUnityCoord = (float)(currentMapNode.ZCoordinate + pathSize * Math.Sin(angleRad));

            MapNode newMapNode = new MapNode(nextMapNodeCoordinates.Item1, nextMapNodeCoordinates.Item2, xUnityCoord, zUnityCoord);
            mapNodes.Add(newMapNode);
            reachableNodes.Add(newMapNode, null);
        }

        // Draw grid lines for 5 minutes, just for visual reference while prototyping
        foreach(var mapElement in reachableNodes.Keys) {
            if(!mapElement.Equals(currentMapNode) || !mapElement.Equals(previousNode)) {
                Debug.DrawLine(currentMapNode.UnityPosition, mapElement.UnityPosition, Color.yellow, 300);
            }
        }
    }

    */

    private (int, int) CalculateNextMapNodeCoordinates(MapNode currentMapNode, int directionIndex)
    {
        switch(directionIndex) {
            case 0:
                return currentMapNode.GetNextNodeBasedOnDirectionCoordinates(2, 0);
            case 1:
                return currentMapNode.GetNextNodeBasedOnDirectionCoordinates(1, -1);
            case 2:
                return currentMapNode.GetNextNodeBasedOnDirectionCoordinates(-1, -1);
            case 3:
                return currentMapNode.GetNextNodeBasedOnDirectionCoordinates(-2, 0);
            case 4:
                return currentMapNode.GetNextNodeBasedOnDirectionCoordinates(-1, +1);
            case 5:
                return currentMapNode.GetNextNodeBasedOnDirectionCoordinates(1, 1);
            default:
                throw new NotImplementedException("Very unlikely to happen");
        }
    }

    private void LogCurrentMapNodeCoordinates(string message)
    {
        Debug.Log(message);
        Debug.Log("X coordinate: " + currentElaboratingNode.XCoordinate);
        Debug.Log("Z coordinate: " + currentElaboratingNode.ZCoordinate);
    }

    private void LogListOfMapNodes(String message, ICollection<MapNode> mapNodes)
    {
        Debug.Log(message);

        foreach(MapNode current in mapNodes) {
            Debug.Log("X coordinate: " + current.XCoordinate);
            Debug.Log("Z coordinate: " + current.ZCoordinate);
            Debug.Log("------------------------------------");
        }
    }
}

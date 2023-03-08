using System;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    private float pathSize = 10.0f;

    private MapNode currentElaboratingNode;

    private List<MapNodeView> possibleStartingNodes = new List<MapNodeView>();

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
        GenerateNewMap();
    }

    private void GenerateNewMap()
    {
        MapModel.Clear();
        GenerateMapNodes();
        InstantiateMapNodesGameObjects();
        SetStartingPosition();
    }

    private void SetStartingPosition()
    {
        var startingNodeIndex = UnityEngine.Random.Range(0, possibleStartingNodes.Count);
        possibleStartingNodes[startingNodeIndex].GetComponent<Renderer>().material.SetColor("_Color", Color.green);

        PlayerMovement.instance.transform.position = possibleStartingNodes[startingNodeIndex].MapNodeData.UnityPosition;
        PlayerMovement.instance.CurrentMapNodeView = possibleStartingNodes[startingNodeIndex];
    }

    private void InstantiateMapNodesGameObjects()
    {
        foreach(var mapNode in MapModel.GetMapNodes()) {

            GameObject mapNodeGameObject = Instantiate(mapNodePrefab.gameObject, mapElementsParent, true);
            mapNodeGameObject.transform.position = mapNode.UnityPosition;

            MapNodeView mapNodeView = mapNodeGameObject.GetComponent<MapNodeView>();
            mapNodeView.MapNodeData = mapNode;

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
                possibleStartingNodes.Add(mapNodeView);
            }
        }
    }

    private void GenerateMapNodes()
    {
        // Start generating map from boss node
        currentElaboratingNode = GenerateIndividualMapNode(0, 0, new Vector3(), 0, NodeTypes.BOSS_ROOM);

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
    }

    private MapNode GenerateIndividualMapNode(int xCoordinate, int zCoordinate, Vector3 unityPosition, int distanceFromBoss, NodeTypes nodeType = NodeTypes.UNREACHABLE)
    {
        MapNode newMapNode = new MapNode(xCoordinate, zCoordinate, unityPosition);
        newMapNode.DistanceFromBoss = distanceFromBoss;
        newMapNode.SetNodeType(nodeType);

        MapModel.AddMapNode(newMapNode);
        return newMapNode;
    }

    private void GenerateNearbyMapNodes()
    {
        // Generate nodes starting from the right one (x+1,x) clockwise
        for(int directionIndex = 0; directionIndex < 6; directionIndex++) {
            MapNode adjacentMapNode = null;
            var nextMapNodeCoordinates = CalculateNextMapNodeCoordinates(currentElaboratingNode, directionIndex);

            // If the coordinates for the map node to generate are already present in the map model, use the MapNode stored there
            foreach(var alreadyGeneratedMapNode in MapModel.GetMapNodes()) {
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

                adjacentMapNode = GenerateIndividualMapNode(nextMapNodeCoordinates.Item1, nextMapNodeCoordinates.Item2, new Vector3(xUnityCoord, 0, zUnityCoord),
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

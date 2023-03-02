using System.Collections.Generic;
using UnityEngine;

public class MapNode
{
    // Virtual coordinates of the current node
    private int xCoordinate;
    private int zCoordinate;

    // Actual position of the node in Unity terms
    private Vector3 unityPosition;

    // Surrounding nodes are all the nodes near the current one
    public List<MapNode> surroundingNodes = new List<MapNode>();
    // Reachable nodes are all surrounding nodes that are actually reachable
    public List<MapNode> reachableNodes = new List<MapNode>();

    private MapNode previouslyVisitedNode = null;

    private int distanceFromBoss;

    private GameObject nodeContent;

    private NodeTypes nodeType;

    #region Properties

    public int XCoordinate => xCoordinate;

    public int ZCoordinate => zCoordinate;

    public Vector3 UnityPosition => unityPosition;

    public int DistanceFromBoss
    {
        get => distanceFromBoss;
        set => distanceFromBoss = value;
    }

    public MapNode PreviouslyVisitedNode
    {
        get => previouslyVisitedNode;
        set => previouslyVisitedNode = value;
    }

    public GameObject NodeContent
    {
        get => nodeContent;
        set => nodeContent = value;
    }

    public NodeTypes NodeType => nodeType;

    #endregion


    public MapNode(int xNodeCoordinate, int zNodeCoordinate, Vector3 unityPosition)
    {
        xCoordinate = xNodeCoordinate;
        zCoordinate = zNodeCoordinate;
        this.unityPosition = unityPosition;
    }

    public bool Equals(MapNode other)
    {
        return xCoordinate == other.xCoordinate && zCoordinate == other.zCoordinate;
    }

    public (int, int) GetNextNodeBasedOnDirectionCoordinates(int xDirection, int zDirection)
    {
        return (xCoordinate + xDirection, zCoordinate + zDirection);
    }

    public void SetNodeType(NodeTypes nodeType)
    {
        this.nodeType = nodeType;
    }
}

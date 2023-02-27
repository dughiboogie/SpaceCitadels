using UnityEngine;

public class MapNode
{
    // Virtual coordinates of the current node
    private int xCoordinate;
    private int zCoordinate;

    // Actual position of the node in Unity terms
    private Vector3 unityPosition;

    #region Properties

    public int XCoordinate
    {
        get => xCoordinate;
    }

    public int ZCoordinate
    {
        get => zCoordinate;
    }

    public Vector3 UnityPosition
    {
        get => unityPosition;
    }

    #endregion


    public MapNode(int xNodeCoordinate, int zNodeCoordinate, float xUnityPosition, float zUnityPosition)
    {
        xCoordinate = xNodeCoordinate;
        zCoordinate = zNodeCoordinate;
        unityPosition = new Vector3(xUnityPosition, 0, zUnityPosition);
    }

    public bool Equals(MapNode other)
    {
        return xCoordinate == other.xCoordinate && zCoordinate == other.zCoordinate;
    }
}

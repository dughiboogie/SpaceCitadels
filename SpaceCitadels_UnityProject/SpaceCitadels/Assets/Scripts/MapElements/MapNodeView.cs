using UnityEngine;

public class MapNodeView : MonoBehaviour
{
    private MapNode mapNodeData;

    public MapNode MapNodeData
    {
        get => mapNodeData;
        set => mapNodeData = value;
    }

    private void OnMouseDown()
    {
        Debug.Log("Selected node coordinates: ");
        Debug.Log("X coordinate: " + mapNodeData.XCoordinate);
        Debug.Log("Z coordinate: " + mapNodeData.ZCoordinate);
    }


}

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
        if(PlayerMovement.instance.CurrentMapNodeView.mapNodeData.reachableNodes.Contains(mapNodeData)) {
            PlayerMovement.instance.CurrentMapNodeView = this;
        }
        else {
            Debug.LogWarning("Selected node is unreachable from this position!");
        }

        Debug.Log("Selected node coordinates: ");
        Debug.Log("X coordinate: " + mapNodeData.XCoordinate);
        Debug.Log("Z coordinate: " + mapNodeData.ZCoordinate);
    }


}

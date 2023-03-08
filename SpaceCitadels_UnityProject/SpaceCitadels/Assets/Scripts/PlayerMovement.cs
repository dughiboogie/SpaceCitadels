using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement instance;

    private MapNodeView currentMapNodeView;

    public MapNodeView CurrentMapNodeView
    {
        get => currentMapNodeView;
        set
        {
            OnCurrentMapNodeChanged(value);
        }
    }

    private void Awake()
    {
        if(instance != null) {
            Debug.LogWarning("Multiple instances of PlayerMovement found!");
            return;
        }
        instance = this;

        currentMapNodeView = null;
    }

    private void OnCurrentMapNodeChanged(MapNodeView newNode)
    {
        if(currentMapNodeView == null) {
            currentMapNodeView = newNode;
        }
        else {
            foreach(var reachableNode in currentMapNodeView.MapNodeData.reachableNodes) {
                MapModelView.GetMapNodeView(reachableNode).GetComponent<Renderer>().material.SetColor("_Color", Color.white);
            }

            currentMapNodeView = newNode;

            MapModelView.GetMapNodeView(currentMapNodeView.MapNodeData.PreviouslyVisitedNode).GetComponent<Renderer>().material.SetColor("_Color", Color.white);
            currentMapNodeView.GetComponent<Renderer>().material.SetColor("_Color", Color.green);

            foreach(var reachableNode in currentMapNodeView.MapNodeData.reachableNodes) {
                MapModelView.GetMapNodeView(reachableNode).GetComponent<Renderer>().material.SetColor("_Color", Color.yellow);
            }
        }
    }
}

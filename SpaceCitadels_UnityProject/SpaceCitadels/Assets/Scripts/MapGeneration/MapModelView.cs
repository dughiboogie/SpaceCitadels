using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MapModelView
{
    private static List<MapNodeView> generatedMapNodes = new List<MapNodeView>();

    public static List<MapNodeView> GetMapNodes()
    {
        return generatedMapNodes;
    }

    public static MapNodeView GetMapNodeView(MapNode mapNode)
    {
        foreach(var mapNodeView in generatedMapNodes) {
            if(mapNodeView.MapNodeData.Equals(mapNode)){
                return mapNodeView;
            }
        }

        return null;
    }

    public static void AddMapNode(MapNodeView mapNode)
    {
        generatedMapNodes.Add(mapNode);
    }

    public static void Clear()
    {
        generatedMapNodes.Clear();
    }
}

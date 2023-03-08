using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MapModel
{
    private static List<MapNode> generatedMapNodes = new List<MapNode>();

    public static List<MapNode> GetMapNodes()
    {
        return generatedMapNodes;
    }

    public static void AddMapNode(MapNode mapNode)
    {
        generatedMapNodes.Add(mapNode);
    }

    public static void Clear()
    {
        generatedMapNodes.Clear();
    }
}

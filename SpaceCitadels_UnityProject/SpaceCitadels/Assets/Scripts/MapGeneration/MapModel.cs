using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapModel
{
    private List<MapNode> generatedMapNodes = new List<MapNode>();

    public List<MapNode> GetMapNodes()
    {
        return generatedMapNodes;
    }

    public void AddMapNode(MapNode mapNode)
    {
        generatedMapNodes.Add(mapNode);
    }
}

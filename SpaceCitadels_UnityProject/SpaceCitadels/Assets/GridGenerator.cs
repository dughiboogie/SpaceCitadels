using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridGenerator : MonoBehaviour
{
    [SerializeField]
    private Vector3 gridCenter = new Vector3(0,0,0);

    [SerializeField]
    private float hexSize = 1.0f;

    private List<Vector3> hexVertices = new List<Vector3>();

    private void Awake()
    {
        GenerateGrid();
        PlacePlanets();
    }

    /*
     * Generate the corners of the hex grid, and store them in hexVertices list
     */
    private void GenerateGrid()
    {
        for(int i = 0; i < 6; i++) {
            var angleDeg = 60 * i;
            var angleRad = Math.PI / 180 * angleDeg;
            float xCoord = (float)(gridCenter.x + hexSize * Math.Cos(angleRad));
            float zCoord = (float)(gridCenter.z + hexSize * Math.Sin(angleRad));
            hexVertices.Add(new Vector3(xCoord, 0, zCoord));
        }

        // Draw grid lines for 5 minutes, just for visual reference while prototyping
        for(int i = 0; i < hexVertices.Count - 1; i++) {
            Debug.DrawLine(hexVertices[i], hexVertices[i + 1], Color.yellow, 300);
        }
        Debug.DrawLine(hexVertices.Last(), hexVertices[0], Color.yellow, 300);
    }

    /*
     * Place "planets" on the corners of the grid just generated
     */
    private void PlacePlanets()
    {
        foreach(var vertex in hexVertices) {
            GameObject planet = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            planet.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            planet.transform.position = vertex;
        }
    }
}

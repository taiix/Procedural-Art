using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BuildingArea : MonoBehaviour
{
    //TODO: Finding the Area of Irregular Polygons
    //https://www.wikihow.com/Calculate-the-Area-of-a-Polygon
    public List<Transform> points = new();
    public Color fillColor = new Color(0.5f, 0.5f, 0.5f, 0.2f); // Fill color of the polygon
    public Color outlineColor = Color.black;

    private void Start()
    {
        Debug.Log(CalculateAreaOfPolygons());

    }

    float CalculateAreaOfPolygons()
    {
        int j = 0;
        float sum1 = 0;

        for (int i = 1; i < points.Count; i++)
        {
            sum1 += points[j].position.x * points[i].position.z;
            j = i;
        }

        int p = 0;
        float sum2 = 0;
        for (int i = 1; i < points.Count; i++)
        {
            sum2 += points[p].position.z * points[i].position.x;
            p = i;
        }

        float result = sum1 - sum2;

        return Mathf.Abs(result / 2);
    }

    private void OnDrawGizmos()
    {
        for (int i = 0; i < points.Count; i++)
        {
            if (i < points.Count - 1)
            {
                Gizmos.DrawLine(points[i].position, points[i + 1].position);
            }
            else
            {
                Gizmos.DrawLine(points[i].position, points[0].position);
            }
        }

        Vector3 centroid = CalculateCentroid();
        for (int i = 0; i < points.Count; i++)
        {
            int nextIndex = (i + 1) % points.Count;

            Gizmos.color = fillColor;
            Gizmos.DrawLine(points[i].position, points[nextIndex].position);
            Gizmos.DrawLine(points[nextIndex].position, centroid);
            Gizmos.DrawLine(centroid, points[i].position);
        }
        DrawFilledPolygon(points.ConvertAll(point => point.position).ToArray(), centroid, fillColor);
    }

    Vector3 CalculateCentroid()
    {
        Vector3 centroid = Vector3.zero;
        foreach (var point in points)
        {
            centroid += point.position;
        }
        centroid /= points.Count;
        return centroid;
    }

    void DrawFilledPolygon(Vector3[] vertices, Vector3 centroid, Color color)
    {
        Mesh mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        // Assign vertices
        Vector3[] newVertices = new Vector3[vertices.Length + 1];
        for (int i = 0; i < vertices.Length; i++)
        {
            newVertices[i] = vertices[i];
        }
        newVertices[vertices.Length] = centroid;

        mesh.vertices = newVertices;

        // Assign triangles
        int[] triangles = new int[vertices.Length * 3];
        for (int i = 0; i < vertices.Length; i++)
        {
            triangles[i * 3] = i;
            triangles[i * 3 + 1] = (i + 1) % vertices.Length;
            triangles[i * 3 + 2] = vertices.Length;
        }
        mesh.triangles = triangles;

        // Set color
        GetComponent<MeshRenderer>().material.color = color;
    }
}

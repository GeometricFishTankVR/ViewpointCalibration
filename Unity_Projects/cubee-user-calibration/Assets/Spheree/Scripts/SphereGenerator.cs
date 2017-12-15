using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereGenerator : MonoBehaviour {

    public float radius;
    public int smoothness;

	// Use this for initialization
	void Start () {
        
        Vector3[] vertexArray = GetComponent<MeshFilter>().mesh.vertices;
        List<Vector3> vertexList = new List<Vector3>();
        foreach (Vector3 vertex in vertexArray)
        {
            vertexList.Add(vertex.normalized*radius);
        }

        Vector3 v1;
        Vector3 v2;
        Vector3 v3;
        Vector3 v12;
        Vector3 v23;
        Vector3 v13;
        Vector3 vMid;
        int listLength;
        for (int i = 0; i < smoothness; i++)
        {
            listLength = vertexList.Count;
            for (int j = 0; j < listLength/3; j++)
            {
                v1 = vertexList[0];
                v2 = vertexList[1];
                v3 = vertexList[2];
                vertexList.RemoveRange(0, 3);
                v12 = ((v1 + v2) / 2).normalized*radius;
                v23 = ((v2 + v3) / 2).normalized*radius;
                v13 = ((v1 + v3) / 2).normalized*radius;
                vMid = ((v1 + v2 + v3) / 3).normalized*radius;

                vertexList.Add(v1);
                vertexList.Add(v12);
                vertexList.Add(vMid);

                vertexList.Add(v12);
                vertexList.Add(v2);
                vertexList.Add(vMid);

                vertexList.Add(v2);
                vertexList.Add(v23);
                vertexList.Add(vMid);

                vertexList.Add(v23);
                vertexList.Add(v3);
                vertexList.Add(vMid);

                vertexList.Add(v3);
                vertexList.Add(v13);
                vertexList.Add(vMid);

                vertexList.Add(v13);
                vertexList.Add(v1);
                vertexList.Add(vMid);

            }

        }
        Color[] colors = new Color[vertexList.Count];
        for (int i = 0; i < vertexList.Count; i++)
        {
            colors[i] = new Color(vertexList[i].x, vertexList[i].y, vertexList[i].z,1);
        }
        Mesh thisMesh = new Mesh();
        thisMesh.vertices = vertexList.ToArray();
        thisMesh.colors = colors;
        
        MeshFilter thisMeshFilter = GetComponent<MeshFilter>();
        thisMeshFilter.mesh = thisMesh;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}

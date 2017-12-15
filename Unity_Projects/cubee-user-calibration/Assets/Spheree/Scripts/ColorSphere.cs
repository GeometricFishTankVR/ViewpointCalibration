using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorSphere : MonoBehaviour {

    private MeshFilter thisMeshFilter;

	// Use this for initialization
	void Start () {
        thisMeshFilter = GetComponent<MeshFilter>();
        Mesh thisMesh = thisMeshFilter.mesh;
        Vector3[] vertices = thisMesh.vertices;
        Color[] colors = new Color[vertices.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
            colors[i] = new Color(vertices[i].x, vertices[i].y, vertices[i].z, 1);
        }
        thisMesh.colors = colors;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialManagerColor : MonoBehaviour {

    public Shader shader;
    public Color color;
    public Transform fishTank;
    private Renderer thisRenderer;


    // Use this for initialization
    void Start () {
        Material[] mats = new Material[5];
        thisRenderer = GetComponent<Renderer>();
        //thisRenderer.materials = new Material[5];
        Vector4[] screenInfo = { new Vector4(1,0,0,fishTank.localScale.x/2),
                                new Vector4(-1,0,0,fishTank.localScale.x/2),
                                new Vector4(0,1,0,fishTank.localScale.y/2),
                                new Vector4(0,0,1,fishTank.localScale.z/2),
                                new Vector4(0,0,-1,fishTank.localScale.z/2)};
        for (int i = 0; i < 5; i++)
        {
            mats[i] = new Material(shader);
            mats[i].SetColor("color", color);
            mats[i].SetVector("planeInfo", screenInfo[i]);
            mats[i].SetVector("tankScale", new Vector4(fishTank.localScale.x, fishTank.localScale.y, fishTank.localScale.z, 1));
        }
        thisRenderer.materials = mats;


    }

    // Update is called once per frame
    void Update () {
		
	}
}

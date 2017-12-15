using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent( typeof( MeshRenderer ) )]
public class CubeDamageFx : MonoBehaviour
{
    //private MeshRenderer MeshRenderer;
    private int FramesToLive = 2;

    void Start()
    {
        //MeshRenderer = GetComponent<MeshRenderer>();
    }

    void FixedUpdate()
    {
        FramesToLive--;
        if( FramesToLive < 0 )
            Destroy( gameObject );
    }
}

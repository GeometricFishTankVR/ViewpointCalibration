using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEnvironmentDemoScript : MonoBehaviour
{
    private ParticleSystem[] Emitters;

    void Start()
    {
        Emitters = GetComponentsInChildren<ParticleSystem>();
    }

    public void BeginParticleEmit()
    {
        foreach( var emitter in Emitters )
            emitter.Emit( 500 );
    }
}

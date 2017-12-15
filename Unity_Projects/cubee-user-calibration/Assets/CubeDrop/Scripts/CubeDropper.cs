using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Random = UnityEngine.Random;

public class CubeDropper : MonoBehaviour
{
    public float InitialDelay = 5F;

    public float SpawnRate = 3F;

    public int Spread = 3;

    public float DropHeight = 8.5F;

    [Range( 1, 3 )]
    public int InitialLayers = 2;

    public GameObject CubePrefab;

    public Material[] CubeMaterials;

    public GameObject FloorPrefab;

    public GameObject PlayerPrefab;

    public Transform ViewpointReference;

    internal GameObject Player;

    internal GameObject Floor;

    void Start()
    {
        // 
        Debug.Assert( CubeMaterials.Length > 1, "Must define at least two cube materials for the cube dropper." );

        // 
        InvokeRepeating( "SpawnCube", InitialDelay, SpawnRate );
        Invoke( "EnablePlayer", InitialDelay );

        // Create Floor
        Floor = Instantiate( FloorPrefab, transform, false );
        Floor.transform.localScale = Vector3.one * ( Spread + 0.5F ) * 2F;

        // Create Player
        Player = Instantiate( PlayerPrefab, transform, false );
        Player.transform.localPosition = new Vector3( 0, InitialLayers + 1.25F, 0 );
        var player_Behaviour = Player.GetComponent<PlayerBehaviour>();
        player_Behaviour.ViewpointReference = ViewpointReference;
        DisablePlayer();

        // Create Inital Block Layers
        for( int y = 0; y < InitialLayers; y++ )
        {
            for( int x = -Spread; x <= Spread; x++ )
            {
                for( int z = -Spread; z <= Spread; z++ )
                {
                    SpawnCube( new Vector3( x, y + 0.5F, z ) );
                }
            }
        }
    }

    void Update()
    {

    }

    void EnablePlayer()
    {
        // Debug.LogFormat( "Enabling Player!" );
        var player_PlayerBehaviour = Player.GetComponent<PlayerBehaviour>();
        var player_Rigidbody = Player.GetComponent<Rigidbody>();
        player_PlayerBehaviour.enabled = true;
        player_Rigidbody.useGravity = true;
    }

    void DisablePlayer()
    {
        // Debug.LogFormat( "Disabling Player!" );
        var player_PlayerBehaviour = Player.GetComponent<PlayerBehaviour>();
        var player_Rigidbody = Player.GetComponent<Rigidbody>();
        player_PlayerBehaviour.enabled = false;
        player_Rigidbody.useGravity = false;
    }

    void SpawnCube()
    {
        var nCubes = GameObject.FindGameObjectsWithTag( "drop_Cube" ).Length;

        if( nCubes < ( Math.Pow( 1 + Spread * 2, 2 ) * 3 ) )
            SpawnCube( CreateRandomCubeSpawnLocation() );
    }

    void SpawnCube( Vector3 pos )
    {
        // Create Cube
        var cube = Instantiate( CubePrefab, transform );
        var cube_Behaviour = cube.GetComponent<CubeBehaviour>();
        var cube_MeshRenderer = cube.GetComponent<MeshRenderer>();
        var cube_Transform = cube.GetComponent<Transform>();

        // Configure Cube
        cube_Behaviour.Dropper = this;
        cube_MeshRenderer.material = GetCubeRandomMaterial();
        cube_Transform.localScale = Vector3.zero;
        cube_Transform.localPosition = pos;
    }

    private Vector3 CreateRandomCubeSpawnLocation()
    {
        var x = Random.Range( -Spread, Spread + 1 );
        var z = Random.Range( -Spread, Spread + 1 );
        return new Vector3( x, DropHeight, z );
    }

    private Material GetCubeRandomMaterial()
    {
        var idx = Random.Range( 0, CubeMaterials.Length );
        return CubeMaterials[idx];
    }
}

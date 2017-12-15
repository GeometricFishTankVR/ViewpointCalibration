using UnityEngine;
using Biglab;

using Random = System.Random;

/// <summary>
/// A component that randomly produces a sequence of targets.
/// </summary>
[RequireComponent( typeof( TargetMaker ) )]
public class TargetRandomizer : MonoBehaviour
{
    private PlayerManager PlayerManager;

    [Tooltip( "The vertical angle range allowed to spawn targets within." )]
    public MinMaxRange SpawnAngleRange;

    [Tooltip( "The radius of the shell/sphere the targets will appear on." )]
    public float SpawnRadius = 4.5F;

    [Tooltip( "How often the targets with appear/vanish." )]
    public float SpawnRate = 1F;

    [Tooltip( "Randomization seed. This provides a uniquely random, but repeatable sequence." )]
    public int RandomSeed = 0;

    private TargetMaker TargetMaker;

    private Random Random;

    void Start()
    {
        PlayerManager = FindObjectOfType<PlayerManager>();

        // 
        TargetMaker = GetComponent<TargetMaker>();

        // 
        Random = new Random( RandomSeed );

        // 
        Invoke( "MakeTarget", SpawnRate );
    }

    void MakeTarget()
    {
        // Compute angle
        var player = PlayerManager.GetPlayerOneTranform();

        // 
        var v = player.position.AsVector2().normalized;
        var heading = ( Mathf.Atan2( -v.y, v.x ) * Mathf.Rad2Deg ) + 90;

        // 
        var targetPitch = SpawnAngleRange.GetRandomValue();
        var targetHeading = heading + Random.NextFloat( -45F, +45F );

        // 
        var orientation = Quaternion.Euler( targetPitch, targetHeading, 0 ) * Vector3.up;
        var target = TargetMaker.SpawnTarget( transform.position + ( orientation * SpawnRadius ), orientation );
        var target_Behaviour = target.GetComponent<TargetBehaviour>();
        target_Behaviour.BirthTime = SpawnRate * ( 1F / 5F );
        target_Behaviour.LifeTime = SpawnRate * ( 3F / 5F );
        target_Behaviour.BreakTime = SpawnRate * ( 1F / 5F );

        // 
        Invoke( "MakeTarget", SpawnRate );
    }
}

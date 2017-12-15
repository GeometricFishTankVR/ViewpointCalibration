using Biglab;
using UnityEngine;

[RequireComponent( typeof( TargetMaker ) )]
public class TargetSequence : MonoBehaviour
{
    public OrbitCamera OrbitCamera;

    public bool SpawnHemisphere = true;

    private CallbackSequence CallbackSequence;
    private GameObject ActiveTarget;

    public CsvTableWriter CsvWriter;

    private TargetMaker TargetMaker;

    void Start()
    {
        // 
        TargetMaker = GetComponent<TargetMaker>();

        // Creates a callback sequence with an initial delay of 3 seconds
        //CallbackSequence = new CallbackSequence();
        //CallbackSequence.AddDelay( 3F );

        ////CsvWriter.SetField( "Step", 0 );

        //// Create 10 targets 1 second apart
        //for( int i = 0; i < 10; i++ )
        //{
        //    CallbackSequence.AddWaitCondition( MouseCenterCheck );
        //    CallbackSequence.AddEvent( 1.0F, () => TargetMaker.SpawnTarget( OrbitCamera.Heading + 50, 30 ) );
        //    CallbackSequence.AddWaitCondition( WaitForTargetDeath );
        //}

        ////CsvWriter.SetField( "Step", 1 );

        //// Create 10 targets 0.5 second apart
        //for( int i = 0; i < 5; i++ )
        //{
        //    CallbackSequence.AddWaitCondition( MouseCenterCheck );
        //    CallbackSequence.AddEvent( 0.5F, () => TargetMaker.SpawnTarget( OrbitCamera.Heading + 50, 30 ) );
        //    CallbackSequence.AddEvent( 0.5F, () => TargetMaker.SpawnTarget( OrbitCamera.Heading + 70, 30 ) );
        //    CallbackSequence.AddWaitCondition( WaitForTargetDeath );
        //}

        ////CsvWriter.SetField( "Step", 2 );

        //// Create 10 targets 0.5 second apart
        //for( int i = 0; i < 5; i++ )
        //{
        //    CallbackSequence.AddWaitCondition( MouseCenterCheck );
        //    CallbackSequence.AddEvent( 0.33F, () => TargetMaker.SpawnTarget( OrbitCamera.Heading + 50, 30 ) );
        //    CallbackSequence.AddEvent( 0.33F, () => TargetMaker.SpawnTarget( OrbitCamera.Heading + 70, 30 ) );
        //    CallbackSequence.AddEvent( 0.33F, () => TargetMaker.SpawnTarget( OrbitCamera.Heading + 90, 30 ) );
        //    CallbackSequence.AddWaitCondition( WaitForTargetDeath );
        //}

        //// Begin executing this sequence
        //CallbackSequence.Execute( this );
    }

    bool MouseCenterCheck()
    {
        var mouse = GetMousePosition();
        var center = GetScreenCenter();

        // Offset in normalized coordinates
        var offset = ( mouse - center ).div( center );

        // Within the 10 center circle
        if( offset.magnitude < 0.2F )
        {
            // Debug.Log( "Mouse In Center" );
            return true;
        }
        else
        {
            // 
            return false;
        }
    }

    bool WaitForTargetDeath()
    {
        return ActiveTarget == null;
    }

    Vector2 GetScreenCenter() { return new Vector2( Screen.width / 2F, Screen.height / 2F ); }

    Vector2 GetMousePosition() { return Input.mousePosition; }

    private Vector3 CreateRandomVector()
    {
        var vec = Random.onUnitSphere;
        // Encourage the vector to always face up
        if( SpawnHemisphere ) return vec * Mathf.Sign( vec.y );
        else return vec;
    }

}

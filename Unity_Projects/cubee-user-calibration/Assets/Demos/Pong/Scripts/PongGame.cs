using Biglab;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

using Random = UnityEngine.Random;

public enum PongPlayer
{
    One,
    Two
}

public enum PongPlayerMode
{
    Human,
    Computer
}

public class PongGame : MonoBehaviour
{
    [Header( "Configuration" )]

    public PongPlayerMode PlayerOneMode = PongPlayerMode.Human;

    public PongPlayerMode PlayerTwoMode = PongPlayerMode.Computer;

    [Range( 1, 10 )]
    public int ComputerStrength = 3;

    [Header( "Objects" )]

    public PlayerManager PlayerManager;

    public Transform PaddleOne;

    public Transform PaddleTwo;

    public Text ScoreTextOne;

    public Text ScoreTextTwo;

    public PongBall Ball;

    public ParticleSystem[] Fireworks;

    public AudioClip FireworksSound;

    //

    private ComputerPlayer ComputerOne;
    private ComputerPlayer ComputerTwo;

    private float ComputerInterpolationFactor { get { return ComputerStrength / 10F; } }

    private int PlayerOneScore = 0;
    private int PlayerTwoScore = 0;
    public float STAGE_RADIUS { get { return 1F * transform.localScale.x; } }


    public const float STAGE_PADDLE_ANGLE_LIMIT = 32.5F;

    void Start()
    {
        ResetScore();
        UpdateText();

        if( PlayerOneMode == PongPlayerMode.Computer )
            ComputerOne = new ComputerPlayer( this );

        if( PlayerTwoMode == PongPlayerMode.Computer )
            ComputerTwo = new ComputerPlayer( this );
    }

    void FixedUpdate()
    {
        var playerOneTransform = PlayerManager.GetPlayerOneTranform();
        var playerTwoTransform = PlayerManager.GetPlayerTwoTransform();

        var ball = Ball.GetComponent<Rigidbody>();

        // Player One
        if( PlayerOneMode == PongPlayerMode.Human ) TransformPaddle( PaddleOne, AsVec2( -playerOneTransform.position ).normalized, PongPlayer.One, 1F );
        else if( ball.velocity.z < 0 )
        {
            var direction = GetAIDirectionVector( ball, ComputerOne.ErrorAngle * ( 1F - ComputerInterpolationFactor ) );
            TransformPaddle( PaddleOne, direction, PongPlayer.One, ComputerInterpolationFactor * ComputerInterpolationFactor );
        }

        // Player Two
        if( PlayerTwoMode == PongPlayerMode.Human ) TransformPaddle( PaddleTwo, AsVec2( -playerTwoTransform.position ).normalized, PongPlayer.Two, 1F );
        else if( ball.velocity.z > 0 )
        {
            var direction = GetAIDirectionVector( ball, ComputerTwo.ErrorAngle * ( 1F - ComputerInterpolationFactor ) );
            TransformPaddle( PaddleTwo, direction, PongPlayer.Two, ComputerInterpolationFactor * ComputerInterpolationFactor );
        }
    }

    /// <summary>
    /// ( x, y, z ) to ( x, z )
    /// </summary>
    public static Vector2 AsVec2( Vector3 v ) { return new Vector2( v.x, v.z ); }

    /// <summary>
    /// ( x, y ) to ( x, 0, y )
    /// </summary>
    public static Vector3 AsVec3( Vector2 v ) { return new Vector3( v.x, 0, v.y ); }

    private Vector2 GetAIDirectionVector( Rigidbody ball, float errorAngle )
    {
        // Construct ball ray
        var origin = AsVec2( ball.position );
        var dir = AsVec2( ball.velocity );
        dir = dir.normalized;

        var stageCenter = AsVec2( transform.position );

        Vector2 hit; // Finds where the ball will intersect the circle
        if( RayCircleIntersect( stageCenter, STAGE_RADIUS, origin, dir, out hit ) )
        {
            // Computes the direction vector
            var aiVector = ( stageCenter - hit ).normalized;

            // Applies and rotational error
            var aiError = Quaternion.AngleAxis( errorAngle, Vector3.forward );
            return aiError * aiVector;
        }

        // No vector?
        return Vector2.zero;
    }

    /// <summary>
    /// Animates a paddle based on a direction vector.
    /// </summary>
    private void TransformPaddle( Transform paddle, Vector2 direction, PongPlayer player, float interpolation )
    {
        // Player two is on the opposite side
        var angleOffset = player == PongPlayer.Two ? 180 : 0;

        // Computes and clamps the paddle angle with in a range
        var angle = ( -Mathf.Atan2( direction.y, direction.x ) * Mathf.Rad2Deg ) + 90 - angleOffset;
        angle = Mathf.Clamp( angle, -STAGE_PADDLE_ANGLE_LIMIT, +STAGE_PADDLE_ANGLE_LIMIT );

        // Spherical interpolation of paddle to destination angle
        var rotStart = paddle.transform.rotation;
        var rotTarget = Quaternion.Euler( -90, 0, angle + angleOffset );
        paddle.transform.rotation = Quaternion.Slerp( rotStart, rotTarget, interpolation );
    }

    /// <summary>
    /// Triggers the scoring event for a particular player.
    /// </summary>
    internal void TriggerGoalEvent( PongPlayer player )
    {
        // If player one has scored
        if( player == PongPlayer.One )
        {
            // Emit red fireworks
            EmitFireworks( Color.yellow, Color.red );
            PlayerOneScore++;

        }
        else
        {
            // Emit blue fireworks
            EmitFireworks( Color.cyan, Color.blue );
            PlayerTwoScore++;
        }

        // Update score text
        UpdateText();
    }

    private void EmitFireworks( Color c1, Color c2, int particleCount = 500 )
    {
        // Fireworks!
        foreach( var emitter in Fireworks )
        {
            // Play sound effect
            AudioSource.PlayClipAtPoint( FireworksSound, emitter.transform.position );

            // 
            var emitterConfig = emitter.main;
            emitterConfig.startColor = new ParticleSystem.MinMaxGradient( c1, c2 );
            emitter.Emit( particleCount );
        }
    }

    /// <summary>
    /// Resets the score to zero
    /// </summary>
    public void ResetScore()
    {
        PlayerOneScore = 0;
        PlayerTwoScore = 0;
        UpdateText();
    }

    /// <summary>
    /// Updates the text elements.
    /// </summary>
    public void UpdateText()
    {
        ScoreTextOne.text = string.Format( "{0} : {1}", PlayerOneScore, PlayerTwoScore );
        ScoreTextTwo.text = string.Format( "{0} : {1}", PlayerTwoScore, PlayerOneScore );
    }

    /// <summary>
    /// Finds a point on the line ( <paramref name="A"/> to <paramref name="B"/> ) nearest to <paramref name="center"/>.
    /// </summary>
    public static Vector2 NearestPointLine( Vector2 center, Vector2 A, Vector2 B )
    {
        var ac = center - A;
        var ab = B - A;
        var ab2 = Vector2.Dot( ab, ab );
        var acab = Vector2.Dot( ac, ab );
        var t = acab / ab2;

        // Clamp to keep within line segment
        // if( t < 0 ) t = 0;
        // else if( t > 1 ) t = 1;

        return A + ab * t;
    }

    /// <summary>
    /// Finds a point where a 2D ray intersects the given circle.
    /// </summary>
    public static bool RayCircleIntersect( Vector2 center, float r, Vector2 origin, Vector2 dir, out Vector2 hit )
    {
        // Finds nearest point on line
        var nearest = NearestPointLine( center, origin, origin + dir );

        // Pythagoras ( to find remaning distance to circle edge )
        var d2 = ( nearest - center ).sqrMagnitude;
        var l = Mathf.Sqrt( ( r * r ) - d2 );

        // Computes the intersection point
        hit = nearest + dir * l;

        // Ray is intersecting circle
        return d2 <= ( r * r );
    }

    private class ComputerPlayer
    {
        public float ErrorAngle = 0F;

        private const float AI_ERROR_RANGE = 20F;

        private const float AI_UPDATE_MIN_TIME = 0.3F;

        private const float AI_UPDATE_MAX_TIME = 0.6F;

        public ComputerPlayer( MonoBehaviour MB )
        {
            MB.StartCoroutine( Logic() );
        }

        private IEnumerator Logic()
        {
            while( true )
            {
                // 
                ErrorAngle = Random.Range( -AI_ERROR_RANGE, +AI_ERROR_RANGE );

                // 
                var waitTime = Random.Range( AI_UPDATE_MIN_TIME, AI_UPDATE_MAX_TIME );
                yield return new WaitForSeconds( waitTime );
            }
        }
    }
}

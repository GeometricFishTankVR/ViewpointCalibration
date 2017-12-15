using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class CallbackTimeline
{
    private List<TimeEvent> Events;

    public Coroutine Coroutine { get; private set; }

    private CustomYieldInstruction Instruction;

    public CallbackTimeline()
    {
        Events = new List<TimeEvent>();
    }

    /// <summary>
    /// Adds an event to the sequence.
    /// </summary>
    public void AddEvent( float time, Action action )
    {
        if( action == null ) throw new ArgumentNullException( "action" );
        Events.Add( new TimeEvent( time, action ) );
    }

    /// <summary>
    /// Executes the event sequence as a coroutine for the specified MonoBehaviour.
    /// </summary>
    public void Execute( MonoBehaviour mono )
    {
        if( Coroutine != null ) mono.StopCoroutine( Coroutine );
        Coroutine = mono.StartCoroutine( CoroutineBody() );
    }

    public void WaitForCondiction( Func<bool> predicate )
    {
        Instruction = new WaitUntil( predicate );
    }

    private IEnumerator CoroutineBody()
    {
        // Sort items by time
        var events = new Queue<TimeEvent>( Events.OrderBy( x => x.Time ) );

        // Begin with zero elapsed time
        var elapsed = 0F;

        // While events remain
        while( events.Count > 0 )
        {
            // Accumulate elapsed time
            elapsed += Time.deltaTime;

            // If elapsed time exceeds event time
            if( elapsed > events.Peek().Time )
            {
                Debug.Log( "Sequence Event" );
                var ev = events.Dequeue();
                ev.Action();
            }

            // If no special instruction has been specified, wait for next frame.
            if( Instruction == null ) yield return null;
            else
            {
                yield return Instruction;
                Instruction = null;
            }
        }

        Debug.Log( "Sequence Complete" );
    }

    private class TimeEvent
    {
        public readonly float Time;
        public readonly Action Action;

        public TimeEvent( float time, Action action )
        {
            Time = time;
            Action = action;
        }
    }
}

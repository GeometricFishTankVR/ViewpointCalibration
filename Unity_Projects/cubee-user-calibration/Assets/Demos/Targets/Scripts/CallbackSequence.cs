using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class CallbackSequence
{
    private List<SequenceEvent> Events;

    public Coroutine Coroutine { get; private set; }

    public CallbackSequence()
    {
        Events = new List<SequenceEvent>();
    }

    #region Add Events

    /// <summary>
    /// Causes a simple delay in the sequence.
    /// </summary>
    public void AddDelay( float time )
    {
        Events.Add( new TimeEvent( time, null ) );
    }

    /// <summary>
    /// Causes a the sequence to wait until a condition is matched.
    /// </summary>
    public void AddWaitCondition( Func<bool> condition )
    {
        Events.Add( new WaitEvent( condition ) );
    }

    /// <summary>
    /// Adds an event to the sequence.
    /// </summary>
    public void AddEvent( float time, Action action )
    {
        if( action == null ) throw new ArgumentNullException( "action" );
        Events.Add( new TimeEvent( time, action ) );
    }

    #endregion

    /// <summary>
    /// Executes the event sequence as a coroutine for the specified MonoBehaviour.
    /// </summary>
    public void Execute( MonoBehaviour mono )
    {
        if( Coroutine != null ) mono.StopCoroutine( Coroutine );
        Coroutine = mono.StartCoroutine( CoroutineBody() );
    }

    private IEnumerator CoroutineBody()
    {
        // Clone queue
        var events = new Queue<SequenceEvent>( Events );

        // Begin with zero elapsed time
        var elapsed = 0F;

        var ev = (SequenceEvent) null;

        // While events remain
        while( events.Count > 0 )
        {
            // Get next event
            ev = events.Dequeue();

            // Accumulate elapsed time
            elapsed += Time.deltaTime;

            // Perform event action
            var result = ev.ExecuteAction();
            yield return result;
        }

        Debug.Log( "Sequence Complete" );
    }

    private abstract class SequenceEvent
    {
        internal protected abstract IEnumerator ExecuteAction();
    }

    private class TimeEvent : SequenceEvent
    {
        public readonly float Duration;
        public readonly Action Action;

        public TimeEvent( float duration, Action action )
        {
            Duration = duration;
            Action = action;
        }

        internal protected override IEnumerator ExecuteAction()
        {
            var now = Time.time;
            if( Action != null ) Action.Invoke();
            return new WaitWhile( () => ( Time.time - now ) < Duration );
        }
    }

    private class WaitEvent : SequenceEvent
    {
        public readonly Func<bool> Condition;

        public WaitEvent( Func<bool> condition )
        {
            Condition = condition;
        }

        internal protected override IEnumerator ExecuteAction()
        {
            return new WaitUntil( Condition );
        }
    }
}

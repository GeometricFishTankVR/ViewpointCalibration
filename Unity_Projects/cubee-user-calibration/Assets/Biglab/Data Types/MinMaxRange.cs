using UnityEngine;

[System.Serializable]
public struct MinMaxRange
// Author: Christopher Chamberlain - 2017
{
    /// <summary>
    /// The minimum value (inclusive).
    /// </summary>
    [Tooltip( "The minimum value (inclusive)." )]
    public float min;

    /// <summary>
    /// The maximum value (inclusive).
    /// </summary>
    [Tooltip( "The maximum value (inclusive)." )]
    public float max;

    /// <summary>
    /// Gets a random value within the min-max range.
    /// </summary>
    public float GetRandomValue()
    {
        return Random.Range( min, max );
    }
}
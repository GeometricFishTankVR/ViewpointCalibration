using System;

public static class EnumExtensions
// Author: Christopher Chamberlain - 2017
{
    /// <summary>
    /// Checks if the given enum has a bit flag match.
    /// </summary>
    /// <param name="this">The tested enum.</param>
    /// <param name="flag">The value to test.</param>
    /// <returns>True if the flag is set. Otherwise false.</returns>
    public static bool HasFlag( this Enum @this, Enum flag )
    {
        // check if from the same type.
        if( @this.GetType() != flag.GetType() )
            throw new ArgumentException( "Both source and flag enums must be of the same type." );

        ulong num = Convert.ToUInt64( flag );
        ulong num2 = Convert.ToUInt64( @this );

        return ( num2 & num ) == num;
    }
}
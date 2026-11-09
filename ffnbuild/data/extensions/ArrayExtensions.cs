namespace ffnbuild.data.extensions;

public static class ArrayExtensions
{
    /// <summary>
    /// Checks a string array to see if ALL of the members are null, empty, or whitespace.
    /// </summary>
    /// <param name="array"></param>
    /// <returns></returns>
    public static bool ArrayIsNullOrEmpty(this string[] array)
    {
        var isNullOrEmpty = false;
        if( array is null || array.Length == 0 )
        {
            isNullOrEmpty = true;
        }
        else
        {
            foreach( string str in array )
            {
                isNullOrEmpty &= string.IsNullOrWhiteSpace(str);
            }
        }

        return isNullOrEmpty;
    }

    public static bool IsIn(this string target, params string[] args)
    {
        bool isIn = false;

        if( args.Length > 0 )
        {
            foreach( string arg in args )
            {
                if( string.Compare(target, arg, System.StringComparison.Ordinal) == 0 )
                {
                    isIn = true;
                    break;
                }
            }
        }

        return isIn;
    }

    public static bool IsIn(this int target, params int[] args)
    {
        bool isIn = false;

        if( args.Length > 0 )
        {
            foreach( int arg in args )
            {
                if( target == arg )
                {
                    isIn = true;
                    break;
                }
            }
        }

        return isIn;
    }
}
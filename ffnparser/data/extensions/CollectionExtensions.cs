namespace ffnparser.data.extensions;

public static class CollectionExtensions
{
    extension<T>(IEnumerable<T>? source)
    {
        /// <summary>
        /// Checks if collection contains nothing but null or empty items
        /// </summary>
        /// <returns>
        /// True if collection contains nothing but null or empty items
        /// </returns>

        public bool CollectionIsNullOrEmpty()
        {
            var isNullOrEmpty = true;
            if( source is null )
            {
                return isNullOrEmpty;
            }
            else
            {
                foreach( T obj in source )
                {
                    if( obj is null )
                    {
                        isNullOrEmpty &= true;
                    }
                    else if( obj is string str )
                    {
                        isNullOrEmpty &= string.IsNullOrWhiteSpace(str);
                    }
                    else
                    {
                        isNullOrEmpty &= false;
                    }
                }
            }
            return isNullOrEmpty;
        }
    }
}
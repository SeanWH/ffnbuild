namespace ffnparser.data.extensions;

using System.Collections.Generic;
using System.Text.Json;

public static class GenericExtensions
{
    extension<T>(T? obj)
    {
        public bool IsIn(IEnumerable<T> collection)
        {
            bool isIn = false;
            if( collection is not null )
            {
                foreach( T item in collection )
                {
                    if( obj is null && item is null )
                    {
                        isIn = true;
                        break;
                    }
                    if( obj is not null && obj.Equals(item) )
                    {
                        isIn = true;
                        break;
                    }
                }
            }
            return isIn;
        }

        public bool IsIn(params T[] args)
        {
            bool isIn = false;
            if( args.Length > 0 )
            {
                foreach( T arg in args )
                {
                    if( obj is null && arg is null )
                    {
                        isIn = true;
                        break;
                    }
                    if( obj is not null && obj.Equals(arg) )
                    {
                        isIn = true;
                        break;
                    }
                }
            }
            return isIn;
        }

        public int Hash256()
        {
            if( obj is null )
            {
                throw new ArgumentNullException(nameof(obj));
            }

            var bytes = obj.ToByteArray();
            var hash = System.Security.Cryptography.SHA256.HashData(bytes);
            return BitConverter.ToInt32(hash, 0);
        }

        public byte[] ToByteArray()
        {
            if( obj is null )
            {
                throw new ArgumentNullException(nameof(obj));
            }

            return JsonSerializer.SerializeToUtf8Bytes(obj);
        }
    }
}
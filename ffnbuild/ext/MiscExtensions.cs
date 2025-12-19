namespace ffnbuild.ext;

public static class MiscExtensions
{
    public static bool IsIn(this string target, params string[] args)
    {
        bool isIn = false;

        if (args.Length > 0)
        {
            foreach (string arg in args)
            {
                if (target == arg)
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

        if (args.Length > 0)
        {
            foreach (int arg in args)
            {
                if (target == arg)
                {
                    isIn = true;
                    break;
                }
            }
        }

        return isIn;
    }
}
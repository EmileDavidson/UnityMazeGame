using System.Collections;

namespace Toolbox.MethodExtensions
{
    public static class IntExtensions
    {
        public static int GetNumberBetweenList(this int index, IList list)
        {
            if (index < 0) index = list.Count + index;
            else if (index > list.Count - 1) index = index % list.Count;

            return index;
        }
    }
}
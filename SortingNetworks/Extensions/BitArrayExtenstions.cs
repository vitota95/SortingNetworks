
namespace SortingNetworks.Extensions
{
    using System.Collections;

    public static class BitArrayExtenstions
    {
        public static int SetCount(this BitArray arr)
        {
            var newValue = new int[1];
            arr.CopyTo(newValue, 0);

            return (int)System.Runtime.Intrinsics.X86.Popcnt.PopCount((uint) newValue[0]);
        }
    }
}

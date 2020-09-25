
namespace SortingNetworks.Extensions
{
    using System.Collections;

    public static class BitArrayExtenstions
    {
        public static ushort SetCount(this BitArray arr)
        {
            var newValue = new uint[1];
            arr.CopyTo(newValue, 0);

            return (ushort)System.Runtime.Intrinsics.X86.Popcnt.PopCount(newValue[0]);
        }
    }
}

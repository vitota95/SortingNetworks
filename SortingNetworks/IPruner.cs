// --------------------------------------------------------------------------------------------------------------------
// <summary>
//   The Pruner interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SortingNetworks
{
    /// <summary>
    /// The Pruner interface.
    /// </summary>
    public interface IPruner
    {
        /// <summary>
        /// The prune.
        /// </summary>
        /// <param name="nets">
        /// The nets.
        /// </param>
        /// <returns>
        /// The <see cref="IComparatorNetwork[]"/>.
        /// </returns>
        IComparatorNetwork[] Prune(IComparatorNetwork[] nets);
    }
}

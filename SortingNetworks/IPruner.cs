// --------------------------------------------------------------------------------------------------------------------
// <summary>
//   The Pruner interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SortingNetworks
{
    using System.Collections.Generic;

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
        IReadOnlyList<IComparatorNetwork> Prune(IReadOnlyList<IComparatorNetwork> nets);
    }
}

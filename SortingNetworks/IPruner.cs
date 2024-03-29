﻿// --------------------------------------------------------------------------------------------------------------------
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
        static int Threads { get; set; }
        /// <summary>
        /// The prune.
        /// </summary>
        /// <param name="nets">
        /// The nets.
        /// </param>
        /// <returns>
        /// The <see cref="IComparatorNetwork[]"/>.
        /// </returns>
        IReadOnlyList<IComparatorNetwork> Prune<T>(IReadOnlyList<T> nets);

        IReadOnlyList<IComparatorNetwork> Remove(IReadOnlyList<IComparatorNetwork> nets1, IReadOnlyList<IComparatorNetwork> nets2);
    }
}

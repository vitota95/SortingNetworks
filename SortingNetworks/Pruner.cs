using System;
using System.Collections.Generic;
using System.Text;

namespace SortingNetworks
{
    public class Pruner : IPruner
    {
        public IComparatorNetwork[] Prune(IComparatorNetwork[] nets)
        {
            List<IComparatorNetwork> result = new List<IComparatorNetwork>();
            for (var i = 0; i < nets.Length; i++) 
            {
                foreach(var n in result) 
                {
                    nets[i].MarkIfSubsumed(n);
                }

                if (!nets[i].IsMarked) 
                {
                    result.Add(nets[i]);
                }
            }

            return result.ToArray();
        }
    }
}

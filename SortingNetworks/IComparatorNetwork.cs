﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Cryptography.X509Certificates;

namespace SortingNetworks
{
    public struct Comparator
    {
        public short x;
        public short y;

        public Comparator(short x, short y) 
        {
            this.x = x;
            this.y = y;
        }
    }

    public interface IComparatorNetwork
    {
        short Inputs { get; }
        bool IsMarked { get; }
        Comparator[] Comparators { get; set; }
        HashSet<short> Outputs{ get; }
        void MarkIfSubsumed(IComparatorNetwork n);       
        bool IsRedundant(IComparatorNetwork n);       
        bool IsSortingNetwork();
        IComparatorNetwork CloneWithNewComparator(Comparator comparator);
    }
}

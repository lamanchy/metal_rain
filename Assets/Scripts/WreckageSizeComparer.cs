using System.Collections.Generic;
using Meteor;
using UnityEngine;

internal class WreckageSizeComparer : IComparer<FallingWreckage> {
    public int Compare(FallingWreckage lhs, FallingWreckage rhs) {
        if (lhs == null || rhs == null) {
            return 0;
        }
        return rhs.Energy.CompareTo(lhs.Energy);
    }
}
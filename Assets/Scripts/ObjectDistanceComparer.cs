using System.Collections.Generic;
using UnityEngine;

internal class ObjectDistanceComparer : IComparer<MonoBehaviour> {
    private readonly MonoBehaviour targetObject;

    public ObjectDistanceComparer(MonoBehaviour targetObject) {
        this.targetObject = targetObject;
    }

    public int Compare(MonoBehaviour lhs, MonoBehaviour rhs) {
        if (lhs == null || rhs == null) {
            return 0;
        }
        return Vector3.Distance(targetObject.transform.position, lhs.transform.position).CompareTo(Vector3.Distance(targetObject.transform.position, rhs.transform.position));
    }
}
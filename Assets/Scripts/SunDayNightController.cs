using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunDayNightController : MonoBehaviour
{
    [SerializeField]
    private Vector3 rotationPerSec;

    private void Update()
    {
        transform.Rotate(rotationPerSec * Time.deltaTime);
    }
}

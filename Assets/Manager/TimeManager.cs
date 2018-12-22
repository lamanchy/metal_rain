using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public bool running;
    public bool hasChanged;

    private void Start()
    {
        running = true;
        hasChanged = false;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (hasChanged) hasChanged = false;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            running = !running;
            hasChanged = true;
        }
    }
}

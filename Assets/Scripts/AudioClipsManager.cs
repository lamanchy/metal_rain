using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioClipsManager : MonoBehaviour
{
    public static AudioClipsManager Instance { get; private set; }

    public AudioClip buildingSound;
    public AudioClip powerDownSound;
    public AudioClip powerUpSound;

    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
        }

        Instance = this;
    }

    private void OnDestroy()
    {
        Instance = null;
    }
}

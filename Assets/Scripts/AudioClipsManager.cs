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

    private void Update()
    {
        // NOTE It's super shit to have this code here in AudioClipsManager but good enought at this moment
        if(Input.GetKeyUp(KeyCode.Escape))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenuScene");
        }
    }

    private void OnDestroy()
    {
        Instance = null;
    }
}

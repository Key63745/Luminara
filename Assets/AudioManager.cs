using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    public AudioSource track1, track2;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void SwapTrack()
    {
        track2.Play();
        track1.Stop();
    }

}

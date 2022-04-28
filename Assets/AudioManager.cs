using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    public AudioSource track1, track2, track3;

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

    public void SwapTrackAmbient()
    {
        track1.Play();
        track2.Stop();
        track3.Stop();
    }

    public void SwapTrackInside()
    {
        track3.Play();
        track2.Stop();
        track1.Stop();
    }

}

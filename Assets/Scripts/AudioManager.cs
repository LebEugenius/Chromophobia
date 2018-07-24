using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource SFXPlayer;

    [SerializeField] private SoundClip correctClick;
    [SerializeField] private SoundClip wrongClick;

    public void PlayCorrectColoredClickClip()
    {
        SFXPlayer.PlayOneShot(correctClick.Clip, correctClick.VolumeScale);
    }

    public void PlayWrongColoredClickClip()
    {
        SFXPlayer.PlayOneShot(wrongClick.Clip, wrongClick.VolumeScale);
    }
}

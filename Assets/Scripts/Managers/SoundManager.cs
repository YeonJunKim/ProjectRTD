using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BG_TYPE
{
    InGame,
    BossSpawned,
}


public class SoundManager : MonoBehaviour
{
    public static SoundManager S;

    public AudioSource BGM_Normal_AudioSource;
    public AudioSource BGM_BossSpawned_AudioSource;
    public AudioSource Effect_AudioSource;

    public AudioClip click;

    BG_TYPE cur_bg;

    private void Awake()
    {
        S = this;
        cur_bg = BG_TYPE.InGame;
    }

    public void ChangeBGSound(BG_TYPE bgType)
    {
        if (cur_bg == bgType)
            return;

        switch (bgType)
        {
            case BG_TYPE.InGame:
                BGM_BossSpawned_AudioSource.Stop();
                BGM_Normal_AudioSource.Play();
                break;
            case BG_TYPE.BossSpawned:
                BGM_Normal_AudioSource.Pause();
                BGM_BossSpawned_AudioSource.Play();
                break;
        }
        cur_bg = bgType;
    }

    public void ClickSound()
    {
        Effect_AudioSource.PlayOneShot(click);
    }

}

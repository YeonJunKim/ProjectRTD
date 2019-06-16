﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BG_TYPE
{
    Intro,
    InGame,
    BossSpawned,
}


public class SoundManager : MonoBehaviour
{
    public static SoundManager S;

    public AudioSource BG_AudioSource;
    public AudioSource Effect_AudioSource;

    public AudioClip intro;
    public AudioClip inGame;
    public AudioClip bossSpawned;
    public AudioClip click;

    BG_TYPE cur_bg;

    private void Awake()
    {
        S = this;
        cur_bg = BG_TYPE.Intro;
    }

    public void ChangeBGSound(BG_TYPE bgType)
    {
        if (cur_bg == bgType)
            return;

        BG_AudioSource.Stop();
        switch (bgType)
        {
            case BG_TYPE.Intro:
                BG_AudioSource.clip = intro;
                break;
            case BG_TYPE.InGame:
                BG_AudioSource.clip = inGame;
                break;
            case BG_TYPE.BossSpawned:
                BG_AudioSource.clip = bossSpawned;
                break;
        }
        BG_AudioSource.Play();
        cur_bg = bgType;
    }

    public void ClickSound()
    {
        Effect_AudioSource.PlayOneShot(click);
    }

}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource bgmPlayer;
    [SerializeField] private AudioSource sfxPlayer;

    [SerializeField] private AudioClip[] bgmClips;
    [SerializeField] private AudioClip[] sfxClips;

    private void Awake()
    {
        bgmPlayer.clip = bgmClips[0];
        PlayBGM();
    }
    

    public void PlayBGM()
    {
        bgmPlayer.Play();
    }

    public void StopBGM()
    {
        bgmPlayer.Stop();
    }

    public void SetBGM(int index)
    {
        bgmPlayer.clip = bgmClips[index];
    }

    public void PlaySfx(int index)
    {
        sfxPlayer.PlayOneShot(sfxClips[index]);
    }
}

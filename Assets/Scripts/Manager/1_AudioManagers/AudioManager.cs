using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public float bgmVolume;
    public float changeSpeed;
    
    [SerializeField] public AudioSource bgmPlayer;
    [SerializeField] public AudioSource sfxPlayer;

    [SerializeField] public AudioClip[] bgmClips;
    [SerializeField] public AudioClip[] sfxClips;

    private void Awake()
    {
        bgmPlayer.clip = bgmClips[0];
        PlayBGM();
    }
    

    public void PlayBGM()
    {
        bgmPlayer.Play();
    }

    public void ChangeBgm(int index)
    {
        StartCoroutine(ChangeBGM(index));
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
    
    public void PlaySfx(AudioClip clip)
    {
        sfxPlayer.PlayOneShot(clip);
    }
    
    IEnumerator ChangeBGM(int index)
    {
        float tmp = bgmPlayer.volume;
        while (bgmPlayer.volume > 0)
        {
            bgmPlayer.volume -= Time.fixedDeltaTime * changeSpeed;
            yield return new WaitForFixedUpdate();
        }
    
        StopBGM();
        SetBGM(index);
        PlayBGM();
        bgmPlayer.volume = tmp;
    }
}

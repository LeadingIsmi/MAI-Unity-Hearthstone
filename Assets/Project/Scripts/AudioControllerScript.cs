using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioControllerScript : MonoBehaviour
{
    public static AudioControllerScript Instance;

    public AudioSource musicSource;
    public AudioClip StartWindowAudio;
    public AudioClip BGAudio;
    public AudioClip HitCard;
    public AudioClip HitSpellCard;
    public AudioClip BuffCard;
    public AudioClip ChangeTurn;
    public AudioClip DrawCard;
    public AudioClip ChoiceCard;
    public AudioClip Settings;

    private void Awake()
    {
        if (!Instance)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(this);
    }

    void Start()
    {
        musicSource = GetComponent<AudioSource>();
        UIControllerScript.Instance.VolumeScrollbar.onValueChanged.AddListener(delegate { ChangeVolume(); });
        UIControllerScript.Instance.VolumeScrollbar.value = musicSource.volume;
    }

    public void ChangeVolume()
    {
        musicSource.volume = UIControllerScript.Instance.VolumeScrollbar.value;
    }

    public void PlayStartWindowAudio()
    {
        GetComponent<AudioSource>().Stop();
        GetComponent<AudioSource>().PlayOneShot(StartWindowAudio);
    }

    public void PlayBGAudio()
    {
        GetComponent<AudioSource>().Stop();
        GetComponent<AudioSource>().PlayOneShot(BGAudio);
    }

    public void PlaySettings()
    {
        GetComponent<AudioSource>().PlayOneShot(Settings);
    }

    public void PlayHitCard()
    {
        GetComponent<AudioSource>().PlayOneShot(HitCard);
    }

    public void PlayHitSpellCard()
    {
        GetComponent<AudioSource>().PlayOneShot(HitSpellCard);
    }

    public void PlayBuffCard()
    {
        GetComponent<AudioSource>().PlayOneShot(BuffCard);
    }

    public void PlayChangeTurn()
    {
        GetComponent<AudioSource>().PlayOneShot(ChangeTurn);
    }

    public void PlayDrawCard()
    {
        GetComponent<AudioSource>().PlayOneShot(DrawCard);
    }

    public void PlayChoiceCard()
    {
        GetComponent<AudioSource>().PlayOneShot(ChoiceCard);
    }

    public void StopAudio()
    {
        GetComponent<AudioSource>().Stop();
    }
}

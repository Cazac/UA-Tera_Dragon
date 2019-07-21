using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using WaveSystem;

public class SoundManager : MonoBehaviour
{
    public AudioSource mainAudioSourceSoundtrack;
    public AudioSource mainAudioSourceUI;
    public bool IsMuteSoundtrack { get; set; }
    public bool IsMuteUI { get; set; }

    public bool ReturnControl { get => autoControl; set => autoControl = value; }


    public SoundObject[] soundClips;
    //private List<GameObject> uiGeneratedSounds = new List<GameObject>();

    private AudioClip currentClip;
    private bool triggerOnLevelLoad = false;
    private WaveManager waveManager;
    private bool autoControl = true;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Awake()
    {
        //AutoDestroySelf();
    }
    private void AutoDestroySelf()
    {
        if (GameObject.FindObjectsOfType<SoundManager>().Length > 1)
        {
            DestroyImmediate(this.gameObject);
            //this.GetComponent<AudioSource>().clip = null;
        }
    }

    private void Start()
    {
        waveManager = GameObject.FindObjectOfType<WaveManager>();
    }

    private void OnLevelWasLoaded(int level)
    {
        triggerOnLevelLoad = true;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        triggerOnLevelLoad = true;
    }


    private void OnScenceLoad()
    {
        triggerOnLevelLoad = true;
    }

    private void Update()
    {

        if (triggerOnLevelLoad)
        {
            LoopThroughSoundList(soundClips);
            triggerOnLevelLoad = false;
        }

        if (!mainAudioSourceSoundtrack.GetComponent<AudioSource>().isPlaying/* && autoControl*/)
            LoopThroughSoundList(soundClips);

        //if (!autoControl)
        //{
        //    LoopThroughSoundList(soundClips);
        //    autoControl = true;
        //}
   

        //if (waveManager!=null && waveManager.EnableSpawning == false)
        //    autoControl = false;
        //else
        //    autoControl = true;
    }

    public void PlaySpecificSound(String soundName)
    {
        foreach (var clip in soundClips)
        {
            if (clip.SoundName.Contains(soundName))
            {
                PlaySoundByName(clip);
            }
        }
    }

    public void PlayOnUIClick(SoundObject clip)
    {
        if (mainAudioSourceUI.GetComponent<AudioSource>().isPlaying)
            mainAudioSourceUI.clip = null;
        mainAudioSourceUI.clip = clip.AudioClip;

        mainAudioSourceUI.Play();
    }

    private void LoopThroughSoundList(SoundObject[] clips)
    {
        foreach (var clip in clips)
        {
            if (clip.SoundName.Contains("Menu") && SceneManager.GetActiveScene().name.Contains("Menu") && autoControl)
            {
                PlaySoundByName(clip);
            }

            if (clip.SoundName.Contains("Game") && SceneManager.GetActiveScene().name.Contains("Game") && autoControl)
            {
                PlaySoundByName(clip);
            }

            if(clip.SoundName.Contains("Inter") && !autoControl)
            {
                PlaySoundByName(clip);
            }
        }
    }

    public void PlaySoundByName(SoundObject audioClip)
    {
        //Set default value from SoundObject
        mainAudioSourceSoundtrack.clip = audioClip.AudioClip;
        mainAudioSourceSoundtrack.pitch = audioClip.Pitch;
        mainAudioSourceSoundtrack.loop = true;
        mainAudioSourceSoundtrack.Play();

        //Begin lerping volume of sound
        //if (audioClip.IsAllowedAudioDampening == true)
        //{
        //    StartCoroutine(AudioVolumeDampeningOnLoad(mainAudioSourceSoundtrack, 0.5f, mainAudioSourceSoundtrack.volume, 0.2f));
        //}
    }

    private IEnumerator AudioVolumeDampeningOnLoad(AudioSource audioSource, float smallestLerpValue, float initialVolumeValue, float lerpTime)
    {
        audioSource.volume = smallestLerpValue;

      

        while (audioSource.volume < initialVolumeValue)
        {
            if (audioSource.volume >= 0.98)
            {
                audioSource.volume = 1;
                Debug.Log("Coroutine stopped");
                StopCoroutine(AudioVolumeDampeningOnLoad(audioSource, smallestLerpValue, initialVolumeValue, lerpTime));
            }
            audioSource.volume += lerpTime * Time.deltaTime;
            yield return null;
        }

       
    }

    public void VolumeChangeSoundtrack(Slider slider)
    {
        mainAudioSourceSoundtrack.volume = slider.value;
    }

    public void VolumeChangeUI(Slider slider)
    {
        mainAudioSourceUI.volume = slider.value;
    }

    public void MuteUI()
    {
        if (IsMuteUI)
        {
            IsMuteUI = false;
        }

        else
        {
            IsMuteUI = true;
        }
        mainAudioSourceUI.mute = IsMuteUI;
    }

    public void MuteSoundtrack()
    {

        if (IsMuteSoundtrack)
        {
            IsMuteSoundtrack = false;
        }

        else
        {
            IsMuteSoundtrack = true;
        }
        mainAudioSourceSoundtrack.mute = IsMuteSoundtrack;
    }


}


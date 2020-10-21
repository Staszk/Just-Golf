///-------------------------------------------------------------------------
///   Copyright Wired Visions 2019
///   Class:            SoundManager
///   Description:      Receives requests to play sounds in 2D and 3D space.
///                     Stores an array of Sounds which can be requested and 
///                     a Queue of AudioSources to carry out the requests.
///   Author:           Parker Staszkiewicz
///-------------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;
using UnityEngine;
using System;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    [SerializeField] private Sound[] sounds = null;
    [SerializeField] private int audioSourceCount = 10;
    private Dictionary<string, AudioClip> keyValuePairs = null;
    private Queue<AudioSource> availableSources = null;
    private List<AudioSource> activeAudioSources = null;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        GenerateAudioSources();
        GenerateDictionary();

        if (transform.parent)
            DontDestroyOnLoad(transform.parent);
        else
            DontDestroyOnLoad(transform);
    }

    private void GenerateDictionary()
    {
        keyValuePairs = new Dictionary<string, AudioClip>();

        for (int i = 0; i < sounds.Length; i++)
        {
            keyValuePairs.Add(sounds[i].Tag, sounds[i].Clip);
        }
    }

    private void GenerateAudioSources()
    {
        availableSources = new Queue<AudioSource>();
        activeAudioSources = new List<AudioSource>();

        for (int i = 0; i < audioSourceCount; i++)
        {
            availableSources.Enqueue(GenerateNewSource());
        }
    }

    private AudioSource GenerateNewSource()
    {
        AudioSource aud = new GameObject("Audio Source", typeof(AudioSource)).GetComponent<AudioSource>();
        aud.transform.SetParent(transform);
        aud.playOnAwake = false;

        return aud;
    }

    public static void PlaySound(string tag)
    {
        instance?.PlayAudioClip(tag);
    }

    public static void StopSound(string tag)
    {
        instance?.StopPlayingSound(tag);
    }

    private void PlayAudioClip(string tag)
    {
        PlayAudioClipAt(tag, transform.position);
    }

    public static void PlaySoundAt(string tag, Vector3 worldPos)
    {
        instance?.PlayAudioClipAt(tag, worldPos);
    }

    private void PlayAudioClipAt(string tag, Vector3 worldPos)
    {
        Sound s = Array.Find(sounds, sound => sound.Tag == tag);

        // Sound not found
        if (s == null)
        {
            Debug.LogWarning("Sound '" + tag + "' not found.");
            return;
        }

        AudioSource source;

        // No more audio sources
        if (availableSources.Count < 1)
        {
            source = GenerateNewSource();
            Debug.LogWarning("Not enough audio sources. Added a new one.");
        }
        else
        {
            source = availableSources.Dequeue();
        }

        source.transform.position = worldPos;
        source.clip = s.Clip;
        source.volume = s.Volume;
        source.pitch = s.Pitch;
        source.spatialBlend = s.SpatialBlend;
        source.loop = s.Loop;

        StartCoroutine(Play(source));
    }

    private IEnumerator Play(AudioSource source)
    {
        activeAudioSources.Add(source);
        source.Play();

        if (!source.loop)
        {
            yield return new WaitForSeconds(source.clip.length);

            if (!availableSources.Contains(source))
                availableSources.Enqueue(source);
        }
    }

    private void StopPlayingSound(string tag)
    {
        AudioSource s = null;

        foreach (AudioSource source in activeAudioSources)
        {
            if (source.clip == keyValuePairs[tag])
            {
                s = source;
                break;
            }
        }

        if (s)
        {
            s.Stop();
            activeAudioSources.Remove(s);
            availableSources.Enqueue(s);
        }
    }
}

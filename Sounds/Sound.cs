///-------------------------------------------------------------------------
///   Copyright Wired Visions 2019
///   Class:            Sound
///   Description:      Holds information for a sound clip, to be used by the
///                     SoundManager.
///   Author:           Parker Staszkiewicz
///-------------------------------------------------------------------------

using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class Sound
{
    [SerializeField] private string tag = "New Sound";
    [SerializeField] private AudioClip clip = null;
    [SerializeField] [Range(0f, 1f)] private float volume = 1;
    [SerializeField] [Range(0.1f, 3f)] private float pitch = 1;
    [SerializeField] [Range(0f, 1f)] private float spatialBlend = 1;
    [SerializeField] private bool loop = false;

    public string Tag { get { return tag; } }
    public AudioClip Clip { get { return clip; } }
    public float Volume { get { return volume; } }
    public float Pitch { get { return pitch; } }
    public float SpatialBlend { get { return spatialBlend; } }
    public bool Loop { get { return loop; } }
}

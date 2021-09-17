using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
    private static AudioPlayer instance = null;

    public static AudioPlayer Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<AudioPlayer>();
            }
            return instance;
        }
    }

    [Header("Attributes")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private List<AudioClip> audioClips;

    public void Play(string audioName)
    {
        AudioClip audioClip = audioClips.Find(a => a.name == audioName);
        if (audioClips == null) return;

        audioSource.PlayOneShot(audioClip);
    }
}

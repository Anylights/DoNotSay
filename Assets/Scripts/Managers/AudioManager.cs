using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 音频管理类
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance = null;

    public Sound[] sounds;

    private Dictionary<string, AudioSource> audioSourcesDic;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        audioSourcesDic = new Dictionary<string, AudioSource>();

        for (int i = 0; i < sounds.Length; i++)
        {
            GameObject soundGameObject = new GameObject("Sound_" + i + "_" + sounds[i].name);
            soundGameObject.transform.SetParent(this.transform);
            sounds[i].SetSource(soundGameObject.AddComponent<AudioSource>());
            audioSourcesDic.Add(sounds[i].name, soundGameObject.GetComponent<AudioSource>());
        }
    }

    public void Play(string soundName, float volume = 1, bool loop = false)
    {
        AudioSource source = audioSourcesDic[soundName];
        if (!source.isPlaying)
        {
            source.volume = volume;
            source.loop = loop;
            source.Play();
        }
    }

    public void Pause(string soundName)
    {
        AudioSource source = audioSourcesDic[soundName];
        source.Pause();
    }

    public void UnPause(string soundName)
    {
        AudioSource source = audioSourcesDic[soundName];
        source.UnPause();
    }

    public void SetVolume(string soundName, float volume)
    {
        AudioSource source = audioSourcesDic[soundName];
        source.volume = volume;
    }

    public void Stop(string soundName)
    {
        AudioSource source = audioSourcesDic[soundName];
        source.Stop();
    }

    public void StopAll()
    {
        foreach (var audioSource in audioSourcesDic.Values)
        {
            audioSource.Stop();
        }
    }

}

[System.Serializable]
public class Sound
{
    public AudioClip clip;
    public string name;

    public float volume = 1;
    public bool loop = false;

    public Sound(string name, AudioClip clip)
    {
        this.name = name;
        this.clip = clip;
    }

    public void SetSource(AudioSource source)
    {
        source.clip = clip;
        source.volume = volume;
        source.loop = loop;
    }



}
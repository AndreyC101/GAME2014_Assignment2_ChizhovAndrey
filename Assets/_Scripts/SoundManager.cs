using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SoundID
{
    Menu_Music,
    Gameplay_Music,
    Coin_Collect,
    HealthPickup_Collect,
    Extra_Life,
    Player_Death,
    Enemy_Death,
    Platform_Crumble,
    Platform_Bounce,
    Checkpoint_Set
}

public class SoundManager : MonoBehaviour
{
    [Range(0.0f, 1.0f)]
    public float Volume = 0.5f;

    public AudioClip[] clips;

    private static SoundManager _Instance = null;
    private static int AudioChannel = 24;
    private static List<AudioSource> Channels = new List<AudioSource>();

    private void Awake()
    {
        if (_Instance == null)
        {
            _Instance = this;
            DontDestroyOnLoad(gameObject);

            // Spawn a number of game object containing AudioSource component
            for (int i = 0; i < AudioChannel; i++)
            {
                var channel = new GameObject("Channel " + i.ToString());
                channel.transform.parent = this.transform;
                channel.AddComponent<AudioSource>().volume = Volume;
                Channels.Add(channel.GetComponent<AudioSource>());
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static SoundManager Instance
    {
        get
        {
            if (_Instance != null)
            {
                return _Instance;
            }
            return null;
        }
    }

    public void PlayAudio(SoundID id)
    {
        foreach (var channel in Channels)
        {
            if (!channel.isPlaying)
            {
                channel.volume = Volume;
                channel.clip = clips[(int)id];
                channel.Play();
                return;
            }
        }
    }

    public void StopAudio(SoundID id)
    {
        foreach (var channel in Channels)
        {
            if (channel.isPlaying && channel.clip == clips[(int)id])
            {
                channel.Stop();
                channel.clip = null;
                return;
            }
        }
    }
}

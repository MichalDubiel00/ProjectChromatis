using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class Music
{
    public AudioClip clip;

    public string name;

    [Range(0f, 1f)]
    public float volume;
    [Range(0.1f, 3f)]
    public float pitch;

    public bool loop;
}

public class SoundManager : MonoBehaviour
{
	//singleton instance of Soundmanager
	public static SoundManager Instance { get; private set; }

    [SerializeField] Music[] music;


    //Setup saver for incoming colliders each frame to not play them multiple times;
    private HashSet<(Collider2D, Collider2D)> registeredPairs = new HashSet<(Collider2D, Collider2D)>();
	//SetupAudioMixer for Options to change Volume
	[SerializeField] private AudioMixer audioMixer;
	[SerializeField] AudioSource musicSource;
	[SerializeField] AudioSource SFXSource;
	[SerializeField] private GameObject player;
	private PlayerMovement playerMovement;
	[Header("----------- Audio Clips ----------")]
	[SerializeField] public AudioClip BGM;
	[SerializeField] public AudioClip walk01;
	[SerializeField] public AudioClip walk02;
	[SerializeField] public AudioClip JumpStart;
	[SerializeField] public AudioClip JumpLand;
	[SerializeField] public AudioClip collectDrop;
	[SerializeField] public AudioClip shootDrop;
	[SerializeField] public AudioClip dropletSplashing;
	[SerializeField] public AudioClip switchColor;

	
	private void Awake()
	{
		playerMovement = player.GetComponent<PlayerMovement>();
		if(Instance == null)
		{
			Instance = this;
		}
		else
		{
			Destroy(gameObject);
		}

        foreach (var sound in music)
        {
            SetMusicVolume(sound.volume);
            musicSource.pitch = sound.pitch;
            musicSource.loop = sound.loop;
        }
    }
    private void Start()
    {
        PlayMusic("Music");
    }
    private void Update()
	{
		//Clearing registered pairs at start of each frame
		registeredPairs.Clear();
	}

	public void PlayCollisionSound(Collision2D collision)
	{
		//Get the colliders for each collision
		Collider2D aCollider = collision.collider;
		Collider2D bCollider = collision.otherCollider;
		
		//Ensure consistent ordering of collider pairs
		var pair = aCollider.GetInstanceID() < bCollider.GetInstanceID()
			? (aCollider, bCollider)
			: (bCollider, aCollider);

		//If collision already happened return
		if (registeredPairs.Contains(pair))
		{
			return;
		}
		
		//Register pair
		registeredPairs.Add(pair);

		Debug.Log($"Tag from collider A: {aCollider.tag} and Tag from collider B: {bCollider.tag}");
		if (aCollider.CompareTag("Ground") && bCollider.CompareTag("Player"))
		{
			Debug.Log($"Playing collision sound for   {aCollider.gameObject.name} and {bCollider.gameObject.name}");
			SFXSource.PlayOneShot(JumpLand);
		}
	}

	public void SetSoundFXVolume(float soundVolume)
	{
		audioMixer.SetFloat("SoundFXMixer", Mathf.Log10(soundVolume)*20f);
	}

	public void SetMusicVolume(float musicVolume)
	{
		audioMixer.SetFloat("MusicVolumeMixer", Mathf.Log10(musicVolume)*20f);
	}

	public void PlaySFX(AudioClip audioClip)
	{
		SFXSource.PlayOneShot(audioClip);
	}

    public void PlayMusic(string name)
    {
        Music m = Array.Find(music, x => x.name == name);
        if (m == null)
        {
            return;
        }
        musicSource.clip = m.clip;
        musicSource.Play();
    }
}



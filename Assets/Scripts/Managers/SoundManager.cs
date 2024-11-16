using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{	

    [SerializeField] AudioSource musicSource;
	[SerializeField] AudioSource SFXSource;
	[Header("----------- Audio Clips ----------")]
	public AudioClip BGM;
	public AudioClip walk01;
	public AudioClip walk02;
	public AudioClip JumpStart;
	public AudioClip JumpLand;
	public AudioClip collectDrop;
	public AudioClip shootDrop;
	public AudioClip switchColor;


	public void PlaySFX(AudioClip audioClip)
	{
		SFXSource.PlayOneShot(audioClip);
	}
}



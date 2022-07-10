using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
	public AudioSource audioSource;
	public void Start() {
		audioSource = GetComponent<AudioSource>();
	}
	public IEnumerator FadeOut(float FadeTime) {
		while (audioSource.volume > 0)
		{
			audioSource.volume -= Time.deltaTime / FadeTime;

			yield return null;
		}

		audioSource.volume = 0f;
	}

	public IEnumerator FadeIn(float FadeTime) {

		while (audioSource.volume < 1)
		{
			audioSource.volume += Time.deltaTime / FadeTime;

			yield return null;
		}

		audioSource.volume = 1f;
	}
}

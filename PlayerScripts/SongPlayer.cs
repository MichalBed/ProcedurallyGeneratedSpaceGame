using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script randomly shuffles the song playlist and plays through the playlist

public class SongPlayer : MonoBehaviour
{
    public AudioClip[] music;

    private AudioSource audioSource;

    public void Awake() {
        audioSource = GetComponent<AudioSource>();        
    }

    public void Start() {
        StartCoroutine("ManageMusic");
    }

    public IEnumerator ManageMusic() {
        while (true) {


            List<AudioClip> musicList = new List<AudioClip>(music);
            for (int i = 0; i < musicList.Count; i++) {
                int randIndex = Random.Range(0, musicList.Count);
                audioSource.clip = musicList[randIndex];
                audioSource.Play();

                while (audioSource.isPlaying) {
                    yield return new WaitForSeconds(1);
                }
                musicList.RemoveAt(randIndex);
            }





            yield return new WaitForSeconds(1);
        }
        yield return new WaitForSeconds(5);
    }
    
}

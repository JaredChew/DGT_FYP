using UnityEngine;

using System;

public class AudioManager : MonoBehaviour {

    public Sound[] sound;
    public Music[] music;

    public void Awake() {

        if(Global.audioManager == null) { Global.audioManager = this; }
        else { Destroy(gameObject); return; }

        //DontDestroyOnLoad(gameObject);

        //Init sound files
        for (int i = 0; i < sound.Length; i++) {

            { //Start of temp scope

                Sound soundTemp = sound[i];

                soundTemp.source = gameObject.AddComponent<AudioSource>();
                soundTemp.source.clip = soundTemp.clip;

                soundTemp.source.volume = soundTemp.volume;
                soundTemp.source.pitch = soundTemp.pitch;

                soundTemp.source.loop = soundTemp.loop;

            } //End of temp scope

        }

        //Init music files
        for (int i = 0; i < music.Length; i++) {

            { //Start of temp scope

                Music musicTemp = music[i];

                musicTemp.source = gameObject.AddComponent<AudioSource>();
                musicTemp.source.clip = musicTemp.clip;

                musicTemp.source.volume = musicTemp.volume;
                musicTemp.source.pitch = musicTemp.pitch;

                musicTemp.source.loop = musicTemp.loop;

            } //End of temp scope

        }
        
    }
    
    public void playSound(string name) {

        int counter = 0;

        for( ; counter < sound.Length; counter++) {

            if(sound[counter].name == name) {
                break;
            }

        }

        if(counter == sound.Length) {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }

        sound[counter].source.Play();

    }

    public void playMusic(String name) {

        int counter = 0;

        for (; counter < music.Length; counter++) {

            if (music[counter].name == name) {
                break;
            }

        }

        if (counter == music.Length) {
            Debug.LogWarning("Music: " + name + " not found!");
            return;
        }

        music[counter].source.Play();

    }
    
}

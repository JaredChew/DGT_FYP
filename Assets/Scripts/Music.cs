using UnityEngine;

[System.Serializable]
public class Music {

    [HideInInspector] public AudioSource source;

    public AudioClip clip;

    public string name;

    public bool loop;

    [Range(0f, 1f)] public float volume = 1;
    [Range(0.1f, 3f)] public float pitch = 1;

}

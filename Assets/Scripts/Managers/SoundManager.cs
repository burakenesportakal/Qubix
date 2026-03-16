using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    [Header("Audio Clips")]
    public AudioClip musicClip;
    public AudioClip defaultClip;
    public AudioClip condAClip;
    public AudioClip condBClip;
    public AudioClip condCClip;
    private void Awake()
    {
        if (Instance == null)
        {
            transform.SetParent(null);

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        float savedVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        AudioListener.volume = savedVolume;
        PlayMusic();
    }

    public void PlayMusic()
    {
        if(musicClip !=null && musicSource != null)
        {
            if (musicSource.isPlaying && musicSource.clip == musicClip) return;

            musicSource.clip = musicClip;
            musicSource.loop = true;
            musicSource.Play();
        }
    }

    public void PlayConditionSounds(int matchSize, int condA, int condB, int condC)
    {
        AudioClip clipToPlay = defaultClip;

        if (matchSize >= condC) clipToPlay = condCClip;
        else if (matchSize >= condB) clipToPlay = condBClip;
        else if (matchSize >= condA) clipToPlay = condAClip;
        else clipToPlay = defaultClip;

        if (clipToPlay != null && sfxSource != null) 
        {
            sfxSource.PlayOneShot(clipToPlay);
        }
    }
}

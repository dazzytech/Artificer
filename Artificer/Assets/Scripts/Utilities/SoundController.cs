using UnityEngine;
using System.Collections;

public class SoundController : MonoBehaviour
{
    // INSTANCE RELATED
    private static 
        SoundController m_instance = null;
    
    public static SoundController Instance
    {
        get
        {
            return m_instance;
        }
    }

    // Source
    [SerializeField]
    private AudioSource musicSource;
    [SerializeField]
    private AudioSource fxSource;

    // Real World Affects
    public float HearingRange;

    // Play list variables
    private static int track;
    private static float playTime;
    private float changeTimer;
    private int[] currentPlayList;
    private int[] playedSongs;
    private bool playList;


    private bool inFade;
    private float fadeInSpeed = 0.1f;
    private float fadeOutSpeed = 0.3f;

    // music list
    public AudioClip[] music;

    // Use this for initialization
    void Awake()
    {
        if (m_instance == null)
        {
            m_instance = this;
            musicSource = gameObject.AddComponent<AudioSource>();
            fxSource = gameObject.AddComponent<AudioSource>();
            HearingRange = 100f;
            inFade = false;
            playList = false;
            track = -1;
        } 

    }

    void Update()
    {
        if (!inFade)
        {
            if (fxSource != null)
                fxSource.volume 
                = Audio_Config.Audio.SoundFX;

            if (musicSource != null)
                musicSource.volume 
                = Audio_Config.Audio.Music;
        }

        if (playList && !inFade)
        {
            if(SoundController.Instance.musicSource.time > changeTimer)
                PlayMusic(currentPlayList);
        }
    }

    public static void PlaySoundFXAt(Vector3 position, AudioClip clip)
    {
        Vector3 thisPos = Camera.main.
            transform.position + new Vector3(0, 0, 10);
        float distance = (position - thisPos).magnitude;

        if (distance <= SoundController.Instance.HearingRange)
        {
            // determine pitch and volume from distance
            float change = (distance / SoundController.Instance.HearingRange );
            float reduction = (Audio_Config.Audio.SoundFX 
                               * change);

            SoundController.Instance.fxSource.volume 
                -= reduction;

            float prevPitch = SoundController.Instance.fxSource.pitch;

            SoundController.Instance.fxSource.pitch *= Random.Range(0.9f, 1.1f);
            SoundController.Instance.fxSource.PlayOneShot(clip);
            SoundController.Instance.fxSource.pitch = prevPitch;
        }
    }

    public static void PlaySoundFX(AudioClip clip)
    {
        SoundController.Instance.fxSource.PlayOneShot(clip);
    }

    public static void PlayMusic(int[] songs)
    {
        if (songs.Length == 0)
            return;

        if (Instance.playedSongs.Length == 0)
        {
            Instance.playedSongs = new int[songs.Length];
            for(int i = 0; i < songs.Length; i++)
                Instance.playedSongs[i] = -1;
        } 

        // find index to next available song
        int available = 0;
        foreach (int item in Instance.playedSongs)
        {
            if(item != -1)
                available++;
        }

        if (available == songs.Length)
        {
            Instance.playedSongs = new int[songs.Length];
            for(int i = 0; i < songs.Length; i++)
                Instance.playedSongs[i] = -1;
            available = 0;
        }

        int newSong = songs[Random.Range(0, songs.Length)];

        if (SoundController.Instance.musicSource.isPlaying)
        {
            // If music is same do nothing
            if (track == newSong && songs.Length > 1)
            {
                // retry by calling function
                PlayMusic(songs);
                return;
            }
            foreach (int item in Instance.playedSongs)
            {
                if (item == newSong && songs.Length > 1)
                {
                    // retry by calling function
                    PlayMusic(songs);
                    return;
                }
            }
        }

        // Play the first song and start the timer;
        PlayMusic(newSong);
        Instance.currentPlayList = songs;
        Instance.playList = true;
    }

    public static void ClearPlayList()
    {
        Instance.currentPlayList = new int[0];
        Instance.playedSongs = new int[0];
        Instance.changeTimer = 0;
        Instance.playList = false;
    }

    public static void PlayMusic(int song)
    {
        if (SoundController.Instance.music != null)
        {
            if (SoundController.Instance.music[song]!= null)
            {
                // If music is same do nothing
                if(track == song)
                    return;

                if(Instance.inFade)
                {
                    // Fade song out then reinvoke this program
                    Instance.StopCoroutine("FadeOut");
                    Instance.StopCoroutine("FadeIn");
                    Instance.StartCoroutine("FadeOut", song);
                    return;
                }

                if(SoundController.Instance.musicSource.isPlaying)
                {
                    // Fade song out then reinvoke this program
                    Instance.StartCoroutine("FadeOut", song);
                    return;
                }

                SoundController.Instance.musicSource.clip =
                    (SoundController.Instance.music [song]);
                SoundController.Instance.musicSource.loop = true;
                // Fade in music
                Instance.StartCoroutine("FadeIn");
                SoundController.Instance.musicSource.Play();

                // incase of playlist
                track = song;
                float seconds = SoundController.Instance.musicSource.clip.length;
                Instance.changeTimer = seconds - seconds*(Instance.fadeOutSpeed * Time.deltaTime);
            }
        }
    }

    public static void PauseMusic()
    {
        if (SoundController.Instance.musicSource.isPlaying)
        {
            playTime = SoundController.Instance.musicSource.time;
            SoundController.Instance.musicSource.Pause();
        }
    }

    public static void ResumeMusic()
    {
        if (!SoundController.Instance.musicSource.isPlaying)
        {
            if(SoundController.Instance.musicSource.clip == null)
            {
                SoundController.Instance.musicSource.clip =
                    (SoundController.Instance.music [track]);
            }
            SoundController.Instance.musicSource.loop = true;
            SoundController.Instance.musicSource.time = playTime;
            Instance.StartCoroutine("FadeIn");
            SoundController.Instance.musicSource.Play();
        }
    }

    /// FADE UTILS
    public IEnumerator FadeIn()
    {
        inFade = true;
        musicSource.volume = 0f;

        while (true)
        {
            musicSource.volume += fadeInSpeed * Time.deltaTime;
            if(musicSource.volume >= Audio_Config.Audio.Music)
            {
                inFade = false;
                break;
            }
            yield return null;
        }
        Instance.StopCoroutine("FadeIn");
        yield return null;
    }

    public IEnumerator FadeOut(int songPending)
    {
        inFade = true;
        
        while (true)
        {
            musicSource.volume -= fadeOutSpeed * Time.deltaTime;
            if(musicSource.volume <= 0)
            {
                inFade = false;
                musicSource.Stop();
                break;
            }
            yield return null;
        }
        Instance.StopCoroutine("FadeIn");
        PlayMusic(songPending);
        yield return null;
    }
}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    public AudioSource sfxSource;
    public AudioSource musicSource;

    [Header("Audio Clips")]
    public AudioClip bgMusic;
    public AudioClip bgMusicRegularArea;
    public AudioClip bossMusic;
    public AudioClip footstepSFX;
    public AudioClip swordSlashSFX;
    public AudioClip dashSFX;
    public AudioClip deathSFX;
    public AudioClip onhitSFX;

    public AudioClip goblinAttackSFX, goblinpurpleAttackSFX, goblinDeathSFX;
    public AudioClip skelichAttackSFX, skelichDeathSFX;
    public AudioClip mimicAttackSFX, mimicDeathSFX;
    public AudioClip bossAttackSFX, bossDeathSFX;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persist between scenes
        }
        else
        {
            Destroy(gameObject); // Prevent duplicates
        }
    }

    private void Start()
    {
        PlayMusicByScene(SceneManager.GetActiveScene().name);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public void PlaySFX(AudioClip clip, float volumeScale = .35f)
    {
        sfxSource.PlayOneShot(clip, volumeScale);
    }

    public void PlayMusic(AudioClip musicClip)
    {
        if (musicSource.clip == musicClip) return;
        musicSource.clip = musicClip;
        musicSource.loop = true;
        musicSource.Play();
    }

    private void PlayMusicByScene(string sceneName)
    {
        switch (sceneName)
        {
            case "MainMenu":
                PlayMusic(bgMusic); // main menu music
                break;

            case "CellRoom":
            case "CellRoomFromHub":
            case "Blacksmith":
            case "BlacksmithToHub":
            case "Library":
            case "LibraryToHub":
            case "Hub":
            case "RespawnToHub":
                PlayMusic(bgMusicRegularArea); // peaceful/neutral areas
                break;

            case "BossRoom":
                PlayMusic(bossMusic); // boss battle
                break;

            default:
                PlayMusic(bgMusic); // fallback
                break;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlayMusicByScene(scene.name);
    }



    public void StopMusic()
    {
        musicSource.Stop();
    }
}

using System;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioClip largeExplosion;
    public AudioClip mediumExplosion;
    public AudioClip smallExplosion;
    public AudioClip beat1;
    public AudioClip beat2;
    public float HeartBeatRateAtStart = .75f;
    public float HeartBeatRateAtEnd = .25f;
    private AudioSource audioSource;
    private bool isPlaying = true;
    private float heartbeatSpeed;
    private float timestamp;
    private AudioClip heartbeat;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }
    private void Start()
    {
        Asteroid.OnDestroyed += OnAsteroidDestruction;
        GameManager.OnLevelStart += OnLevelStart;
        GameManager.OnLevelEnd += OnLevelEnd;
        BigSaucer.OnDestroyed += OnBigSaucerDestroyed;
        CubeHunter.OnDestroyed += OnCubeHunterDestroyed;
        DiamondHunter.OnDestroyed += OnDiamondHunterDestruction;
        Hunter.OnDestroyed += OnHunterDestruction;
    }

    private void OnHunterDestruction(Hunter hunter)
    {
        audioSource.PlayOneShot(smallExplosion);
    }

    private void OnDiamondHunterDestruction(DiamondHunter hunter)
    {
        audioSource.PlayOneShot(mediumExplosion);
    }

    private void OnCubeHunterDestroyed(CubeHunter hunter)
    {
        audioSource.PlayOneShot(largeExplosion);
    }

    private void OnBigSaucerDestroyed(BigSaucer saucer)
    {
        audioSource.PlayOneShot(largeExplosion);
    }

    private void OnDestroy()
    {
        BigSaucer.OnDestroyed -= OnBigSaucerDestroyed;
        Asteroid.OnDestroyed -= OnAsteroidDestruction;
        GameManager.OnLevelStart -= OnLevelStart;
        GameManager.OnLevelEnd -= OnLevelEnd;
    }

    private void Update()
    {
        if (isPlaying)
        {
            playHeartbeat();
        }
    }
    private void OnAsteroidDestruction(Asteroid asteroid)
    {
        if (asteroid.getSize() == Asteroid.LARGE)
        {
            audioSource.PlayOneShot(largeExplosion);
        }
        else if (asteroid.getSize() == Asteroid.MEDIUM)
        {
            audioSource.PlayOneShot(mediumExplosion);
        }
        else
        {
            audioSource.PlayOneShot(smallExplosion);
        }
    }


    void playHeartbeat()
    {
        var now = Time.time;
        if (now - timestamp > heartbeatSpeed)
        {
            timestamp = now;
            heartbeat = (heartbeat == beat1) ? beat2 : beat1;
            audioSource.PlayOneShot(heartbeat);
            heartbeatSpeed = Mathf.Max(heartbeatSpeed -.01f, HeartBeatRateAtEnd);
        }
    }

    void OnLevelStart(int level)
    {
        isPlaying = true;
        heartbeatSpeed = HeartBeatRateAtStart;
    }

    void OnLevelEnd(int level)
    {
        isPlaying = false;
    }
}

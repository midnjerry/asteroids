using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // 1 diamond 3 big level 1
    // 3 diamond 3 big?
    // Looks like level 1 spawn rate is 10s
    // decrements by one per level until 3s spawn rate
    // speed increases by a little bit per level


    public delegate void LevelStartEvent(int level);
    public static event LevelStartEvent OnLevelStart;
    public delegate void LevelEndEvent(int level);
    public static event LevelEndEvent OnLevelEnd;

    public WorldBorder worldBorder;
    public Asteroid asteroidPreFab;
    public BigSaucer bigSaucerPreFab;
    public CubeHunter cubeHunterFab;
    private HashSet<Asteroid> asteroidSet = new HashSet<Asteroid>();
    private BigSaucer activeBigSaucer;
    int level;
    int saucerCount;

    void Awake()
    {
        level = 0;
    }

    private void Start()
    {
        Asteroid.OnDestroyed += OnAsteroidDestruction;
        BigSaucer.OnDestroyed += OnBigSaucerDestruction;
        CubeHunter.OnDestroyed += OnCubeHunterDestruction;
    }

    void Spawn(float size)
    {
        float x = Random.Range(0f, worldBorder.getSize().x) - worldBorder.getSize().x / 2;
        float y = Random.Range(0f, worldBorder.getSize().y) - worldBorder.getSize().y / 2;

        Spawn(size, new Vector2(x, y));
    }

    void Spawn(float size, Vector2 spawnPoint)
    {
        float angle = Random.Range(0.0f, 360.0f);
        Quaternion rotation = Quaternion.Euler(0, 0, angle);
        Asteroid asteroid = Instantiate(asteroidPreFab, spawnPoint, rotation);
        asteroid.ScaleSize(size);
        asteroid.worldBorder = worldBorder;
        asteroidSet.Add(asteroid);
    }

    private void OnAsteroidDestruction(Asteroid asteroid)
    {
        asteroidSet.Remove(asteroid);
        if (asteroid.getSize() == Asteroid.LARGE)
        {
            Spawn(Asteroid.MEDIUM, asteroid.transform.position);
            Spawn(Asteroid.MEDIUM, asteroid.transform.position);
            AttemptBigSaucerSpawn();
            AttemptCubeHunterSpawn();
        } else if (asteroid.getSize() == Asteroid.MEDIUM)
        {
            Spawn(Asteroid.SMALL, asteroid.transform.position);
            Spawn(Asteroid.SMALL, asteroid.transform.position);
            AttemptBigSaucerSpawn();
            AttemptCubeHunterSpawn();

        }
    }

    private void OnBigSaucerDestruction(BigSaucer saucer)
    {
        activeBigSaucer = null;
    }

    private void OnCubeHunterDestruction(CubeHunter cubeHunter)
    {
        
    }

    void AttemptCubeHunterSpawn()
    {
        float x = Random.Range(0f, worldBorder.getSize().x) - worldBorder.getSize().x / 2;
        float y = Random.Range(0f, worldBorder.getSize().y) - worldBorder.getSize().y / 2;
        var spawnPoint = new Vector2(x, y);
        float angle = Random.Range(0.0f, 360.0f);
        Quaternion rotation = Quaternion.Euler(0, 0, angle);
        CubeHunter cubeHunter = Instantiate(cubeHunterFab, spawnPoint, rotation);
        cubeHunter.worldBorder = worldBorder;
    }


    void AttemptBigSaucerSpawn()
    {
        if (activeBigSaucer == null)
        {
            saucerCount++;
            bool leftSide = Random.Range(0, 2) == 0;
            float x = leftSide ? -worldBorder.getSize().x / 2 : worldBorder.getSize().x / 2;
            float y = Random.Range(0f, worldBorder.getSize().y) - worldBorder.getSize().y / 2;
            activeBigSaucer = Instantiate(bigSaucerPreFab, new Vector2(x, y), Quaternion.Euler(0, 0, 0));
            activeBigSaucer.worldBorder = worldBorder;
            activeBigSaucer.setAsteroids(asteroidSet);
            if (level > 0 && saucerCount % 3 == 0)
            {
                activeBigSaucer.setSmall();
            }
        }
    }
   
    void Update()
    {
        if (asteroidSet.Count == 0)
        {
            if (level >= 1)
            {
                OnLevelEnd?.Invoke(level);
            }
            level++;
            int count = getAsteroidCount();
            for (int i = 0; i < count; i++)
            {
                Spawn(Asteroid.LARGE);
            }
            AttemptBigSaucerSpawn();
            AttemptCubeHunterSpawn();
            OnLevelStart?.Invoke(level);
        }
    }

    private void OnDestroy()
    {
        Asteroid.OnDestroyed -= OnAsteroidDestruction;
    }

    private int getAsteroidCount()
    {
        if (level <= 1) return 4;
        if (level <= 2) return 5;
        if (level <= 4) return 6;
        if (level <= 5) return 7;
        if (level <= 6) return 8;
        return 9;
    }
}

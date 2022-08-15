using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public WorldBorder worldBorder;
    public Asteroid asteroidPreFab;
    const float LARGE = 2.0f;
    const float MEDIUM = 1.0f;
    const float SMALL = .5f;
    private HashSet<Asteroid> asteroidSet = new HashSet<Asteroid>();
    int level;
    // Start is called before the first frame update
    void Awake()
    {
        level = 1;
    }

    private void Start()
    {
        Asteroid.OnDestroyed += OnAsteroidDestruction;
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
        if (asteroid.getSize() == LARGE)
        {
            Spawn(MEDIUM, asteroid.transform.position);
            Spawn(MEDIUM, asteroid.transform.position);
        } else if (asteroid.getSize() == MEDIUM)
        {
            Spawn(SMALL, asteroid.transform.position);
            Spawn(SMALL, asteroid.transform.position);
        }
    }
   
    void Update()
    {
        if (asteroidSet.Count == 0)
        {
            level++;
            int count = getAsteroidCount();
            for (int i = 0; i < count; i++)
            {
                Spawn(LARGE);
            }
        }
    }

    private int getAsteroidCount()
    {
        if (level <= 2) return 5;
        if (level <= 4) return 6;
        if (level <= 5) return 7;
        if (level <= 6) return 8;
        return 9;
    }
}

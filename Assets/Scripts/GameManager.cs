using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public delegate void LevelStartEvent(int level);
    public static event LevelStartEvent OnLevelStart;
    public delegate void LevelEndEvent(int level);
    public static event LevelEndEvent OnLevelEnd;
    public Dashboard dashboard;
    public Player player;
    public WorldBorder worldBorder;
    public Explosion explosionPreFab;
    public Asteroid asteroidPreFab;
    public Saucer bigSaucerPreFab;
    public CubeHunter cubeHunterFab;
    public DiamondHunter diamondHunterPreFab;
    public Hunter hunterPreFab;
    private HashSet<GameObject> asteroidSet = new HashSet<GameObject>();
    private HashSet<GameObject> hunterSet = new HashSet<GameObject>();
    private Saucer activeSaucer;
    private bool isPlayerDead = true;
    public int lives = 5;
    int level;
    int saucerCount;
    int score = 0;
    int goal = 10000;
    int goalIncrement = 10000;
    float saucerTimestamp;
    float hunterTimestamp;
    float playerDeathTimestamp;

    void Awake()
    {
        lives = 5;
        level = 0;
        saucerTimestamp = Time.time;
        hunterTimestamp = Time.time;
        playerDeathTimestamp = 0f;
    }

    private void Start()
    {
        Asteroid.OnDestroyed += OnAsteroidDestruction;
        Saucer.OnDestroyed += OnSaucerDestruction;
        CubeHunter.OnDestroyed += OnCubeHunterDestruction;
        DiamondHunter.OnDestroyed += OnDiamondHunterDestruction;
        Hunter.OnDestroyed += OnHunterDestruction;
        Player.OnDestroyed += OnPlayerDeath;
        dashboard.setGameOver(false);
    }

    private void OnPlayerDeath(Player player)
    {
        SpawnExplosion(player.transform);
        isPlayerDead = true;
        playerDeathTimestamp = Time.time;
        if (lives == 0)
        {
            dashboard.setGameOver(true);
        }
    }

    void SpawnExplosion(Transform transform)
    {
        Instantiate(explosionPreFab, transform.position, Quaternion.identity);
    }

    void SpawnAsteroid(float size)
    {
        float width = worldBorder.getSize().x;
        float height = worldBorder.getSize().y;
        float randomNeg = Random.Range(0, 2) == 0 ? -1 : 1;
        float x = Random.Range(width * .8f, width) / 2 * randomNeg;
        float y = Random.Range(height * .8f, height) / 2 * randomNeg;
        SpawnAsteroid(size, new Vector2(x, y));
    }

    void SpawnAsteroid(float size, Vector2 spawnPoint)
    {
        float angle = Random.Range(0.0f, 360.0f);
        Quaternion rotation = Quaternion.Euler(0, 0, angle);
        Asteroid asteroid = Instantiate(asteroidPreFab, spawnPoint, rotation);
        asteroid.ScaleSize(size);
        asteroid.worldBorder = worldBorder;
        asteroidSet.Add(asteroid.gameObject);
    }

    private void OnAsteroidDestruction(Asteroid asteroid)
    {
        asteroidSet.Remove(asteroid.gameObject);
        SpawnExplosion(asteroid.transform);
        if (asteroid.getSize() == Asteroid.LARGE)
        {
            updateScore(20);
            SpawnAsteroid(Asteroid.MEDIUM, asteroid.transform.position);
            SpawnAsteroid(Asteroid.MEDIUM, asteroid.transform.position);
        } else if (asteroid.getSize() == Asteroid.MEDIUM)
        {
            updateScore(50);
            SpawnAsteroid(Asteroid.SMALL, asteroid.transform.position);
            SpawnAsteroid(Asteroid.SMALL, asteroid.transform.position);
        } else
        {
            updateScore(100);
        }
    }

    private void updateScore(int points)
    {
        if (player.gameObject.activeSelf)
        {
            score += points;
            dashboard.updateScore(score);

            if (score > goal)
            {
                goal += goalIncrement;
                dashboard.updateGoal(goal);
                lives++;
                dashboard.updateLives(lives);
            }
        }
    }


    private void OnSaucerDestruction(Saucer saucer)
    {
        SpawnExplosion(saucer.transform);
        activeSaucer = null;
        if (saucer.isSmall())
        {
            updateScore(1000);
        }
        else
        {
            updateScore(200);
        }
    }

    private void OnCubeHunterDestruction(CubeHunter cubeHunter)
    {
        asteroidSet.Remove(cubeHunter.gameObject);
        hunterSet.Remove(cubeHunter.gameObject);
        SpawnExplosion(cubeHunter.transform);
        float x = cubeHunter.transform.position.x;
        float y = cubeHunter.transform.position.y;

        SpawnDiamondHunter(x+.415f, y, -3, player.gameObject);
        SpawnDiamondHunter(x+.05f, y+.3f, 125, player.gameObject);
        SpawnDiamondHunter(x-.05f, y-.3f, -130, player.gameObject);
        updateScore(50);
    }

    private void OnDiamondHunterDestruction(DiamondHunter diamondHunter)
    {
        SpawnExplosion(diamondHunter.transform);
        hunterSet.Remove(diamondHunter.gameObject);
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        var top = diamondHunter.transform.position + (diamondHunter.transform.up * .3f);
        var bottom = diamondHunter.transform.position - (diamondHunter.transform.up * .3f);
        var bottomRotation = diamondHunter.transform.rotation * Quaternion.Euler(0, 0, 180f);
        SpawnHunter(top.x, top.y, diamondHunter.transform.rotation, player);
        SpawnHunter(bottom.x, bottom.y, bottomRotation, player);
        updateScore(100);
    }

    private void OnHunterDestruction(Hunter hunter)
    {
        SpawnExplosion(hunter.transform);
        hunterSet.Remove(hunter.gameObject);
        updateScore(200);
    }

    void SpawnHunter(float x, float y, Quaternion rotation, GameObject target)
    {
        var spawnPoint = new Vector2(x, y);
        Hunter hunter = Instantiate(hunterPreFab, spawnPoint, rotation);
        hunter.worldBorder = worldBorder;
        hunter.setTarget(target);
        hunterSet.Add(hunter.gameObject);
    }

    void SpawnDiamondHunter(float x, float y, float angle, GameObject target)
    {
        var spawnPoint = new Vector2(x, y);
        Quaternion rotation = Quaternion.Euler(0, 0, angle);
        DiamondHunter diamondHunter = Instantiate(diamondHunterPreFab, spawnPoint, rotation);
        diamondHunter.worldBorder = worldBorder;
        diamondHunter.setTarget(target);
        hunterSet.Add(diamondHunter.gameObject);
    }

    void AttemptCubeHunterSpawn()
    {
        if (hunterSet.Count == 0)
        {
            bool leftSide = Random.Range(0, 2) == 0;
            float x = leftSide ? -worldBorder.getSize().x / 2 : worldBorder.getSize().x / 2;
            float y = Random.Range(0f, worldBorder.getSize().y) - worldBorder.getSize().y / 2;
            var spawnPoint = new Vector2(x, y);
            Quaternion rotation = Quaternion.Euler(0, 0, 0);
            CubeHunter cubeHunter = Instantiate(cubeHunterFab, spawnPoint, rotation);
            cubeHunter.worldBorder = worldBorder;
            asteroidSet.Add(cubeHunter.gameObject);
            hunterSet.Add(cubeHunter.gameObject);
        }
    }


    void AttemptSaucerSpawn()
    {
        if (activeSaucer == null)
        {
            saucerCount++;
            bool leftSide = Random.Range(0, 2) == 0;
            float x = leftSide ? -worldBorder.getSize().x / 2 : worldBorder.getSize().x / 2;
            float y = Random.Range(0f, worldBorder.getSize().y) - worldBorder.getSize().y / 2;
            activeSaucer = Instantiate(bigSaucerPreFab, new Vector2(x, y), Quaternion.Euler(0, 0, 0));
            activeSaucer.worldBorder = worldBorder;
            activeSaucer.setTargets(asteroidSet);
            if (score > 10000 && saucerCount % 3 == 0)
            {
                activeSaucer.setSize(Saucer.SMALL);
            } else
            {
                activeSaucer.setSize(Saucer.BIG);
            }
        }
    }
   
    void Update()
    {
        var now = Time.time;

        if (canPlayerSpawn())
        {
            lives--;
            isPlayerDead = false;
            player.transform.position = Vector2.zero;
            player.gameObject.SetActive(true);
            OnLevelStart?.Invoke(level);
            dashboard.updateLives(lives);
        }

        if (asteroidSet.Count == 0)
        {
            level++;
            if (level > 1)
            {
                OnLevelEnd?.Invoke(level);
            }
            int count = getAsteroidCount(level);
            for (int i = 0; i < count; i++)
            {
                SpawnAsteroid(Asteroid.LARGE);
            }
            OnLevelStart?.Invoke(level);
        }

        if (now - saucerTimestamp > getSaucerSpawnTime(level))
        {
            AttemptSaucerSpawn();
            saucerTimestamp = now;
        }
        else if (now - hunterTimestamp > getHunterSpawnTime(level))
        {
            AttemptCubeHunterSpawn();
            hunterTimestamp = now;
        }

    }

    private bool canPlayerSpawn()
    {
        var now = Time.time;
        return isPlayerDead && lives > 0 && (now - playerDeathTimestamp > 3f || now < 3f);
    }

    private void OnDestroy()
    {
        Asteroid.OnDestroyed -= OnAsteroidDestruction;
    }

    private int getAsteroidCount(int level)
    {
        return Mathf.Min(level + 3, 9);
    }

    private float getSaucerSpawnTime(int level)
    {
        return Mathf.Max(11 - level, 3);
    }

    private float getHunterSpawnTime(int level)
    {
        return Mathf.Max(11 - level, 3);
    }
}

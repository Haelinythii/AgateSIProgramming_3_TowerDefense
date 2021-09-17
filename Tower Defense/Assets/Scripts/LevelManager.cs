using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    private static LevelManager instance = null;

    public static LevelManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<LevelManager>();

            }
            return instance;
        }
    }

    [SerializeField] private Transform towerUIParent;
    [SerializeField] private GameObject towerUIPrefab;
    [SerializeField] private Tower[] towerPrefabs;
    [SerializeField] private Enemy[] enemyPrefabs;

    private List<Tower> spawnedTowers = new List<Tower>();
    private List<Enemy> spawnedEnemies = new List<Enemy>();
    private List<Bullet> spawnedBullets = new List<Bullet>();

    [SerializeField] private Transform[] enemyPaths;
    [SerializeField] private float enemySpawnDelay = 5f;

    [SerializeField] private int maxLives = 3;
    [SerializeField] private int totalEnemies = 15;

    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private Text statusText;
    [SerializeField] private Text livesText;
    [SerializeField] private Text totalEnemyText;

    private float enemySpawnDelayTimer;
    private int currentLives;
    private int enemyCounter;

    public bool IsOver { get; private set; }

    private void Start()
    {
        SetCurrentLives(maxLives);
        SetEnemyCounter(totalEnemies);
        CreateAllTowerUI();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        if (IsOver)
        {
            return;
        }

        enemySpawnDelayTimer -= Time.unscaledDeltaTime;
        if(enemySpawnDelayTimer <= 0f)
        {
            SpawnEnemy();
            enemySpawnDelayTimer = enemySpawnDelay;
        }

        foreach (Enemy enemy in spawnedEnemies)
        {
            if (!enemy.gameObject.activeSelf) continue;

            if (Vector2.Distance(enemy.transform.position, enemy.Target) < 0.1f)
            {
                enemy.SetCurrentPathIndex(enemy.CurrentPathIndex + 1);
                if(enemy.CurrentPathIndex < enemyPaths.Length)
                {
                    enemy.SetTarget(enemyPaths[enemy.CurrentPathIndex].position);
                }
                else
                {
                    enemy.gameObject.SetActive(false);
                }
            }
            else
            {
                enemy.MoveToTarget();
            }
        }

        foreach (Tower tower in spawnedTowers)
        {
            tower.CheckNearestEnemy(spawnedEnemies);
            tower.SeekTarget();
            tower.ShootTarget();
        }
    }

    private void SpawnEnemy()
    {
        SetEnemyCounter(--enemyCounter);
        if(enemyCounter < 0)
        {
            bool isAllEnemyDestroyed = spawnedEnemies.Find(e => e.gameObject.activeSelf) == null;
            if (isAllEnemyDestroyed)
            {
                SetGameOver(true);
            }
            return;
        }

        int randomNumber = UnityEngine.Random.Range(0, enemyPrefabs.Length);

        GameObject enemyGO = GetOrCreateEnemy(randomNumber);

        Enemy enemy = enemyGO.GetComponent<Enemy>();
        if (!spawnedEnemies.Contains(enemy))
        {
            spawnedEnemies.Add(enemy);
        }

        enemy.transform.position = enemyPaths[0].position;
        enemy.SetTarget(enemyPaths[1].position);
        enemy.SetCurrentPathIndex(1);
        enemy.gameObject.SetActive(true);
    }

    private void SetGameOver(bool isPlayerWin)
    {
        IsOver = true;

        gameOverPanel.SetActive(true);
        statusText.text = isPlayerWin ? "You Win!" : "You Lose!";
    }

    private GameObject GetOrCreateEnemy(int enemyIndex)
    {
        string enemyPrefabIndex = (enemyIndex + 1).ToString();
        GameObject inactiveEnemy = spawnedEnemies.Find(e => !e.gameObject.activeSelf && e.name.Contains(enemyPrefabIndex))?.gameObject;

        if (inactiveEnemy == null)
        {
            inactiveEnemy = Instantiate(enemyPrefabs[enemyIndex].gameObject);
        }

        return inactiveEnemy;
    }

    private void CreateAllTowerUI()
    {
        foreach (Tower tower in towerPrefabs)
        {
            GameObject towerUIGO = Instantiate(towerUIPrefab, towerUIParent);
            TowerUI towerUI = towerUIGO.GetComponent<TowerUI>();
            towerUI.SetTowerPrefab(tower);
            towerUI.transform.name = tower.name;
        }
    }

    public void ExplodeAt(Vector2 point, float radius, int damage)
    {
        foreach (Enemy enemy in spawnedEnemies)
        {
            if (enemy.gameObject.activeSelf)
            {
                if(Vector2.Distance(point, enemy.transform.position) <= radius)
                {
                    enemy.ReduceHealth(damage);
                }
            }
        }
    }

    public Bullet GetBulletFromPool(Bullet prefab)
    {
        GameObject bulletGO = spawnedBullets.Find(b => !b.gameObject.activeSelf && b.name.Contains(prefab.name))?.gameObject;

        if(bulletGO == null)
        {
            bulletGO = Instantiate(prefab.gameObject);
        }

        Bullet bullet = bulletGO.GetComponent<Bullet>();

        if (!spawnedBullets.Contains(bullet))
        {
            spawnedBullets.Add(bullet);
        }

        return bullet;
    }

    public void RegisterSpawnedTower(Tower tower)
    {
        spawnedTowers.Add(tower);
    }

    private void SetCurrentLives(int lives)
    {
        currentLives = Mathf.Max(lives, 0);
        livesText.text = $"Lives: {currentLives}";
    }

    private void SetEnemyCounter(int enemies)
    {
        enemyCounter = enemies;
        totalEnemyText.text = $"Total Enemy: {Mathf.Max(enemyCounter, 0)}";
    }

    public void ReduceLives(int value)
    {
        SetCurrentLives(currentLives - value);
        if (currentLives <= 0)
        {
            SetGameOver(false);
        }
    }

    private void OnDrawGizmos()
    {
        for (int i = 0; i < enemyPaths.Length - 1; i++)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(enemyPaths[i].position, enemyPaths[i + 1].position);
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private void Start()
    {
        CreateAllTowerUI();
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
}

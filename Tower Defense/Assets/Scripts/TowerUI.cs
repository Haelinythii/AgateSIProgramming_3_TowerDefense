using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TowerUI : MonoBehaviour
{
    [SerializeField] private Image towerIcon;

    private Tower towerPrefab;

    public void SetTowerPrefab(Tower tower)
    {
        towerPrefab = tower;
        towerIcon.sprite = tower.GetTowerHeadIcon();
    }
}

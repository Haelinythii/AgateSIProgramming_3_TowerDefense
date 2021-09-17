using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TowerUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private Image towerIcon;

    private Tower towerPrefab;
    private Tower currentSpawnedTower;

    public void OnBeginDrag(PointerEventData eventData)
    {
        GameObject towerGO = Instantiate(towerPrefab.gameObject);
        currentSpawnedTower = towerGO.GetComponent<Tower>();
        currentSpawnedTower.ToggleOrderInLayer(true);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = -Camera.main.transform.position.z;
        Vector3 target = Camera.main.ScreenToWorldPoint(mousePos);

        currentSpawnedTower.transform.position = target;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if(currentSpawnedTower.PlacePosition == null)
        {
            Destroy(currentSpawnedTower.gameObject);
        }
        else
        {
            currentSpawnedTower.LockPlacement();
            currentSpawnedTower.ToggleOrderInLayer(false);
            LevelManager.Instance.RegisterSpawnedTower(currentSpawnedTower);
            currentSpawnedTower = null;
        }
    }

    public void SetTowerPrefab(Tower tower)
    {
        towerPrefab = tower;
        towerIcon.sprite = tower.GetTowerHeadIcon();
    }
}

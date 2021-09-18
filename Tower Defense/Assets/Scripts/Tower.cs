using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    [Header("Properties")]
    [SerializeField] private int shootPower = 1;
    [SerializeField] private float shootDistance = 1f;
    [SerializeField] private float shootDelay = 5f;
    [SerializeField] private float bulletSpeed = 1f;
    [SerializeField] private float bulletSplashRadius = 0f;
    [SerializeField] private float bulletSlowDuration = 0f;
    [SerializeField] private Bullet bulletPrefab;
    private float shootDelayTimer;

    public Vector2? PlacePosition { get; private set; }

    [Header("Reference")]
    [SerializeField] private SpriteRenderer towerPlace;
    [SerializeField] private SpriteRenderer towerHead;
    public Enemy enemyTarget;
    private Quaternion targetRotation;

    public void SetPlacePosition(Vector2? newPosisiton)
    {
        PlacePosition = newPosisiton;
    }

    public void LockPlacement()
    {
        transform.position = (Vector2)PlacePosition;
    }

    public void ToggleOrderInLayer(bool toFront)
    {
        int orderInLayer = toFront ? 2 : 0;
        towerPlace.sortingOrder = orderInLayer;
        towerHead.sortingOrder = orderInLayer;
    }

    public void CheckNearestEnemy(List<Enemy> enemies)
    {
        if(enemyTarget != null)
        {
            if(!enemyTarget.gameObject.activeSelf || Vector3.Distance(transform.position, enemyTarget.transform.position) > shootDistance)
            {
                enemyTarget = null;
            }
            else
            {
                return;
            }
        }

        float nearestDistance = Mathf.Infinity;
        Enemy nearestEnemy = null;

        foreach (Enemy enemy in enemies)
        {
            if (!enemy.gameObject.activeSelf) continue;
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if(distance > shootDistance)
            {
                continue;
            }

            if(distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestEnemy = enemy;
            }
        }

        enemyTarget = nearestEnemy;
    }

    public void SeekTarget()
    {
        if (enemyTarget == null) return;

        Vector3 directionToTarget = enemyTarget.transform.position - transform.position;
        float angleToTarget = Mathf.Atan2(directionToTarget.y, directionToTarget.x) * Mathf.Rad2Deg;
        targetRotation = Quaternion.Euler(new Vector3(0f, 0f, angleToTarget - 90f));

        towerHead.transform.rotation = Quaternion.RotateTowards(towerHead.transform.rotation, targetRotation, Time.deltaTime * 180f);
    }

    public void ShootTarget()
    {
        if(enemyTarget == null)
        {
            return;
        }

        shootDelayTimer -= Time.unscaledDeltaTime;
        if(shootDelayTimer <= 0f)
        {
            bool headHasAimed = Mathf.Abs(towerHead.transform.rotation.eulerAngles.z - targetRotation.eulerAngles.z) < 10f;
            if (!headHasAimed)
            {
                return;
            }

            Bullet bullet = LevelManager.Instance.GetBulletFromPool(bulletPrefab);
            bullet.transform.position = transform.position;
            bullet.SetAttributes(shootPower, bulletSpeed, bulletSplashRadius, bulletSlowDuration);
            bullet.SetTarget(enemyTarget);
            bullet.gameObject.SetActive(true);

            shootDelayTimer = shootDelay;
        }
    }

    public Sprite GetTowerHeadIcon()
    {
        return towerHead.sprite;
    }

    public Color GetTowerHeadColor()
    {
        return towerHead.color;
    }
}

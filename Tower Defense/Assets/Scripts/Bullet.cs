using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Attributes")]
    private int power;
    private float speed;
    private float splashRadius;
    private float slowDuration;
    private int maxBounceToOtherTarget;
    private float bounceRadius;
    public LayerMask enemyLayerMask;

    [Header("Reference")]
    private Enemy target;

    private void FixedUpdate()
    {
        if (LevelManager.Instance.IsOver) return;

        if (target != null)
        {
            if (!target.gameObject.activeSelf)
            {
                gameObject.SetActive(false);
                target = null;
                return;
            }

            transform.position = Vector3.MoveTowards(transform.position, target.transform.position, speed * Time.fixedDeltaTime);

            Vector3 directionToTarget = target.transform.position - transform.position;
            float angleToTarget = Mathf.Atan2(directionToTarget.y, directionToTarget.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, angleToTarget - 90f));
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (target == null) return;

        if (collision.gameObject.Equals(target.gameObject))
        {
            if (splashRadius > 0f)
            {
                LevelManager.Instance.ExplodeAt(transform.position, splashRadius, power);
            }
            else
            {
                target.ReduceHealth(power);
                if(slowDuration > 0f)
                {
                    target.ApplySlowEffect(slowDuration);
                }
            }

            if (maxBounceToOtherTarget <= 0)
            {
                target = null;
                gameObject.SetActive(false);
            }
            else
                RicochetToOtherTarget();

            //target = null;
        }
    }

    private void RicochetToOtherTarget()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, bounceRadius, enemyLayerMask);
        Enemy newEnemyTarget = null;
        foreach (Collider2D enemy in hits)
        {
            if (!enemy.GetComponent<Enemy>().Equals(target))
            {
                newEnemyTarget = enemy.GetComponent<Enemy>();
                break;
            }
        }

        if(newEnemyTarget == null)
        {
            gameObject.SetActive(false);
            maxBounceToOtherTarget = 0;
            bounceRadius = 0;
            target = null;
        }
        else
        {
            target = newEnemyTarget;
            maxBounceToOtherTarget--;
        }
    }

    public void SetAttributes(int _power, float _speed, float _splashRadius, float _slowDuration, int _maxBounceToOtherTarget, float _bounceRadius)
    {
        power = _power;
        speed = _speed;
        splashRadius = _splashRadius;
        slowDuration = _slowDuration;
        maxBounceToOtherTarget = _maxBounceToOtherTarget;
        bounceRadius = _bounceRadius;
    }

    public void SetTarget(Enemy enemy)
    {
        target = enemy;
    }
}

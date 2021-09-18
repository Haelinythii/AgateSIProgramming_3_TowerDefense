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
            gameObject.SetActive(false);

            if(splashRadius > 0f)
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

            target = null;
        }
    }

    public void SetAttributes(int _power, float _speed, float _splashRadius, float _slowDuration)
    {
        power = _power;
        speed = _speed;
        splashRadius = _splashRadius;
        slowDuration = _slowDuration;
    }

    public void SetTarget(Enemy enemy)
    {
        target = enemy;
    }
}

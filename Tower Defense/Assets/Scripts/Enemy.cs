using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private int maxHealth = 1;
    [SerializeField] private float movementSpeed = 1f;
    [SerializeField] private SpriteRenderer healthBar;
    [SerializeField] private SpriteRenderer healthFill;

    private int currentHealth;

    public Vector3 Target { get; private set; }
    public int CurrentPathIndex { get; private set; }

    public void MoveToTarget()
    {
        transform.position = Vector3.MoveTowards(transform.position, Target, movementSpeed * Time.deltaTime);
    }

    public void SetTarget(Vector3 target)
    {
        Target = target;
        healthBar.transform.parent = null;

        SetEnemyRotation();

        healthBar.transform.parent = transform;
    }

    private void SetEnemyRotation()
    {
        Vector3 distance = Target - transform.position;
        if (Mathf.Abs(distance.y) > Mathf.Abs(distance.x)) //ambil jarak x atau y yang paling besar
        {
            if (distance.y > 0) //target ada di atas
            {
                transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 90f));
            }
            else //target ada di bawah
            {
                transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, -90f));
            }
        }
        else
        {
            if (distance.x > 0)//target ada dikanan
            {
                transform.rotation = Quaternion.Euler(Vector3.zero);
            }
            else //target ada dikiri
            {
                transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 180f));
            }
        }
    }

    public void SetCurrentPathIndex(int currentIndex)
    {
        CurrentPathIndex = currentIndex;
    }

    public void ReduceHealth(int damage)
    {
        currentHealth -= damage;
        SetHealthBarSize();
        AudioPlayer.Instance.Play("hit-enemy");
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            gameObject.SetActive(false);
            AudioPlayer.Instance.Play("enemy-die");
        }
    }

    private void SetHealthBarSize()
    {
        healthFill.size = new Vector2((float)currentHealth / (float)maxHealth * healthBar.size.x, healthFill.size.y);
    }

    private void OnEnable()
    {
        currentHealth = maxHealth;
        healthFill.size = healthBar.size;
    }
}

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

    [Header("Reference")]
    [SerializeField] private SpriteRenderer towerPlace;
    [SerializeField] private SpriteRenderer towerHead;

    public Sprite GetTowerHeadIcon()
    {
        return towerHead.sprite;
    }

}

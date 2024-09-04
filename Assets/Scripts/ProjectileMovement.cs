using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class ProjectileMovement : MonoBehaviour
{
    
    [SerializeField] private float _speed = 0f;
    [SerializeField] private float _horDir = 0f;
    private Rigidbody2D _rb2dRef;

    private void Awake()
    {
        _rb2dRef = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        _rb2dRef.AddForce(_speed * Vector2.right * _horDir);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Destroy(gameObject);
    }

}

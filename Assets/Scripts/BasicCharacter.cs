using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicCharacter : MonoBehaviour
{
  
    protected MovementBehavior _movementBehavior;
    protected Rigidbody2D _rigidBody2D;
    protected Health _playerHealth;


    protected virtual void Awake()
    {
        _movementBehavior = GetComponent<MovementBehavior>();
        _rigidBody2D = gameObject.GetComponent<Rigidbody2D>();
        _playerHealth = GetComponent<Health>();
    }
}

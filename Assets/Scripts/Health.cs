using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private int _maxHealth;
    private int _currentHealth;
    private bool _isValid = true;


    public bool IsValid 
    {
        get { return _isValid; } 
    }
    

    private void Awake()
    {
        _currentHealth = _maxHealth;
    }

    public void TakeDamage(int damageAmount)
    {
        _currentHealth -= damageAmount;
        if (_currentHealth < 1 )
        {
            _currentHealth = 0;
            _isValid = false;
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileSpawner : MonoBehaviour
{
    [SerializeField] private GameObject _prefab;
    [SerializeField] private float _timeInterval = 2f;
    [SerializeField] private float _offset = 0f;
    private float _timer;
    private bool _shouldStartSpawning = false;

    [SerializeField] private Tutorial _tutorial;

    private void Start()
    {
        _timer = _timeInterval;    
    }

    private void Update()
    {


        if (!_shouldStartSpawning)
        {
            if (_tutorial !=null)
            {
                if (_tutorial.IsTutorialDone)
                {
                    _shouldStartSpawning = true;
                }
            }
        }
        else
        {
            _timer -= Time.deltaTime;

            if (_timer <= 0)
            {
                _timer = _timeInterval;

                Instantiate(_prefab, transform.position + Vector3.right * _offset, transform.rotation);
            }
        }

        


    }

}
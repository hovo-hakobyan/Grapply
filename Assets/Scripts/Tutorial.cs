using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    private bool _canMove = false;
    private bool _canGrapple = false;
    private bool _canDetatch = false;
    private bool _tutorialDone = false;

    private MovementBehavior _movementBehavior;
    [SerializeField] private ShowInfo[] _showInfo;

    private float _timer = 4f;
    private float _currentTime = 0f;


    public bool IsTutorialDone
    {
        get { return _tutorialDone; }
    }
    private void Awake()
    {
        _movementBehavior = GetComponent<MovementBehavior>();
    }
    private void Update()
    {
        if (_tutorialDone)
            return;

        if(_canMove && _canGrapple && _canDetatch)
            _tutorialDone = true;

        DetermineWhatInfoToShow();

        if (!_canMove)
        {
            _currentTime += Time.deltaTime;
            if (_currentTime > _timer)
            {
                _currentTime = 0f;
                _showInfo[0].Activate();
            }
        }
        else if (!_canGrapple)
        {
            _currentTime += Time.deltaTime;
            if (_currentTime > _timer)
            {
                _currentTime = 0f;
                _showInfo[1].Activate();
            }
        }
        else if (!_canDetatch)
        {
            _currentTime += Time.deltaTime;
            if (_currentTime > _timer)
            {
                _currentTime = 0f;
                _showInfo[2].Activate();
            }
        }
    }

    private void DetermineWhatInfoToShow()
    {
        //Determine wheter the character discovered the keys to move
        if (!_canMove)
        {
            if (_movementBehavior.rb2D.velocity.x != 0f)
            {
                _canMove = true;
                _showInfo[0].Deactivate();
            }
        }

        //Discovered the key to grapple?
        if (!_canGrapple)
        {
            if (_movementBehavior.IsGrappling)
            {
                _canGrapple = true;
                _showInfo[1].Deactivate();
            }
        }

        //Discovered the key to detatch?
        if (_movementBehavior.CurrentState == MovementBehavior.MovementState.Ungrappled)
        {
            _canDetatch = true;
            _showInfo[2].Deactivate();
        }

    }
}

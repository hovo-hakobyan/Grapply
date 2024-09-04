using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEnd : MonoBehaviour
{
    private string PLAYER = "Player";
    private bool _isEnded = false;
    [SerializeField] GameOver _gameOver;

    public bool IsEnded
    {
        get { return _isEnded; }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == PLAYER)
        {
            _isEnded = true;
            _gameOver.ActivateBackground();
        }
    }
}

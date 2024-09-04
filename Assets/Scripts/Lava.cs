using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Lava : MonoBehaviour
{
    [SerializeField] private float _riseSpeed = 0f;
    private bool _shouldStartRising = false;
    const string PLAYER = "Player";

    [SerializeField] private Tutorial _tutorial;

    void Update()
    {
        if (!_shouldStartRising)
        {
            if (_tutorial != null)
            {
                if (_tutorial.IsTutorialDone)
                {
                    _shouldStartRising = true;
                }
            }
           
        }

        if (_shouldStartRising)
        {
            //Rises the lava
             transform.parent.localScale += Vector3.up * _riseSpeed * Time.deltaTime;
        }
       

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == PLAYER )
        {
            SceneManager.LoadScene(0);
        }
    }
}

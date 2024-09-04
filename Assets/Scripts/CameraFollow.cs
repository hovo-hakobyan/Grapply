using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private float _followSpeed = 2f;
    private float _yOffset = 1f;
    public Transform _target = null;
   

    void Update()
    {
        if (_target == null)
            return;

        //Camera follows the character
        Vector3 newPos = new Vector3(_target.position.x, _target.position.y + _yOffset, -10f);
        transform.position = Vector3.Slerp(transform.position, newPos, _followSpeed * Time.deltaTime);
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCamera : MonoBehaviour
{
    [SerializeField]
    private Vector2 cameraOffset = Vector3.zero;
    private Transform target;

    private void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void LateUpdate()
    {
        if (target == null)
        {
            return;
        }

        Vector3 postion = new Vector3(target.position.x + cameraOffset.x, target.position.y + cameraOffset.y, transform.position.z);

        transform.position = postion;
    }
}

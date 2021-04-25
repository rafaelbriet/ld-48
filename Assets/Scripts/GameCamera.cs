using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCamera : MonoBehaviour
{
    [SerializeField]
    private Vector2 cameraOffset = Vector3.zero;
    [SerializeField]
    private float fallingOrthographicSize = 8;
    [SerializeField]
    private float fallingYOffset = -2;

    private Transform target;
    private bool isFalling = false;

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

        Vector3 postion;

        if (isFalling)
        {
            postion = new Vector3(target.position.x + cameraOffset.x, target.position.y + cameraOffset.y + fallingYOffset, transform.position.z); 
        }
        else
        {
            postion = new Vector3(target.position.x + cameraOffset.x, target.position.y + cameraOffset.y, transform.position.z);
        }

        transform.position = postion;
    }

    public void ZoomOut()
    {
        Camera cam = Camera.main;

        cam.orthographicSize = fallingOrthographicSize;
        isFalling = true;
    }
}

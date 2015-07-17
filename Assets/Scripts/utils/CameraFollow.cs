using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraFollow : MonoBehaviour
{
    public enum FollowType { HORIZONTAL,VERTICAL}
    public FollowType type;
    public Transform target;
    public float smoothing = 5f;
    public Transform[] fixedList;
    private List<Vector3> _offsetList;
    public Transform[] fixedYList;
    private List<Vector3> _offsetYList;
    private Vector3 _targetOffset;

    void Awake()
    {

    }
    // Use this for initialization
    void Start()
    {
        _offsetList = new List<Vector3>();
        for (int i = 0; i < fixedList.Length; i++)
        {
            _offsetList.Add(fixedList[i].position - transform.position);
        }

        _offsetYList = new List<Vector3>();
        for (int i = 0; i < fixedYList.Length; i++)
        {
            _offsetYList.Add(fixedYList[i].position - transform.position);
        }
        _targetOffset = transform.position - target.transform.position;
    }

    public void Reset()
    {
        Camera.main.transform.position = new Vector3(0, 0, -10);
        for (int i = 0; i < fixedList.Length; i++)
        {
            fixedList[i].position = transform.position + _offsetList[i];
        }
        for (int i = 0; i < fixedYList.Length; i++)
        {
            fixedYList[i].position = new Vector3(fixedYList[i].position.x, transform.position.y + _offsetYList[i].y, fixedYList[i].position.z);
        }
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (target != null)
        {
            if(type == FollowType.HORIZONTAL)
                transform.position = new Vector3(target.transform.position.x + _targetOffset.x, transform.position.y, transform.position.z);
            else
                transform.position = new Vector3(transform.position.x, target.position.y + _targetOffset.y, transform.position.z);

            for (int i = 0; i < fixedList.Length; i++)
            {
                fixedList[i].position = transform.position + _offsetList[i];
            }
            for (int i = 0; i < fixedYList.Length; i++)
            {
                fixedYList[i].position = new Vector3(fixedYList[i].position.x, transform.position.y + _offsetYList[i].y, fixedYList[i].position.z);
            }
        }
    }

    public void FollowOnce()
    {
        LateUpdate();
        enabled = false;
    }
}

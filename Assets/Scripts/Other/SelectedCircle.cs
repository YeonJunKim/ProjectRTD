using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedCircle : MonoBehaviour
{
    const float rotateSpeed = 200f;

    Transform target;
    bool followTarget;

    private void Awake()
    {
        target = null;
        followTarget = false;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0, rotateSpeed * Time.deltaTime, 0);

        if (followTarget && target.gameObject.activeInHierarchy)
        {
            Vector3 pos = target.position;
            pos.y += 0.2f;  // to put it on top of the block
            transform.position = pos;
        }
    }

    public void SetTarget(Transform _target, bool _followTarget)
    {
        target = _target;
        followTarget = _followTarget;

        Vector3 pos = target.position;
        pos.y += 0.2f;  // to put it on top of the block
        transform.position = pos;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedCircle : MonoBehaviour
{
    const float rotateSpeed = 200f;

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0, rotateSpeed * Time.deltaTime, 0);
    }
}

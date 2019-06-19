using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlUnit : MonoBehaviour
{
    Animator animator;

    Vector3 destination;
    List<Node> pathToGO;


    void Awake()
    {
        pathToGO = new List<Node>();
        animator = GetComponentInChildren<Animator>();
    }

    public void SetPath(List<Node> path)
    {
        pathToGO = path;
        animator.SetBool("Run Forward", true);
        StartCoroutine("StartMove");
    }

    IEnumerator StartMove()
    {
        for (int i = 0; i < pathToGO.Count; i++)
        {
            Vector3 targetPos = pathToGO[i].transform.position;
            targetPos.y += 1f;

            while (true)
            {
                transform.LookAt(targetPos);
                transform.Translate(Vector3.forward * Time.deltaTime * 10);

                float sqrMagnitude = (transform.position - targetPos).sqrMagnitude;
                if (sqrMagnitude < 0.5f)
                {
                    break;
                }
                yield return null;
            }
        }
        animator.SetBool("Run Forward", false);
    }

    public bool IsArrivedAtDestination()
    {
        if (animator.GetBool("Run Forward") == true)
            return false;
        else
            return true;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Node : MonoBehaviour, IComparable<Node>
{
    public List<Node> neighborNodes;
    public List<GameObject> touchingObstacles;

    public Renderer nodeSphere;

    public bool isVisited;
    public bool isWalkable;
    public Node from;
    public float distanceTraveled;

    private void Awake()
    {
        neighborNodes = new List<Node>();
        touchingObstacles = new List<GameObject>();
        isVisited = false;
        isWalkable = true;
    }

    public void Init()
    {
        SetVisited(false);
        distanceTraveled = 0;
        CheckIsWalkable();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Node")
        {
            Node node = other.transform.GetComponent<Node>();
            neighborNodes.Add(node);
        }
        else if(other.tag == "Obstacle")
        {
            touchingObstacles.Add(other.gameObject);
            CheckIsWalkable();
        }
        else if(other.tag == "Ground")  // for game mode
        {
            if ((transform.position - other.transform.position).sqrMagnitude < 0.3f)
            {
                isWalkable = false;
                nodeSphere.material.color = Color.blue;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Node")
        {
            Node node = other.transform.GetComponent<Node>();
            touchingObstacles.Remove(other.gameObject);
        }
        else if (other.tag == "Obstacle")
        {
            touchingObstacles.Remove(other.gameObject);
            CheckIsWalkable();
        }
        else if (other.tag == "Ground")
        {

        }
    }


    public void SetVisited(bool visited)
    {
        if (visited)
        {
            isVisited = true;
            nodeSphere.material.color = Color.green;
        }
        else
        {
            isVisited = false;
            nodeSphere.material.color = Color.white;
        }
    }


    public void SetColorRed()
    {
        nodeSphere.material.color = Color.red;
    }

    void CheckIsWalkable()
    {
        if(touchingObstacles.Count == 0)
        {
            isWalkable = true;
            nodeSphere.material.color = Color.white;
        }
        else
        {
            isWalkable = false;
            nodeSphere.material.color = Color.blue;
        }
    }

    public int CompareTo(Node node)
    {
        if (distanceTraveled < node.distanceTraveled)
            return -1;
        else if (distanceTraveled == node.distanceTraveled)
            return 0;
        else
            return 1;
    }
}

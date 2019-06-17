using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Node : MonoBehaviour
{
    public List<Node> neighbor_none_diagonal;
    public List<Node> neighbor_diagonal;
    public List<GameObject> touching_obstacles;

    public Renderer nodeSphere;

    public bool isVisited;
    public bool isWalkable;
    public Node from;

    private void Awake()
    {
        neighbor_none_diagonal = new List<Node>();
        neighbor_diagonal = new List<Node>();
        touching_obstacles = new List<GameObject>();
        isVisited = false;
        isWalkable = true;
    }

    public void Init()
    {
        SetVisited(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Node")
        {
            Node node = other.transform.GetComponent<Node>();
            float dist = Vector3.Distance(transform.position, other.transform.position);
            if(Mathf.Abs(transform.localScale.x - dist) < transform.localScale.x * 0.1f)    // little awkward..
            {
                neighbor_none_diagonal.Add(node);
            }
            else
            {
                neighbor_diagonal.Add(node);
            }
        }
        else if(other.tag == "Obstacle")
        {
            touching_obstacles.Add(other.gameObject);
            CheckIsWalkable();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Node")
        {
            Node node = other.transform.GetComponent<Node>();
            float dist = Vector3.Distance(transform.position, other.transform.position);
            if (Mathf.Abs(transform.localScale.x - dist) < transform.localScale.x * 0.1f)    // little awkward..
            {
                neighbor_none_diagonal.Remove(node);
            }
            else
            {
                neighbor_diagonal.Remove(node);
            }
        }
        else if (other.tag == "Obstacle")
        {
            touching_obstacles.Remove(other.gameObject);
            CheckIsWalkable();
        }
    }


    public void SetVisited(bool visited, Node _from = null)
    {
        from = _from;

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
        if(touching_obstacles.Count == 0)
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
}

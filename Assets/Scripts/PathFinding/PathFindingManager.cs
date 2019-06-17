using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFindingManager : MonoBehaviour
{
    public GameObject startPosCube;
    public GameObject targetPosCube;

    Node startNode;
    Node targetNode;

    bool isStartPosSelect;

    Stack<Node> shortestPath;

    List<Node> visitedNodes;

    List<Node> nextStepCheckNodes;
    List<Node> tempNextStepCheckNodes;
    const float stepSpeed = 0.2f;

    private void Awake()
    {
        shortestPath = new Stack<Node>();
        visitedNodes = new List<Node>();
        nextStepCheckNodes = new List<Node>();
        tempNextStepCheckNodes = new List<Node>();
        startNode = null;
        targetNode = null;
        isStartPosSelect = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        startPosCube.SetActive(false);
        targetPosCube.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100))    // only hit Towers and Enemys
            {
                if (hit.transform.tag == "Node")
                {
                    Node node = hit.transform.GetComponent<Node>();
                    if (isStartPosSelect)
                    {
                        StopAllCoroutines();

                        startNode = node;

                        startPosCube.SetActive(true);
                        targetPosCube.SetActive(false);

                        Vector3 pos = node.transform.position;
                        pos.y += 2;
                        startPosCube.transform.position = pos;
                        isStartPosSelect = false;
                        InitVisitedNodes();
                    }
                    else
                    {
                        targetNode = node;

                        targetPosCube.SetActive(true);

                        Vector3 pos = node.transform.position;
                        pos.y += 2;
                        targetPosCube.transform.position = pos;

                        isStartPosSelect = true;

                        StartCoroutine("FindPath_Astar");
                    }
                }
            }
        }
    }

    IEnumerator FindPath_Dijkstra()
    {
        shortestPath.Clear();
        nextStepCheckNodes.Clear();
        tempNextStepCheckNodes.Clear();
        visitedNodes.Clear();

        nextStepCheckNodes.Add(startNode);
        visitedNodes.Add(startNode);

        bool targetFound = false;
        while(true)
        {
            targetFound = CheckOneStep_Dijkstra();
            if(targetFound)
            {
                break;
            }
            Debug.Log("Finding Path.. Dijkstra");
            yield return new WaitForSeconds(stepSpeed);
        }

        if (targetFound)
        {
            BacktrackRecursive(targetNode);
        }
        else
        {
            Debug.LogError("Path was not Found");
        }
    }

    bool CheckOneStep_Dijkstra()
    {
        tempNextStepCheckNodes.Clear();
        foreach (var node in nextStepCheckNodes)
        {
           bool targetFound = CheckNeighborNodes_Dijkstra(node);
           if(targetFound)
            {
                return true;
            }
        }

        nextStepCheckNodes.Clear();
        foreach (var node in tempNextStepCheckNodes)
        {
            nextStepCheckNodes.Add(node);
        }

        return false;
    }

    bool CheckNeighborNodes_Dijkstra(Node node)
    {
        foreach (var neighborNode in node.neighbor_none_diagonal)
        {
            if(neighborNode.isWalkable && !neighborNode.isVisited)
            {
                neighborNode.SetVisited(true, node);
                visitedNodes.Add(neighborNode);

                if (neighborNode == targetNode)
                {
                    return true;
                }
                else
                {
                    tempNextStepCheckNodes.Add(neighborNode);
                }
            }
        }

        return false;
    }


    IEnumerator FindPath_Astar()
    {
        shortestPath.Clear();
        nextStepCheckNodes.Clear();
        tempNextStepCheckNodes.Clear();
        visitedNodes.Clear();

        nextStepCheckNodes.Add(startNode);
        visitedNodes.Add(startNode);

        bool targetFound = false;
        while (true)
        {
            targetFound = CheckOneStep_Astar();
            if (targetFound)
            {
                break;
            }
            Debug.Log("Finding Path.. Astar");
            yield return new WaitForSeconds(stepSpeed);
        }

        if (targetFound)
        {
            BacktrackRecursive(targetNode);
        }
        else
        {
            Debug.LogError("Path was not Found");
        }
    }

    bool CheckOneStep_Astar()
    {
        tempNextStepCheckNodes.Clear();
        foreach (var node in nextStepCheckNodes)
        {
            bool targetFound = CheckNeighborNodes_Astar(node);
            if (targetFound)
            {
                return true;
            }
        }

        nextStepCheckNodes.Clear();
        foreach (var node in tempNextStepCheckNodes)
        {
            nextStepCheckNodes.Add(node);
        }

        return false;
    }

    bool CheckNeighborNodes_Astar(Node node)
    {
        float closestDist = float.MaxValue;
        Node closestNodeToTarget = null;

        foreach (var neighborNode in node.neighbor_none_diagonal)
        {
            if (neighborNode.isWalkable && !neighborNode.isVisited)
            {
                neighborNode.SetVisited(true, node);
                visitedNodes.Add(neighborNode);

                if (neighborNode == targetNode)
                {
                    return true;
                }
                else
                {
                    float x = Mathf.Abs(neighborNode.transform.position.x - targetNode.transform.position.x);
                    float z = Mathf.Abs(neighborNode.transform.position.z - targetNode.transform.position.z);
                    float dist = x + z;
                    if(dist < closestDist)
                    {
                        closestDist = dist;
                        closestNodeToTarget = neighborNode;
                    }
                }
            }
        }

        tempNextStepCheckNodes.Add(closestNodeToTarget);

        return false;
    }


    void BacktrackRecursive(Node node)
    {
        shortestPath.Push(node);
        if (node == startNode)
        {
            return;
        }
        else
        {
            node.SetColorRed();
            BacktrackRecursive(node.from);
        }
    }

    void InitVisitedNodes()
    {
        foreach(var node in visitedNodes)
        {
            node.Init();
        }
        visitedNodes.Clear();
    }
}

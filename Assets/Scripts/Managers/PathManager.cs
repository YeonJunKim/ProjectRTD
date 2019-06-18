using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PathManager : MonoBehaviour
{
    public static PathManager S;

    public GameObject ground;

    public Node startNode;
    public Node targetNode;

    Node[] nodeArray;   // all of the nodes currently in map
    List<Node> visitedNodes;
    PriorityQueue<Node> priorityQueue;
    Stack<Node> shortestPath;
    List<Node> constrainPath;
    Node previousNode;

    private void Awake()
    {
        S = this;

        shortestPath = new Stack<Node>();
        visitedNodes = new List<Node>();
        priorityQueue = new PriorityQueue<Node>();
        constrainPath = new List<Node>();
    }

    // Start is called before the first frame update
    void Start()
    {
        nodeArray = ground.GetComponentsInChildren<Node>();
        Invoke("FindPath", 1);
    }


    IEnumerator FindPath_Dijkstra()
    {
        shortestPath.Clear();
        priorityQueue.Clear();
        visitedNodes.Clear();

        startNode.distanceTraveled = 0;
        priorityQueue.Enqueue(startNode);

        bool targetFound = false;
        while (true)
        {
            targetFound = SearchOneStep_Dijkstra();
            if (targetFound)
            {
                break;
            }
            yield return null;
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

    bool SearchOneStep_Dijkstra()
    {
        bool targetFound = false;

        for (int i = 0; i < priorityQueue.Count(); i++)
        {
            Node nodeToSearch = priorityQueue.Dequeue();
            targetFound = SearchNeighborNodes_Dijkstra(nodeToSearch);
            if (targetFound)
            {
                return true;
            }
        }
        return false;
    }

    bool SearchNeighborNodes_Dijkstra(Node node)
    {
        foreach (var neighborNode in node.neighborNodes)
        {
            if (neighborNode.isWalkable && !neighborNode.isVisited)
            {
                neighborNode.distanceTraveled = node.distanceTraveled + GetDistance(node, neighborNode);
                neighborNode.from = node;
                priorityQueue.Enqueue(neighborNode);
                visitedNodes.Add(neighborNode);
                neighborNode.SetVisited(true);

                if (neighborNode == targetNode)
                {
                    return true;
                }
            }
        }
        return false;
    }


    IEnumerator FindPath_Astar()
    {
        Debug.Log("Start Astar");

        shortestPath.Clear();
        priorityQueue.Clear();
        visitedNodes.Clear();

        startNode.distanceTraveled = 0;
        priorityQueue.Enqueue(startNode);

        bool targetFound = false;
        while (true)
        {
            targetFound = SearchOneStep_Astar();
            if (targetFound)
            {
                break;
            }
            yield return null;
        }

        if (targetFound)
        {
            BacktrackRecursive(targetNode);
        }
        else
        {
            Debug.LogError("Path was not Found");
        }

        while(shortestPath.Count != 0)
        {
            constrainPath.Add(shortestPath.Pop());
        }
    }

    bool SearchOneStep_Astar()
    {
        bool targetFound = false;

        for (int i = 0; i < priorityQueue.Count(); i++)
        {
            Node nodeToSearch = priorityQueue.Dequeue();

            visitedNodes.Add(nodeToSearch);
            nodeToSearch.SetVisited(true);
            targetFound = SearchNeighborNodes_Astar(nodeToSearch);
            if (targetFound)
            {
                return true;
            }
        }
        return false;
    }

    bool SearchNeighborNodes_Astar(Node node)
    {
        float closestDistSoFar = float.MaxValue;
        Node closestNodeSoFar = null;

        foreach (var neighborNode in node.neighborNodes)
        {
            if (neighborNode.isWalkable && !neighborNode.isVisited)
            {
                neighborNode.distanceTraveled = node.distanceTraveled + GetDistance(node, neighborNode) + GetDistanceFromTarget(neighborNode);

                if (neighborNode.distanceTraveled < closestDistSoFar)
                {
                    closestDistSoFar = neighborNode.distanceTraveled;
                    closestNodeSoFar = neighborNode;
                    neighborNode.from = node;
                }

                if (neighborNode == targetNode)
                {
                    return true;
                }
            }
        }

        if (closestNodeSoFar != null)
        {
            priorityQueue.Enqueue(closestNodeSoFar);
        }

        Debug.Log("fail");

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
        foreach (var node in visitedNodes)
        {
            node.Init();
        }
        visitedNodes.Clear();
    }

    float GetDistance(Node node1, Node node2)
    {
        if (node1.transform.position.x == node2.transform.position.x
            || node1.transform.position.z == node2.transform.position.z)
        {
            return 1;
        }
        else
        {
            return 1.4f;
        }
    }

    float GetDistanceFromTarget(Node node)
    {
        float dist;
        float x = Mathf.Abs(targetNode.transform.position.x - node.transform.position.x) / 2;
        float z = Mathf.Abs(targetNode.transform.position.z - node.transform.position.z) / 2;
        dist = x + z;

        return dist;
    }
    

    public void FindPath()
    {
        InitVisitedNodes();
        StartCoroutine("FindPath_Astar");
    }

    public List<Node> GetPath()
    {
        List<Node> list = new List<Node>();
        list = constrainPath;
        return list;
    }
}

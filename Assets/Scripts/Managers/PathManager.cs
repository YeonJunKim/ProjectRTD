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
    PriorityQueue<Node> priorityQueue;
    List<Node> shortestPath;

    private void Awake()
    {
        S = this;

        shortestPath = new List<Node>();
        priorityQueue = new PriorityQueue<Node>();
    }

    // Start is called before the first frame update
    void Start()
    {
        nodeArray = ground.GetComponentsInChildren<Node>();
        Invoke("FindPath", 0.5f);
    }


    IEnumerator FindPath_Dijkstra()
    {
        shortestPath.Clear();
        priorityQueue.Clear();

        startNode.distanceTraveled = 0;
        startNode.SetVisited(true);
        priorityQueue.Enqueue(startNode);

        bool targetFound = false;
        while (true)
        {
            targetFound = SearchOneStep_Dijkstra();
            if (targetFound || priorityQueue.Count() == 0)
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

        int count = priorityQueue.Count();
        for (int i = 0; i < count; i++)
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
                neighborNode.SetVisited(true);
                neighborNode.from = node;

                // Core of Dijkstra Algorithm
                neighborNode.distanceTraveled = node.distanceTraveled + GetDistance(node, neighborNode);

                priorityQueue.Enqueue(neighborNode);

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
        shortestPath.Clear();
        priorityQueue.Clear();

        startNode.distanceTraveled = 0;
        startNode.SetVisited(true);
        priorityQueue.Enqueue(startNode);

        bool targetFound = false;
        while (true)
        {
            targetFound = SearchOneStep_Astar();
            if (targetFound || priorityQueue.Count() == 0)
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

    bool SearchOneStep_Astar()
    {
        bool targetFound = false;

        int count = priorityQueue.Count();
        for (int i = 0; i < count; i++)
        {
            Node nodeToSearch = priorityQueue.Dequeue();
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
            if (neighborNode.isWalkable && !neighborNode.isVisited && !neighborNode.isDeadEnd)
            {
                neighborNode.SetVisited(true);
                neighborNode.from = node;

                // Core of A* Algorithm
                neighborNode.distanceTraveled = node.distanceTraveled + GetDistance(node, neighborNode) + GetDistanceFromTarget(neighborNode);

                if (neighborNode.distanceTraveled < closestDistSoFar)
                {
                    closestDistSoFar = neighborNode.distanceTraveled;
                    closestNodeSoFar = neighborNode;
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
        else
        {
            node.isDeadEnd = true;
            if (node.from != null)
            {
                closestDistSoFar = float.MaxValue;
                foreach (var neighborNode in node.from.neighborNodes)
                {
                    if (neighborNode.isWalkable && !neighborNode.isDeadEnd)
                    {
                        if (neighborNode.distanceTraveled < closestDistSoFar)
                        {
                            closestDistSoFar = neighborNode.distanceTraveled;
                            closestNodeSoFar = neighborNode;
                        }

                        if (closestNodeSoFar != null)
                        {
                            priorityQueue.Enqueue(closestNodeSoFar);
                        }
                    }
                }
            }
        }
        return false;
    }


    void BacktrackRecursive(Node node)
    {
        shortestPath.Insert(0, node);
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

    void InitNodes()
    {
        foreach (var node in nodeArray)
        {
            node.Init();
        }
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
        float x = Mathf.Abs(targetNode.transform.position.x - node.transform.position.x) / 2;   // divided by 2, because the cube size is 2
        float z = Mathf.Abs(targetNode.transform.position.z - node.transform.position.z) / 2;

        // little trick (diagonal is faster than 2 straights)
        float diagonal = Mathf.Min(x, z);
        float straight = Mathf.Abs(x - z);

        diagonal *= 1.4f;

        dist = straight + diagonal;

        return dist;
    }


    public void FindPath()
    {
        //InitNodes();  // do not init nodes at game mode
        StartCoroutine("FindPath_Astar");
    }

    public List<Node> GetPath()
    {
        return shortestPath;
    }
}

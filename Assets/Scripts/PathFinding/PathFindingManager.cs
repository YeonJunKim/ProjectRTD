using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// this is for PathFindingTest Scene
public class PathFindingManager : MonoBehaviour
{
    public static PathFindingManager S;

    public GameObject ground;
    public GameObject startPosCube;
    public GameObject targetPosCube;
    public GameObject obstacle;
    public Slider slider;
    public ControlUnit controlUnit;

    Node startNode;
    Node targetNode;

    bool isStartPosSelect;

    Node[] nodeArray;   // all of the nodes currently in map (only for initialization)
    PriorityQueue<Node> priorityQueue;
    List<Node> shortestPath;

    float searchSpeed;
    bool searchWithAstar;

    ControlUnit requestFrom;

    private void Awake()
    {
        S = this;

        shortestPath = new List<Node>();
        priorityQueue = new PriorityQueue<Node>();

        startNode = null;
        targetNode = null;
        isStartPosSelect = true;

        searchWithAstar = false;
        searchSpeed = 0;
        requestFrom = null;
    }

    // Start is called before the first frame update
    void Start()
    {
        startPosCube.SetActive(false);
        targetPosCube.SetActive(false);
        nodeArray = ground.GetComponentsInChildren<Node>();
    }

    // Update is called once per frame
    void Update()
    {
        // move the obstacle
        if(Input.GetMouseButton(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100))
            {
                if (hit.transform.tag == "Node")
                {
                    Vector3 pos = hit.transform.position;
                    pos.y += 1;
                    obstacle.transform.position = pos;
                }
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            // start pathfinding only if the control unit is arrived at the destination
            if (!controlUnit.IsArrivedAtDestination())
                return;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100))
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
                        pos.y += 1;
                        startPosCube.transform.position = pos;
                        isStartPosSelect = false;
                        InitNodes();

                        controlUnit.transform.position = pos;
                    }
                    else
                    {
                        targetNode = node;

                        targetPosCube.SetActive(true);

                        Vector3 pos = node.transform.position;
                        pos.y += 1;
                        targetPosCube.transform.position = pos;

                        isStartPosSelect = true;

                        FindPath(controlUnit, targetNode);
                    }
                }
            }
        }
    }

    IEnumerator FindPath_Dijkstra()
    {
        shortestPath.Clear();
        priorityQueue.Clear();

        startNode.distanceTraveled = 0;
        startNode.SetVisited(true);
        priorityQueue.Enqueue(startNode);

        bool targetFound = false;
        while(true)
        {
            targetFound = SearchOneStep_Dijkstra();
            if(targetFound)
            {
                break;
            }
            yield return new WaitForSeconds(searchSpeed);
        }

        if (targetFound)
        {
            BacktrackRecursive(targetNode);
            requestFrom.SetPath(shortestPath);
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
                // Core of Dijkstra Algorithm
                neighborNode.distanceTraveled = node.distanceTraveled + GetDistance(node, neighborNode);

                neighborNode.from = node;
                priorityQueue.Enqueue(neighborNode);
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
        shortestPath.Clear();
        priorityQueue.Clear();

        startNode.distanceTraveled = 0;
        startNode.SetVisited(true);
        priorityQueue.Enqueue(startNode);

        bool targetFound = false;
        while (true)
        {
            targetFound = SearchOneStep_Astar();
            if (targetFound)
            {
                break;
            }
            yield return new WaitForSeconds(searchSpeed);
        }

        if (targetFound)
        {
            BacktrackRecursive(targetNode);
            requestFrom.SetPath(shortestPath);
        }
        else
        {
            Debug.LogError("Path was not Found");
        }
    }

    bool SearchOneStep_Astar()
    {
        bool targetFound = false;

        for (int i = 0; i < priorityQueue.Count(); i++)
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
            if (neighborNode.isWalkable && !neighborNode.isVisited)
            {
                neighborNode.from = node;
                neighborNode.SetVisited(true);

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

        if(closestNodeSoFar != null)
        {
            priorityQueue.Enqueue(closestNodeSoFar);
        }
        else
        {
            foreach (var neighborNode in node.neighborNodes)
            {
                if (neighborNode.isWalkable)
                {
                    // Core of A* Algorithm
                    neighborNode.distanceTraveled = node.distanceTraveled + GetDistance(node, neighborNode) + GetDistanceFromTarget(neighborNode);
                    if (neighborNode.distanceTraveled < closestDistSoFar)
                    {
                        closestDistSoFar = neighborNode.distanceTraveled;
                        closestNodeSoFar = neighborNode;
                    }
                }
            }

            if (closestNodeSoFar != null)
            {
                closestNodeSoFar.from = node;
                priorityQueue.Enqueue(closestNodeSoFar);
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
        if(node1.transform.position.x == node2.transform.position.x 
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

    public void SearchWithAstar(bool toggle)
    {
        searchWithAstar = toggle;
    }

    public void OnValueChanged()
    {
        searchSpeed = slider.value;
    }

    public void FindPath(ControlUnit _requestFrom, Node _targetNode)
    {
        requestFrom = _requestFrom;

        Vector3 requestPos = requestFrom.transform.position;
        startNode = FindClosestNodeFromPoint(requestPos);
        targetNode = _targetNode;

        if (searchWithAstar)
            StartCoroutine("FindPath_Astar");
        else
            StartCoroutine("FindPath_Dijkstra");
    }

    Node FindClosestNodeFromPoint(Vector3 point)
    {
        float closestDistSoFar = float.MaxValue;
        Node closestNodeSoFar = null;

        Vector3 requestPos = requestFrom.transform.position;
        foreach (var node in nodeArray)
        {
            float dist = (requestPos - node.transform.position).sqrMagnitude;
            if (dist < closestDistSoFar)
            {
                closestDistSoFar = dist;
                closestNodeSoFar = node;
            }
        }
        return closestNodeSoFar;
    }

}

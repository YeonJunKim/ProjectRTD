using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum PlayerControlState
{
    Idle,
    PlacingTower,
}


public class PlayerControlManager : MonoBehaviour
{
    public static PlayerControlManager S;

    public PlayerControlState state;

    public GameObject prePlacer;

    public GameObject screenMove;
    const float screenMoveSpeed = 20;

    TowerType selectedTower;

    Vector3 screenMoveDelta;

    const float UNIT_SIZE = 2f;

    private void Awake()
    {
        if (S == null)
            S = this;

        screenMoveDelta = Vector3.zero;
    }

    private void Start()
    {
        prePlacer.SetActive(false);
    }

    private void Update()
    {
        if(state == PlayerControlState.PlacingTower)
        {
            if (Input.GetMouseButtonDown(0))
            {
                prePlacer.SetActive(true);
            }
            else if (Input.GetMouseButton(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 100, 1 << 9))
                {
                    Vector3 pos = hit.point;
                    Vector3 prePos = LimitPosByUnit(pos);
                    prePos.y += 0.5f;   // little twick becuase of miss match
                    prePlacer.transform.position = prePos;
                }
            }
            else if (Input.GetMouseButtonUp(0))
            {
                if (prePlacer.GetComponent<PrePlacer>().IsSuitable())
                {
                    GameObject tower = EntityManager.S.GetEntity(selectedTower).gameObject;
                    Vector3 pos = prePlacer.transform.position;
                    pos.y -= 0.5f;     // little twick back becuase of miss match
                    tower.transform.position = pos;
                    tower.transform.Rotate(0, 180, 0);

                    prePlacer.SetActive(false);
                    prePlacer.GetComponent<PrePlacer>().Init();
                    ChangeState(PlayerControlState.Idle);
                }
            }
        }

        Camera.main.transform.Translate(screenMoveDelta * Time.deltaTime * screenMoveSpeed);
    }

    void ChangeState(PlayerControlState _state)
    {
        state = _state;
        switch (state)
        {
            case PlayerControlState.Idle:
                break;
            case PlayerControlState.PlacingTower:
                break;
        }
        UIManager.S.ChangeState(state);
    }

    public void OnRandomTowerButton()
    {
        if (state != PlayerControlState.Idle)
            return;

        selectedTower = RandomTowerDraw();
        ChangeState(PlayerControlState.PlacingTower);
    }

    public TowerType GetCurrentlySelectedTower()
    {
        return selectedTower;
    }

    Vector3 LimitPosByUnit(Vector3 pos)
    {
        float x = Mathf.Round((pos.x / UNIT_SIZE));
        float z = Mathf.Round((pos.z / UNIT_SIZE));

        pos.x = x * UNIT_SIZE;
        pos.z = z * UNIT_SIZE;

        return pos;
    }

    // for screen move
    public void OnLeftDown()
    {
        screenMoveDelta.x -= 1;
    }

    public void OnLeftUp()
    {
        screenMoveDelta.x += 1;
    }

    public void OnRightDown()
    {
        screenMoveDelta.x += 1;
    }

    public void OnRightUp()
    {
        screenMoveDelta.x -= 1;
    }

    public void OnUpDown()
    {
        screenMoveDelta.y += 1;
    }

    public void OnUpUp()
    {
        screenMoveDelta.y -= 1;
    }

    public void OnDownDown()
    {
        screenMoveDelta.y -= 1;
    }

    public void OnDownUp()
    {
        screenMoveDelta.y += 1;
    }


    TowerType RandomTowerDraw()
    {
        Dictionary<TowerType, int> chances = new Dictionary<TowerType, int>();

        chances.Add(TowerType.Chick, 100);
        chances.Add(TowerType.LittleBoar, 5000);
        chances.Add(TowerType.Dragon, 50);
        chances.Add(TowerType.Penguin, 50);
        chances.Add(TowerType.Mushroom, 50);
        chances.Add(TowerType.Momo, 5000);

        int sum = 0;
        foreach(var chance in chances)
        {
            sum += chance.Value;
        }

        int num = Random.Range(0, sum);

        int addUp = 0;
        TowerType tower = TowerType.Chick;
        foreach (var chance in chances)
        {
            addUp += chance.Value;
            if (addUp > num)
            {
                tower = chance.Key;
                break;
            }
        }
        return tower;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum PlayerControlState
{
    Idle,
    PlacingTower,
    TowerSelected,
}


public class PlayerControlManager : MonoBehaviour
{
    public static PlayerControlManager S;

    public PlayerControlState state;

    public GameObject prePlacer;

    public GameObject screenMove;
    const float screenMoveSpeed = 20;

    TowerType randomSelectedTower;

    Vector3 screenMoveDelta;

    public GameObject selectedCircle;
    Tower selectedTower;

    const int numOfLevel1Tower = 6;

    private void Awake()
    {
        if (S == null)
            S = this;

        screenMoveDelta = Vector3.zero;
        selectedTower = null;
    }

    private void Start()
    {
        prePlacer.SetActive(false);
    }

    private void Update()
    {
        // don't process player control if mouse is on UI
        if(!EventSystem.current.IsPointerOverGameObject())
            PlayerControl();

        // Screen Move by button
        Camera.main.transform.Translate(screenMoveDelta * Time.deltaTime * screenMoveSpeed);
    }

    void PlayerControl()
    {
        if (state == PlayerControlState.PlacingTower)
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
                    Vector3 pos = hit.transform.position;
                    pos.y += 1.75f;  // to put it on top of the block
                    prePlacer.transform.position = pos;
                }
            }
            else if (Input.GetMouseButtonUp(0))
            {
                if (prePlacer.GetComponent<PrePlacer>().IsSuitable())
                {
                    Vector3 pos = prePlacer.transform.position;
                    pos.y -= 0.75f;     // to put it down perfectly on the block
                    CreateTower(randomSelectedTower, pos);

                    prePlacer.SetActive(false);
                    prePlacer.GetComponent<PrePlacer>().Init();
                    ChangeState(PlayerControlState.Idle);
                }
            }
        }
        else if (state == PlayerControlState.Idle || state == PlayerControlState.TowerSelected)
        {
            // Select Tower
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 100, 1 << 10))    // only hit Towers
                {
                    Vector3 pos = hit.transform.position;
                    pos.y += 0.2f;  // to put it on top of the block
                    selectedCircle.transform.position = pos;
                    selectedCircle.SetActive(true);
                    selectedTower = hit.transform.GetComponent<Tower>();
                    ChangeState(PlayerControlState.TowerSelected);
                }
                else
                {
                    selectedCircle.SetActive(false);
                    selectedTower = null;
                    ChangeState(PlayerControlState.Idle);
                }
            }
        }
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
            case PlayerControlState.TowerSelected:
                break;
        }
        UIManager.S.ChangeState(state);
    }

    public void OnRandomTowerButton()
    {
        if (state != PlayerControlState.Idle)
            return;

        if (GameManager.S.GetMoney() < GameManager.MONEY_FOR_TOWER_DRAW)
            return;

        randomSelectedTower = RandomTowerDraw();
        GameManager.S.DecreaseMoney(GameManager.MONEY_FOR_TOWER_DRAW);
        ChangeState(PlayerControlState.PlacingTower);
    }

    public TowerType GetRandomSelectedTower()
    {
        return randomSelectedTower;
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


    public void OnUpgradeTowerButton()
    {
        if (state != PlayerControlState.TowerSelected || selectedTower == null)
            return;

        if (GameManager.S.GetMoney() < GameManager.MONEY_FOR_TOWER_UPGRADE)
            return;

        List<Tower> closestTowers = FindClosestSameTowersFromSelectedTower();
        // need two more towers to upgrade
        if (closestTowers.Count < 2)
            return;

        EntityManager.S.ReturnEntity(closestTowers[0]);
        EntityManager.S.ReturnEntity(closestTowers[1]);

        CreateTower(selectedTower.GetTowerType() + numOfLevel1Tower, selectedTower.transform.position);
        EntityManager.S.ReturnEntity(selectedTower);

        selectedCircle.SetActive(false);
        selectedTower = null;
        GameManager.S.DecreaseMoney(GameManager.MONEY_FOR_TOWER_UPGRADE);
        ChangeState(PlayerControlState.Idle);
    }

    List<Tower> FindClosestSameTowersFromSelectedTower()
    {
        List<BaseGameEntity> towerList = EntityManager.S.GetTowerList();
        List<Tower> sameTowerList = new List<Tower>();
        Vector3 selectedTowerPos = selectedTower.transform.position;

        foreach(var entity in towerList)
        {
            Tower t = entity as Tower;
            if(t != selectedTower && t.GetTowerType() == selectedTower.GetTowerType())
            {
                sameTowerList.Add(t);
            }
        }

        for (int i = 0; i < sameTowerList.Count; i++)
        {
            float dist1 = (sameTowerList[i].transform.position - selectedTowerPos).sqrMagnitude;
            for (int j = i + 1; j < sameTowerList.Count; j++)
            {
                float dist2 = (sameTowerList[j].transform.position - selectedTowerPos).sqrMagnitude;
                if (dist1 > dist2)
                {
                    Tower temp = sameTowerList[i];
                    sameTowerList[i] = sameTowerList[j];
                    sameTowerList[j] = temp;
                }
            }
        }
        return sameTowerList;
    }

    void CreateTower(TowerType towerType, Vector3 pos)
    {
        GameObject tower = EntityManager.S.GetEntity(towerType).gameObject;
        tower.transform.position = pos;
        tower.transform.eulerAngles = new Vector3(0, 180, 0);
    }


    TowerType RandomTowerDraw()
    {
        Dictionary<TowerType, int> chances = new Dictionary<TowerType, int>();

        chances.Add(TowerType.Chick_1, 5000);
        chances.Add(TowerType.LittleBoar_1, 50);
        chances.Add(TowerType.Dragon_1, 50);
        chances.Add(TowerType.Penguin_1, 50);
        chances.Add(TowerType.Mushroom_1, 50);
        chances.Add(TowerType.Momo_1, 50);

        int sum = 0;
        foreach(var chance in chances)
        {
            sum += chance.Value;
        }

        int num = Random.Range(0, sum);

        int addUp = 0;
        TowerType tower = 0;
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

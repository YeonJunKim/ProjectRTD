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

    public SelectedCircle selectedCircle;
    BaseGameEntity selectedEntity;

    const int numOfLevel1Tower = 6;

    private void Awake()
    {
        if (S == null)
            S = this;

        screenMoveDelta = Vector3.zero;
        selectedEntity = null;
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

        // Check Selected Entity Death
        if (selectedEntity != null)
        {
            if(selectedEntity.gameObject.activeInHierarchy == false)
            {
                selectedEntity = null;
                selectedCircle.gameObject.SetActive(false);
                UIManager.S.HideInfoView();
            }
        }
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
                    SoundManager.S.ClickSound();
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
                if (Physics.Raycast(ray, out hit, 100, (1 << 10) + (1 << 11)))    // only hit Towers and Enemys
                {
                    selectedEntity = hit.transform.GetComponent<BaseGameEntity>();
                    UIManager.S.ShowInfoView(selectedEntity);
                    selectedCircle.gameObject.SetActive(true);

                    if (selectedEntity.ENTITY_TYPE == EntityType.Enemy)
                    {
                        selectedCircle.SetTarget(hit.transform, true);
                        ChangeState(PlayerControlState.Idle);
                    }
                    else if(selectedEntity.ENTITY_TYPE == EntityType.Tower)
                    {
                        selectedCircle.SetTarget(hit.transform, false);
                        ChangeState(PlayerControlState.TowerSelected);
                    }
                }
                else
                {
                    UIManager.S.HideInfoView();
                    selectedCircle.gameObject.SetActive(false);
                    selectedEntity = null;
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
        SoundManager.S.ClickSound();
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
        if (state != PlayerControlState.TowerSelected || selectedEntity == null)
            return;

        int towerType = (int)((Tower)selectedEntity).towerType;
        // return if tower level is 3
        if (towerType >= 12)
            return;

        if (GameManager.S.GetMoney() < GameManager.MONEY_FOR_TOWER_UPGRADE)
            return;

        List<Tower> closestTowers = FindClosestSameTowersFromSelectedTower();
        // need two more towers to upgrade
        if (closestTowers.Count < 2)
            return;

        EntityManager.S.ReturnEntity(closestTowers[0]);
        EntityManager.S.ReturnEntity(closestTowers[1]);

        BaseGameEntity temp = CreateTower(((Tower)selectedEntity).GetTowerType() + numOfLevel1Tower, selectedEntity.transform.position);

        EntityManager.S.ReturnEntity(selectedEntity);
        selectedEntity = temp;

        UIManager.S.ShowInfoView(selectedEntity);

        GameManager.S.DecreaseMoney(GameManager.MONEY_FOR_TOWER_UPGRADE);
        SoundManager.S.ClickSound();
    }

    List<Tower> FindClosestSameTowersFromSelectedTower()
    {
        List<BaseGameEntity> towerList = EntityManager.S.GetTowerList();
        List<Tower> sameTowerList = new List<Tower>();
        Vector3 selectedTowerPos = selectedEntity.transform.position;
        Tower selectedTower = selectedEntity as Tower;

        foreach (var entity in towerList)
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

    Tower CreateTower(TowerType towerType, Vector3 pos)
    {
        GameObject tower = EntityManager.S.GetEntity(towerType).gameObject;
        tower.transform.position = pos;
        tower.transform.eulerAngles = new Vector3(0, 180, 0);

        return tower.GetComponent<Tower>();
    }


    TowerType RandomTowerDraw()
    {
        Dictionary<TowerType, int> chances = new Dictionary<TowerType, int>();

        chances.Add(TowerType.Chick_1, 50);
        chances.Add(TowerType.LittleBoar_1, 30);
        chances.Add(TowerType.Dragon_1, 20);
        chances.Add(TowerType.Penguin_1, 10);
        chances.Add(TowerType.Mushroom_1, 1);
        chances.Add(TowerType.Momo_1, 10);

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

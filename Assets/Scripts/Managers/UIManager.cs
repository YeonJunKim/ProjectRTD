using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UIManager : MonoBehaviour
{
    public static UIManager S;

    public List<Sprite> towerIcons;

    public GameObject randomTowerButton;
    public Image randomTowerIcon;
    public GameObject randomTowerIconBG;

    public GameObject upgradeTowerButton;

    public Image[] playerLife;
    public Text playerMoney;
    public Text stageLevel;

    public SmartCamera info_camera;
    public GameObject info_view;
    public GameObject info_tower;
    public GameObject info_enemy;
    public Text info_name;
    public Text info_tower_damage;
    public Text info_tower_attkSpeed;
    public Text info_tower_attkRange;
    public Text info_enemy_hp;
    public Text info_enemy_armor;
    public Text info_enemy_speed;
    public Text info_ps;

    private void Awake()
    {
        S = this;
    }

    private void Start()
    {
        upgradeTowerButton.SetActive(false);
        HideInfoView();
    }

    public void ChangeState(PlayerControlState state)
    {
        upgradeTowerButton.SetActive(false);
        randomTowerIcon.gameObject.SetActive(false);
        randomTowerButton.SetActive(false);
        randomTowerIconBG.SetActive(false);

        switch (state)
        {
            case PlayerControlState.Idle:
                randomTowerButton.SetActive(true);
                break;
            case PlayerControlState.PlacingTower:
                randomTowerIcon.gameObject.SetActive(true);
                randomTowerIconBG.SetActive(true);
                int type = (int)PlayerControlManager.S.GetRandomSelectedTower();
                randomTowerIcon.sprite = towerIcons[type];
                break;
            case PlayerControlState.TowerSelected:
                upgradeTowerButton.SetActive(true);
                break;
        }
    }

    // money = 0~9999
    public void SetMoney(int money)
    {
        playerMoney.text = money.ToString();
    }

    // life = 0~1
    public void SetLife(float life)
    {
        if(Mathf.Approximately(life, 1))
        {
            for (int i = 0; i < 4; i++)
            {
                playerLife[i].fillAmount = 1;
            }
            return;
        }
        else if(Mathf.Approximately(life, 0))
        {
            for (int i = 0; i < 4; i++)
            {
                playerLife[i].fillAmount = 0;
            }
            return;
        }

        int index = (int)(life * 10 / 2.5f);
        float fill = (life % 0.25f) / 0.25f;

        for (int i = 0; i < 4; i++)
        {
            if(i < index)
            {
                playerLife[i].fillAmount = 1;
            }
            else if(i == index)
            {
                playerLife[i].fillAmount = fill;
            }
            else
            {
                playerLife[i].fillAmount = 0;
            }
        }
    }

    public void SetStageLevel(int level)
    {
        stageLevel.text = level.ToString();
    }

    public void ShowInfoView(BaseGameEntity entity)
    {
        info_view.gameObject.SetActive(true);
        info_camera.gameObject.SetActive(true);

        if (entity.ENTITY_TYPE == EntityType.Enemy)
        {
            info_tower.gameObject.SetActive(false);
            info_enemy.gameObject.SetActive(true);
            info_camera.SetLookAtTarget(entity.transform, true);
            SetInfoView((Enemy)entity);
        }
        else if(entity.ENTITY_TYPE == EntityType.Tower)
        {
            info_tower.gameObject.SetActive(true);
            info_enemy.gameObject.SetActive(false);
            info_camera.SetLookAtTarget(entity.transform, false);
            SetInfoView((Tower)entity);
        }
    }

    public void HideInfoView()
    {
        info_view.gameObject.SetActive(false);
        info_camera.gameObject.SetActive(false);
    }

    void SetInfoView(Enemy enemy)
    {
        switch (enemy.enemyType)
        {
            case EnemyType.Wolf_1:
                info_name.text = "Lv1 늑대";
                info_ps.text = "으르르..";
                break;
            case EnemyType.KingCobra_1:
                info_name.text = "Lv1 킹코브라";
                info_ps.text = "스스스..";
                break;
            case EnemyType.GiantBee_1:
                info_name.text = "Lv1 여왕벌";
                info_ps.text = "위이잉~";
                break;
            case EnemyType.Magma_1:
                info_name.text = "Lv1 마그마";
                info_ps.text = "보글보글..";
                break;
            case EnemyType.Golem_1:
                info_name.text = "Lv1 골렘";
                info_ps.text = "..";
                break;
            case EnemyType.Wolf_2:
                info_name.text = "Lv2 늑대";
                info_ps.text = "으르르..";
                break;
            case EnemyType.KingCobra_2:
                info_name.text = "Lv2 킹코브라";
                info_ps.text = "스스스..";
                break;
            case EnemyType.GiantBee_2:
                info_name.text = "Lv2 여왕벌";
                info_ps.text = "위이잉~";
                break;
            case EnemyType.Magma_2:
                info_name.text = "Lv2 마그마";
                info_ps.text = "보글보글..";
                break;
            case EnemyType.Golem_2:
                info_name.text = "Lv2 골렘";
                info_ps.text = "..";
                break;
            case EnemyType.Wolf_3:
                info_name.text = "Lv3 늑대";
                info_ps.text = "으르르..";
                break;
            case EnemyType.KingCobra_3:
                info_name.text = "Lv3 킹코브라";
                info_ps.text = "스스스..";
                break;
            case EnemyType.GiantBee_3:
                info_name.text = "Lv3 여왕벌";
                info_ps.text = "위이잉~";
                break;
            case EnemyType.Magma_3:
                info_name.text = "Lv3 마그마";
                info_ps.text = "보글보글..";
                break;
            case EnemyType.Golem_3:
                info_name.text = "Lv3 골렘";
                info_ps.text = "..";
                break;
            case EnemyType.Boss_1:
                info_name.text = "스텀프 (보스)";
                info_ps.text = "아이 엠 그루트..";
                break;
            case EnemyType.Boss_2:
                info_name.text = "스파이더왕 (보스)";
                info_ps.text = "물리면 스파이더맨 됨";
                break;
            case EnemyType.Boss_3:
                info_name.text = "드래곤 (보스)";
                info_ps.text = "크와아아아ㅏ!!";
                break;
        }
        info_enemy_hp.text = ((int)enemy._HP).ToString();
        info_enemy_armor.text = ((int)enemy._ARMOR).ToString();
        info_enemy_speed.text = ((int)enemy._MOVE_SPEED).ToString();
    }

    void SetInfoView(Tower tower)
    {
        switch (tower.towerType)
        {
            case TowerType.Chick_1:
                info_name.text = "Lv1 삐약이";
                info_ps.text = "삐약삐약!";
                break;
            case TowerType.LittleBoar_1:
                info_name.text = "Lv1 리틀호그";
                info_ps.text = "호잉호잉~";
                break;
            case TowerType.Dragon_1:
                info_name.text = "Lv1 용용이";
                info_ps.text = "슈아아아~";
                break;
            case TowerType.Penguin_1:
                info_name.text = "Lv1 펭귄이";
                info_ps.text = "프렐요드를 위하여";
                break;
            case TowerType.Mushroom_1:
                info_name.text = "Lv1 머슈룸";
                info_ps.text = "가스가스";
                break;
            case TowerType.Momo_1:
                info_name.text = "Lv1 모모";
                info_ps.text = "찌요옹~";
                break;
            case TowerType.Chick_2:
                info_name.text = "Lv2 삐약이";
                info_ps.text = "삐약삐약!";
                break;
            case TowerType.LittleBoar_2:
                info_name.text = "Lv2 리틀호그";
                info_ps.text = "호잉호잉~";
                break;
            case TowerType.Dragon_2:
                info_name.text = "Lv2 용용이";
                info_ps.text = "슈아아아~";
                break;
            case TowerType.Penguin_2:
                info_name.text = "Lv2 펭귄이";
                info_ps.text = "프렐요드를 위하여";
                break;
            case TowerType.Mushroom_2:
                info_name.text = "Lv2 머슈룸";
                info_ps.text = "가스가스";
                break;
            case TowerType.Momo_2:
                info_name.text = "Lv2 모모";
                info_ps.text = "찌요옹~";
                break;
            case TowerType.Chick_3:
                info_name.text = "Lv3 삐약이";
                info_ps.text = "삐약삐약!";
                break;
            case TowerType.LittleBoar_3:
                info_name.text = "Lv3 리틀호그";
                info_ps.text = "호잉호잉~";
                break;
            case TowerType.Dragon_3:
                info_name.text = "Lv3 용용이";
                info_ps.text = "슈아아아~";
                break;
            case TowerType.Penguin_3:
                info_name.text = "Lv3 펭귄이";
                info_ps.text = "프렐요드를 위하여";
                break;
            case TowerType.Mushroom_3:
                info_name.text = "Lv3 머슈룸";
                info_ps.text = "가스가스";
                break;
            case TowerType.Momo_3:
                info_name.text = "Lv3 모모";
                info_ps.text = "찌요옹~";
                break;
        }
        info_tower_damage.text = ((int)tower._DAMAGE).ToString();
        info_tower_attkSpeed.text = ((float)tower._ATTK_SPEED).ToString();
        info_tower_attkRange.text = ((int)tower._ATTK_RANGE).ToString();
    }
}

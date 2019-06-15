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

    private void Awake()
    {
        S = this;
    }

    private void Start()
    {
        upgradeTowerButton.SetActive(false);
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
}

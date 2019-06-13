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

    private void Awake()
    {
        S = this;
    }

    public void ChangeState(PlayerControlState state)
    {
        switch (state)
        {
            case PlayerControlState.Idle:
                randomTowerButton.SetActive(true);
                randomTowerIcon.gameObject.SetActive(false);
                break;
            case PlayerControlState.PlacingTower:
                randomTowerButton.SetActive(false);
                randomTowerIcon.gameObject.SetActive(true);
                int type = (int)PlayerControlManager.S.GetCurrentlySelectedTower();
                randomTowerIcon.sprite = towerIcons[type];
                break;
        }
    }
}

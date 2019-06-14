using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager S;

    public Transform spawnPoint_N;
    public Transform spawnPoint_S;
    public Text waveTimeText;

    bool spawnOn;

    const float TIME_BETWEEN_SPAWNS = 0.5f;
    float nextSpawnTime;

    const int SPAWN_AMOUNT_PER_WAVE = 5;
    int countSpawn;

    const float TIME_BETWEEN_WAVES = 5;
    float nextWaveTime;
    int nextEnemyType;

    float playerLife;
    int playerMoney;
    public static int MONEY_FOR_TOWER_DRAW = 50;
    public static int MONEY_FOR_TOWER_UPGRADE = 100;
    const int moneyByStageLevel = 5;
    const int startMoney = 9999;

    int stageLevel;
    bool increaseStageLevel;    // not to increase stage level at level 1

    private void Awake()
    {
        S = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        //nextSpawnTime = TIME_BETWEEN_WAVES;
        nextSpawnTime = 1f; // for testing
        countSpawn = 0;
        nextWaveTime = Time.time;
        spawnOn = false;
        nextEnemyType = 0;

        playerLife = 1;
        playerMoney = startMoney;
        UIManager.S.SetLife(playerLife);
        UIManager.S.SetMoney(playerMoney);

        stageLevel = 1;
        UIManager.S.SetStageLevel(stageLevel);
        increaseStageLevel = false;
    }

    // Update is called once per frame
    void Update()
    {
        waveTimeText.text = ((int)(nextWaveTime - Time.time)).ToString();

        if (nextWaveTime < Time.time)
        {
            spawnOn = true;
            nextWaveTime = Time.time + TIME_BETWEEN_WAVES;
            IncreaseStageLevel();
            IncreaseLife(0.25f);
        }

        if(spawnOn && nextSpawnTime < Time.time)
        {
            Enemy enemyN = EntityManager.S.GetEntity((EnemyType)nextEnemyType) as Enemy;
            enemyN.transform.position = spawnPoint_N.position;
            enemyN.transform.Translate(0, 0.5f, 0); // for enemy to be on NavMesh
            enemyN.SetDestination(spawnPoint_S.position);


            // thinking of having monsters just spawn in one side
            //Enemy enemyS = EntityManager.S.GetEntity((EnemyType)nextEnemyType) as Enemy;
            //enemyS.transform.position = spawnPoint_S.position;
            //enemyS.transform.Translate(0, 0.5f, 0); // for enemy to be on NavMesh
            //enemyS.SetDestination(spawnPoint_N.position);

            countSpawn++;
            nextSpawnTime = Time.time + TIME_BETWEEN_SPAWNS;

            if(countSpawn >= SPAWN_AMOUNT_PER_WAVE)
            {
                spawnOn = false;
                nextEnemyType++;
                nextEnemyType = (int)Mathf.Repeat(nextEnemyType, System.Enum.GetNames(typeof(EnemyType)).Length);
                countSpawn = 0;
            }
        }
    }

    void IncreaseStageLevel()
    {
        // for not to increase level at start
        if (increaseStageLevel == false)
            increaseStageLevel = true;
        else
        {
            stageLevel++;
            UIManager.S.SetStageLevel(stageLevel);
        }
    }

    public void IncreaseMoneyByStageLevel()
    {
        IncreaseMoney(stageLevel * moneyByStageLevel);
    }

    public void IncreaseMoney(int amount)
    {
        playerMoney += amount;
        playerMoney = Mathf.Min(playerMoney, 9999); // the UI looks awful if it goes over 9999
        UIManager.S.SetMoney(playerMoney);
    }

    public void DecreaseMoney(int amount)
    {
        playerMoney -= amount;
        playerMoney = Mathf.Max(playerMoney, 0);
        UIManager.S.SetMoney(playerMoney);
    }

    public void IncreaseLife(float amount)
    {
        playerLife += amount;
        playerLife = Mathf.Min(playerLife, 1); // the UI looks awful if it goes over 9999
        UIManager.S.SetLife(playerLife);
    }

    public void DecreaseLife(float amount)
    {
        playerLife -= amount;

        playerLife = Mathf.Max(playerLife, 0); // the UI looks awful if it goes over 9999
        UIManager.S.SetLife(playerLife);

        if(Mathf.Approximately(playerLife, 0))
        {
            // Game Over
        }
    }

    public void DecreaseLife()
    {
        DecreaseLife(0.1f);
    }

    public int GetMoney()
    {
        return playerMoney;
    }
}

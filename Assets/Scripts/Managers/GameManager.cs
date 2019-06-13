using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Transform spawnPoint_N;
    public Transform spawnPoint_S;
    public Text waveTimeText;

    const float TIME_BETWEEN_SPAWNS = 0.5f;
    float nextSpawnTime;

    const int SPAWN_AMOUNT_PER_WAVE = 20;
    int countSpawn;

    const float TIME_BETWEEN_WAVES = 1f;
    float nextWaveTime;

    bool spawnOn;

    int nextEnemyType;

    // Start is called before the first frame update
    void Start()
    {
        nextSpawnTime = TIME_BETWEEN_WAVES;
        countSpawn = 0;
        nextWaveTime = Time.time;
        spawnOn = false;
        nextEnemyType = 0;
    }

    // Update is called once per frame
    void Update()
    {
        waveTimeText.text = ((int)(nextWaveTime - Time.time)).ToString();

        if (nextWaveTime < Time.time)
        {
            spawnOn = true;
            nextWaveTime = Time.time + TIME_BETWEEN_WAVES;
        }

        if(spawnOn && nextSpawnTime < Time.time)
        {
            Enemy enemyN = EntityManager.S.GetEntity((EnemyType)nextEnemyType) as Enemy;
            enemyN.transform.position = spawnPoint_N.position;
            enemyN.transform.Translate(0, 0.5f, 0); // for enemy to be on NavMesh
            enemyN.SetDestination(spawnPoint_S.position);

            Enemy enemyS = EntityManager.S.GetEntity((EnemyType)nextEnemyType) as Enemy;
            enemyS.transform.position = spawnPoint_S.position;
            enemyS.transform.Translate(0, 0.5f, 0); // for enemy to be on NavMesh
            enemyS.SetDestination(spawnPoint_N.position);

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


}

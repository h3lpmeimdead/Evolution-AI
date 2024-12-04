using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawnerManager : Singleton<MonsterSpawnerManager>
{
    [SerializeField] private GameObject _monsterPrefab;
    private GameObject[] _monsterList;
    public int _floorScale = 1;

    private void FixedUpdate()
    {
        _monsterList = GameObject.FindGameObjectsWithTag("Monster");
        if(_monsterList.Length < 1)
        {
            SpawnMonster();
        }
    }

    void SpawnMonster()
    {
        int x = Random.Range(-100, 101) * _floorScale;
        int z = Random.Range(-100, 101) * _floorScale;
        Instantiate(_monsterPrefab, new Vector3((float)x, 1, (float)z), Quaternion.identity);
    }
}

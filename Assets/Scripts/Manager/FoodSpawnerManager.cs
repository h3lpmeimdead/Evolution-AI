using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodSpawnerManager : Singleton<FoodSpawnerManager>
{
    public float _spawnRate = 1;
    public int _floorScale = 1;
    [SerializeField] private GameObject _food;
    public float _timeElasped = 0;

    private void Start()
    {
        for(int i = 0; i< 100; i++)
        {
            SpawnFood();
        }
    }

    private void FixedUpdate()
    {
        _timeElasped += Time.deltaTime;
        if(_timeElasped >= _spawnRate)
        {
            _timeElasped = _timeElasped % _spawnRate;
            SpawnFood();
        }
    }

    void SpawnFood()
    {
        int x = Random.Range(-100, 101) * _floorScale;
        int z = Random.Range(-100,101) * _floorScale;
        //Instantiate(_food, new Vector3((float)x, 0.75f, (float)z), Quaternion.identity);

        
        //object pooling for food
        GameObject food = ObjectPool.Instance.GetPooledObject();
        if(food != null)
        {
            food.transform.position = new Vector3((float)x, 0f, (float)z);
            food.SetActive(true);
        }
    }
}

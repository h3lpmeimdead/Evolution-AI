using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : Singleton<ObjectPool>
{
    private List<GameObject> _poolObjects = new List<GameObject>();
    private int _poolAmount = 3000;

    [SerializeField] private GameObject _food;

    private void Start()
    {
        for(int i = 0; i < _poolAmount; i++)
        {
            GameObject obj = Instantiate(_food);
            obj.SetActive(false);
            _poolObjects.Add(obj);
        }
    }

    public GameObject GetPooledObject()
    {
        for(int i = 0; i < _poolObjects.Count; i++)
        {
            if(!_poolObjects[i].activeInHierarchy)
                return _poolObjects[i];
        }

        return null;
    }
}

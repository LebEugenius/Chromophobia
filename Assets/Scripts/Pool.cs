using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pool : MonoBehaviour
{    
    public GameObject Prefab;

    private List<GameObject> _pool = new List<GameObject>();

    private Transform _parent;

    public void SetParent(Transform parent)
    {
        _parent = parent;
    }

    public GameObject GetEntity()
    {
        if(!Prefab) Debug.LogError("No Prefab in a pool");

        foreach (var entity in _pool)
        {
            if (!entity.gameObject.activeInHierarchy)
            {
                return entity;
            }
        }

        if(!_parent) 
        {
            _parent = new GameObject().transform;
            _parent.name = Prefab.name;
        }     

        var newEntity = Instantiate(Prefab, _parent);
        _pool.Add(newEntity);

        newEntity.gameObject.SetActive(false);

        return newEntity;
    }

    public int ActiveEntitiesCount()
    {
        var activeEntities = 0;

        foreach (var entity in _pool)
        {
            if (entity.gameObject.activeInHierarchy)
                activeEntities++;
        }
        return activeEntities;
    }

    public List<GameObject> GetActiveEntities()
    {
        var activeEntities = new List<GameObject>();

        foreach (var entity in _pool)
        {
            if (entity.gameObject.activeInHierarchy)
                activeEntities.Add(entity);
        }
        return activeEntities;
    }
}

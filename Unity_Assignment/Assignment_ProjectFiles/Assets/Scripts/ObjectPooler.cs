using UnityEngine;
using System.Collections.Generic;

public class ObjectPooler
{
    private readonly GameObject prefab;
    private readonly Transform parent;
    private readonly Stack<GameObject> inactive = new Stack<GameObject>();

    public ObjectPooler(GameObject _prefab, Transform _parent, int count = 0)
    {
        prefab = _prefab;
        parent = _parent;
        for(int i=0;i<count;i++)
        {
            GameObject go= Object.Instantiate(prefab,parent);
            go.SetActive(false);
            inactive.Push(go);
        }
    }

    public GameObject Get()
    {
        GameObject go = (inactive.Count>0)?inactive.Pop():Object.Instantiate(prefab,parent);
        go.SetActive(true);
        return go;
    }

    public void Release(GameObject go)
    {
        go.SetActive(false);
        go.transform.SetParent(parent,false);
        inactive.Push(go);

    }
}

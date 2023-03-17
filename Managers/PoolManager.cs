using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//오브젝트 풀링을 위한 매니저
public class PoolManager
{
    #region Pool
    //@Pool_Root 산하의 Pool
    class Pool
    {
        public GameObject Original { get; private set; }
        public Transform Root { get; set; }

        Stack<Poolable> _poolStack = new Stack<Poolable>();

        public void Init(GameObject original, int count = 5)
        {
            Original = original;
            Root = new GameObject().transform;
            Root.name = $"{original.name}_Root";

            for (int i = 0; i < count; i++)
            {
                Push(Create());
            }
        }

        Poolable Create()
        {
            GameObject go = GameObject.Instantiate<GameObject>(Original);
            go.name = Original.name;
            return go.GetOrAddComponent<Poolable>();
        }

        public void Push(Poolable poolable)
        {
            if (poolable == null)
                return;

            poolable.transform.parent = Root;
            poolable.gameObject.SetActive(false);
            poolable.IsUsing = false;

            _poolStack.Push(poolable);
        }

        public Poolable Pop(Transform parent)
        {
            Poolable poolable;

            if (_poolStack.Count > 0)
                poolable = _poolStack.Pop();
            else
                poolable = Create();

            poolable.gameObject.SetActive(true);
            poolable.transform.SetParent(parent);
            poolable.IsUsing = true;

            return poolable;
        }
    }
    #endregion

    //PoolManager는 여러개의 Pool들을 가지고 있음 각각 Pool들은 이름을 통해서 관리할 거임
    Dictionary<string, Pool> _pools = new Dictionary<string, Pool>();

    Transform _root;

    //초기화를 위한 메서드
    public void Init()
    {
        //_root가 초기화 되지 않았다면 루트 오브젝트를 생성하고 DontDestroy시킴
        if (_root == null)
        {
            _root = new GameObject { name = "@Pool_Root" }.transform;
            Object.DontDestroyOnLoad(_root);
        }
    }

    public void CreatePool(GameObject original, int count = 5)
    {
        Pool pool = new Pool();
        pool.Init(original, count);
        pool.Root.parent = _root.transform;

        _pools.Add(original.name, pool);
    }

    //풀링에 푸시해주는 메서드
    public void Push(Poolable poolable)
    {
        string name = poolable.gameObject.name;
        if (_pools.ContainsKey(name))
        {
            GameObject.Destroy(poolable.gameObject);
            return;
        }

        _pools[name].Push(poolable); 
    }

    //풀에서 꺼내서 생성하는 메서드
    public Poolable Pop(GameObject original, Transform parent = null)
    {
        if (_pools.ContainsKey(original.name))
            CreatePool(original );

        return _pools[original.name].Pop(parent);
    }

    public GameObject GetOriginal(string name)
    {
        if (_pools.ContainsKey(name))
            return null;

        return _pools[name].Original; 
    }

    public void Clear()
    {
        foreach (Transform child in _root)
        {
            GameObject.Destroy(child.gameObject);
            _pools.Clear();
        }
    }
}

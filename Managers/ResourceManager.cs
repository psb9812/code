using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Resource 폴더의 오브젝트들을 관리하는 클래스
public class ResourceManager
{
    //경로를 입력해서 제네릭 타입으로 원본을 메모리에 로드하도록 한 메서드
    public T Load<T>(string path) where T : Object
    {
        //풀링에 이미 로드하려는 게 존재한다면 로드시키지 않고 존재하는 것을 리턴한다.
        if(typeof(T) == typeof(GameObject))
        {
            string name = path;
            int index = name.LastIndexOf("/");
            if (index >= 0)
                name = name.Substring(index + 1);

            GameObject go = Managers.Pool.GetOriginal(name);
            if (go != null)
                return go as T;
        }

        return Resources.Load<T>(path);
    }

    //경로를 입력해서 경로 위치에 존재하는 에셋을 씬에 생성하는 메서드
    public GameObject Instantiate(string path, Transform parent = null)
    {
        //원본을 메모리에 로드 하고 prefab 변수에 저장
        GameObject original = Load<GameObject>($"Prefabs/{path}");
        if (original == null)
        {
            Debug.Log($"Failed to load prefab : {path}");
            return null;
        }

        //2. 혹시 이미 Pooling된 게 있을까?
        if (original.GetComponent<Poolable>() != null)
            return Managers.Pool.Pop(original, parent).gameObject;

        //원본을 복사한 후 씬에 생성
        GameObject go = Object.Instantiate(original, parent); 
        go.name = original.name;

        return go;
    }

    //매개변수로 온 GameObject를 Destroy하는 메서드
    public void Destroy(GameObject go)
    {
        if (go == null)
            return;

        //만약 풀링이 필요한 아이라면 -> 풀링 매니저에게 위탁
        Poolable poolable = go.GetComponent<Poolable>();
        if (poolable != null)
        {
            Managers.Pool.Push(poolable);
            return; 
        }

        Object.Destroy(go);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScene : BaseScene
{
    protected override void Init()
    {
        base.Init();

        SceneType = Define.Scene.Game;

        Managers.UI.ShowSceneUI<UI_Inven>();

        StartCoroutine(ExplodeAfterSeconds(4.0f));
    }

    IEnumerator ExplodeAfterSeconds(float seconds)
    {
        Debug.Log("Explode Enter");
        yield return new WaitForSeconds(seconds);
        Debug.Log("Explode end");
    }

    //씬이 종료되었을 때 날릴 것들을 넣는 메서드
    public override void Clear()
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

}

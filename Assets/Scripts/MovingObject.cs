using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game;
using static Game.MapStatic;

//動かせるオブジェクトに付加するスーパークラス
public class MovingObject : MapObject {
  public const int MOVE_STEPS = 30;
  public bool isMoving = false;
  protected GameObject thisObj;
  protected GameObject nextObj;

  //最終的な移動先をmap上の動きのみから算出した座標に修正
  protected Vector3 ModifyPos(MapPos mapPos) {
    thisObj = this.gameObject;
    Vector3 ModifiedPos;
    //playerは高さが半分ずれているので処理を分ける
    if (thisObj.tag == "Player" || thisObj.tag == "Doppel") {
      ModifiedPos = MapposToUnipos(mapPos) - new Vector3(0, 0.5f, 0);
    } else {
      ModifiedPos = MapposToUnipos(mapPos);
    }
    return ModifiedPos;
  }

  //オブジェクトが動く時の共通処理はここに書く
  protected IEnumerator Move(Vector3 direc) {
    isMoving = true;
    transform.localRotation = Quaternion.LookRotation(direc);

    //現在地をnullに
    goMap[nowPos.floor, nowPos.x, nowPos.z] = null;
    //位置更新
    nowPos = GetNextPos(nowPos, direc);
    //移動先に自身を代入
    goMap[nowPos.floor, nowPos.x, nowPos.z] = gameObject;

    for (int i = 0; i < MOVE_STEPS; i++) {
      transform.Translate(Vector3.forward / MOVE_STEPS);
      yield return null;
    }
    transform.position = ModifyPos(nowPos);
    isMoving = false;

    //氷によるさらなる移動があるか調べる
    MapPos beneath = nowPos + new MapPos(-1, 0, 0);
    MapPos nextPos = GetNextPos(nowPos, direc);
    GameObject uObj = goMap[beneath.floor, beneath.x, beneath.z];
    GameObject nObj = goMap[nextPos.floor, nextPos.x, nextPos.z];
    if (uObj != null && uObj.tag.Contains("Ice") && nObj == null) {
      StartCoroutine(Move(direc));
    }
  }

  protected IEnumerator Fall() {
    isMoving = true;
    goMap[nowPos.floor, nowPos.x, nowPos.z] = null;
    for (int i = 0; i < MOVE_STEPS * 10; i++) {
      transform.Translate(Vector3.down * 15 / (MOVE_STEPS * 10));
      yield return null;
    }
    GameObject.DestroyImmediate(gameObject);
  }


  void Start()
  {
  }

  // Update is called once per frame
  void Update () {
  }
}

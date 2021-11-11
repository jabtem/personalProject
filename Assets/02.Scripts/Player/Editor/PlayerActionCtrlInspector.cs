using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]//다중선택을 가능하게함
[CustomEditor(typeof(PlayerActionCtrl))]

public class PlayerActionCtrlInspector : Editor
{

    PlayerActionCtrl playerActionCtrl = null;

    //public specialAction Sa;




    void OnEnable()
    {
        playerActionCtrl = (PlayerActionCtrl)target;
    }

    //인스펙터를 커스텀형태로 덮어씌움
    public override void OnInspectorGUI()
    {

 
        //base.OnInspectorGUI();
        DrawDefaultInspector();//기존인스펙터의 정보를 불러옴 기존 + 추가버튼 생성방식으로 구현할수있음
        //EditorGUI.BeginChangeCheck();

        switch (playerActionCtrl.SA)
        {
            case PlayerActionCtrl.specialAction.Dodge :
                playerActionCtrl.dodgeDistance = EditorGUILayout.FloatField("회피거리", playerActionCtrl.dodgeDistance);
                break;
            case PlayerActionCtrl.specialAction.Guard:
                playerActionCtrl.guardCount = EditorGUILayout.IntField("가드횟수", playerActionCtrl.guardCount);
                break;
        }


        playerActionCtrl.maxComboCount = EditorGUILayout.IntField("캐릭터기본콤보수 ", playerActionCtrl.maxComboCount);

        if(GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }
    }
}

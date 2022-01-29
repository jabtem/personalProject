using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
[CanEditMultipleObjects]//다중선택을 가능하게함
[CustomEditor(typeof(MonsterAction))]
public class MnsterActionInspector : Editor
{


    MonsterAction monsterMove = null;
    SerializedProperty roamingPoints;
    SerializedProperty viewGizmo;
    SerializedProperty speed;
    SerializedProperty attackRange;
    //게임 실행 판단여부
    private void OnEnable()
    {
        if (monsterMove == null)
            monsterMove = (MonsterAction)target;

        
    }

    public override void OnInspectorGUI()
    {
        GUI.enabled = false;
        EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour((MonsterAction)target), typeof(MonsterAction), false);
        if(!monsterMove.GameStart)
            GUI.enabled = true;
        //정찰모드가 변경시 값초기화
        EditorGUI.BeginChangeCheck();
        var roamingMode = (MonsterAction.RoamingMode)EditorGUILayout.EnumPopup("정찰 모드", monsterMove.roamingMode);
        if(EditorGUI.EndChangeCheck())
        {
            RoamingModeChangeTest();
            Undo.RecordObject(monsterMove, "Change Mode");
            monsterMove.roamingMode = roamingMode;


        }

        switch(monsterMove.roamingMode)
        {
  
            case MonsterAction.RoamingMode.영역내무작위이동:
                monsterMove.roamingAreaWidth = EditorGUILayout.Slider("정찰영역 가로길이", monsterMove.roamingAreaWidth,0.0f,100.0f);
                monsterMove.roamingAreaHeight = EditorGUILayout.Slider("정찰영역 세로길이", monsterMove.roamingAreaHeight, 0.0f, 100.0f);
                monsterMove.moveRomaingAreaPosionX = EditorGUILayout.Slider("정찰영역 X좌표이동", monsterMove.moveRomaingAreaPosionX, -100.0f, 100.0f);
                monsterMove.moveRomaingAreaPosionZ = EditorGUILayout.Slider("정찰영역 Z좌표이동", monsterMove.moveRomaingAreaPosionZ, -100.0f, 100.0f);
                break;
            case MonsterAction.RoamingMode.지점순회:


                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("정찰 지점(로컬좌표), Y값은 Z축으로 계산");


                if (GUILayout.Button("추가"))
                {
                    Array.Resize(ref monsterMove.roamingPoints, monsterMove.roamingPoints.Length + 1);
                }

                else if (GUILayout.Button("삭제") && monsterMove.roamingPoints.Length != 0)
                {
                    Array.Resize(ref monsterMove.roamingPoints, monsterMove.roamingPoints.Length - 1);
                }

                EditorGUILayout.EndHorizontal();
                SerializedProperty(roamingPoints, "roamingPoints", "정찰 지점");
                break;
        }
        if (monsterMove.GameStart)
            GUI.enabled = true;
        SerializedProperty(viewGizmo, "viewGizmo", "정찰 영역(지점) 기즈모 출력");
        SerializedProperty(speed, "speed", "기본 이동속도");
        SerializedProperty(attackRange, "attackRange", "공격 사거리");

        //인스펙터 값 변경시 값유지위함
        if (GUI.changed)
        {
            //EditorGUI.PropertyField()
            EditorUtility.SetDirty(target);
        }
    }

    void SerializedProperty(SerializedProperty property,string propertyName, string name)
    {
        property = serializedObject.FindProperty(propertyName);
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(property,new GUIContent(name), true);
        if(EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();
        }
    }
    public void RoamingModeChangeTest()
    {

        if (monsterMove.roamingMode == MonsterAction.RoamingMode.지점순회)
        {
            monsterMove.roamingAreaWidth = 0f;
            monsterMove.roamingAreaHeight = 0f;
            monsterMove.moveRomaingAreaPosionX = 0f;
            monsterMove.moveRomaingAreaPosionZ = 0f;
        }
        else if (monsterMove.roamingMode == MonsterAction.RoamingMode.영역내무작위이동)
        {
            if (monsterMove.roamingPoints.Length != 0)
            {
                Array.Resize(ref monsterMove.roamingPoints, 0);
            }
        }

    }
}

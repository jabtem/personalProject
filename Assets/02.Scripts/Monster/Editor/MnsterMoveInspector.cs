using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CanEditMultipleObjects]//다중선택을 가능하게함
[CustomEditor(typeof(MonsterMove))]
public class MnsterMoveInspector : Editor
{


    MonsterMove monsterMove = null;
    SerializedProperty roamingPoints;


    private void OnEnable()
    {
        if (monsterMove == null)
            monsterMove = (MonsterMove)target;

        roamingPoints = serializedObject.FindProperty("roamingPoints");
    }

    public override void OnInspectorGUI()
    {


        //정찰모드가 변경시 값초기화
        EditorGUI.BeginChangeCheck();
        var roamingMode = (MonsterMove.RoamingMode)EditorGUILayout.EnumPopup("정찰 모드", monsterMove.roamingMode);
        if(EditorGUI.EndChangeCheck())
        {

            Undo.RecordObject(monsterMove, "Change Mode");
            monsterMove.roamingMode = roamingMode;
            monsterMove.RoamingModeChange();

        }

        switch(monsterMove.roamingMode)
        {
  
            case MonsterMove.RoamingMode.영역내무작위이동:
                monsterMove.roamingAreaWidth = EditorGUILayout.Slider("정찰영역 가로길이", monsterMove.roamingAreaWidth,0.0f,100.0f);
                monsterMove.roamingAreaHeight = EditorGUILayout.Slider("정찰영역 세로길이", monsterMove.roamingAreaHeight, 0.0f, 100.0f);
                monsterMove.moveRomaingAreaPosionX = EditorGUILayout.Slider("정찰영역 X좌표이동", monsterMove.moveRomaingAreaPosionX, -100.0f, 100.0f);
                monsterMove.moveRomaingAreaPosionZ = EditorGUILayout.Slider("정찰영역 Z좌표이동", monsterMove.moveRomaingAreaPosionZ, -100.0f, 100.0f);
                break;
            case MonsterMove.RoamingMode.지점순회:
                SerializedProperty(roamingPoints,"roamingPoints");
                break;
        }




        //인스펙터 값 변경시 값유지위함
        if (GUI.changed)
        {
            //EditorGUI.PropertyField()
            EditorUtility.SetDirty(target);
        }
    }

    void SerializedProperty(SerializedProperty property,string name)
    {
        property = serializedObject.FindProperty(name);
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(property, true);
        if(EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();
        }
    }

}

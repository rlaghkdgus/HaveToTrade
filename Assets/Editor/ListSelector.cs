using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;

[CustomEditor(typeof(DialogSystem))]
public class ListSeleCtor : Editor // 리스트 선택을 위한 유틸 기능
{
    public override void OnInspectorGUI()
    {
        DialogSystem selector = (DialogSystem)target;

        if (selector.DialogDB != null)
        {
            // ScriptableObject의 리스트 필드 이름 가져오기
            var fields = selector.DialogDB.GetType()
                .GetFields()
                .Where(f => f.FieldType == typeof(List<TextData>))
                .Select(f => f.Name)
                .ToArray();

            // 드롭다운으로 선택
            int currentIndex = Array.IndexOf(fields, selector.seletedDialogName);
            int newIndex = EditorGUILayout.Popup("Selected List", currentIndex, fields);

            if (newIndex >= 0 && newIndex < fields.Length)
            {
                selector.seletedDialogName = fields[newIndex];
            }
        }
        else
        {
            EditorGUILayout.LabelField("ListData가 설정되지 않았습니다!");
        }

        if (GUILayout.Button("Dialog Load"))
        {
            selector.DialogListLoading();
        }

        // 기본 Inspector 그리기
        DrawDefaultInspector();
    }
}

using System;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Reflection;

[CustomEditor(typeof(MonoBehaviour), true)]
public class MonoBehaviourInspector : Editor
{
    private MonoBehaviour mono;

    private bool willBeExecuted;
    private bool isClicked = false;

    private string searchVariableName = "";

    public List<Type> excludTypeList = new List<Type>()
    {
        typeof(MonoBehaviour),
        typeof(Behaviour),
        typeof(Component),
        typeof(UnityEngine.Object),
        typeof(System.Object),
    };

    void UpdateInspector()
    {
        if (!willBeExecuted)
            return;

        willBeExecuted = false;
        Highlight();
    }

    void Reset()
    {
        searchVariableName = string.Empty;
        isClicked = false;
        willBeExecuted = false;
    }

    void OnEnable()
    {
        Reset();
        mono = target as MonoBehaviour;
        EditorApplication.update += UpdateInspector;
    }

    void OnDisable()
    {
        EditorApplication.update -= UpdateInspector;
    }

    public override void OnInspectorGUI()
    {
        bool isClickedAtNow = false;

        if (isClicked == false)
        {
            GUILayout.BeginHorizontal(GUILayout.MaxWidth(300.0f));
            GUILayout.Label(string.Format("target in {0} :", mono.GetType().FullName), GUILayout.MaxWidth(300.0f));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal(GUILayout.MaxWidth(1000.0f));
            searchVariableName = EditorGUILayout.TextField(searchVariableName, GUILayout.Width(300.0f));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal(GUILayout.MaxWidth(300.0f));
            isClickedAtNow = GUILayout.Button("highlight", GUILayout.MaxWidth(300.0f));
            GUILayout.EndHorizontal();
        }

        if (isClicked == false && isClickedAtNow == true)
        {
            isClicked = true;
            willBeExecuted = true;
        }

        if (isClicked)
        {
            DrawEditorGUI();
        }
        else
        {
            base.OnInspectorGUI();
        }
    }

    void Highlight()
    {
        if (string.IsNullOrEmpty(searchVariableName) == false)
        {
            Highlighter.Highlight("Inspector", searchVariableName, HighlightSearchMode.Auto);
        }
        Reset();
    }

    void DrawEditorGUI()
    {
        var members = mono.GetType().GetMembers();
        for (int i = 0; i < members.Length; ++i)
        {
            var member = members[i];
            var memberType = member.MemberType;

            if ( memberType == MemberTypes.Constructor || memberType == MemberTypes.Method || memberType == MemberTypes.Property || memberType == MemberTypes.NestedType)
                continue;

            var declaringType = member.DeclaringType;

            if ( excludTypeList.Contains(declaringType))
                continue;

            EditorGUILayout.TextField(member.Name, "");
        }
    }

}

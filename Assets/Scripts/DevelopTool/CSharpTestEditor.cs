#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CSharpTest))]

class CSharpTestEditor : Editor
{

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        CSharpTest myScript = (CSharpTest)target;
        if (GUILayout.Button("Test1"))
        {
            myScript.Test1();
        }

        //Debug.Log("아하!");
    }
}
#endif
#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Ground))]

class GroundEditor : Editor
{

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Ground myScript = (Ground)target;
        if (GUILayout.Button("Set Childrens correspond with grid"))
        {
            myScript.SetChildrenGrid();
        }

        //Debug.Log("아하!");
    }
}
#endif
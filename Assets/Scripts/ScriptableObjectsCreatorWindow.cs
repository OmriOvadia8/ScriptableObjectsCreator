using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Text;
using System.IO;

public class ScriptableObjectsCreatorWindow : EditorWindow
{
    private string scriptName = "NewScriptableObject";
    private string fileName = "NewCustomScriptableObject";
    private string menuName = "ScriptableObjects/CustomScriptableObject";
    private string savePath = "Assets/Scripts/";
    private List<Field> fields = new List<Field>();

    [MenuItem("Tools/ScriptableObjects Creator")]
    public static void ShowWindow()
    {
        GetWindow<ScriptableObjectsCreatorWindow>("ScriptableObjects Creator");
    }

    void OnGUI()
    {
        GUILayout.Label("Create a new ScriptableObject script", EditorStyles.boldLabel);

        scriptName = EditorGUILayout.TextField("Script Name", scriptName);
        fileName = EditorGUILayout.TextField("File Name", fileName);
        menuName = EditorGUILayout.TextField("Menu Name", menuName);
        savePath = EditorGUILayout.TextField("Save Path", savePath);

        if (GUILayout.Button("Add Field"))
        {
            fields.Add(new Field());
        }

        for (int i = 0; i < fields.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Type", GUILayout.Width(50));
            fields[i].type = EditorGUILayout.TextField(fields[i].type, GUILayout.Width(100));
            GUILayout.Label("Name", GUILayout.Width(50));
            fields[i].name = EditorGUILayout.TextField(fields[i].name, GUILayout.Width(100));
            GUILayout.Label("Public", GUILayout.Width(50));
            fields[i].isPublic = EditorGUILayout.Toggle(fields[i].isPublic, GUILayout.Width(100));
            if (GUILayout.Button("Remove"))
            {
                fields.RemoveAt(i);
                i--;
            }
            EditorGUILayout.EndHorizontal();
        }

        if (GUILayout.Button("Create Script"))
        {
            CreateScriptableObjectScript();
        }
    }

    private void CreateScriptableObjectScript()
    {
        if (IsMenuNameDuplicate(menuName))
        {
            EditorUtility.DisplayDialog("Error", "A ScriptableObject with the same menuName already exists.", "OK");
            return;
        }

        StringBuilder sb = new StringBuilder();
        sb.AppendLine("using UnityEngine;");
        sb.AppendLine("");
        sb.AppendLine("[CreateAssetMenu(fileName = \"" + fileName + "\", menuName = \"" + menuName + "\")]");
        sb.AppendLine("public class " + scriptName + " : ScriptableObject");
        sb.AppendLine("{");

        foreach (var field in fields)
        {
            sb.AppendLine("    " + (field.isPublic ? "public" : "private") + " " + field.type + " " + field.name + ";");
        }

        sb.AppendLine("}");

        string path = Path.Combine(savePath, scriptName + ".cs");

        // Ensure directory exists
        if (!Directory.Exists(Path.GetDirectoryName(path)))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path));
        }

        File.WriteAllText(path, sb.ToString());
        AssetDatabase.Refresh();
    }

    private bool IsMenuNameDuplicate(string menuNameToCheck)
    {
        string[] allScripts = Directory.GetFiles(Application.dataPath, "*.cs", SearchOption.AllDirectories);

        foreach (string scriptPath in allScripts)
        {
            string scriptContent = File.ReadAllText(scriptPath);
            if (scriptContent.Contains("[CreateAssetMenu(") && scriptContent.Contains("menuName = \"" + menuNameToCheck + "\""))
            {
                return true;
            }
        }

        return false;
    }

    class Field
    {
        public string type;
        public string name;
        public bool isPublic = true;
    }
}

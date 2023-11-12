using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System;

namespace OmriOvadia.Utils
{
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
            RenderFields();
            RenderButtons();
        }

        private void RenderFields()
        {
            GUILayout.Label("Create a new ScriptableObject script", EditorStyles.boldLabel);
            scriptName = EditorGUILayout.TextField("Script Name", scriptName);
            fileName = EditorGUILayout.TextField("File Name", fileName);
            menuName = EditorGUILayout.TextField("Menu Name", menuName);
            savePath = EditorGUILayout.TextField("Save Path", savePath);
        }

        private void RenderButtons()
        {
            if (GUILayout.Button("Add Field"))
            {
                fields.Add(new Field());
            }

            for (int i = 0; i < fields.Count; i++)
            {
                RenderField(i);
            }

            if (GUILayout.Button("Create Script"))
            {
                TryCreateScriptableObjectScript();
            }
        }

        private void RenderField(int index)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Type", GUILayout.Width(50));
            fields[index].type = EditorGUILayout.TextField(fields[index].type, GUILayout.Width(100));
            GUILayout.Label("Name", GUILayout.Width(50));
            fields[index].name = EditorGUILayout.TextField(fields[index].name, GUILayout.Width(100));
            GUILayout.Label("Public", GUILayout.Width(50));
            fields[index].isPublic = EditorGUILayout.Toggle(fields[index].isPublic, GUILayout.Width(100));
            if (GUILayout.Button("Remove"))
            {
                fields.RemoveAt(index);
            }
            EditorGUILayout.EndHorizontal();
        }

        private void TryCreateScriptableObjectScript()
        {
            string scriptFilePath = Path.Combine(savePath, scriptName + ".cs");
            if (IsScriptNameDuplicate(scriptFilePath))
            {
                EditorUtility.DisplayDialog("Error", "A script with the same name already exists.", "OK");
                return;
            }

            if (IsMenuNameDuplicate(menuName))
            {
                EditorUtility.DisplayDialog("Error", "A ScriptableObject with the same menuName already exists.", "OK");
                return;
            }

            CreateScriptableObjectScript();
        }

        private void CreateScriptableObjectScript()
        {
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
            WriteScriptToFile(sb.ToString());
        }

        private void WriteScriptToFile(string scriptContent)
        {
            string path = Path.Combine(savePath, scriptName + ".cs");

            if (!Directory.Exists(Path.GetDirectoryName(path)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path));
            }

            File.WriteAllText(path, scriptContent);
            AssetDatabase.Refresh();
        }

        private bool IsMenuNameDuplicate(string menuNameToCheck)
        {
            string[] allScripts = Directory.GetFiles(Application.dataPath, "*.cs", SearchOption.AllDirectories);
            string pattern = @"\[CreateAssetMenu\(.*menuName\s*=\s*""" + Regex.Escape(menuNameToCheck) + @""".*\)\]";

            foreach (string scriptPath in allScripts)
            {
                string scriptContent = File.ReadAllText(scriptPath);
                if (Regex.IsMatch(scriptContent, pattern))
                {
                    Debug.Log("Duplicate menuName found in script: " + scriptPath);
                    return true;
                }
            }

            return false;
        }

        private bool IsScriptNameDuplicate(string scriptFilePath)
        {
            string scriptFileName = Path.GetFileName(scriptFilePath);
            string[] allScripts = Directory.GetFiles(Application.dataPath, scriptFileName, SearchOption.AllDirectories);

            foreach (string scriptPath in allScripts)
            {
                // Check if the found script's name matches the intended new script's name
                if (Path.GetFileName(scriptPath).Equals(scriptFileName, StringComparison.OrdinalIgnoreCase))
                {
                    Debug.Log("Duplicate script name found: " + scriptPath);
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
}

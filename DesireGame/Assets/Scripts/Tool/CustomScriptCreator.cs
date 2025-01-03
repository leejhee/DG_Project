using UnityEditor;
using UnityEngine;
using System.IO;

#if UNITY_EDITOR
public class CustomScriptCreator
{
    // Project View���� ��Ŭ�� �޴� �߰�
    [MenuItem("Assets/Create/Custom C# Script with Namespace", false, 80)]
    public static void CreateCustomCSharpScript()
    {
        string path = GetSelectedPathOrFallback();
        string scriptName = "NewCustomScript.cs";
        string fullPath = Path.Combine(path, scriptName);

        // ��ũ��Ʈ ���� ����
        string scriptContent =
@"using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Client
{
    public class NewCustomScript
    {
        
    }
}";

        // ���� ����
        File.WriteAllText(fullPath, scriptContent);
        AssetDatabase.Refresh();

        // ���� ���� ��ũ��Ʈ ����
        Object obj = AssetDatabase.LoadAssetAtPath<Object>(fullPath);
        Selection.activeObject = obj;
    }

    // ���õ� ������ ��θ� �������ų�, ���õ��� �ʾҴٸ� �⺻ ��θ� ��ȯ
    private static string GetSelectedPathOrFallback()
    {
        string path = "Assets";

        if (Selection.activeObject != null)
        {
            path = AssetDatabase.GetAssetPath(Selection.activeObject);

            if (!string.IsNullOrEmpty(path) && File.Exists(path))
            {
                path = Path.GetDirectoryName(path);
            }
        }

        return path;
    }
}
#endif
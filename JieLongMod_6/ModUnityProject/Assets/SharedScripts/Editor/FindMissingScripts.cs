using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using System;

public class FindMissingScriptsEditor : EditorWindow
{
    [MenuItem("Tools/查找失效的资源")]
    public static void FindMissingScripts()
    {
        EditorWindow.GetWindow(typeof(FindMissingScriptsEditor));
    }

    static int missingCount = -1;
    void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();
        {
            EditorGUILayout.LabelField("Missing Scripts:");
            EditorGUILayout.LabelField("" + (missingCount == -1 ? "---" : missingCount.ToString()));
        }
        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("Find Error Res"))
        {
            FindAllMissingScript();
        }
    }

    private static void FindAllMissingScript()
    {
        missingCount = 0;
        EditorUtility.DisplayProgressBar("Searching Prefabs", "", 0.0f);

        string[] files = System.IO.Directory.GetFiles(Application.dataPath, "*.prefab", System.IO.SearchOption.AllDirectories);
        EditorUtility.DisplayCancelableProgressBar("Searching Prefabs", "Found " + files.Length + " prefabs", 0.0f);

        Scene currentScene = EditorSceneManager.GetActiveScene();
        string scenePath = currentScene.path;
        EditorSceneManager.NewScene(NewSceneSetup.EmptyScene);

        for (int i = 0; i < files.Length; i++)
        {
            string prefabPath = files[i].Replace(Application.dataPath, "Assets");
            if (EditorUtility.DisplayCancelableProgressBar("Processing Prefabs " + i + "/" + files.Length, prefabPath, (float)i / (float)files.Length))
                break;

            GameObject go = UnityEditor.AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject)) as GameObject;

            if (go != null)
            {
                FindInGO(go);
                go = null;
                EditorUtility.UnloadUnusedAssetsImmediate(true);
            }
        }

        EditorUtility.DisplayProgressBar("Cleanup", "Cleaning up", 1.0f);
        EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);

        EditorUtility.UnloadUnusedAssetsImmediate(true);
        GC.Collect();

        EditorUtility.ClearProgressBar();
    }

    private static void FindInGO(GameObject go, string prefabName = "")
    {
        Component[] components = go.GetComponents<Component>();
        Component tagComp;
        for (int i = 0; i < components.Length; i++)
        {
            tagComp = components[i];
            if (tagComp == null)
            {
                missingCount++;

                Debug.LogError("Prefab " + prefabName + " has an empty script attached:\n" + GetObjPath(go), go);
            }
            else
            {
                if (tagComp.GetType() == typeof(ParticleSystem))
                {
                    Renderer bindRender = tagComp.GetComponent<Renderer>();
                    if (bindRender != null && bindRender.enabled)
                    {
                        Material tagMat = bindRender.sharedMaterial;
                        if (tagMat == null)
                        {
                            Debug.LogError("Particle " + prefabName + " has an Empty Material attached:\n" + GetObjPath(go), go);
                        }
                        else
                        {
                            Shader tagShader = tagMat.shader;
                            if (tagShader == null || tagShader.name.Contains("Error"))
                            {
                                Debug.LogError("Particle " + prefabName + " has an Error Shader attached:\n" + GetObjPath(go), go);
                            }
                        }
                    }
                }
            }
        }

        foreach (Transform child in go.transform)
        {
            FindInGO(child.gameObject, prefabName);
        }
    }

    private static string GetObjPath(GameObject go)
    {
        Transform t = go.transform;
        string componentPath = go.name;
        while (t.parent != null)
        {
            componentPath = t.parent.name + "/" + componentPath;
            t = t.parent;
        }
        return componentPath;
    }

}

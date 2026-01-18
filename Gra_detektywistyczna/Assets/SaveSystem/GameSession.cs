using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SceneData { }

[Serializable]
public class SceneListWrapper
{
    public List<SceneData> scenes;
}

public static class GameSession
{
    public static string CurrentScenarioName { get; private set; }
    public static int CurrentSceneNumber { get; set; }
    public static int MaxSceneNumber { get; private set; }

    public static string PendingVerdictText;

    public static string PendingVerdictNpcName;

    public static void StartSession(string scenarioName, int currentScene)
    {
        CurrentScenarioName = scenarioName;
        CurrentSceneNumber = currentScene;
        MaxSceneNumber = GetMaxScene();
    }

    public static int GetMaxScene()
    {
        string fullPath = Application.dataPath + "/Database/" + CurrentScenarioName + ".json";

        if (File.Exists(fullPath))
        {
            string jsonRaw = File.ReadAllText(fullPath);

            SceneListWrapper data = JsonUtility.FromJson<SceneListWrapper>(jsonRaw);

            if (data != null && data.scenes != null)
            {
                int count = data.scenes.Count;
                return count;
            }
        }
        return 0;
    }
}
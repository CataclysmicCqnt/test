using System;
using DTOModel;

public static class GameSession
{
	public static string CurrentScenarioName { get; private set; }
	public static int CurrentSceneNumber { get; set; }
	public static int MaxSceneNumber { get; private set; }

    public static void StartSession(string scenarioName, int currentScene, int maxScene)
    {
        CurrentScenarioName = scenarioName;
        CurrentSceneNumber = currentScene;
        MaxSceneNumber = maxScene;
    }
}

using DTOModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Debug = UnityEngine.Debug;


public class DialogueEngineClient : IDisposable
{
    private readonly string _exePath;
    private Process _process;
    private NamedPipeClientStream _pipe;
    private StreamReader _reader;
    private StreamWriter _writer;
    private readonly SemaphoreSlim _sendLock = new(1, 1);

    private const string PIPENAME = "DialogueEnginePipe";

    public DialogueEngineClient(string exePath)
    {
        _exePath = exePath;
    }

    public async Task InitializeAsync()
    {
        StartProcess();

        _pipe = new NamedPipeClientStream(
            ".",
            PIPENAME,
            PipeDirection.InOut,
            PipeOptions.Asynchronous);

        await _pipe.ConnectAsync();

        _reader = new StreamReader(_pipe);
        _writer = new StreamWriter(_pipe)
        {
            AutoFlush = true
        };
    }

    private void StartProcess()
    {
        if (File.Exists(_exePath) == false)
        {
            Debug.LogError($"Nie znaleziono pliku: {_exePath}");
            return;
        }

        ProcessStartInfo processStartInfo = new ProcessStartInfo
        {
            FileName = _exePath,
            UseShellExecute = false,
            CreateNoWindow = true,
            WindowStyle = ProcessWindowStyle.Hidden
        };

        _process = Process.Start(processStartInfo);
        if (_process == null)
        {
            Debug.LogError("Nie udalo sie wystartowac DialogueEngine!");
        }
    }

    private async Task<string> SendCommandAsync(string line)
    {
        await _sendLock.WaitAsync();
        try
        {
            if (_writer == null || _reader == null)
                throw new InvalidOperationException("Brak writer'a lub reader'a!");

            await _writer.WriteLineAsync(line);
            string response = await _reader.ReadLineAsync();
            return response;
        }
        finally
        {
            _sendLock.Release();
        }
    }

    public async Task<GamesToContinueDTO> GetGamesToContinueAsync()
    {
        MethodDTO methodDTO = new MethodDTO() { MethodName = "GetGamesToContinue" };
        string serializedMethodDTO = JsonUtility.ToJson(methodDTO);

        string response = await SendCommandAsync(serializedMethodDTO);

        object responseJSON = JsonUtility.FromJson(response, typeof(GamesToContinueDTO));
        GamesToContinueDTO convertedResponse = responseJSON as GamesToContinueDTO;

        return convertedResponse;
    }

    public async Task<SceneScriptDTO> GetSceneAsync(string[] parameters)
    {
        MethodDTO methodDTO = new MethodDTO() { MethodName = "GetScene", ParameterValues = parameters };
        string serializedMethodDTO = JsonUtility.ToJson(methodDTO);

        string response = await SendCommandAsync(serializedMethodDTO);

        object responseJSON = JsonUtility.FromJson(response, typeof(SceneScriptDTO));
        SceneScriptDTO convertedResponse = responseJSON as SceneScriptDTO;

        return convertedResponse;
    }

    public async Task<SettingsDTO> GetSettingsAsync()
    {
        MethodDTO methodDTO = new MethodDTO() { MethodName = "GetSettings" };
        string serializedMethodDTO = JsonUtility.ToJson(methodDTO);

        string response = await SendCommandAsync(serializedMethodDTO);

        object responseJSON = JsonUtility.FromJson(response, typeof(SettingsDTO));
        SettingsDTO convertedResponse = responseJSON as SettingsDTO;

        return convertedResponse;
    }

    public async Task SaveSettingsAsync(SettingsDTO settingsDTO)
    {
        string[] parameters = new string[1];
        parameters[0] = JsonUtility.ToJson(settingsDTO);
        MethodDTO methodDTO = new MethodDTO() { MethodName = "SaveSettings", ParameterValues = parameters };
        string serializedMethodDTO = JsonUtility.ToJson(methodDTO);

        await SendCommandAsync(serializedMethodDTO);
    }

    public async Task<NPCResponseDTO> AskNPCAsync(NPCRequestDTO requestDTO)
    {
        string[] parameters = new string[1];
        parameters[0] = JsonUtility.ToJson(requestDTO);
        MethodDTO methodDTO = new MethodDTO() { MethodName = "AskNPC", ParameterValues = parameters };
        string serializedMethodDTO = JsonUtility.ToJson(methodDTO);
        string response = await SendCommandAsync(serializedMethodDTO);

        object responseJSON = JsonUtility.FromJson(response, typeof(NPCResponseDTO));
        NPCResponseDTO convertedResponse = responseJSON as NPCResponseDTO;

        return convertedResponse;

        //object responseJSON = JsonUtility.FromJson(response, typeof(SettingsDTO));
        //SettingsDTO convertedResponse = responseJSON as SettingsDTO;

    }

    public async Task SaveGameAsync(CreatedGameDTO gameDTO)
    {
        string[] parameters = new string[1];
        parameters[0] = JsonUtility.ToJson(gameDTO);
        
        MethodDTO methodDTO = new MethodDTO() 
        { 
            MethodName = "SaveGame", 
            ParameterValues = parameters 
        };
        
        string serializedMethodDTO = JsonUtility.ToJson(methodDTO);
        await SendCommandAsync(serializedMethodDTO);
    }

    public async Task<string> GenerateNewSceneAsync(SceneDTO sceneDTO)
    {
        string[] parameters = new string[1];
        parameters[0] = JsonUtility.ToJson(sceneDTO);

        MethodDTO methodDTO = new MethodDTO()
        {
            MethodName = "GenerateNewScene",
            ParameterValues = parameters
        };

        string serializedMethodDTO = JsonUtility.ToJson(methodDTO);
        string response = await SendCommandAsync(serializedMethodDTO);

        return response;
    }

    public async Task<VerdictResponseDTO> GetNpcVerdictAsync(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            return new VerdictResponseDTO()
            {
                isPlayerRight = false,
                speech = "Przegra³eœ"
            };
        }
        string[] parameters = new string[2];
        parameters[0] = name;
        parameters[1] = GameSession.CurrentScenarioName;
        MethodDTO methodDTO = new MethodDTO()
        {
            MethodName = "GetNpcVerdict",
            ParameterValues = parameters
        };

        string serializedMethodDTO = JsonUtility.ToJson(methodDTO);

        string response = await SendCommandAsync(serializedMethodDTO);

        object verdict = JsonUtility.FromJson(response, typeof(VerdictResponseDTO));
        return verdict as VerdictResponseDTO;
    }

    public void Dispose()
    {
        try
        {
            _reader?.Dispose();
            _writer?.Dispose();
            _pipe?.Dispose();
            _sendLock?.Dispose();
        }
        catch { }

        ForceKill();
    }

    public void ForceKill()
    {
        try
        {
            if (_process != null && !_process.HasExited)
                _process.Kill();
        }
        catch { }
    }

    public async Task<string> GetRandomScenarioAsync()
    {
        MethodDTO methodDTO = new MethodDTO() { MethodName = "GetRandomScenario" };
        string serializedMethodDTO = JsonUtility.ToJson(methodDTO);

        string response = await SendCommandAsync(serializedMethodDTO);
        return response;
    }
}

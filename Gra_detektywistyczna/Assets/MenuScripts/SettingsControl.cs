using DTOModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.MenuScripts
{
    public class SettingsControl : MonoBehaviour
    {
        private async void Awake()
        {
            SettingsDTO settings = await DialogueEngineManager.Instance.GetSettingsAsync();
            Debug.Log("Volume:  " + settings.Volume);

            settings.Volume = 15;
            await DialogueEngineManager.Instance.SaveSettingsAsync(settings);

            settings = await DialogueEngineManager.Instance.GetSettingsAsync();
            Debug.Log("Volume:  " + settings.Volume);
        }
    }
}

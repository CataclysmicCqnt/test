using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static class BackgroundService
{
    public static MenuControl View;

    public static void SetBackground(string backgroundName)
    {
        if (!View)
            return;

        Sprite sprite = Resources.Load<Sprite>(backgroundName);
        if (!sprite)
        {
            Debug.LogError($"Nie znaleziono grafiki: {backgroundName}");
            return;
        }

        View.SetBackground(sprite);
    }
}
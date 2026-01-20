using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static class NewSceneService
{
    public static DialogueManager View;

    public static void LoadCurrentScene()
    {
        View.LoadCurrentScene();
    }
}

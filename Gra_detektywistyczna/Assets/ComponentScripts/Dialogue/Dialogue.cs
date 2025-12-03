using UnityEngine;
using System.Collections;
using System.Collections.Generic;
[System.Serializable]
public class Dialogue
{
    public Dialogue(string name, string sentence)
    {
        this.name = name;
        this.sentence = sentence;
    }
    public string name;
    public string sentence;
    public bool isPlayerPrompt = false;
}

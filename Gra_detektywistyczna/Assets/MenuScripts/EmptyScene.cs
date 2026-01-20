using DTOModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class EmptyScene : MonoBehaviour
{
    public TMP_Text myText;   // Podpinamy TextMeshPro w Inspectorze
    private VerdictResponseDTO _VerdictResponseDTO;

    // OnEnable jest wywoływane za każdym razem, gdy obiekt lub jego rodzic jest aktywny
    void OnEnable()
    {
        string speech = GameSession.IsWin ? "WYGRAŁEŚ!" : "PRZEGRAŁEŚ!";
        speech += $"\n\nWybrałeś: {GameSession.PendingVerdictNpcName}\n\n{GameSession.PendingVerdictText}";
        SetText(speech);
    }

    // Metoda do ustawiania tekstu w TMP
    public void SetText(string newText)
    {
        if (myText != null)
            myText.text = newText;
        else
            Debug.LogWarning("Nie przypisano TMP_Text!");
    }
}


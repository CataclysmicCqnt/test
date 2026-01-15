using UnityEngine;
using UnityEngine.UI;
using System.Threading;

[RequireComponent(typeof(Button))]
public class NewGameButtonSFX : MonoBehaviour
{
    public AudioClip clickSound;
    public AudioClip newGameSound;

    void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            AudioManager.Instance.PlaySFX(clickSound);
            AudioManager.Instance.PlayDelayed(newGameSound, 0.05f);
        });

    }
}
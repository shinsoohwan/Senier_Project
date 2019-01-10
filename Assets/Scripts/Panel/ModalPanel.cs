using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;

//  This script will be updated in Part 2 of this 2 part series.
public class ModalPanel : MonoBehaviour
{

    public Button ResumeButton;
    public Button RestartButton;
    public Button QuitButton;
    public GameObject modalPanelObject;

    private static ModalPanel modalPanel;

    public static ModalPanel Instance()
    {
        if (!modalPanel)
        {
            modalPanel = FindObjectOfType(typeof(ModalPanel)) as ModalPanel;
            if (!modalPanel)
                Debug.LogError("There needs to be one active ModalPanel script on a GameObject in your scene.");
        }

        return modalPanel;
    }

    // Yes/No/Cancel: A string, a Yes event, a No event and Cancel event
    public void Choice(string question, UnityAction yesEvent, UnityAction noEvent, UnityAction cancelEvent)
    {
        modalPanelObject.SetActive(true);

        ResumeButton.onClick.RemoveAllListeners();
        ResumeButton.onClick.AddListener(yesEvent);
        ResumeButton.onClick.AddListener(ClosePanel);

        RestartButton.onClick.RemoveAllListeners();
        RestartButton.onClick.AddListener(noEvent);
        RestartButton.onClick.AddListener(ClosePanel);

        QuitButton.onClick.RemoveAllListeners();
        QuitButton.onClick.AddListener(cancelEvent);
        QuitButton.onClick.AddListener(ClosePanel);

        ResumeButton.gameObject.SetActive(true);
        RestartButton.gameObject.SetActive(true);
        QuitButton.gameObject.SetActive(true);
    }

    void ClosePanel()
    {
        modalPanelObject.SetActive(false);
    }
}
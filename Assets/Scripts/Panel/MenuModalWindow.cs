using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using UnityEngine.SceneManagement;

//  This script will be updated in Part 2 of this 2 part series.
public class MenuModalWindow : MonoBehaviour
{
    public string ThisScene;
    public string QuitScene;

    private ModalPanel modalPanel;
    private DisplayManager displayManager;

    private UnityAction myResumeAction;
    private UnityAction myRestartAction;
    private UnityAction myQuitAction;

    void Awake()
    {
        modalPanel = ModalPanel.Instance();
        displayManager = DisplayManager.Instance();

        myResumeAction = new UnityAction(TestResumeFunction);
        myRestartAction = new UnityAction(TestRestartFunction);
        myQuitAction = new UnityAction(TestQuitFunction);
    }

    //  Send to the Modal Panel to set up the Buttons and Functions to call
    public void TestRRQ()
    {
        modalPanel.Choice("", TestResumeFunction, TestRestartFunction, TestQuitFunction);
        
    }

    //  These are wrapped into UnityActions
    void TestResumeFunction()
    {
       
    }

    void TestRestartFunction()
    {
        SceneManager.LoadScene(ThisScene);
    }

    void TestQuitFunction()
    {
        SceneManager.LoadScene(QuitScene);
    }
}
using UnityEngine;
using System.Collections;

public class MenuManager : MonoBehaviour {
    public GameObject Arrow;
    public GameObject ArrowManager;

    public GameObject Button_State_Main;
    public GameObject Button_State_Options;
    public GameObject State_Sound;
    public GameObject State_Gameplay;
    public GameObject State_Controls;

    public GameObject Image_Up;
    public GameObject Image_Middle;
    public GameObject Image_Down;

    public GameObject ArrowMove;
    public bool MouseMove = true;

    public GameObject Logo;
    // Use this for initialization
    void Start () {
        MoveArrow();
       // MoveLogo();
    }
    //Move arrow backward with following conditions
    void MoveArrow() {
        iTween.MoveTo(Arrow, iTween.Hash(
           "position", new Vector3(Arrow.transform.localPosition.x + 20, Arrow.transform.localPosition.y, Arrow.transform.localPosition.z),
           "time", .5f,
           "delay", 0f,
           "islocal", true,
           "easetype", iTween.EaseType.linear,
           "loopType", "pingPong"));
    }

    void MoveLogo() {
        iTween.ScaleFrom(Logo, iTween.Hash(
           "scale", new Vector3(Logo.transform.localScale.x - 0.04f, Logo.transform.localScale.y - 0.04f, Logo.transform.localScale.z),
           "time", 1f,
           "delay", 0f,
           "islocal", true,
           "easetype", iTween.EaseType.linear,
           "loopType", "pingPong"));
    }

    // Update is called once per frame
    void Update() {
        GoingBack();
    }
    //Position when the Arrow is up
    public void ArrowUpPos() {
        ArrowManager.transform.position = Image_Up.transform.position;
    }
        //Position when the Arrow is in the midle
    public void ArrowMiddlePos() {
        ArrowManager.transform.position = Image_Middle.transform.position;
    }
    //Position when the Arrow is down
    public void ArrowDownPos() {
        ArrowManager.transform.position = Image_Down.transform.position;
    }

    //Load world
    public void LoadGame() {
        Application.LoadLevel("Game_Wereld");
    }

    //Load Main Menu
    public void LoadMainMenu() {
        Application.LoadLevel("Game_Main_Menu");
    }
    //Make graphics worse
    public void GraphicLow() {
        QualitySettings.DecreaseLevel(false);
    }
    //Makes graphics better
    public void GraphicHigh() {
        QualitySettings.IncreaseLevel(false);
    }
    //Open option menu
    public void OptionMenu() {
        Button_State_Main.active = false;
        Button_State_Options.active = true;
    }
    //Open SoundMenu
    public void SoundMenu() {
        Button_State_Options.active = false;
        State_Sound.active = true;
        ArrowMove.active = false;
    }
    //Open Controls
    public void ControlMenu() {
        Button_State_Options.active = false;
        State_Controls.active = true;
        ArrowMove.active = false;
    }
    //Open Gameplay
    public void GameplayMenu() {
        Button_State_Options.active = false;
        State_Gameplay.active = true;
        ArrowMove.active = false;
    }
    //Quit game
    public void ExitGame() {
        Application.Quit();
    }
    //Go back from 1 menu state to another
    void GoingBack() {
        //From option to main
        if (Input.GetKeyDown("escape") && Button_State_Options.active == true) {
            Button_State_Options.active = false;
            Button_State_Main.active = true;
        }
        //From gameplay to options
        if (Input.GetKeyDown("escape") && State_Gameplay.active == true) {
            State_Gameplay.active = false;
            Button_State_Options.active = true;
            ArrowMove.active = true;
        }
        //From controls to options
        if (Input.GetKeyDown("escape") && State_Controls.active == true) {
            State_Controls.active = false;
            Button_State_Options.active = true;
            ArrowMove.active = true;
        }
        //From sound to options
        if (Input.GetKeyDown("escape") && State_Sound.active == true) {
            State_Sound.active = false;
            Button_State_Options.active = true;
            ArrowMove.active = true;
        }
    }
}
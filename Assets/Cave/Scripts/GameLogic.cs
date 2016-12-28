using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameLogic : MonoBehaviour
{
    private enum State
    {
        Intro,
        Started,
        Inside,
        Puzzle,
        Success,
        End
    }

    private State _state = State.Intro;

    public GameObject player;
    public GameObject startUI, playUI, restartUI;
    public GameObject startPoint, playPoint, restartPoint;

    public bool playerWon = false;



    // Use this for initialization
    void Start()
    {
        UpdateUI();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && player.transform.position == playPoint.transform.position)
        {
            //puzzleSuccess();
        }
    }

    public void startGame()
    {
        _state = State.Started;
        UpdateUI();
    }

    public void startPuzzle()
    {
        _state = State.Puzzle;
        UpdateUI();
    }

    public void puzzleSuccess()
    {
        _state = State.Success;
        UpdateUI();
    }


    public void enterCave()
    {
        iTween.MoveTo(player,
            iTween.Hash(
                "position", playPoint.transform.position,
                "time", 2,
                "easetype", "linear",
                "oncompletetarget", gameObject,
                "oncomplete", "_onEnterCave"
            )
        );
    }

    private void _onEnterCave()
    {
        _state = State.Inside;
        UpdateUI();
    }

    public void resetPuzzle()
    { //Reset the puzzle sequence
      //        _state = State.Intro;
      // player.transform.position = startPoint.transform.position;
      // UpdateUI();
        SceneManager.LoadScene(0);
    }

    public void leaveCave()
    { 
        iTween.MoveTo(player,
            iTween.Hash(
                "position", restartPoint.transform.position,
                "time", 2,
                "easetype", "linear",
                "oncompletetarget", gameObject,
                "oncomplete", "_onLeaveCave"
            )
        );
    }

    private void _onLeaveCave()
    {
        _state = State.End;
        UpdateUI();
    }

    private void UpdateUI()
    {
        bool start = false;
        bool play = false;
        bool end = false;

        switch (_state)
        {
            case State.Intro:
                start = true;
                break;

            case State.Inside:
                play = true;
                break;

            case State.End:
                end = true;
                break;
        }

        startUI.SetActive(start);
        playUI.SetActive(play);
        restartUI.SetActive(end);
    }
}
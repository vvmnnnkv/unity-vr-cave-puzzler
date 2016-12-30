using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Linq;

public class GameLogic : MonoBehaviour
{
    // game states
    private enum State
    {
        Intro, // just loaded the game, seeing UI panel with intro test
        Started, // closed intro popup
        Inside, // entered the cave
        PuzzleDisplay, // started puzzle
        PuzzleInput, // puzzle awaits user input
        Success, // puzzle is solved
        End // exited the cave
    }

    private State _state = State.Intro;

    public GameObject player;
    public GameObject startUI, playUI, restartUI;
    public GameObject startPoint, playPoint, restartPoint;
    public bool playerWon = false;

    // An array to hold our puzzle crystals
    public GameObject[] puzzleCrystals;

    //How many times we light up.  This is the difficulty factor.  The longer it is the more you have to memorize in-game.
    //For now let's have 5 orbs
    public int puzzleLength = 5;

    //How many seconds between puzzle display pulses
    public float puzzleSpeed = 1f;

    private int[] puzzleOrder;

    //Temporary variable for storing the index when displaying the pattern
    private int currentDisplayIndex = 0;

    public bool currentlyDisplayingPattern = true;

    //Temporary variable for storing the index that the player is solving for in the pattern.
    private int currentSolveIndex = 0;

    // Array of sounds produced by crystals
    public AudioClip[] crystalChimes;

    // Invalid answer sound
    public AudioClip failureChimes;

    // Puzzle solved sound
    public GvrAudioSource winSound;

    // Stone opening animator
    public Animator stoneAnimator;

    // Use this for initialization
    void Start()
    {
        _setState(State.Intro);
        puzzleOrder = new int[puzzleLength]; //Set the size of our array to the declared puzzle length
    }

    public void playerSelection(GameObject item)
    {
        if (playerWon != true)
        {
            //If the player hasn't won yet
            int selectedIndex = 0;
            //Get the index of the selected object
            for (int i = 0; i < puzzleCrystals.Length; i++)
            {
                //Go through the puzzleCrystals array
                if (puzzleCrystals[i] == item)
                {
                    //If the object we have matches this index, we're good
                    //Debug.Log("Looks like we hit sphere: " + i);
                    selectedIndex = i;
                }
            }
            // Check if selection was correct
            if (solutionCheck(selectedIndex))
            {
                // if correct, light up item
                item.GetComponent<lightUp>().patternLightUp(puzzleSpeed);
            }
            else
            {
                // if not, set invalid sound and darken item
                item.GetComponent<GvrAudioSource>().clip = failureChimes;
                item.GetComponent<lightUp>().patternLightDown(puzzleSpeed);
            }
        }
    }

    //We check whether or not the passed index matches the solution index
    public bool solutionCheck(int playerSelectionIndex)
    {
        bool Result = false;
        if (playerSelectionIndex == puzzleOrder[currentSolveIndex])
        {
            // Check if the index of the object the player passed is the same as the current solve index in our solution array
            Result = true;
            currentSolveIndex++;
            // Debug.Log("Correct!  You've solved " + currentSolveIndex + " out of " + puzzleLength);
            if (currentSolveIndex >= puzzleLength)
            {
                puzzleSuccess();
            }
        }
        else
        {
            puzzleFailure();
        }
        return Result;
    }

    // Begin the puzzle sequence
    public void startPuzzle()
    {
        _setState(State.PuzzleDisplay);
        //Generate the puzzle sequence for this playthrough.
        generatePuzzleSequence();
        CancelInvoke("displayPattern");
        // Start running through the displaypattern function
        InvokeRepeating("displayPattern", 2, puzzleSpeed);
        //Set our puzzle index at 0
        currentSolveIndex = 0;
    }

    //Invoked repeating
    void displayPattern()
    {
        currentlyDisplayingPattern = true; //Let us know were displaying the pattern
        if (currentlyDisplayingPattern == true)
        {
            // If we are not finished displaying the pattern
            if (currentDisplayIndex < puzzleOrder.Length)
            {
                //If we haven't reached the end of the puzzle
                //Debug.Log(puzzleOrder[currentDisplayIndex] + " at index: " + currentDisplayIndex);
                //Light up the item at the proper index.
                // For now we keep it lit up the same amount of time as our interval, but could adjust this to be less.
                puzzleCrystals[puzzleOrder[currentDisplayIndex]]
                    .GetComponent<lightUp>()
                    .patternLightUp(puzzleSpeed);
                currentDisplayIndex++; //Move one further up.
            }
            else
            {
                //Debug.Log("End of puzzle display");
                currentlyDisplayingPattern = false; //Let us know were done displaying the pattern
                currentDisplayIndex = 0;
                CancelInvoke(); //Stop the pattern display.  May be better to use coroutines for this but oh well
                //eventSystem.SetActive(true); //Enable gaze input when we aren't displaying the pattern.
                _setState(State.PuzzleInput);
            }
        }
    }

    public void generatePuzzleSequence()
    {
        // generate array of and shuffle it
        int[] itemsShuffled = Enumerable.Range(0, puzzleLength).OrderBy(x => Random.value).ToArray();
        for (int i = 0; i < puzzleLength; i++)
        {
            // set the puzzle index to our randomly generated reference number
            puzzleOrder[i] = itemsShuffled[i] % puzzleCrystals.Length;
        }

        // set sounds randomly, w/o repeats
        int[] notes = Enumerable.Range(0, crystalChimes.Length).OrderBy(x => Random.value).ToArray();
        for (int i = 0; i < puzzleCrystals.Length; i++)
        {
            // assign to crystals
            puzzleCrystals[i].GetComponent<GvrAudioSource>().clip = crystalChimes[notes[i] % crystalChimes.Length];
        }
    }

    // Function is called when puzzle is solved
    public void puzzleSuccess()
    {
        _setState(State.Success);
        winSound.Play();
        Invoke("_openCave", 2);
    }

    // Function is called when mistake is made
    public void puzzleFailure()
    {
        //Do this when the player gets it wrong
        //Debug.Log("You've Failed, Resetting puzzle");
        currentSolveIndex = 0;
        CancelInvoke("startPuzzle");
        Invoke("startPuzzle", 1);
    }


    // Update is called once per frame
    void Update()
    {
    }

    // Called from first UI panel
    public void startGame()
    {
        _setState(State.Started);
    }

    // Called when clicking on the cave entrance
    public void enterCave()
    {
        iTween.MoveTo(player,
            iTween.Hash(
                "position", playPoint.transform.position,
                "time", 1.5f,
                "easetype", "linear",
                "oncompletetarget", gameObject,
                "oncomplete", "_onEnterCave"
            )
        );
    }

    // Displays next UI panel
    private void _onEnterCave()
    {
        _setState(State.Inside);
    }

    // Called to open the cave (move stone)
    private void _openCave()
    {
        stoneAnimator.SetTrigger("MoveStone");
        stoneAnimator.GetComponent<GvrAudioSource>().Play();
    }

    // Restarts the game
    public void resetPuzzle()
    {
        SceneManager.LoadScene(0);
    }

    // Called when clicking on cave exit
    public void leaveCave()
    {
        iTween.MoveTo(player,
            iTween.Hash(
                "position", restartPoint.transform.position,
                "time", 1.5f,
                "easetype", "linear",
                "oncompletetarget", gameObject,
                "oncomplete", "_onLeaveCave"
            )
        );
    }

    // Shows restart UI panel
    private void _onLeaveCave()
    {
        _setState(State.End);
    }

    // Function that sets game state and sets game components states
    private void _setState(State newState)
    {
        _state = newState;
        _updateUI();
    }

    // Updates UI and elements depending on the state
    private void _updateUI()
    {
        bool start = false;
        bool play = false;
        bool end = false;
        _toggleCrystals(false);
        restartPoint.SetActive(false);
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
                playPoint.SetActive(false);
                break;

            case State.PuzzleInput:
                _toggleCrystals(true);
                break;

            case State.Success:
                restartPoint.SetActive(true);
                break;
        }

        startUI.SetActive(start);
        playUI.SetActive(play);
        restartUI.SetActive(end);
    }

    // Enables/disables crystals
    private void _toggleCrystals(bool enable)
    {
        for (int i = 0; i < puzzleCrystals.Length; i++)
        {
            puzzleCrystals[i].GetComponent<Collider>().enabled = enable;
        }
    }
}
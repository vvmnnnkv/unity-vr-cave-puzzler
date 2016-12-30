using UnityEngine;
using System.Collections;

public class lightUp : MonoBehaviour
{
    public GameObject gameLogic;

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    // The lightup behavior when displaying the pattern
    public void patternLightUp(float duration)
    {
        StartCoroutine(lightFor(duration));
    }

    //The lightup behavior when answer is invalid
    public void patternLightDown(float duration)
    {
        StartCoroutine(darkFor(duration));
    }

    public void playerSelection()
    {
        gameLogic.GetComponent<GameLogic>().playerSelection(this.gameObject);
        this.GetComponent<GvrAudioSource>().Play();
    }

    public void aestheticReset()
    {
        this.GetComponentInChildren<ParticleSystem>().enableEmission = false; //Turn off particle emission
    }

    // Lightup behavior when the pattern shows.
    public void patternLightUp()
    {
        //Turn on particle emmission
        this.GetComponentInChildren<ParticleSystem>().enableEmission = true;
        // Highlight
        gameObject.GetComponent<Blink>().Shine();
        //Play the audio attached
        this.GetComponent<GvrAudioSource>().Play();
    }

    // Behavior do darken crystal when answer is not correct
    public void patternLightDown()
    {
        gameObject.GetComponent<Blink>().Darken();
        //Play the audio attached
        this.GetComponent<GvrAudioSource>().Play();
    }

    IEnumerator lightFor(float duration)
    {
        // Light us up for a duration.  Used during the pattern display
        patternLightUp();
        yield return new WaitForSeconds(duration - .1f);
        aestheticReset();
    }

    IEnumerator darkFor(float duration)
    {
        // Light us up for a duration.  Used during the pattern display
        patternLightDown();
        yield return new WaitForSeconds(duration - .1f);
        aestheticReset();
    }
}
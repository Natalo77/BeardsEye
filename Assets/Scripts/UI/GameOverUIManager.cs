using System;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUIManager : MonoBehaviour
{   
    [HideInInspector] public static int plungersThrown;
    [HideInInspector] public static int leaksPlunged;
    [HideInInspector] public static int timesSlipped;
    [HideInInspector] public static int timesPumped;

    [SerializeField] [Tooltip("TimeSurvivedText from Game Over in Canvas goes here")] public GameObject TimeSurvived;
    [SerializeField] [Tooltip("PlungersThrownText from Game Over in Canvas goes here")] private GameObject PlungersThrown;
    [SerializeField] [Tooltip("LeaksPlungedText from Game Over in Canvas goes here")] private GameObject LeaksPlunged;
    [SerializeField] [Tooltip("TimesSlippedText from Game Over in Canvas goes here")] private GameObject TimesSlipped;
    [SerializeField] [Tooltip("TimesPumpedText from Game Over in Canvas goes here")] private GameObject TimesPumped;
    [SerializeField] [Tooltip("NameText from Game Over in Canvas goes here")] private GameObject PlayerName;

    [Space(5)]
    [SerializeField] [Tooltip("How many decimal places the seconds in 'Time Survived' should be rounded to")] private int survivedSecondsRounding;

    private bool alreadyAdded;

    void OnValidate()
    {
        if (survivedSecondsRounding < 0)
        {
            survivedSecondsRounding *= -1;
        }
    }

    void Start()
    {
        plungersThrown = 0;
        leaksPlunged = 0;
        timesSlipped = 0;
        timesPumped = 0;
        alreadyAdded = false;
    }

    public void newGame()
    {
        alreadyAdded = false;
        UIManager.changeScene("Main");
        UIManager.changeGameState(0);
    }

    public void mainMenu()
    {
        alreadyAdded = false;
        UIManager.changeScene("Menu");
        UIManager.changeGameState(0);
    }

    public void addToLeaderBoard()
    {
        if (!alreadyAdded)
        {
            alreadyAdded = true;

            string name = PlayerName.GetComponent<Text>().text;

            int seconds = (int)Time.timeSinceLevelLoad;
            int minutes = seconds / 60;
            seconds = seconds - (minutes * 60);

            Stats stats = new Stats(name, leaksPlunged, minutes, seconds, plungersThrown, timesSlipped, timesPumped);

            bool wasGoodEnough = SavingService.Instance.leaderBoard.isGoodEnough(stats);
            if (wasGoodEnough)
            {
                SavingService.Instance.WriteLeaderBoard();
                var a = PlayerName.GetComponent<Text>();
                a.enabled = false;
                a.text = "Added!";
                a.enabled = true;
            }
            else
            {
                var a = PlayerName.GetComponent<Text>();
                a.enabled = false;
                a.text = "Not good enough!";
                a.enabled = true;
            }
        }

    }

    public void populateGameOver()
    {
        Text timeSurvivedText = TimeSurvived.GetComponent<Text>();
        Text plungersThrownText = PlungersThrown.GetComponent<Text>();
        Text leaksPlungedText = LeaksPlunged.GetComponent<Text>();
        Text timesSlippedText = TimesSlipped.GetComponent<Text>();
        Text timesPumpedText = TimesPumped.GetComponent<Text>();

        var timeSurvived = TimeSpan.FromSeconds(Time.timeSinceLevelLoad);
        
        if (timeSurvived.Hours != 0)
        {
            if (timeSurvived.Hours == 1)
            {
                timeSurvivedText.text += timeSurvived.Hours + " hour";
            }
            else
            {
                timeSurvivedText.text += timeSurvived.Hours + " hours";
            }
        }
        if (timeSurvived.Minutes != 0)
        {
            if (timeSurvived.Hours != 0)
            {
                if (timeSurvived.TotalSeconds != 0)
                {
                    timeSurvivedText.text += ", ";
                }
                else
                {
                    timeSurvivedText.text += " & ";
                }
            }
            if (timeSurvived.Minutes == 1)
            {
                timeSurvivedText.text += timeSurvived.Minutes % 60 + " minute";
            }
            else
            {
                timeSurvivedText.text += timeSurvived.Minutes % 60 + " minutes";
            }
        }
        if (timeSurvived.TotalSeconds != 0)
        {
            if (timeSurvived.Minutes != 0)
            {
                timeSurvivedText.text += " & ";
            }
            if (timeSurvived.TotalSeconds == 1)
            {
                timeSurvivedText.text += Math.Round(timeSurvived.TotalSeconds % 60, survivedSecondsRounding) + " second";
            }
            else
            {
                timeSurvivedText.text += Math.Round(timeSurvived.TotalSeconds % 60, survivedSecondsRounding) + " seconds";
            }
        }

        plungersThrownText.text += plungersThrown;

        leaksPlungedText.text += leaksPlunged;

        timesSlippedText.text += timesSlipped;

        timesPumpedText.text += timesPumped;

        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(true);
        }

        
    }
}
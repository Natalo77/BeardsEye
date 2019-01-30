using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;

public class SavingService : MonoBehaviour
{
    private static SavingService _instance = null;
    public static SavingService Instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = FindObjectOfType<SavingService>();
                if(_instance == null)
                {
                    GameObject savingObject = new GameObject(typeof(SavingService).ToString());
                    _instance = savingObject.AddComponent<SavingService>();
                }
            }
            return _instance;
        }
    }

    public void OnEnable()
    {
        if(Instance != this)
        {
            Destroy(this);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnLevelFinishedLoading;
            if (leaderBoard == null)
            {
                LoadLeaderBoard();
                leaderBoard.sortList();
            }
        }
    }



    public LeaderBoard leaderBoard = null;

    public LeaderBoard GetLeaderBoard()
    {
        return this.leaderBoard;
    }
     
    public void SetLeaderBoard(LeaderBoard leaderBoard)
    {
        this.leaderBoard = leaderBoard;
    }

    public void LoadLeaderBoard()
    {
        if(File.Exists(GetFilePath()))
        {
            this.leaderBoard = LeaderBoard.ReadFromFile(GetFilePath());
        }
        else
        {
            this.leaderBoard = new LeaderBoard();
        }
    }

    public void WriteLeaderBoard()
    {
        if(leaderBoard == null)
        {
            leaderBoard = new LeaderBoard();
        }

        leaderBoard.WriteToFile(GetFilePath());
    }

    private const string LEADERBOARD_FILE_NAME_BASE = "leaderboard";
    private const string LEADERBOARD_FILE_EXT = ".json";
    private string LEADERBOARD_DIRECTORY { get { return Application.dataPath + "/leaderBoard/"; } }

    public string GetFilePath()
    {
        if(!Directory.Exists(LEADERBOARD_DIRECTORY))
        {
            Directory.CreateDirectory(LEADERBOARD_DIRECTORY);
        }
        return LEADERBOARD_DIRECTORY + LEADERBOARD_FILE_NAME_BASE + LEADERBOARD_FILE_EXT;
    }

    private void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode) { }



}

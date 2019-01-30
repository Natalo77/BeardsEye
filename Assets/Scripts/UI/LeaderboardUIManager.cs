using UnityEngine;
using UnityEngine.UI;

public class LeaderboardUIManager : MonoBehaviour
{
    public GameObject textsObject;
    private Text[] texts;

    public void menu()
    {
        UIManager.changeScene("Menu");
    }

    public void Start()
    {
        LeaderBoard leaderBoard = SavingService.Instance.GetLeaderBoard();

        leaderBoard.sortList();

        Stats[] statsList = leaderBoard.getStatsList().ToArray();

        texts = textsObject.GetComponentsInChildren<Text>();

        int y = 4;
        for(int x = 0; x < statsList.Length; x++)
        {
            texts[y].text = statsList[x].ToString();
            y--;
        }
    }
}
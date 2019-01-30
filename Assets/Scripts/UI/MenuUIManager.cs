using UnityEngine;

public class MenuUIManager : MonoBehaviour
{
	public void play()
    {
        Destroy(menuMusicStartupScript.audioSource.gameObject);
        UIManager.changeScene("Main");
        Time.timeScale = 1;
    }

    public void leaderboard()
    {
        UIManager.changeScene("Leaderboard");
    }

    public void options()
    {
        UIManager.changeScene("Options");
    }

    public void exit()
    {
        Application.Quit();
    }
}
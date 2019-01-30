using UnityEngine;

public class PauseUIManager : MonoBehaviour
{
    public void pause()
    {
        UIManager.changeGameState(1);
    }

    public void resume()
    {
        UIManager.changeGameState(0);
    }

    public void restart()
    {
        UIManager.changeScene("Main");
        UIManager.changeGameState(0);
    }

    public void menu()
    {
        UIManager.changeScene("Menu");
        UIManager.changeGameState(0);
    }
}
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [SerializeField] [Tooltip("PauseButton in HUD on Canvas goes here")] private GameObject pauseButton;
    [SerializeField] [Tooltip("PausePanel on Canvas goes here")] private GameObject pausePanel;
    [SerializeField] [Tooltip("GameOverPanel on Canvas goes here")] private GameObject gameOverPanel;

    private static GameObject pauseButtonStatic;
    private static GameObject pausePanelStatic;
    private static GameObject gameOverPanelStatic;

    private static AudioSource audioSource;

    public enum GameState {PLAY, PAUSE, GAMEOVER};
    public static GameState gameState = GameState.PLAY;

    void Start()
    {
        pauseButtonStatic = pauseButton;
        pausePanelStatic = pausePanel;
        gameOverPanelStatic = gameOverPanel;

        audioSource = GetComponent<AudioSource>();
    }

    public static void changeGameState(int stateInt)
    {
        gameState = (GameState)stateInt;

        switch (gameState)
        {
            case GameState.PLAY:
                Time.timeScale = 1;
                pauseButtonStatic.SetActive(true);
                pausePanelStatic.SetActive(false);
                gameOverPanelStatic.SetActive(false);
                audioSource.Play();
                break;
            case GameState.PAUSE:
                Time.timeScale = 0;
                pauseButtonStatic.SetActive(false);
                pausePanelStatic.SetActive(true);
                gameOverPanelStatic.SetActive(false);
                audioSource.Pause();
                break;
            case GameState.GAMEOVER:
                Time.timeScale = 0;
                pauseButtonStatic.SetActive(false);
                pausePanelStatic.SetActive(false);
                gameOverPanelStatic.SetActive(true);
                audioSource.Stop();
                break;
        }
    }

    public static int getGameState()
    {
        return (int)gameState;
    }

    public static void changeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
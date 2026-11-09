using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Scene Setup")]
    [SerializeField] private bool isMainMenuScene = false;
    [SerializeField] private string mainMenuSceneName = "MainMenu";
    [SerializeField] private string gameSceneName = "GameScene";


    [Header("Canisters")]
    [SerializeField] private int totalCanisters = 5;
    private int collectedCanisters = 0;

    [Header("UI")]
    [SerializeField] private TMP_Text canisterText;
    [SerializeField] private GameObject winScreen;
    [SerializeField] private GameObject startScreen;
    [SerializeField] private GameObject loseScreen;

    [Header("High Score")]
    [SerializeField] private TMP_Text gameHighScoreText;
    [SerializeField] private TMP_Text menuHighScoreText;
    [SerializeField] private GameObject highScorePanel;

    private const string BestTimeKey = "BestTime";

    private float elapsedTime = 0f;

    private bool gameOver = false;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Time.timeScale = 1f;

        UpdateHighScoreUI();

        if (isMainMenuScene )
        {
            return;
        }

        if (winScreen != null ) winScreen.SetActive(false);
        if (loseScreen != null ) loseScreen.SetActive(false);

        UpdateCanisterUI();
    }

    private void Update()
    {
        if (isMainMenuScene || gameOver)
        {
            return ;
        }

        elapsedTime += Time.deltaTime; 
    }


    public void PlayGame()
    {
        Time.timeScale = 1f; 
        SceneManager.LoadScene(gameSceneName);
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void QuiGame()
    {
        Debug.Log("Quit Game");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;


#else
        Application.Quit();
#endif


    }

    public void CanisterCollected()
    {
        if (gameOver)
            return;

        collectedCanisters++;

        UpdateCanisterUI();

        if (collectedCanisters >= totalCanisters)
        {
            WinGame();
        }
    }



    private void UpdateCanisterUI()
    {
        canisterText.text = "Canisters: " + collectedCanisters + " / " + totalCanisters;
    }

    public void StartGame()
    {
        startScreen.SetActive (false);

        Time.timeScale = 1f;

        Debug.Log("Game Started");
    }

    private void WinGame()
    {
        gameOver = true;

        Debug.Log("You Win!");

        SaveHighScoreIfBest(); 

        winScreen.SetActive(true);

        Time.timeScale = 0f;
    }

    public void PlayerDied()
    {
        if (gameOver)
            return ;

        gameOver = true;

        Debug.Log("You Lose!");

        loseScreen.SetActive(true);

        Time .timeScale = 0f;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene( SceneManager.GetActiveScene().buildIndex );
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene ( "MainMenu" );
    }

    public void ShowHighScorePanel()
    {
        if (highScorePanel != null) highScorePanel.SetActive(true);
    }

    public void HideHighScorePanel()
    {
        if (highScorePanel != null) highScorePanel.SetActive(false);
    }

    public void ResetHighScore()
    {
        PlayerPrefs.DeleteKey(BestTimeKey);
        PlayerPrefs.Save();
        UpdateCanisterUI();
    }

    private void SaveHighScoreIfBest()
    {
        bool hasRecord = PlayerPrefs.HasKey(BestTimeKey);

        if(!hasRecord || elapsedTime < PlayerPrefs.GetFloat(BestTimeKey))
        {
            PlayerPrefs.SetFloat(BestTimeKey, elapsedTime);
            PlayerPrefs.Save();
            Debug.Log("New best time: " + FormatTime(elapsedTime));
        }

        UpdateCanisterUI();
    }

    private void UpdateHighScoreUI()
    {
        string text = PlayerPrefs.HasKey(BestTimeKey)
            ? "Best Time: " + FormatTime(PlayerPrefs.GetFloat(BestTimeKey))
            : "Best Time: --:--"; 

        if (gameHighScoreText != null) gameHighScoreText.text = text;
        if (menuHighScoreText != null) menuHighScoreText.text = text;
    }

    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        float seconds = time % 60f;
        return minutes.ToString("00") + ":" + seconds.ToString("00.00"); 
    }
}
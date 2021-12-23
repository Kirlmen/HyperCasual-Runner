using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class LevelController : MonoBehaviour
{

    public static LevelController Current;
    public bool isGameActive = false;
    public float maxDistance;
    public DailyReward dailyReward;


    int currentLevel;
    int score;

    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip gameOverClip;
    [SerializeField] AudioClip finishGameClip;
    [SerializeField] GameObject finishLine;
    [SerializeField] Slider levelProgressBar;
    [SerializeField] public GameObject startMenu, gameMenu, gameOverMenu, finishMenu;
    [SerializeField] TMP_Text scoreText, finishScoreTxt, currentLvlTxt, nextLvlTxt, startMenuGoldTxt, gameOverGoldTxt, finishMenuGoldText;




    // Start is called before the first frame update
    void Start()
    {
        currentLevel = PlayerPrefs.GetInt("currentLevel");
        Current = this;
        PlayerController.CurrentPlayerController = GameObject.FindObjectOfType<PlayerController>();
        GameObject.FindObjectOfType<MarketController>().InitializeMarketController();
        dailyReward.InitilalizeDailyReward();
        currentLvlTxt.text = (currentLevel + 1).ToString();
        nextLvlTxt.text = (currentLevel + 2).ToString();
        UpdateGoldText();
        audioSource = Camera.main.GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (isGameActive) //slider güncellemesi.
        {
            PlayerController player = PlayerController.CurrentPlayerController;
            float distance = finishLine.transform.position.z - PlayerController.CurrentPlayerController.transform.position.z; //player-finish aradaki mesafe
            levelProgressBar.value = 1 - (distance / maxDistance);
        }
    }


    public void StartLevel()
    {
        maxDistance = finishLine.transform.position.z - PlayerController.CurrentPlayerController.transform.position.z;
        PlayerController.CurrentPlayerController.ChangeSpeed(PlayerController.CurrentPlayerController.runnigSpeed);
        startMenu.SetActive(false);
        gameMenu.SetActive(true);
        isGameActive = true;
        PlayerController.CurrentPlayerController.animator.SetBool("running", true);
    }

    public void RestartLevel()
    {
        LevelLoader.Current.ChangeLevel(SceneManager.GetActiveScene().name);
    }

    public void LoadNextLevel()
    {
        LevelLoader.Current.ChangeLevel("Level " + (currentLevel + 1));
    }

    public void GameOver()
    {
        UpdateGoldText();
        audioSource.Stop();
        audioSource.PlayOneShot(gameOverClip);
        gameMenu.SetActive(false);
        gameOverMenu.SetActive(true);
        isGameActive = false;
    }

    public void FinishGame()
    {
        GiveGoldToPlayer(score);
        audioSource.Stop();
        audioSource.PlayOneShot(finishGameClip);
        PlayerPrefs.SetInt("currentLevel", currentLevel + 1);
        finishScoreTxt.text = score.ToString();
        gameMenu.SetActive(false);
        finishMenu.SetActive(true);
        isGameActive = false;
    }

    public void ChangeScore(int incrase)
    {
        score += incrase;
        scoreText.text = score.ToString();

    }

    public void UpdateGoldText()
    {
        int gold = PlayerPrefs.GetInt("gold");
        startMenuGoldTxt.text = gold.ToString();
        finishMenuGoldText.text = gold.ToString();
        gameOverGoldTxt.text = gold.ToString();
    }

    public void GiveGoldToPlayer(int increment)
    {
        int gold = PlayerPrefs.GetInt("gold");
        gold = Mathf.Max(0, gold + increment);
        PlayerPrefs.SetInt("gold", gold);
        UpdateGoldText();
    }


}

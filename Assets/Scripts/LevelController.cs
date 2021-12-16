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

    int currentLevel;
    int score;


    [SerializeField] GameObject finishLine;
    [SerializeField] Slider levelProgressBar;
    [SerializeField] GameObject startMenu, gameMenu, gameOverMenu, finishMenu;
    [SerializeField] TMP_Text scoreText, finishScoreTxt, currentLvlTxt, nextLvlTxt;


    // Start is called before the first frame update
    void Start()
    {
        currentLevel = PlayerPrefs.GetInt("currentLevel");
        Current = this;
        if (SceneManager.GetActiveScene().name != "Level " + currentLevel) //player prefsten o anki level neyse onu çekecek.
        {
            SceneManager.LoadScene("Level " + currentLevel);
        }
        else
        {
            currentLvlTxt.text = (currentLevel + 1).ToString();
            nextLvlTxt.text = (currentLevel + 2).ToString();
        }
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
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void LoadNextLevel()
    {
        SceneManager.LoadScene("Level " + (currentLevel + 1));
    }

    public void GameOver()
    {
        gameMenu.SetActive(false);
        gameOverMenu.SetActive(true);
        isGameActive = false;
    }

    public void FinishGame()
    {
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


}

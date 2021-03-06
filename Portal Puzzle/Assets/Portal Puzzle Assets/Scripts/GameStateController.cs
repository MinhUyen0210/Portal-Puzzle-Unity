﻿using UnityEngine;
using UnityEngine.UI;
using MadLevelManager;
using System.Collections;

public class GameStateController : MonoBehaviour {

    public static GameStateController Instance;

    public static gameState currentState;

    public GameObject gameOverPanel;
    public GameObject pauseGamePanel;
    
    public Text scoreText;
    public Text bestScoreText;

    private DestroyChainedUnit destroyUnit;
    private GeneratingPuzzle puzzleGen;

    private RectTransform gameOverPanelRectTransform;
    private RectTransform pauseGamePanelRectTransform;
    private Vector2 originalScale = new Vector2(1, 1);
    private Vector2 zeroScale = new Vector2(0, 0);

    public enum gameState
    {
        idle = 0,
        movingUnit,
        destroyingUnit,
        regeneratingUnit,
        _statesCount
    }

    //public delegate void gameStateHandler(GameStateController.gameState newState);
    //public static event gameStateHandler onStateChange;

    public void Awake()
    {
        Instance = this;
        currentState = gameState.idle;

        destroyUnit = DestroyChainedUnit.Instance;
        puzzleGen = GeneratingPuzzle.Instance;

        gameOverPanelRectTransform = gameOverPanel.GetComponent<RectTransform>();
        pauseGamePanelRectTransform = pauseGamePanel.GetComponent<RectTransform>();
    }

    public IEnumerator endGame()
    {
        yield return new WaitForSeconds(0.2f);
        // Show Game Over panel
        //gameOverPanel.SetActive(true);
        //iTween.Stop(gameObject);
        iTween.ValueTo(gameObject, iTween.Hash("from", zeroScale, "to", originalScale, "time", 0.2f, "ignoretimescale", true, "onUpdate", "scaleGameOverPanel"));
        Time.timeScale = 0.0f;

        // Get current Level best score
        int bestScore = MadLevelProfile.GetLevelInteger(MadLevel.currentLevelName, "best score");

        // If Player's score is greater than best score,set new best score
        if (destroyUnit._score > bestScore)
        {
            MadLevelProfile.SetLevelInteger(MadLevel.currentLevelName, "best score", destroyUnit._score);
            bestScore = destroyUnit._score;
        }

        // Show player Score and best score
        scoreText.text = destroyUnit._score.ToString();
        bestScoreText.text = bestScore.ToString();

        // Set current level Star rating based on Player's Score
        if (destroyUnit._score >= puzzleGen._3starPoint)
        {
            MadLevelProfile.SetLevelBoolean(MadLevel.currentLevelName, "star_3", true);
        }
        if (destroyUnit._score >= puzzleGen._2starPoint)
        {
            MadLevelProfile.SetLevelBoolean(MadLevel.currentLevelName, "star_2", true);
        }
        if (destroyUnit._score >= puzzleGen._1starPoint)
        {
            MadLevelProfile.SetLevelBoolean(MadLevel.currentLevelName, "star_1", true);
            MadLevelProfile.SetCompleted(MadLevel.currentLevelName, true);
        }
    }

    public void restartGame()
    {
        MadLevel.LoadLevelByName(MadLevel.currentLevelName);
        Time.timeScale = 1.0f;
    }

    public void pauseGame()
    {
        iTween.Stop(gameObject);
        iTween.ValueTo(gameObject, iTween.Hash("from", zeroScale, "to", originalScale, "time", 0.2f, "ignoretimescale", true, "onUpdate", "scalePauseGamePanel"));

        //pauseGamePanel.SetActive(true);
        Time.timeScale = 0.0f;        
    }

    public void resumeGame()
    {
        iTween.Stop(gameObject);
        iTween.ValueTo(gameObject, iTween.Hash("from", originalScale, "to", zeroScale, "time", 0.2f, "ignoretimescale", true, "onUpdate", "scalePauseGamePanel"));

        //pauseGamePanel.SetActive(false);
        Time.timeScale = 1.0f;
    }

    public void selectLevel()
    {
        MadLevel.LoadLevelByName("Level Select");
    }

    public void quitGame()
    {
        MadLevel.LoadLevelByName("Main Menu");
        Time.timeScale = 1.0f;
    }

    // Scaling PauseGamePanel display
    void scalePauseGamePanel(Vector2 newSize)
    {
        pauseGamePanelRectTransform.localScale = newSize;
    }
    // Scaling GameOverPanel display
    void scaleGameOverPanel(Vector2 newSize)
    {
        gameOverPanelRectTransform.localScale = newSize;
    }
}

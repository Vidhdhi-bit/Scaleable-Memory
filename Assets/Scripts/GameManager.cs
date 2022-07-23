using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    int diceNumber;
    public int activePlayer = 1;
    public Dice dice;


    //Question Bank Variables
    public List<string> questionsListA;
    public List<string> questionsListB;
    public List<string> questionsListC;
    public List<string> currentQuestionBank;
    public GameObject menuPanel;
    public GameObject questionPanel;
    public TextMeshProUGUI questionText;
    public TextAsset questionBank;
    //private string[] allLines;
    //private string wholeText;
    public int buttonPressed;
    public GameObject BoardRefernce;
    public GameObject Board;
    public GameObject MenuButton;
    public GameObject InfoPanel;
    [System.Serializable]
    public class Player
    {
        public string playerName;
        public PlayerController playerController;
        public GameObject rollDiceButton;

        public enum PlayerTypes
        {
            CPU,
            HUMAN
        }
        public PlayerTypes playerType;
    }

    public List<Player> playerList = new List<Player>();

    public enum States
    {
        WAITING,
        ROLL_DICE,
        SWITCH_PLAYER
    }
    public States state;




    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        DeactivateAllButtons();
        ReadCSV();
    }

    private void Update()
    {
        // CPU player turn cycle
        if (playerList[activePlayer].playerType == Player.PlayerTypes.CPU)
        {
            switch (state)
            {
                case States.WAITING:
                    {
                        //IDLE
                    }
                    break;

                case States.ROLL_DICE:
                    {
                        //Roll dice actions
                        StartCoroutine(RollDiceDelay());
                        state = States.WAITING;
                    }
                    break;

                case States.SWITCH_PLAYER:
                    {
                        //Switch players actions
                        activePlayer++;
                        activePlayer %= playerList.Count;
                        Debug.Log("PLayer switch CPU");
                        InfoText.instance.ShowMessage("It's " + playerList[activePlayer].playerName + "'s turn.");
                        state = States.ROLL_DICE;
                    }
                    break;
            }
        }

        //Human turn cycle
        if (playerList[activePlayer].playerType == Player.PlayerTypes.HUMAN)
        {
            switch (state)
            {
                case States.WAITING:
                    {
                        //IDLE
                    }
                    break;

                case States.ROLL_DICE:
                    {
                        StartCoroutine(MissATurnCheck());
                        //Roll dice actions
                        ActivateSpecificButton(true);
                        state = States.WAITING;
                    }
                    break;

                case States.SWITCH_PLAYER:
                    {
                        //Switch players actions
                        activePlayer++;
                        activePlayer %= playerList.Count;
                        Debug.Log("PLayer switch Human");
                        InfoText.instance.ShowMessage("It's " + playerList[activePlayer].playerName + "'s turn.");
                        state = States.ROLL_DICE;
                    }
                    break;
            }
        }

        //Check for restart
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            RestartGame();
        }

    }

    IEnumerator RollDiceDelay()
    {
        yield return new WaitForSeconds(0.2f);

        // Roll the physical dice
        dice.RollDice();

    }

    public void RolledDiceNumber(int _diceNumber) // Called from the dice
    {
        diceNumber = _diceNumber;
        //Info box message
        InfoText.instance.ShowMessage(playerList[activePlayer].playerName + " has rolled a " + diceNumber);


        //Take turn
        playerList[activePlayer].playerController.TakeTurn(diceNumber);

    }

    //void ActivateButton(bool on)
    //{
    //    rollDiceButton.SetActive(on);
    //}

    //This function is on the button
    public void HumanRollDice()
    {
        ActivateSpecificButton(false);
        StartCoroutine(RollDiceDelay());
    }

    void ActivateSpecificButton(bool on)
    {
        playerList[activePlayer].rollDiceButton.SetActive(on);
    }

    void DeactivateAllButtons()
    {
        for (int i = 0; i < playerList.Count; i++)
        {
            playerList[i].rollDiceButton.SetActive(false);
        }
    }

    public void ReportWinner()
    {
        // Show winning screen
        //Debug.Log(playerList[activePlayer].playerName + " has won the game");
        InfoText.instance.ShowMessage("Well done to " + playerList[activePlayer].playerName + " for reaching the end of the ride!");

    }

    public void AskQuestion()
    {
        int listLength = currentQuestionBank.Count;
        int questionNumber = Random.Range(1, listLength);
        Debug.Log("List Length is: " + listLength + " & question number is: " + questionNumber);
        Debug.Log(currentQuestionBank[questionNumber]);
        questionPanel.SetActive(true);
        questionText.text = currentQuestionBank[questionNumber];
        currentQuestionBank.Remove(currentQuestionBank[questionNumber]);
        //questionShown = true;
        if (currentQuestionBank.Count == 1)
        {
            currentQuestionBank.Remove(currentQuestionBank[0]);
            Debug.Log("Reloading question bank");
            ResetQuestions();

        }
    }

    IEnumerator MissATurnCheck()
    {
        if (playerList[activePlayer].playerController.missATurn)
        {
            InfoText.instance.ShowMessage("Player " + GameManager.instance.playerList[GameManager.instance.activePlayer].playerName + " Misses a Turn");
            yield return new WaitForSeconds(0.8f); // Pause for effect
            playerList[activePlayer].playerController.missATurn = false;
            ActivateSpecificButton(false);
            state = States.SWITCH_PLAYER;
        }
    }

    public void QuestionButtonContinue()
    {
        questionPanel.SetActive(false);
        playerList[activePlayer].playerController.EndTurnAfterQuestion();
    }

    void ReadCSV()
    {
        string[] data = questionBank.text.Split('\n');

        for (int i = 0; i < data.Length - 1; i++)
        {
            // Split CSF line and add question text to question list.
            string[] lineSplit = data[i].Split(',');
            questionsListA.Add(lineSplit[0]);
            questionsListB.Add(lineSplit[1]);
            questionsListC.Add(lineSplit[2]);

        }

    }

    public void MenuButtonAPressed()
    {
        BoardRefernce.SetActive(true);
        Board.SetActive(true);
        MenuButton.SetActive(true);
        InfoPanel.SetActive(true);
        Debug.Log("Button A Pressed");
        currentQuestionBank = questionsListA;
        buttonPressed = 1;
        activePlayer = 1;
        InfoText.instance.ShowMessage("It's " + playerList[activePlayer].playerName + "'s turn.");
        state = States.ROLL_DICE;
        menuPanel.SetActive(false);

    }

    public void MenuButtonBPressed()
    {
        BoardRefernce.SetActive(true);
        Board.SetActive(true);
        MenuButton.SetActive(true);
        InfoPanel.SetActive(true);
        Debug.Log("Button B Pressed");
        currentQuestionBank = questionsListB;
        buttonPressed = 2;
        activePlayer = 1;
        InfoText.instance.ShowMessage("It's " + playerList[activePlayer].playerName + "'s turn.");
        state = States.ROLL_DICE;
        menuPanel.SetActive(false);
    }

    public void MenuButtonCPressed()
    {
        BoardRefernce.SetActive(true);
        Board.SetActive(true);
        MenuButton.SetActive(true);
        InfoPanel.SetActive(true);
        Debug.Log("Button C Pressed");
        currentQuestionBank = questionsListC;
        buttonPressed = 3;
        activePlayer = 1;
        InfoText.instance.ShowMessage("It's " + playerList[activePlayer].playerName + "'s turn.");
        state = States.ROLL_DICE;
        menuPanel.SetActive(false);
    }

    public void ResetQuestions()
    {
        ReadCSV();
    }
    public void MenuButtonClick()
    {
        SceneManager.LoadScene("BoardGameV3");
    }
    public void RestartGame()
    {
        SceneManager.LoadScene("BoardGameV3");
    }

}


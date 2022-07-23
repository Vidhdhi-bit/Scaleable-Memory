using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerController : MonoBehaviour
{
    int routePosition;
    int playerID;
    public BoardRoute currentRoute;
    public int stepsToMove;
    int doneSteps;
    public List<SquareNode> squareList = new List<SquareNode>(); //New list of  squares

    public GameObject QuestionPanel;
    public TextMeshProUGUI questionText;
    public Button questionButton; // do I need this?
    public bool missATurn = false;
    public AudioSource moveSound;
    public AudioSource ladderSound;
    public AudioSource slideSound;
    public ParticleSystem ps;

    bool isMoving = false;  // protect inputs etc whilst moving
    float speed = 7f;
    float cTime = 0;
    float amplitude = 0.3f;


    private void Start()
    {

        foreach (Transform c in currentRoute.childSquareList)
        {
            SquareNode s = c.GetComponentInChildren<SquareNode>();
            if (s != null)
            {
                squareList.Add(s);
            }
        }
    }
   
    IEnumerator Move()
    {
        if (isMoving)
        {
            yield break;
        }
        isMoving = true;

        //Check to see if the player is missing a turn
        //if (missATurn)
        //{
        //    InfoText.instance.ShowMessage("Player " + GameManager.instance.playerList[GameManager.instance.activePlayer].playerName + " Misses a Turn");
        //    yield return new WaitForSeconds(0.8f); // Pause for effect
        //    GameManager.instance.state = GameManager.States.SWITCH_PLAYER;
        //    isMoving = false;
        //    missATurn = false;
        //    yield break;
        //}

        //Remove this player from node
        squareList[routePosition].RemovePlayer(this);

        while (stepsToMove > 0)
        {
            routePosition++;
            Vector3 nextPos = squareList[routePosition].transform.position;

            // Arc Movement
            Vector3 startPos = squareList[routePosition - 1].transform.position;
            while (MoveInArcToNextSquare(startPos, nextPos, 4f)) { yield return null; }

            // Straight Movement
            //while (MoveToNextSquare(nextPos)) { yield return null; }
            moveSound.volume = 0.3f;
            moveSound.Play();
           
            yield return new WaitForSeconds(0.2f); //brief pause between each square move
            cTime = 0;
            
            stepsToMove--;
            doneSteps++;
        }

        yield return new WaitForSeconds(0.2f);
        // Slide and Ladder checks
        if (squareList[routePosition].connectedNode != null)
        {
            int conSquareId = squareList[routePosition].connectedNode.squareNumber;
            Vector3 nextPos = squareList[routePosition].connectedNode.transform.position;
            if (squareList[routePosition].connectedNode.squareNumber > squareList[routePosition].squareNumber)
            {
                slideSound.volume = 0.3f;
                slideSound.Play();
                
            }
            else
            {
                ladderSound.volume = 0.3f;
                ladderSound.Play();
            }
            yield return new WaitForSeconds(0.3f);

            // Straight Movemen t
            while (MoveToNextSquare(nextPos)) { yield return null; }

            doneSteps = conSquareId;
            routePosition = conSquareId;
        }
        // Check for a WIN
        if (doneSteps == squareList.Count - 1)
        {
            ps.gameObject.SetActive(true);
            ps.transform.position = this.transform.position;
            ps.Play();
            // Report to gamemanager
            
            GameManager.instance.ReportWinner();
            yield break;
        }

        // Check for Question
        if (squareList[routePosition].specialSquare == 1 && 
            GameManager.instance.playerList[GameManager.instance.activePlayer].playerType == GameManager.Player.PlayerTypes.HUMAN)
        {
            Debug.Log("Ask Question");
            //QuestionPanel.SetActive(true);
            //questionText.text = "testing the question text...";
            GameManager.instance.AskQuestion();
            //squareList[routePosition].AddPlayer(this);
            yield break;
        }

        // Check for roll again
        if (squareList[routePosition].specialSquare == 2)
        {
            Debug.Log("Roll Again");
            //QuestionPanel.SetActive(true);
            //questionText.text = "Roll Again!";
            InfoText.instance.ShowMessage("Roll Again!");
            isMoving = false;
            GameManager.instance.state = GameManager.States.ROLL_DICE;
            yield break;
        }

        // Check for Miss a turn
        if (squareList[routePosition].specialSquare == 3)
        {
            missATurn = true;
        }

        //Add player to final stone
        squareList[routePosition].AddPlayer(this);
        // Update the game manager to change player.
        GameManager.instance.state = GameManager.States.SWITCH_PLAYER;
        isMoving = false;
    }

    bool MoveToNextSquare(Vector3 nextSquare)
    {
        return nextSquare != (transform.position = Vector3.MoveTowards(transform.position, nextSquare, speed * Time.deltaTime));
    }

    bool MoveInArcToNextSquare(Vector3 startPos, Vector3 nextPos, float _speed)
    {
        cTime += _speed * Time.deltaTime;
        Vector3 myPosition = Vector3.Lerp(startPos, nextPos, cTime);
        myPosition.y += amplitude * Mathf.Sin(Mathf.Clamp01(cTime) * Mathf.PI);
        return nextPos != (transform.position = Vector3.Lerp(transform.position, myPosition, cTime));
    }

    public void TakeTurn(int diceNumber)
    {
        stepsToMove = diceNumber;
        StartCoroutine(Move());
        /*if (doneSteps + stepsToMove < currentRoute.childSquareList.Count)
        {
            StartCoroutine(Move());
        }
        else
        {
            //Debug.Log("Rolled number is too high.");
            InfoText.instance.ShowMessage(GameManager.instance.playerList[GameManager.instance.activePlayer].playerName + " Roll was too high.");
            //Update the gamemanager to pchange player.
            GameManager.instance.state = GameManager.States.SWITCH_PLAYER;
        }*/
    }

    public void EndTurnAfterQuestion()
    {
        //end players turn
        //Add player to final stone
        squareList[routePosition].AddPlayer(this);
        // Update the game manager to change player.
        GameManager.instance.state = GameManager.States.SWITCH_PLAYER;
        isMoving = false;
    } 
}

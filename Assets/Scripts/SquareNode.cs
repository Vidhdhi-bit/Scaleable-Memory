using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SquareNode : MonoBehaviour
{
    public int squareNumber;  // Id The number of the square
    public TextMeshProUGUI numberText; // text to display for the square number
    public TextMeshProUGUI specialText; // Special Text to display (?, roll again, etc.)
    public int specialSquare; // 0 = none, 1 = question, 2 = Roll Again, 3 = Miss a turn.
    public SquareNode connectedNode; //Slide, Ladder etc.

    public List<PlayerController> playerList = new List<PlayerController>();

    public void SetNodeID(int _squareNumber)
    {
        // Enter the Square Text
        squareNumber = _squareNumber;
        if (numberText != null)
        {
            if (_squareNumber == 0)
            {
                numberText.text = "Start";
            }

            else
            {
                numberText.text = _squareNumber.ToString();
            }
        }
        // Enter the special Text
        if (specialSquare == 1)
        {
            specialText.text = "?";
        }
        if (specialSquare == 2)
        {
            specialText.text = "Roll Again";
        }
        if (specialSquare == 3)
        {
            specialText.text = "Miss a Turn";
        }
    }

    private void OnDrawGizmos()
    {
        if (connectedNode != null)
        {
            Color col = Color.white;

            col = (connectedNode.squareNumber > squareNumber) ? Color.blue : Color.red;
            Debug.DrawLine(transform.position, connectedNode.transform.position, col);
        }
    }

    public void AddPlayer(PlayerController player)
    {
        playerList.Add(player);
        //Rearange code
        ReArrangeStones();
    }

    public void RemovePlayer(PlayerController player)
    {
        playerList.Remove(player);
        //Rearange code
        ReArrangeStones();
    }

    void ReArrangeStones()
    {
        if (playerList.Count > 1)
        {
            int squaresize = Mathf.CeilToInt(Mathf.Sqrt(playerList.Count));  // calculate the grid square size required
            int playerCount = -1;
            for (int x = 0; x < squaresize; x++) // iterate on the x xsis
            {
                for (int y = 0; y < squaresize; y++) // iterate on the y axis
                {
                    playerCount++;
                    if (playerCount > playerList.Count - 1)
                    {
                        break;
                    }

                    Vector3 newPos = transform.position + new Vector3(-0.25f + x * 0.5f, 0, -0.25f + y * 0.5f);
                    playerList[playerCount].transform.position = newPos;
                }
            }
        }
        else if (playerList.Count == 1)
        {
            playerList[0].transform.position = transform.position;
        }
    }


}




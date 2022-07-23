using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardRoute : MonoBehaviour
{
    Transform[] squares;
    public List<Transform> childSquareList = new List<Transform>();

    private void Start()
    {
        FillNodes(); //populate the squares route list
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        FillNodes(); //populate the squares route list

        for (int i = 0; i < childSquareList.Count; i++)
        {
            Vector3 currentPos = childSquareList[i].position;
            if (i > 0)
            {
                Vector3 previousPos = childSquareList[i - 1].position;
                Gizmos.DrawLine(previousPos, currentPos);
            }
        }
    }


    void FillNodes()
    {
        childSquareList.Clear(); // clear teh list ready for population
        squares = GetComponentsInChildren<Transform>(); // get the transform of each child
        int num = -1;

        foreach (Transform child in squares)
        {
            SquareNode n = child.GetComponent<SquareNode>();
            if (child != this.transform && n != null)
            {
                num++;
                childSquareList.Add(child);
                child.gameObject.name = "Square " + num;
                // Fill in node ID in the node.

                n.SetNodeID(num); // add number to square

            }

            
        }
        //Debug.Log(squares.Length);
        childSquareList[childSquareList.Count - 1].GetComponentInChildren<SquareNode>().numberText.text = "Finish";
    }

}

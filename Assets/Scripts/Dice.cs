using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dice : MonoBehaviour
{
    Rigidbody rb;
    bool hasLanded;
    bool thrownDice;

    Vector3 initPosition;

    public DiceSides[] diceSides;
    public int diceValue;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        initPosition = transform.position;
        rb.useGravity = false;

    }

    public void RollDice()
    {
        //Reset dice before roll
        ResetDice();

        if (!thrownDice && !hasLanded)
        {
            thrownDice = true;
            rb.useGravity = true;
              rb.AddTorque(Random.Range(-500, 1500), Random.Range(0, 1500), Random.Range(0, 1500), ForceMode.Impulse);
           // rb.AddTorque(Random.Range(-1500, 500), Random.Range(0, 1500), Random.Range(0, 1500), ForceMode.Impulse);
        }
        else if (thrownDice && hasLanded)
        {
            //Reset dice
            ResetDice();
        }

    }

    void ResetDice()
    {
        transform.position = initPosition;
        rb.useGravity = false;
        thrownDice = false;
        hasLanded = false;
        rb.isKinematic = false;

    }



    // Update is called once per frame
    void Update()
    {
        if(rb.IsSleeping() && !hasLanded && thrownDice)
        {
            hasLanded = true;
            rb.useGravity = false;
            rb.isKinematic = true;
            SideValueCheck();
        }
        else if (rb.IsSleeping() && hasLanded && diceValue == 0)
        {
            RollAgain();
        }
    }


    void RollAgain()
    {
        ResetDice();
        rb.useGravity = true;
        thrownDice = true;
        rb.AddTorque(Random.Range(0, 1000), Random.Range(0, 1000), Random.Range(0, 1000));

    }

    void SideValueCheck()
    {
        diceValue = 0;
        foreach (DiceSides side in diceSides)
        {
            if (side.OnGround())
            {
                diceValue = side.sideValue;
                GameManager.instance.RolledDiceNumber(diceValue);
            }
        }
    }


}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainToken : MonoBehaviour
{
    GameObject gameControl;
    SpriteRenderer spriteRenderer;
    public Sprite[] faces;
    public Sprite back;
    public int faceIndex;
    public bool matched = false;
    public bool AllowRotate = true;
    public void OnMouseDown()
    {
        if (matched == false)
        {
            GameControl controlScript = gameControl.GetComponent<GameControl>();
            if (spriteRenderer.sprite == back)
            {
                if (controlScript.TokenUp(this))
                {
                    //spriteRenderer.sprite = faces[faceIndex];
                    StartRotation();
                    controlScript.CheckTokens();
                    print(matched + "matched");
                }
            }
            else
            {
                StartRotation();
                //spriteRenderer.sprite = back;
                controlScript.TokenDown(this);
            }
        }
    }
    public void StartRotation()
    {
        if (AllowRotate)
            StartCoroutine(RotateCard());


    }
    private IEnumerator RotateCard()
    {
        AllowRotate = false;
        if (spriteRenderer.sprite == back)
        {
            for (float i = 0; i < 180f; i += 15f)
            {
                transform.rotation = Quaternion.Euler(0f, i, 0f);
                if (i == 90.0f)
                {
                    spriteRenderer.sprite = faces[faceIndex];
                }
                yield return new WaitForSeconds(0.05f);

            }

        }

        else
        {
            for (float i = 180.0f; i >=0f; i -= 15f)
            {
                transform.rotation = Quaternion.Euler(0f, i, 0f);
                if (i == 90.0f)
                {
                    spriteRenderer.sprite = back;
                }
                yield return new WaitForSeconds(0.05f);

            }
        }
        AllowRotate = true;
    }
    private void Awake()
    {
        gameControl = GameObject.Find("GameControl");
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
}

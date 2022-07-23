using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class GameControl : MonoBehaviour
{
    GameObject token;
    public Text clickCountTxt;
    public Button easyBtn;
    public Button mediumBtn;
    public Button hardBtn;
    public Button MenuBtn;
    MainToken tokenUp1 = null;
    MainToken tokenUp2 = null;
    List<int> faceIndexes =
        new List<int>{ 0, 1, 2, 3, 0, 1, 2, 3, 4, 4, 5, 5, 6, 6, 7, 7, 8, 8, 9, 9, 10, 10, 11, 11};
    public static System.Random rnd = new System.Random();
    private int shuffleNum = 0;
    float tokenScale = 0.5f;
    //float yStart = 2.5f;
    //float xStart = -6.2f;
    float yStart = 2.1f;
    float xStart = -3.49f;
    int numOfTokens = 8;
    float yChange = -4.0f;
    float xChange = 3.0f;
    private int clickCount = 0;
    int counter = 0;
    int levelCounter = 0;
    public ParticleSystem Particle;
    public ParticleSystem Particle2;
    void StartGame()
    {
        int startTokenCount = numOfTokens;
        //float xPosition = -6.2f;
        float xPosition = xStart;
        float yPosition = yStart;
        int row = 1;
        // The camera orthographicSize is 1/2 the height of the window
        float ortho = Camera.main.orthographicSize / 2.0f;
        for (int i = 1; i < startTokenCount + 1; i++)
        {
            shuffleNum = rnd.Next(0, (numOfTokens));
            var temp = Instantiate(token, new Vector3(
                xPosition, yPosition, 0),
                Quaternion.identity);
            temp.GetComponent<MainToken>().faceIndex = faceIndexes[shuffleNum];
            //temp.transform.localScale = 
            //    new Vector3(ortho / tokenScale, ortho / tokenScale, 0);
            temp.transform.localScale = new Vector3(tokenScale, tokenScale, 0);
            faceIndexes.Remove(faceIndexes[shuffleNum]);
            numOfTokens--;
            xPosition = xPosition + xChange;
            if (i % 4 < 1)
            {
                yPosition = yPosition + yChange;
                //xPosition = -6.2f;
                xPosition = -3.49f;
                row++;
            }
        }
        token.SetActive(false);
    }

    public void TokenDown(MainToken tempToken)
    {
        if (tokenUp1 == tempToken)
        {
            tokenUp1 = null;
        }
        else if (tokenUp2 == tempToken)
        {
            tokenUp2 = null;
        }
    }

    public bool TokenUp(MainToken tempToken)
    {
        bool flipCard = true;
        if (tokenUp1 == null)
        {
            tokenUp1 = tempToken;
        }
        else if (tokenUp2 == null)
        {
            tokenUp2 = tempToken;
        }
        else
        {
            flipCard = false;
        }
        return flipCard;
    }

    public void CheckTokens()
    {
        clickCount++;
        clickCountTxt.text = clickCount.ToString();
        if (tokenUp1 != null && tokenUp2 != null &&
            tokenUp1.faceIndex == tokenUp2.faceIndex)
        {
            counter++;
            print("counter =" + counter);
            if (counter == levelCounter)
                Invoke("MoveToGameScene", 3.0f);

            tokenUp1.matched = true;
            tokenUp2.matched = true;
            Invoke("StartParticle", 1.0f);


        }
        else if (tokenUp1 != null && tokenUp2 != null &&
           tokenUp1.faceIndex != tokenUp2.faceIndex)
        {

            //Invoke("tokenUp1.StartRotation()", 1.0f);
            //Invoke("tokenUp2.StartRotation()", 1.0f);
            Invoke("StartRotate", 1.0f);

        }
    }
    void MoveToGameScene()
    {
        SceneManager.LoadScene("GameScene");

    }
    void StartParticle()
    {
        Particle.gameObject.SetActive(true);
        Particle2.gameObject.SetActive(true);

        Particle.transform.localPosition = tokenUp1.transform.position;
        Particle2.transform.localPosition = tokenUp2.transform.position;

        Particle.Play();
        Particle2.Play();
        //tokenUp1 = null;
        //tokenUp2 = null;
        Invoke("StopParticle", 2.0f);
    }
    void StopParticle()
    {
        Particle.Stop();
        Particle2.Stop();
        Particle.gameObject.SetActive(false);
        Particle2.gameObject.SetActive(false);
        //tokenUp1.
        tokenUp2.gameObject.SetActive(false);
        tokenUp1.gameObject.SetActive(false);
        tokenUp1 = null;
        tokenUp2 = null;
        
    }
    void StartRotate() {
        tokenUp1.StartRotation();
            tokenUp2.StartRotation();
        tokenUp1 = null;
        tokenUp2 = null;
    }
    public void HardSetup()
    {
        HideButtons();
        ShowMenuButton();
        tokenScale = 0.32f;
        yStart = 3.8f;
        numOfTokens = 16;
        yChange = -2.5f;
        xChange = 2.3f;
        //tokenScale = 12;
        //yStart = 3.8f;
        //numOfTokens = 24;
        //yChange = -1.5f;
        levelCounter = 8;
        StartGame();
    }

    public void MediumSetup()
    {
        HideButtons();
        ShowMenuButton();
        tokenScale = 0.4f;
        yStart = 3.1f;
        numOfTokens = 12;
        yChange = -3.15f;
        xChange = 2.7f;
        //tokenScale = 8;
        //yStart = 3.4f;
        //numOfTokens = 16;
        //yChange = -2.2f;
        levelCounter = 6;
        StartGame();
    }

    public void EasySetup()
    {
        HideButtons();
        ShowMenuButton();
        StartGame();
        levelCounter = 4;
    }
    private void ShowMenuButton()
    {
        MenuBtn.gameObject.SetActive(true);
    }
    private void HideButtons()
    {
        easyBtn.gameObject.SetActive(false);
        mediumBtn.gameObject.SetActive(false);
        hardBtn.gameObject.SetActive(false);
        GameObject[] startImages = 
            GameObject.FindGameObjectsWithTag("startImage");
        foreach (GameObject item in startImages)
            Destroy(item);
    }

    private void Awake()
    {
        token = GameObject.Find("Token");
    }

    void OnEnable()
    {
        easyBtn.onClick.AddListener(() => EasySetup());
        mediumBtn.onClick.AddListener(() => MediumSetup());
        hardBtn.onClick.AddListener(() => HardSetup());
    }

    public void MenuButtonClick()
    {
        SceneManager.LoadScene("GameScene");
    }
}

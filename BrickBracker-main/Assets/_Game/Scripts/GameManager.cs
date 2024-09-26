using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject LeftBorder, RightBorder, UpBorder, DownBorder, Paddle, GameOverPanel, PausePanel;
    [SerializeField] List<GameObject> AllBricksDesign, SelectedBricksDesign, AllSpecialObjects;

    public List<GameObject> AllBalls;
    [SerializeField] GameObject BallObject, BallTargetObject, paddleObject;
    [SerializeField] AudioClip ClickSound, DestroySound, GameOverSound;
    public static GameManager instance;
    public Vector2 screenSize;
    bool isHold;
    private void Awake()
    {
        instance = this;
        Vector2 size = new Vector2(Screen.width, Screen.height);
        screenSize = Camera.main.ScreenToWorldPoint(size);

        LeftBorder.GetComponent<BoxCollider2D>().size = new Vector2(1, screenSize.y * 2);
        LeftBorder.transform.position = new Vector2(-screenSize.x + LeftBorder.GetComponent<BoxCollider2D>().size.x / 2 - 1, 0);

        RightBorder.GetComponent<BoxCollider2D>().size = new Vector2(1, screenSize.y * 2);
        RightBorder.transform.position = new Vector2(screenSize.x + RightBorder.GetComponent<BoxCollider2D>().size.x / 2, 0);

        UpBorder.GetComponent<BoxCollider2D>().size = new Vector2(screenSize.x * 2, 1);
        UpBorder.transform.position = new Vector2(0, screenSize.y - UpBorder.GetComponent<BoxCollider2D>().size.y / 2 + 1);

        DownBorder.GetComponent<BoxCollider2D>().size = new Vector2(screenSize.x * 2, 1);
        DownBorder.transform.position = new Vector2(0, -screenSize.y + DownBorder.GetComponent<BoxCollider2D>().size.y / 2 - 1);
    }
    private void Start()
    {
        PausePanel.SetActive(false);
        GameOverPanel.SetActive(false);
        Time.timeScale = 1;

        if (GameObject.Find("Ball") != null)
        {
            GameObject ball = GameObject.Find("Ball");
            AllBalls.Add(ball);
        }

        BrickDesignSetter();
    }
    int val;
    public bool isRelease;
    Vector3 currentPos;
    float currentPosY;
    float currentPosX;
    private void Update()
    {
        if (!isRelease)
        {
            BallObject.transform.position = new Vector3(paddleObject.transform.position.x, paddleObject.transform.position.y + 0.5f, paddleObject.transform.position.z);
            BallTargetObject.transform.position = new Vector3(BallObject.transform.position.x, BallObject.transform.position.y + 0.5f, BallObject.transform.position.z);
            //Debug.Log("Release called");
            if (Input.GetMouseButton(0))
            {
                if (!EventSystem.current.IsPointerOverGameObject())
                {
                    currentPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    currentPosX = Mathf.Clamp(currentPos.x, -3, 3);
                    currentPosY = Mathf.Abs(currentPos.y);
                    //Debug.Log("X val = " + currentPosX);
                    //Debug.Log("YY val = " + currentPosY);
                    BallTargetObject.transform.up = new Vector3(currentPosX, currentPosY, 0);
                }
            }
            if (Input.GetMouseButtonUp(0))
            {
                if (!EventSystem.current.IsPointerOverGameObject())
                {
                    BallObject.transform.up = new Vector3(currentPos.x, currentPosY, 0);
                    ThrowBall();
                    BallTargetObject.SetActive(false);
                    isRelease = true;
                }
            }
        }
    }
    int throwSpeed;
    public void ThrowBall()
    {
        throwSpeed = 200;
        Common.Instance.gameObject.transform.GetChild(0).GetComponent<AudioSource>().PlayOneShot(ClickSound);
        if (currentPosY > 1.2f || currentPosX > 1.2f)
        {
            throwSpeed = 250;
        }
        else if (currentPosY < 0.5f || currentPosX < 0.5f)
        {
            throwSpeed = 250;
        }
        else
        {
            throwSpeed = 220;
        }
        //BallObject.GetComponent<Rigidbody2D>().velocity = new Vector3(currentPosX, currentPosY, 0);
        //BallObject.GetComponent<Rigidbody2D>().AddForce(new Vector3(currentPosX, currentPosY, 0) * throwSpeed);
        Vector2 throwDirection = new Vector2(currentPosX, currentPosY).normalized;

        // Apply force with a fixed magnitude
        AllBalls[0].GetComponent<Rigidbody2D>().AddForce(throwDirection * throwSpeed);
    }
    
    public void SpecialObjectSpawn(Vector3 spawnPos)
    {
        GameObject g;
        int val;
        val = Random.Range(0, AllSpecialObjects.Count);

        Instantiate(AllSpecialObjects[val], spawnPos, Quaternion.identity);
    }

    public void GenerateBall()
    {
        Vector2 spawnPos = new Vector2(screenSize.x / 2, screenSize.y / 2);
        GameObject obj = Instantiate(BallObject, spawnPos, Quaternion.identity);
        AllBalls.Add(obj);
        obj.GetComponent<Rigidbody2D>().AddForce(Vector2.up * 200);
    }
    public void PaddleWidthIncrease()
    {
        Paddle.GetComponent<SpriteRenderer>().size = new Vector2(Paddle.GetComponent<SpriteRenderer>().size.x + 0.07f, Paddle.GetComponent<SpriteRenderer>().size.y);
        Paddle.GetComponent<BoxCollider2D>().size = new Vector2(Paddle.GetComponent<BoxCollider2D>().size.x + 0.07f, Paddle.GetComponent<BoxCollider2D>().size.y);
    }
    public void ConvertIntoFireBall()
    {
        foreach(GameObject obj in AllBalls)
        {
            obj.GetComponent<BallController>().isFireBall = true;
            StartCoroutine(WaitForFireBallToNormal());
        }
    }
    IEnumerator WaitForFireBallToNormal()
    {
        yield return new WaitForSeconds(5);
        Debug.Log("Ball normal convert");
        foreach (GameObject obj in AllBalls)
        {
            obj.GetComponent<BallController>().isFireBall = false;
        }
    }
    public void CheckingAllBallExist()
    {
        foreach (GameObject obj in AllBalls)
        {
            // Check if the object is null
            if (obj == null)
            {
                Debug.Log("Ball removed");
                AllBalls.Remove(obj);
            }
        }
    }
    public void CheckBrickAvailablity()
    {
        Common.Instance.gameObject.transform.GetChild(0).GetComponent<AudioSource>().PlayOneShot(ClickSound);
        Debug.Log(SelectedBricksDesign[0].transform.childCount);
        if (SelectedBricksDesign[0].transform.childCount <= 1)
        {
            Common.Instance.gameObject.transform.GetChild(0).GetComponent<AudioSource>().PlayOneShot(GameOverSound);
            GameOverPanel.SetActive(true);
            Time.timeScale = 1;
            Debug.LogError("Game overrrrrrrrrrrrrr");
        }
        else
        {
            Debug.Log("Else called");
        }
    }
    public void BrickDesignSetter()
    {
        int val = Random.Range(0, AllBricksDesign.Count);

        // Get the canvas attached to the screen space camera
        Canvas canvas = FindObjectOfType<Canvas>();

        // Ensure the canvas is found
        if (canvas != null)
        {
            // Calculate the half of the screen height
            float halfScreenHeight = Screen.height * 0.5f;

            // Get the screen position at the center of the top edge of the canvas
            Vector3 spawnPosScreen = new Vector3(Screen.width * 0.5f, Screen.height / 2 + (Screen.height / 4), 0);
            Debug.Log(Screen.height);
            // Convert screen position to world space
            Vector3 spawnPosWorld = Camera.main.ScreenToWorldPoint(spawnPosScreen);

            // Ensure the z-coordinate is appropriate for the canvas
            spawnPosWorld.z = canvas.planeDistance;

            // Instantiate the object onto the canvas
            GameObject g = Instantiate(AllBricksDesign[val], spawnPosWorld, Quaternion.identity, canvas.transform);

            // Optionally, adjust the scale of the instantiated object
            g.transform.localScale = new Vector3(120, 120, 1); // Adjust scale as needed

            // Optionally, add the instantiated object to a list
            SelectedBricksDesign.Add(g);
        }
        else
        {
            Debug.LogError("Canvas not found!");
        }
    }
    public void PausePanelOpen()
    {
        Common.Instance.gameObject.transform.GetChild(0).GetComponent<AudioSource>().PlayOneShot(ClickSound);
        PausePanel.SetActive(true);
        Time.timeScale = 0;
    }
    public void OnClick_ResetBtn()
    {
        Common.Instance.gameObject.transform.GetChild(0).GetComponent<AudioSource>().PlayOneShot(ClickSound);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        PausePanel.SetActive(false);
        GameOverPanel.SetActive(false);
        Time.timeScale = 1;
    }
    public void OnClick_HomeBtn()
    {
        Common.Instance.gameObject.transform.GetChild(0).GetComponent<AudioSource>().PlayOneShot(ClickSound);
        SceneManager.LoadScene(0);
        PausePanel.SetActive(false);
        GameOverPanel.SetActive(false);
        Time.timeScale = 1;
    }
    public void OnClick_ResumeBtn()
    {
        Common.Instance.gameObject.transform.GetChild(0).GetComponent<AudioSource>().PlayOneShot(ClickSound);
        PausePanel.SetActive(false);
        Time.timeScale = 1;
    }
}
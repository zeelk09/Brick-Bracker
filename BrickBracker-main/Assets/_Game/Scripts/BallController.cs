using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BallController : MonoBehaviour
{
    public bool isFireBall;

    public static BallController instance;
    private void Awake()
    {
        instance = this;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Over")
        {
            Debug.Log("overr");
            if (GameManager.instance.AllBalls.Count == 0)
            {
                SceneManager.LoadScene(1);
            }
            else
            {
                Destroy(this.gameObject);
                GameManager.instance.AllBalls.Remove(this.gameObject);;
                GameManager.instance.CheckingAllBallExist();
                if (GameManager.instance.AllBalls.Count == 0)
                {
                    Debug.Log("Game Over");
                    SceneManager.LoadScene(1);
                }
            }
        }
    }
    
}

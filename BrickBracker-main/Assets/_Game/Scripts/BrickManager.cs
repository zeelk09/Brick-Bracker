using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class BrickManager : MonoBehaviour
{
    [SerializeField] Sprite BrockenSprite;
    [SerializeField] GameObject parent;
    [SerializeField] ParticleSystem particle;

    [SerializeField] GameObject specialObject;
    public bool isTouch, isSpecialObj;

    private void Start()
    {
        specialObject = Resources.Load("SpecialObj") as GameObject;
        //parent = GameObject.Find("BricksDesign2").transform.gameObject;
        particle = GameObject.Find("Particle").GetComponent<ParticleSystem>();
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ball")
        {
            GameManager.instance.CheckBrickAvailablity();
            bool isFireGameObj = collision.gameObject.GetComponent<BallController>().isFireBall;

            if (!isFireGameObj)
            {
                if (!isTouch)
                {
                    isTouch = true;
                    this.gameObject.GetComponent<SpriteRenderer>().sprite = BrockenSprite;
                }
                else
                {
                    Vector3 collisionPosition = collision.contacts[0].point;

                    particle.transform.position = collisionPosition;
                    particle.Play();
                    if (isSpecialObj)
                    {
                        GameManager.instance.SpecialObjectSpawn(collision.transform.position);
                    }
                    Destroy(this.gameObject);
                }
            }
            else
            {
                Vector3 collisionPosition = collision.contacts[0].point;

                particle.transform.position = collisionPosition;
                particle.Play();
                if (isSpecialObj)
                {
                    Debug.Log("Special");
                    GameManager.instance.SpecialObjectSpawn(collision.transform.position);
                }
                Destroy(this.gameObject);
            }
        }
    }
}

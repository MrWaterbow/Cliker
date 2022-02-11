using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClickOnTheScreenControl : MonoBehaviour
{    
    public GameObject screensaver;
    public GameObject screensaverText;
    public Text scoreText;
    private int score;
    private static bool idle = true; //режим сна
    private float idlTime = 15.0f; //лимит времени до режима сна
    private float lastUi=0.0f;
    public GameObject[] colorSqares;
    public GameObject particle;
    private GameObject[] purpleSquares;
    private Animator purpleSquareAnimator;

    public int Score
    {
        get
        {
            return score;
        }
        set
        {
            if (value < 0)
                score = 0;
            else
                score = value;
        }
    }

    private void AwakePerpleSquare() //Появление Фиолетовых квадратов сразу после пробуждения
    {
        if (idle == false)
        {
            foreach (GameObject objekt in purpleSquares)
            {
                objekt.GetComponent<Animator>().Play("PurpleSquaresON");
            }
        }
        else
        {
            foreach (GameObject objekt in purpleSquares)
            {
                objekt.GetComponent<Animator>().Play("PurpleSquaresDisable");
            }
        }
    }

    private IEnumerator PlayPerpleSquare() //Анимация фиолетовых квадратов
    {
        AwakePerpleSquare();
        while (true)
        {
            GameObject Square = purpleSquares[Random.Range(0, purpleSquares.Length)];
                
            Square.GetComponent<Animator>().SetBool("idle", idle);
            Square.GetComponent<Animator>().Play("PurpleSquaresToGreen");
            yield return new WaitForSeconds(5.0f);

            Square.GetComponent<Animator>().Play("PurpleSquaresOnlyGreen");
            Square.tag = "GreenSquare";
            GameObject particleClone = Instantiate(particle, Square.transform.position, Square.transform.rotation);
            particleClone.GetComponent<ParticleSystem>().Play();
            yield return new WaitForSeconds(5.0f);

            Destroy(particleClone);
            Square.tag = "PurpleSquare";
            Square.GetComponent<Animator>().Play("PurpleSquaresToPurple");
            yield return new WaitForSeconds(5.0f);
        }
    }

    private IEnumerator SpawnColorSquares() //Спавн цветных квадратов
    {
        while (true)
        {
            if (idle == false)
            {
                int s = Random.Range(1, 3);

                if (s == 1)
                {
                    GameObject colorSqaresClone1 = Instantiate(colorSqares[Random.Range(0, colorSqares.Length)], new Vector3(-24.0f, 5.0f, 0), Quaternion.Euler(0, 0, 0));
                    colorSqaresClone1.GetComponent<Animator>().Play("ColorSquaresStartToRight");
                    Destroy(colorSqaresClone1, 5.0f);
                }
                if (s == 2)
                {
                    GameObject colorSqaresClone2 = Instantiate(colorSqares[Random.Range(0, colorSqares.Length)], new Vector3(24.0f, 3.0f, 0), Quaternion.Euler(0, 0, 0));
                    colorSqaresClone2.GetComponent<Animator>().Play("ColorSquaresStartToLeft");
                    Destroy(colorSqaresClone2, 5.0f);
                }
            }
            yield return new WaitForSeconds(1.0f);
        }
    }

    private void IDLE() //Проверка режима сна
    {
        scoreText.GetComponent<Animator>().SetBool("idle", idle);
        screensaverText.GetComponent<Animator>().SetBool("idle", idle);
    }

    private void ScoreText() //отображение количества очков
    {
        if (idle == true)
        {
            Score = 0;
        }
        scoreText.text = "Количество очков: " + Score;
    }

    private void IdleTimer() //таймер режима сна
    {
        if (Input.GetKey("mouse 0"))
        { 
            if (idle)
            {
                idle = false;
                StartCoroutine("SpawnColorSquares");
                StartCoroutine("PlayPerpleSquare");
            }
            lastUi = Time.time;
        }
        if ((Time.time - lastUi) > idlTime)
        {
            idle = true;
            StopCoroutine("SpawnColorSquares");
            StopCoroutine("PlayPerpleSquare");
        }
    }
    
    public void ScreensaverOff() //отключение застаки режима сна
    {
        screensaverText.GetComponent<Animator>().Play("ScreensaverOff");
        screensaver.SetActive(false);
    }

    public void ScreensaverOn() //включение застаки режима сна
    {
        if (idle == true)
        {
            screensaver.SetActive(true);
        }
    }    

    private void RayInMouse() //Создание луча через мышь плюс набор очков за счет коллизии луча с обьектами
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hits2D = Physics2D.GetRayIntersection(ray);
            if (hits2D.collider.name == "Screensaver")
            {
                ScreensaverOff();
            }
            if(hits2D.collider.name == "GreenSquare(Clone)")
            {
                Score += 100;
            }
            if (hits2D.collider.name == "OrangeSquare(Clone)")
            {
                Score += 200;
            }
            if (hits2D.collider.name == "RedSquare(Clone)")
            {
                Score -= 300;
            }
            if(hits2D.collider.tag == "GreenSquare")
            {
                Score += 100;
            }
        }
    }

    void FixedUpdate()
    {
        IdleTimer();
    }

    void Update()
    {
        ScreensaverOn();
        ScoreText();
        RayInMouse();
        IDLE();
        SpawnColorSquares();
        purpleSquares = GameObject.FindGameObjectsWithTag("PurpleSquare");
    }
}

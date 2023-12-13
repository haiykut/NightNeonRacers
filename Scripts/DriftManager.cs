using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using haiykut;
public class DriftManager : MonoBehaviour
{
    RCC_CarControllerV3 playerCar;
    bool isDriftMode;
    bool isDrifting;
    float smallestValue;
    float greatestValue;
    bool activateScore;
    [SerializeField]Text txt;
    bool timer = false;
    bool canResumeTimer = false;
    float canResumeTime = 1.5f;
    float driftTime = 0;
    bool gameobjectCloseTimer = false;
    float gameobjectCloseTime = 2;
    [SerializeField] float[] carRates = { 0.1f, 0.15f, 0.2f, 0.3f, 0.4f, 0.45f, 0.5f, 0.6f, 0.65f, 0.75f };
    [SerializeField]float currentCarRate;
    [SerializeField] Transform driftAngleTextPrefab;
    [SerializeField] Transform driftAngleText;
    [SerializeField] Vector3[] driftAngleLocalPositions = new Vector3[10];
    Vector3 driftAngleLocalPosition;
    float totalScore;
    [SerializeField] Text totalScoreTxtTemp;
    float tempTotalScore;
    bool totalScorePrint;
    void Start()
    {
        canResumeTime =1.5f;
        playerCar = GameManager.instance.playerCar.GetComponent<RCC_CarControllerV3>();
        currentCarRate = carRates[GameManager.instance.playerCarPointer];
        driftAngleLocalPosition = driftAngleLocalPositions[GameManager.instance.playerCarPointer];
        if (PlayerPrefs.GetInt("mode") == 2)
        {
            isDriftMode = true;
            driftAngleText = Instantiate(driftAngleTextPrefab,playerCar.transform,false);
            driftAngleText.transform.localPosition = driftAngleLocalPosition;
            driftAngleText.gameObject.SetActive(false);
            totalScoreTxtTemp.gameObject.SetActive(true);
            totalScoreTxtTemp.text = "<size=40>Total Score: </size>" + "<b>" + tempTotalScore.ToString("F") + "</b>";

        }
        else
        {
            isDriftMode= false;
            totalScoreTxtTemp.gameObject.SetActive(false);
        }
    }

    void FixedUpdate()
    {
        //Debug.Log(playerCar.direction);
        if (timer)
        {
            driftTime = driftTime + Time.fixedDeltaTime * 1.2f;
        }
        if (canResumeTimer)
        {
            canResumeTime -= Time.fixedDeltaTime;
            if (canResumeTime <= 0 && driftTime > 0)
            {
                timer = false;
                canResumeTime = 1.5f;
                canResumeTimer = false;
                driftAngleText.gameObject.SetActive(false);
                tempTotalScore = totalScore;
                totalScore += driftTime;
                totalScorePrint = true;
                //Debug.Log(Mathf.FloorToInt(driftTime * currentCarRate).ToString());

                //StopCoroutine(GameManager.instance.DiamondSet(Mathf.FloorToInt(driftTime * currentCarRate),false));
                StartCoroutine(GameManager.instance.DiamondSet(Mathf.FloorToInt(driftTime * currentCarRate),false));
                driftTime = 0;
                activateScore = false;
                
            }
        }

        if (isDriftMode)
        {
            //if (playerCar.driftAngle > 0.10f || playerCar.driftAngle < -0.10f )
            //if (Mathf.Abs(playerCar.driftAngle) > 0 )
            if (!playerCar.isCollidedForDrift)
            {
                if (playerCar.speed > 30)
                {
                    if(Mathf.Abs(playerCar.driftAngle * 100) > 10f)
                    {
                        if(playerCar.direction == 1)
                        {
                            canResumeTimer = false;
                            canResumeTime = 1.5f;
                            timer = true;
                            driftAngleText.gameObject.SetActive(true);
                            activateScore = true;
                            gameobjectCloseTimer = false;
                        }
                        else
                        {
                            timer = false;
                            canResumeTimer = true;
                            //tme = 0;
                            //driftAngleText.gameObject.SetActive(true);
                            activateScore = false;
                            gameobjectCloseTimer = false;
                        }

                    }
                    else
                    {
                        timer = false;
                        //canResumeTime = 3;
                        canResumeTimer = true;
                        //tme = 0;
                        //driftAngleText.gameObject.SetActive(true);
                        activateScore = false;
                        gameobjectCloseTimer = false;
                    }

                }
                else
                {
                    activateScore = false;
                    timer = false;
                    //canResumeTime = 3;
                    canResumeTimer = true;
                    //canResumeTime = 3;
                    //driftAngleText.GetComponentInChildren<TextMesh>().text = "<b>" + "   Drift:" + "</b>" + "<size=270> " + driftTime.ToString("F") + "</size>m";
                    //driftAngleText.GetComponentInChildren<TextMesh>().color = Color.red;
                    //driftTime = 0;
                    //gameobjectCloseTimer = true;
                }
            }
            else
            {
                activateScore= false;
                timer = false;
                canResumeTimer = false;
                canResumeTime = 1.5f;
                driftAngleText.GetComponentInChildren<TextMesh>().text = "<b>" + "   Drift:" + "</b>" + "<size=270> " + driftTime.ToString("F") + "</size>m";
                driftAngleText.GetComponentInChildren<TextMesh>().color = Color.red;
                driftTime = 0;
                gameobjectCloseTimer = true;
            }







        }
        if (activateScore)
        {

            driftAngleText.GetComponentInChildren<TextMesh>().text = "<b>" + "   Drift:" + "</b>" + "<size=270> " + driftTime.ToString("F") + "</size>m";
            driftAngleText.GetComponentInChildren<TextMesh>().color = GameManager.instance.mainColor;

        }

        if (totalScorePrint)
        {
            if (tempTotalScore < totalScore)
            {
                tempTotalScore += Time.fixedDeltaTime * 10;
                totalScoreTxtTemp.text = "<size=40>Total Score: </size>" + "<b>" + tempTotalScore.ToString("F") + "</b>";
            }
            else
            {
                tempTotalScore = totalScore;
                totalScoreTxtTemp.text = "<size=40>Total Score: </size>" + "<b>" + totalScore.ToString("F") + "</b>";
                totalScorePrint = false;
            }

        }

        if (gameobjectCloseTimer)
        {
            gameobjectCloseTime -= Time.fixedDeltaTime;
            if (gameobjectCloseTime <= 0)
            {
                driftAngleText.gameObject.SetActive(false);
                gameobjectCloseTimer = false;
                gameobjectCloseTime = 2;
            }
        }
            


        }

    }
  //void DiamondSetter(float value)
  //  {
  //      int diamond = PlayerPrefs.GetInt("diamond");
  //      previousDiamond = diamond;
  //      diamond += (int)value;
  //      Debug.Log(diamond);
  //      PlayerPrefs.SetInt("diamond", diamond);
  //      //GameManager.instance.diamondText.text = diamond.ToString();
  //      StartCoroutine(DiamondCounting(diamond));
  //  }

  //  IEnumerator DiamondCounting(int value)
  //  {
  //      WaitForSeconds wfs = new WaitForSeconds(0.1f);
  //      //int previousValue = score;
  //      int stepAmount;
  //      stepAmount = Mathf.CeilToInt((value - previousDiamond) /60f);

  //      while (previousDiamond < value)
  //      {
  //          previousDiamond += stepAmount;
  //          if (previousDiamond > value)
  //          {
  //              previousDiamond = value;
  //              //ScoreStepCheck((int)score);
  //          }
  //          GameManager.instance.diamondText.text = previousDiamond.ToString();
  //          yield return wfs;
  //      }

  //  }


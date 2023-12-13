using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

namespace haiykut
{


    public class CarBlastManager : MonoBehaviour
    {
        [SerializeField] internal Camera cam;
        bool isSceneLoaded;
        public static CarBlastManager instance;
        [Header("Explode Mode Settings")]
        [SerializeField] Transform explodeEffect;
        [SerializeField] internal bool inAnExplosion = false;
        [SerializeField] float explodeForceUpRatio = 100f;
        [SerializeField] float explodeForceNormalizedRatio = 8f;
        Rigidbody explodedObject;
        [SerializeField] ForceMode forceModeUp = ForceMode.Acceleration;
        [SerializeField] ForceMode forceModeForNormalized = ForceMode.VelocityChange;
        [SerializeField] ForceMode forceModePlayer = ForceMode.Acceleration;
        [SerializeField] Vector3 offset = Vector3.zero;
        Vector3 lastPositionForPlayer;
        Vector3 lastPositionForAI;
        float score;
        float totalScore;
        int scoreStep = 1;
        [SerializeField] Text scoreText;
        int counterFps = 3;
        float[] carStrengths = { 0.1f, 0.15f, 0.2f, 0.3f, 0.4f, 0.45f, 0.5f, 0.6f, 0.65f, 0.75f };
        float currentCarStrength;
        [SerializeField] Color miniMapExplodedColor;
        [SerializeField] Color miniMapNormalColor;
        int diamond;
        bool scorePrinter;
        public float getCurrentCarStrength()
        {
            return currentCarStrength;
        }
        private void Awake()
        {
            instance = this;
            currentCarStrength = carStrengths[GameManager.instance.playerCarPointer];
        }
        IEnumerator Start()
        {
            yield return new WaitForEndOfFrame();
            isSceneLoaded = true;
            diamond = PlayerPrefs.GetInt("diamond");
            scoreText.text = "<size=40>Total Score: </size>" + "<b>" + score.ToString("F") + "</b>";
            scoreText.gameObject.SetActive(true);
            //GameManager.instance.diamondText.text = diamond.ToString();
            //PlayerPrefs.DeleteKey("diamond");
            //Debug.Log(PlayerPrefs.GetInt("diamond"));   
        }

        void FixedUpdate()
        {
            if (GameManager.instance.playMode == 1)
            {
                if (isSceneLoaded)
                {
                    if (inAnExplosion)
                    {
                        if (!explodedObject.isKinematic)
                        {

                            explodedObject.AddForceAtPosition((lastPositionForPlayer + offset) * explodeForceNormalizedRatio, lastPositionForAI, forceModeForNormalized);
                            explodedObject.AddExplosionForce(explodeForceNormalizedRatio, explodedObject.position, 270);
                            explodedObject.AddForce(Vector3.up * explodeForceUpRatio, forceModeUp);
                            GameManager.instance.playerCar.GetComponent<Rigidbody>().AddForce(-GameManager.instance.playerCar.transform.forward * GameManager.instance.playerCar.GetComponent<RCC_CarControllerV3>().speed, forceModePlayer);
                        }

                        else
                        {
                            inAnExplosion = false;
                            explodedObject = null;
                            lastPositionForPlayer = Vector3.zero;
                            lastPositionForAI = Vector3.zero;
                        }
                    }

                    if (scorePrinter)
                    {
                        if (previousScore < score)
                        {
                            previousScore += Time.fixedDeltaTime * 30;
                            scoreText.text = "<size=40>Total Score: </size>" + "<b>" + previousScore.ToString("F") + "</b>";
                        }
                        else
                        {
                            previousScore = score;
                            scoreText.text = "<size=40>Total Score: </size>" + "<b>" + score.ToString("F") + "</b>";
                            scorePrinter = false;
                        }
                    }
                }
            }


        }

        bool explodeControl;
        void ScorePrinter(float sc)
        {

            StartCoroutine(CountingCoroutine(sc));
        }
        float previousScore;
        IEnumerator CountingCoroutine(float value)
        {
            WaitForSeconds wfs = new WaitForSeconds(1 / counterFps);
            //int previousValue = score;
            float stepAmount;
            stepAmount = Mathf.CeilToInt((value - score) / counterFps);

            while (score < value)
            {
                score += stepAmount;
                if (score > value)
                {
                    score = value;
                    ScoreStepCheck((int)score);
                }

                scoreText.text = score.ToString("F2");

                yield return wfs;
            }

        }
        IEnumerator DiamondCounting(int value)
        {
            WaitForSeconds wfs = new WaitForSeconds(0.1f);
            //int previousValue = score;
            int stepAmount;
            stepAmount = Mathf.CeilToInt((value - previousDiamond) / counterFps);

            while (previousDiamond < value)
            {
                previousDiamond += stepAmount;
                if (previousDiamond > value)
                {
                    previousDiamond = value;
                    //ScoreStepCheck((int)score);
                }
                GameManager.instance.diamondText.text = previousDiamond.ToString();
                yield return wfs;
            }

        }
        float previousDiamond;
        void ScoreStepCheck(float ss)
        {
            //if (ss / 100  == scoreStep)
            //{

            int diamond = PlayerPrefs.GetInt("diamond");
            previousDiamond = diamond;
            diamond += (int)((ss - previousScore) * currentCarStrength) / 10;
            PlayerPrefs.SetInt("diamond", diamond);
            //Debug.Log(diamond);
            StartCoroutine(DiamondCounting(diamond));
            scoreStep++;
            //Debug.Log(scoreStep);
            //}
        }

        public void ExplodeEvent(TSSimpleCar car)
        {
            car.bodyNormal.SetActive(false);
            car.bodyExploded.SetActive(true);
            SoundManager.instance.PlaySoundOneShot("explosionsound", .65f);
            Renderer renderer = new Renderer();
            foreach (Transform t in car.transform)
            {
                if (t.CompareTag("Target"))
                {
                    renderer = t.GetComponent<Renderer>();
                }
            }
            renderer.material.color = miniMapExplodedColor;
            //car.transform.GetChild(car.transform.childCount - 1).GetComponent<Renderer>().material.color = miniMapExplodedColor;
            car.body.velocity = Vector3.zero;
            lastPositionForAI = car.transform.position;
            lastPositionForPlayer = car.lastPoint;
            Transform particle = Instantiate(explodeEffect);
            particle.SetParent(car.transform);
            particle.localPosition = Vector3.zero;
            Destroy(particle.gameObject, 1f);
            StartCoroutine(ActivateAgain());
            IEnumerator ActivateAgain()
            {
                explodedObject = car.body;
                inAnExplosion = true;
                //car.body.velocity = Vector3.zero;
                yield return new WaitForSeconds(0.3f);
                car.body.isKinematic = true;
                car.bodyExploded.SetActive(false);
                for (int i = 0; i < 2; i++)
                {
                    car.BackWheels[i].transform.GetChild(0).gameObject.SetActive(false);
                    car.FrontWheels[i].transform.GetChild(0).gameObject.SetActive(false);
                }
                previousScore = score;
                score += car.scoreFactor * currentCarStrength;
                //totalScore = score + previousScore;
                //Debug.Log(score - previousScore);
                //StopCoroutine(GameManager.instance.DiamondSet(Mathf.CeilToInt(score - previousScore), false));
                StartCoroutine(GameManager.instance.DiamondSet(Mathf.CeilToInt(score - previousScore), false));
                //totalScore += score;
                //ScorePrinter(score + (car.scoreFactor * currentCarStrength));
                scorePrinter = true;
                //explodeControl = true;
                yield return new WaitForSeconds(1f);
                car.transform.position = new Vector3(0, -50f, 0);
                car.body.mass = 1500f;
                car.body.isKinematic = false;
                car.bodyNormal.SetActive(true);
                //car.transform.GetChild(car.transform.childCount - 1).GetComponent<Renderer>().material.color = miniMapNormalColor;
                renderer.material.color = miniMapNormalColor;
                for (int i = 0; i < 2; i++)
                {
                    car.BackWheels[i].transform.GetChild(0).gameObject.SetActive(true);
                    car.FrontWheels[i].transform.GetChild(0).gameObject.SetActive(true);
                }
                car.exploded = false;
            }
        }


    }
}

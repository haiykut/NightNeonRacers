using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
namespace haiykut
{


    public class RaceManager : MonoBehaviour
    {
        public static RaceManager instance;
        internal bool hasRaceFinished = false;
        [SerializeField]internal List<Transform> checkList;
        internal RCC_AIWaypointsContainer waypointContainer;
        [SerializeField] internal Transform checkpointPrefab;
        [SerializeField] internal Transform finishLineModel;
        internal bool startTheRace;
        internal int chId;
        [SerializeField] internal List<RacePart> racePart;
        [SerializeField] internal List<RCC_AIWaypointsContainer> waypoints;
        [SerializeField] int[] raceWinnerPrices;
        [SerializeField] internal GameObject postProcessForLights;
        [SerializeField] internal Transform arrowPrefab;
        [SerializeField] internal CameraAnimation cameraAnim;
        [SerializeField] Transform fireworkPrefab;
        [SerializeField] Transform losePrefab;
        int lv;
        [SerializeField] List<Transform> chaseTarget;
        private void Awake()
        {
            instance = this;
            if(PlayerPrefs.GetInt("mode") == 0)
            {
                int level = PlayerPrefs.GetInt("currentrace");
                lv = level;
                waypoints[level].gameObject.SetActive(true);
                waypointContainer = waypoints[level];
                racePart[level].levelObject.SetActive(true);
                foreach(GameObject brake in racePart[level].brakeZones)
                {
                    brake.SetActive(true);
                }
            }
        }
        
        IEnumerator Start()
        {
            GameObject checkPointParent = new GameObject("Checkpoint Parent");
            if (PlayerPrefs.GetInt("mode") == 0)
            {
                yield return new WaitForEndOfFrame();
                int checkValue = 0;
                for (int i = 1; i < waypointContainer.waypoints.Count; i++)
                {
                    if (i % racePart[GameManager.instance.raceId].checkpointRange == 0)
                    {
                        Transform newCheckPoint = Instantiate(checkpointPrefab).transform;
                        newCheckPoint.position = waypointContainer.waypoints[i].transform.position;
                        newCheckPoint.GetComponent<CheckpointScript>().checkpointId = checkValue;
                        newCheckPoint.localScale = racePart[GameManager.instance.raceId].checkpointScale;
                        if (i == 0 || i == waypointContainer.waypoints.Count - 1)
                        {
                            newCheckPoint.transform.LookAt(waypointContainer.transform.GetChild(1));
                        }
                        else
                        {
                            newCheckPoint.transform.LookAt(waypointContainer.transform.GetChild(i + 1));
                        }
                        checkList.Add(newCheckPoint);
                        newCheckPoint.gameObject.name = "Checkpoint: " + checkValue.ToString();
                        newCheckPoint.SetParent(checkPointParent.transform);
                        checkValue++;
                    }
                }
                if(checkList.Count % 2 == 0)
                {
                    Transform finishLine = Instantiate(checkpointPrefab).transform;
                    finishLine.position = waypointContainer.waypoints[waypointContainer.waypoints.Count - 1].transform.position;
                    finishLine.GetComponent<CheckpointScript>().checkpointId = checkValue;
                    finishLine.localScale = racePart[GameManager.instance.raceId].checkpointScale;
                    finishLine.transform.LookAt(checkList[checkValue - 1]);
                    checkList.Add(finishLine);
                    finishLine.GetComponent<CheckpointScript>().finishLine = true;
                    finishLine.gameObject.name = "FinishLine";
                    finishLine.SetParent(checkPointParent.transform);
                }
                else
                {
                    checkList[checkList.Count-1].GetComponent<CheckpointScript>().finishLine = true;
                    checkList[checkList.Count-1].gameObject.name = "FinishLine";
                }
                
                for(int i = 0; i<GameManager.instance.cars.Count; i++)
                {
                    //GameManager.instance.cars[i].GetComponent<RCC_CarControllerV3>().rigid.isKinematic = true;
                    GameManager.instance.cars[i].GetComponent<RCC_CarControllerV3>().canControl = false;
                    GameManager.instance.cars[i].orderText.gameObject.SetActive(false);
                    GameManager.instance.cars[i].sounds.GetChild(0).gameObject.SetActive(false);
                }
                cameraAnim.SetTheThings(GameManager.instance.cars[0].transform.localPosition, GameManager.instance.cars[0].transform.eulerAngles);
                cameraAnim.start = true;
            }
            
        }
        internal IEnumerator RaceDone(int winnerId)
        {
            racePart[lv].levelObject.SetActive(false);
     
            for (int i = 0; i < GameManager.instance.cars.Count; i++)
            {
                GameManager.instance.cars[i].orderText.gameObject.SetActive(false);
                GameManager.instance.cars[i].sounds.GetChild(0).gameObject.SetActive(false);

                if (winnerId != i)
                {
                    Transform lose = Instantiate(losePrefab, GameManager.instance.cars[i].transform);
                    lose.localPosition = new Vector3(0, 1, 0);
                    lose.localScale = new Vector3(1f, 1, 1f);
                    Destroy(lose.gameObject,2f);
                }
                else
                {
                    Transform fireworkWin = Instantiate(fireworkPrefab, GameManager.instance.cars[i].transform);
                    fireworkWin.localPosition = new Vector3(0, 1, 0);
                    fireworkWin.localScale = new Vector3(1f, 1, 1f);
                    Destroy(fireworkWin.gameObject, 2f);
                }
                if (i > 1)
                {
                    RCC_AICarController rai = GameManager.instance.cars[i].GetComponent<RCC_AICarController>();
                    rai.waypointsContainer = waypointContainer;
                    rai._AIType = RCC_AICarController.AIType.ChaseTarget;
                    rai.targetChase = chaseTarget[lv];
                    waypointContainer.target = chaseTarget[lv];
                    waypointContainer.type = RCC_AIWaypointsContainer.Type.ChaseThisObject;
                }
            }
            if(winnerId == 0)
            {
                SoundManager.instance.PlaySoundOneShot("winsound", CustomSettings.instance.volume * 2f);
                int diamond = PlayerPrefs.GetInt("diamond");
                //PlayerPrefs.SetInt("diamond", diamond + racePart[GameManager.instance.raceId].racePrice);
                if(PlayerPrefs.GetInt("currentrace") < 9)
                PlayerPrefs.SetInt("race" + (PlayerPrefs.GetInt("currentrace") + 1),1);
                RCC_AICarController rai = GameManager.instance.cars[0].AddComponent<RCC_AICarController>();
                //rai.lap = 2;
                rai.waypointsContainer = waypointContainer;
                rai._AIType = RCC_AICarController.AIType.ChaseTarget;
                rai.targetChase = chaseTarget[lv];
                waypointContainer.target = chaseTarget[lv];
                waypointContainer.type = RCC_AIWaypointsContainer.Type.ChaseThisObject;
                //waypointContainer.target = RCC_AIWaypointsContainer.Type.ChaseThisObject;
                //GameManager.instance.rccCam.cameraMode = RCC_Camera.CameraMode.WHEEL;
                GameManager.instance.rccCam.ChangeCamera(RCC_Camera.CameraMode.WHEEL);
                GameManager.instance.cars[0].transform.Find("All Audio Sources").gameObject.SetActive(false);


            }
            else
            {
                SoundManager.instance.PlaySoundOneShot("losesound", CustomSettings.instance.volume *2f);
                RCC_AICarController rai = GameManager.instance.cars[0].AddComponent<RCC_AICarController>();
                //rai.lap = 2;
                rai.waypointsContainer = waypointContainer;
                rai._AIType = RCC_AICarController.AIType.ChaseTarget;
                rai.targetChase = chaseTarget[lv];
                waypointContainer.target = chaseTarget[lv];
                waypointContainer.type = RCC_AIWaypointsContainer.Type.ChaseThisObject;
                //GameManager.instance.rccCam.cameraMode = RCC_Camera.CameraMode.WHEEL;
                GameManager.instance.rccCam.ChangeCamera(RCC_Camera.CameraMode.WHEEL);
                GameManager.instance.cars[0].sounds.GetChild(0).gameObject.SetActive(false);
                GameManager.instance.cars[0].transform.Find("All Audio Sources").gameObject.SetActive(false);

            }
            yield return new WaitForSeconds(7f);
            GameManager.instance.playerCar.GetComponent<RCC_CarControllerV3>().canControl = false;
            if (winnerId == 0)
            {
                GameManager.instance.blurPP.SetActive(true);
                GameManager.instance.winnerPanel.SetActive(true);
                GameManager.instance.diamondText.transform.parent.SetParent(GameManager.instance.winnerPanel.transform);
                GameManager.instance.winDiamondText.transform.parent.gameObject.SetActive(false);
                StopCoroutine(GameManager.instance.DiamondSet(racePart[GameManager.instance.raceId].racePrice, true));
                StartCoroutine(GameManager.instance.DiamondSet(racePart[GameManager.instance.raceId].racePrice, true));
                if (PlayerPrefs.GetInt("currentrace") >= 8)
                {
                    GameManager.instance.nextLevelButton.SetActive(false);
                    GameManager.instance.congratText.text = "Congratulations!";
                    GameManager.instance.congratText.GetComponent<RectTransform>().anchoredPosition-= new Vector2(0,50);
                    GameManager.instance.winnerButtonsMain.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                }
               
            }
            else
            {
                GameManager.instance.blurPP.SetActive(true);
                GameManager.instance.loserPanel.SetActive(true);
                GameManager.instance.loseDiamondText.text = PlayerPrefs.GetInt("diamond").ToString();
            }
        }
        internal IEnumerator StartRace()
        {
            int level = PlayerPrefs.GetInt("currentrace");
            racePart[level].levelObject.transform.GetChild(0).transform.GetChild(0).gameObject.SetActive(true);
            yield return new WaitForSeconds(1.5f);
            postProcessForLights.SetActive(true);
            racePart[level].levelObject.transform.GetChild(0).transform.GetChild(0).gameObject.SetActive(false);
            racePart[level].levelObject.transform.GetChild(0).transform.GetChild(1).gameObject.SetActive(true);
            yield return new WaitForSeconds(1.5f);
            racePart[level].levelObject.transform.GetChild(0).transform.GetChild(1).gameObject.SetActive(false);
            racePart[level].levelObject.transform.GetChild(0).transform.GetChild(2).gameObject.SetActive(true);
            yield return new WaitForSeconds(0.4f);
            postProcessForLights.SetActive(false);
            racePart[level].levelObject.transform.GetChild(0).transform.GetChild(2).gameObject.SetActive(false);
            GameManager.instance.topRightRace.SetActive(true);
            for (int i = 0; i < GameManager.instance.cars.Count; i++)
            {
                //GameManager.instance.cars[i].GetComponent<RCC_CarControllerV3>().rigid.isKinematic = false;
                GameManager.instance.cars[i].GetComponent<RCC_CarControllerV3>().canControl = true;
                //Debug.Log(GameManager.instance.cars[i].transform.forward.z + 5);
                //GameManager.instance.cars[i].GetComponent<RCC_CarControllerV3>().rigid.AddTorque(new Vector3( 0, 2, GameManager.instance.cars[i].transform.forward.z + 20) * 10, ForceMode.Acceleration);
                //GameManager.instance.cars[i].GetComponent<RCC_CarControllerV3>().canControl = true;
                //GameManager.instance.cars[i].GetComponent<RCC_CarControllerV3>().ChangeGear(1);
                //Debug.Log(i.ToString() + " " + GameManager.instance.cars[i].GetComponent<RCC_CarControllerV3>().currentGear.ToString());
                GameManager.instance.cars[i].orderText.gameObject.SetActive(true);
                GameManager.instance.cars[i].sounds.GetChild(0).gameObject.SetActive(true);
            }
            startTheRace = true;
        }
        void Update()
        {

            //AudioListener[] al = GameObject.FindObjectsOfType<AudioListener>();
            //for (int i = 0; i<al.Length; i++)
            //{
            //    Debug.Log(al[i].gameObject.name);
            //}
            if (startTheRace)
            {
                Control();
            }
            if (cameraAnim.start)
            {
                StartCoroutine(cameraAnim.Animation());
                cameraAnim.start = false;
            }
            if(cameraAnim.raceStart)
            {
                StartCoroutine(StartRace());
                cameraAnim.raceStart = false;
            }
        }
        internal void Control()
        {
            for (int i = 0; i < GameManager.instance.cars.Count; i++)
            {
                CalculatePositionForRacers(i);
            }
        }
        internal void CalculatePositionForRacers(int playerIndex)
        {
            int maxPos = 1;
            for (int i = 0; i < GameManager.instance.cars.Count; i++)
            {
                if (GameManager.instance.cars[playerIndex].cpoint < GameManager.instance.cars[i].cpoint)
                {
                    maxPos = maxPos + 1;
                }
            }
            GameManager.instance.cars[playerIndex].order = maxPos;
        }
    }
    [Serializable]
    public class RacePart
    {
        [SerializeField] internal int raceId;
        [SerializeField] internal List<Transform> startPoints;
        [SerializeField] internal List<GameObject> brakeZones;
        [SerializeField] internal GameObject levelObject;
        [SerializeField] internal int checkpointRange = 2;
        [SerializeField] internal Vector3 checkpointScale = new Vector3(37, 21, 1);
        [SerializeField] internal int racePrice;
        [SerializeField] internal int crossId;
    }

    [Serializable]
    [ExecuteInEditMode]
    public class CameraAnimation
    {
        [SerializeField] Transform animationCamera;
        [SerializeField] Transform normalCamera;
        [SerializeField] internal float offsetForPos = 11;
        [SerializeField] internal Vector3 startPoint;
        [SerializeField] internal Vector3 startEuler;
        [SerializeField] internal Vector3[] positions = new Vector3[3];
        [SerializeField] internal Vector3[] eulerAngles = new Vector3[3];
        [SerializeField] internal Vector3[] offsetForOne = new Vector3[10];
        [SerializeField] internal Vector3[] offsetForTwo = new Vector3[10];
        [SerializeField] internal Vector3[] offsetForZeroRot = new Vector3[10];
        [SerializeField] internal Vector3[] offsetForOneRot = new Vector3[10];
        [SerializeField] internal Vector3[] offsetForTwoRot = new Vector3[10];
        [SerializeField] internal float time;
        [SerializeField] internal bool start = false;
        int one;
        int two;
        internal bool raceStart;
        internal void SetTheThings(Vector3 carPos, Vector3 carEuler)
        {
            int level = PlayerPrefs.GetInt("currentrace");
            normalCamera.GetComponentInChildren<Camera>().enabled = false;
            animationCamera.gameObject.SetActive(true);
            positions[0] = carPos + new Vector3(-offsetForPos, 0 , 0);
            startPoint = positions[0];
            animationCamera.position = startPoint;
            positions[1] = carPos + offsetForOne[level];
            positions[2] = carPos + offsetForTwo[level];
            ///////////////////////////////////////////////
            eulerAngles[0] = carEuler + offsetForZeroRot[level];
            startEuler = eulerAngles[0];
            animationCamera.eulerAngles = eulerAngles[0];
            eulerAngles[1] = carEuler + offsetForOneRot[level];
            eulerAngles[2] = carEuler + offsetForTwoRot[level];
        }
        internal IEnumerator Animation()
        {
            for (int i = 1; i < 3; i++)
            {
                one = LeanTween.move(animationCamera.gameObject, positions[i], time).setEaseOutBack().setOnComplete(() =>
                {
                    LeanTween.cancel(animationCamera.gameObject, one);
                }).id;
                two = LeanTween.rotateLocal(animationCamera.gameObject, eulerAngles[i], time).setEaseOutBack().setOnComplete(() =>
                {
                    LeanTween.cancel(animationCamera.gameObject, two);
                }).id;

                yield return new WaitForSeconds(time);
            }
            one = LeanTween.move(animationCamera.gameObject, normalCamera.position, time).setEaseOutBack().setOnComplete(() =>
            {
                raceStart = true;
                LeanTween.cancel(animationCamera.gameObject,one);
                animationCamera.gameObject.SetActive(false);
                normalCamera.GetComponentInChildren<Camera>().enabled = true;
            }).id;
            two = LeanTween.rotate(animationCamera.gameObject, normalCamera.eulerAngles, time).setEaseOutBack().id;
        }
    }
}
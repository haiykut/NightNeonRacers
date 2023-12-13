namespace haiykut
{
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using haiykut;
using UnityEngine.UI;
using UnityEngine.Rendering;
using Unity.VisualScripting;
using JetBrains.Annotations;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [SerializeField] List<GameObject> carPrefabs;
    [HideInInspector][SerializeField] List<RCC_AIWaypointsContainer> waypoints;
    internal int playerCarPointer;
    [SerializeField]internal PlayerScript playerCar;
    public List<PlayerScript> cars;
    [SerializeField] List<Transform> startPositions;
    public GameObject orderTextPrefab;
    public GameObject gamePanel;
    public GameObject finishPanel;
    public Text winnerText;
    internal int playMode;
    internal int raceId;
    [SerializeField] GameObject carBlastManager;
    [SerializeField] GameObject driftManager;
    [SerializeField] GameObject raceManager;
    [SerializeField] internal SpawnSystem spawn;
    public List<GameObject> freeModeObjects;
    [SerializeField] internal GameObject pausePanelMain;
    [SerializeField] internal GameObject winnerPanel;
    [SerializeField] internal GameObject loserPanel;
    [SerializeField] internal GameObject rewindPanel;
    [SerializeField] internal Text diamondText;
    [SerializeField] internal Text diamondTexts;
    [SerializeField] internal Text congratText;
    [SerializeField] internal Transform winnerButtonsMain;
    [SerializeField] internal RCC_Camera rccCam;
    internal bool sceneLoaded = false;
    [SerializeField] internal GameObject blurPP;
    [SerializeField] internal GameObject[] rainObjects;
    [SerializeField] internal Vector3[] rainObjectsRootPositions;
    [SerializeField] internal Color mainColor;
    [SerializeField] internal GameObject ppr;
    [SerializeField] internal Text pauseDiamondText;
    [SerializeField] internal Text winDiamondText;
    [SerializeField] internal Text loseDiamondText;
    [SerializeField] internal GameObject mainPanel;
    [SerializeField] GameObject guide;
        internal bool isGuiding;
        [SerializeField] internal GameObject topRightRace;
    private void Awake()
    {
        instance = this;
        mainPanel.SetActive(true);
        playMode = PlayerPrefs.GetInt("mode");
        playerCarPointer = PlayerPrefs.GetInt("playercar");
        raceId = PlayerPrefs.GetInt("currentrace");
        if(playMode == 0)
        {
            for (int i = 0; i<freeModeObjects.Count; i++)
            {
                freeModeObjects[i].SetActive(false);
            }
            carBlastManager.SetActive(false);
            driftManager.SetActive(false);
         
        }
        else if(playMode == 1)
        {
            raceManager.SetActive(false);
            driftManager.SetActive(false);

        }
        else
        {
            carBlastManager.SetActive(false);
            raceManager.SetActive(false);
        }

            

    }
        public void SkipGuide()
        {
            StopCoroutine(Guide());
            guideSkipped = true;
            guide.SetActive(false);
            blurPP.SetActive(false);
            mainPanel.SetActive(true);
            if(playMode != 0)
            playerCar.sounds.GetChild(0).gameObject.SetActive(true);
            Time.timeScale = 1;
            PlayerPrefs.SetInt("guide", PlayerPrefs.GetInt("guide") + 1);
        }
        bool guideSkipped = false;
        public IEnumerator Guide()
        {
            if (PlayerPrefs.GetInt("guide") < 3)
            {
                blurPP.SetActive(true);
                guide.SetActive(true);
                mainPanel.SetActive(false);
                playerCar.sounds.GetChild(0).gameObject.SetActive(false);
                Time.timeScale = 0.1f;
                yield return new WaitForSeconds(0.4f);
                if (!guideSkipped)
                {
                    guide.SetActive(false);
                    blurPP.SetActive(false);
                    mainPanel.SetActive(true);
                    if (playMode != 0)
                        playerCar.sounds.GetChild(0).gameObject.SetActive(true);
                    Time.timeScale = 1;
                    PlayerPrefs.SetInt("guide", PlayerPrefs.GetInt("guide") + 1);
                }
            }

        }
        void Start()
        {
            AdsManager.instance.BannerSetter("scene_night");
            Debug.Log(PlayerPrefs.GetInt("guidee").ToString());
            //RaceManager.instance.cameraAnim.start = true;
            playerCar = Instantiate(LoadingManager.instance.cars[playerCarPointer]).gameObject.GetComponent<PlayerScript>(); //TODO: deactivated for test.
            //StartCoroutine(Guide());

            //Debug.Log(startPositions[0].position.ToString());
            playerCar.id = 0;
            cars.Add(playerCar);
            diamondText.text = PlayerPrefs.GetInt("diamond").ToString();

            if (playMode == 0)
            {
                playerCar.transform.SetPositionAndRotation(RaceManager.instance.racePart[raceId].startPoints[0].position, RaceManager.instance.racePart[raceId].startPoints[0].rotation);
                for (int i = 0; i < 5; i++)
                {
                    int tmpId = Random.Range(0, 10);
                    if (tmpId == 1)
                        tmpId += 1;
                    Transform aiCarr = Instantiate(LoadingManager.instance.cars[tmpId].transform);
                    PlayerScript aiCar = aiCarr.GetComponent<PlayerScript>();
                    aiCar.ai = true;
                    aiCarr.Find("All Audio Sources").gameObject.SetActive(false);
                    if (aiCar.gameObject.GetComponent<RCC_AICarController>() == null)
                    {
                        RCC_AICarController rccAi = aiCar.AddComponent<RCC_AICarController>();
                        rccAi.carController.canControl = false;
                        rccAi.carController.rigid.angularDrag = 0;
                        rccAi.carController.rigid.angularVelocity = Vector3.zero;
                        //rccAi.carController.rigid. = Vector3.zero;

                        //rccAi.waypointsContainer = RaceManager.instance.waypoints[raceId];

                    }
                    aiCar.transform.SetPositionAndRotation(RaceManager.instance.racePart[raceId].startPoints[i + 1].position, RaceManager.instance.racePart[raceId].startPoints[i + 1].rotation);
                    aiCar.id = i + 1;
                    cars.Add(aiCar);

                }
                FindObjectOfType<TopRight>().gameObject.SetActive(false);
                GameManager.instance.spawn.gameObject.SetActive(false);
                GameManager.instance.topRightRace.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(delegate { GameManager.instance.playerCar.RaceSpawnForMobile(); });
                StartCoroutine(CloseTraffic());
            }
            else
            {
                int tmpPlayerTransformId = Random.Range(0, startPositions.Count);
                //Debug.Log("SPAWNED POINT NUMBER: " + tmpPlayerTransformId.ToString());
                playerCar.transform.SetPositionAndRotation(startPositions[tmpPlayerTransformId].position + new Vector3(0, 2, 0), startPositions[tmpPlayerTransformId].rotation);
                //spawn = (SpawnSystem)FindObjectOfType(typeof(SpawnSystem));
                spawn.player = playerCar.transform;
                spawn.playerCols = playerCar.transform.GetComponentsInChildren<Collider>();
                //playerCar.transform.position = startPositions[0].position; //( + new Vector3(10, 0,0), startPositions[0].localRotation);
                //playerCar.transform.localRotation = startPositions[0].localRotation; //( + new Vector3(10, 0,0), startPositions[0].localRotation);
                //Debug.Log("OC UNTY");
            }

            sceneLoaded = true;

        }

    [SerializeField] internal GameObject nextLevelButton;
        public void ChooseNext(bool next)
        {
            SoundManager.instance.PlaySoundOneShot("click", 1f);
            if (next)
            {
                if (raceId < 9)
                {
                    PlayerPrefs.SetInt("currentrace", raceId + 1);
                    raceId = PlayerPrefs.GetInt("currentrace");
                }
                //SceneManager.UnloadSceneAsync(SceneManager.GetSceneByName("scene_night"));
                winnerPanel.SetActive(false);
                loserPanel.SetActive(false);
                LoadingManager.instance.LoadScene("scene_night");
                playerCar.checkRain = false;
            }
            else
            {
                loserPanel.SetActive(false);
                winnerPanel.SetActive(false);

                //SceneManager.UnloadSceneAsync(SceneManager.GetSceneByName("scene_night"));
                LoadingManager.instance.LoadScene("scene_night");
                playerCar.checkRain = false;

            }
        }

        IEnumerator CloseTraffic()
        {
            yield return new WaitForSeconds(1f);
            if (GameObject.Find("TrafficCarsContainer") != null)
            {
                GameObject.Find("TrafficCarsContainer").SetActive(false);

            }
        }
        public void BackToGarage()
        {
            SoundManager.instance.PlaySoundOneShot("click", 1f);
            Time.timeScale = 1;
            pausePanelMain.SetActive(false);
            winnerPanel.SetActive(false);
            loserPanel.SetActive(false);
            LoadingManager.instance.LoadScene("Garage");
            playerCar.checkRain = false;
            //Destroy(rccCam.gameObject);

        }
    public void Next()
    {
        SoundManager.instance.PlaySoundOneShot("click", 1f);
        finishPanel.SetActive(false);
        LoadingManager.instance.LoadScene("Garage");
    }

    public void Pause(bool pause)
    {
        SoundManager.instance.PlaySoundOneShot("click", 1f);
        if (!pause)
        {
            blurPP.SetActive(true);
            mainPanel.SetActive(false);
            pausePanelMain.SetActive(true);
            pauseDiamondText.text = PlayerPrefs.GetInt("diamond").ToString();
            Time.timeScale = 0;
            AudioListener.pause = true;
            AudioListener.volume = 0;
            pause = true;
        }
        else
        {
            blurPP.SetActive(false);
            mainPanel.SetActive(true);
            pausePanelMain.SetActive(false);
            Time.timeScale = 1;
                AdsManager.instance.isPausedGame = false;
            if(SettingsManagerNew.Instance.sounds == 0)
            {
                AudioListener.pause = false;
                AudioListener.volume = 1;
            }
            else
            {
                AudioListener.pause = true;
                AudioListener.volume = 0;
            }

            pause = false;
        }
    }
    public void Reward()
    {
            if (Time.timeScale == 0)
                AdsManager.instance.isPausedGame = true;
            else
                AdsManager.instance.isPausedGame = false;
            AdsManager.instance.freeCar = false;
            AdsManager.instance.ShowRewardedAd();
        //RewardManager.instance.Reward();

    }

    public IEnumerator DiamondSet(int value, bool visible)
    {
        if (value > 0)
        {
            if (!visible)
            {
                //pauseDiamondText.transform.parent.gameObject.SetActive(false);
                int diamond = PlayerPrefs.GetInt("diamond");
                diamond += value;
                PlayerPrefs.SetInt("diamond", diamond);
                diamondText.text = "+" + value.ToString();
                diamondText.transform.parent.localScale = Vector3.zero;
                diamondText.transform.parent.gameObject.SetActive(true);
                LeanTween.scale(diamondText.transform.parent.gameObject, Vector3.one, 0.5f).setEaseInCubic().setOnComplete(() =>
                {
                    //yield return new WaitForSeconds(2);

                });
                yield return new WaitForSeconds(2);
                diamondText.transform.parent.gameObject.SetActive(false);
                //pauseDiamondText.transform.parent.gameObject.SetActive(true);
            }
            else
            {
                //pauseDiamondText.transform.parent.gameObject.SetActive(false);
                int diamond = PlayerPrefs.GetInt("diamond");
                diamond += value;
                PlayerPrefs.SetInt("diamond", diamond);
                diamondText.text = "+" + value.ToString();
                winDiamondText.text = diamond.ToString();
                diamondText.transform.parent.localScale = Vector3.zero;
                diamondText.transform.parent.gameObject.SetActive(true);
                LeanTween.scale(diamondText.transform.parent.gameObject, Vector3.one, 0.5f).setEaseInCubic().setOnComplete(() =>
                {
                    //yield return new WaitForSeconds(2);

                });
                yield return new WaitForSeconds(2);
                diamondText.transform.parent.gameObject.SetActive(false);
                winDiamondText.transform.parent.gameObject.SetActive(true);
                //pauseDiamondText.transform.parent.gameObject.SetActive(true);
            }
        }
    }
          
      
        
    }





}
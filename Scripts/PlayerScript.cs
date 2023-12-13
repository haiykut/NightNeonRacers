using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using haiykut;
using UnityEngine.UI;

public class PlayerScript : MonoBehaviour
{
    internal bool ai;
    internal int id;
    [SerializeField]internal int skor = 0;
    [SerializeField] internal int chId;
    [SerializeField] internal float distance;
    [SerializeField]internal float cpoint;
    [SerializeField]internal int order;
    int mode;
    internal TextMesh orderText;
    [SerializeField][HideInInspector]internal List<GameObject> checkList = new List<GameObject>();
    [SerializeField] internal Transform sounds;
    Transform arrow;
    bool sceneStarted;
    internal Vector3 lastPosition;
    internal Vector3 lastRotation;
    [SerializeField] Transform nose;
    Camera camera;
    internal bool checkRain = true;
    private void Awake()
    {
        foreach (Transform sound in transform)
        {
            if (sound.gameObject.active == true)
                if (sound.CompareTag("Sounds")) sounds = sound;
        }
    }
    void Start()
    {
        camera = GameManager.instance.rccCam.GetComponentInChildren<Camera>();
        nose = new GameObject("nose").transform;
        nose.SetParent(transform);
        nose.SetLocalPositionAndRotation(new Vector3(0,0,5), Quaternion.identity);
        mode = PlayerPrefs.GetInt("mode");
        lastPosition = transform.position;
        lastRotation = transform.eulerAngles;
        if(mode == 0)
        {
            orderText = Instantiate(GameManager.instance.orderTextPrefab).GetComponent<TextMesh>();
            orderText.transform.SetParent(transform);
            orderText.transform.localPosition = new Vector3(0, 1f, 0);
            orderText.transform.localEulerAngles = Vector3.zero;
            //FindObjectOfType<TopRight>().gameObject.SetActive(false);
            
        }
      /*  arrow = Instantiate(RaceManager.instance.arrowPrefab);
        arrow.SetParent(transform);
        arrow.localPosition = new Vector3(0, 3.5f, 0);*/
        sceneStarted = true;
        if (GameManager.instance.sceneLoaded)
            if (ai)
                sounds.gameObject.SetActive(false);
        
    }
    public IEnumerator Spawn(Transform player, Vector3 pos, Vector3 rot)
    {
        player.GetComponent<Rigidbody>().isKinematic = true;
        player.position = pos + new Vector3(0,3,0);
        player.eulerAngles = rot;
        yield return new WaitForSeconds(0.2f);
        player.GetComponent<Rigidbody>().isKinematic = false;
    }


    public IEnumerator SpawnForMobile()
    {

        GetComponent<Rigidbody>().isKinematic = true;
        GetComponent<RCC_CarControllerV3>().canControl = false;
            SoundManager.instance.PlaySoundForAWhile("rewindsound", true);
        LeanTween.move(gameObject, lastPosition, 2f);
        LeanTween.rotate(gameObject, lastRotation, 2f).setOnComplete(() =>
        {
            LeanTween.cancel(gameObject);
            GetComponent <Rigidbody>().isKinematic = false;
            GetComponent<RCC_CarControllerV3>().canControl = true;
            //SoundManager.instance.ReturnThemeMusic();
            isRewinding = false;
            if (PlayerPrefs.GetInt("music") == 1)
                SoundManager.instance.ReturnThemeMusic();
            else
                SoundManager.instance.StopSound();

            GameManager.instance.ppr.SetActive(false);
            GameManager.instance.rewindPanel.SetActive(false);
            GameManager.instance.playerCar.sounds.GetChild(0).gameObject.SetActive(true);
            GameManager.instance.topRightRace.transform.GetChild(1).gameObject.SetActive(true);
        });
        yield return new WaitForSeconds(2f);
    }


    bool isRewinding = false;

    public void RaceSpawnForMobile()
    {

        if (!RaceManager.instance.hasRaceFinished)
        {
          
                if (!isRewinding)
                {
                    GameManager.instance.topRightRace.transform.GetChild(1).gameObject.SetActive(false);
                    GameManager.instance.ppr.SetActive(true);
                    GameManager.instance.rewindPanel.SetActive(true);
                    GameManager.instance.playerCar.sounds.GetChild(0).gameObject.SetActive(false);
                    StartCoroutine(SpawnForMobile());
                    isRewinding = true;
                }
            

        }
        
    }
    void Update()
    {
        if (sceneStarted)
        {
            if (mode == 0)
            {

                // PlayerScript komut dosyasi
                RaceManager.instance.chId = chId;
                if (chId < RaceManager.instance.checkList.Count)
                {
                    distance = Vector3.Distance(RaceManager.instance.checkList[chId].position, transform.position);
                    cpoint = skor - distance;
                }
                if (Input.GetKeyDown(KeyCode.R) && id == 0 && !RaceManager.instance.hasRaceFinished)
                {
                    if (chId > 0)
                    {
                        if (!isRewinding)
                        {
                            GameManager.instance.ppr.SetActive(true);
                            GameManager.instance.rewindPanel.SetActive(true);
                            GameManager.instance.playerCar.sounds.GetChild(0).gameObject.SetActive(false);
                            StartCoroutine(SpawnForMobile());
                            isRewinding = true;
                        }
                    }
                }

                orderText.text = order.ToString();
            }

        }
    
    }


    private void FixedUpdate()
    {
        if (checkRain)
        {
            int layerMask = 1 << 15;
            RaycastHit hit;
            // Does the ray intersect any objects excluding the player layer
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.up), out hit, 100f, layerMask))
            {
                Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.up) * hit.distance, Color.yellow);
                foreach (GameObject go in GameManager.instance.rainObjects)
                {
                    if (go.transform.parent != null)
                    {
                        go.transform.SetParent(null);
                        //Debug.Log("1");
                    }
                    else
                        break;
                }

            }
            else
            {
                Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.up) * 1000, Color.white);
                for (int i = 0; i < GameManager.instance.rainObjects.Length; i++)
                {
                    if (GameManager.instance.rainObjects[i].transform.parent == null)
                    {
                        GameManager.instance.rainObjects[i].transform.SetParent(camera.transform);
                        GameManager.instance.rainObjects[i].transform.localPosition = GameManager.instance.rainObjectsRootPositions[i];
                        //Debug.Log("2");

                    }
                    else
                        break;
                }
            }
        }
          
        
      
    }
    /* private void LateUpdate()
     {
         if (sceneStarted)
         {
             if (chId  < RaceManager.instance.checkList.Count)
             {
                 arrow.LookAt(RaceManager.instance.checkList[chId]);
             }
         } 
     }*/
}

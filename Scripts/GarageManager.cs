using GoogleMobileAds.Api;
using haiykut;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Security;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class GarageManager : MonoBehaviour
{
    [SerializeField] GameObject mainPanel;
    [SerializeField] GameObject selectPanel;
    [SerializeField] GameObject cars;
    [SerializeField] internal GameObject modeText;
    internal int pointer;
    int carCount;
    int[] carPrices = { 0, 100, 250, 350, 500, 600, 750, 850, 1000, 1150 };
    int diamondCount;
    [SerializeField] GameObject notEnoughText;
    internal string modeMainText;
    [SerializeField] int closeTime;
    [SerializeField] RCC_Settings rccSettings;
    RCC_Settings.BehaviorType behaviorTypeNormal = RCC_Settings.BehaviorType.SemiArcade;
    RCC_Settings.BehaviorType behaviorTypeDrift = RCC_Settings.BehaviorType.Drift;
    [SerializeField] List<Button> levelButtons;
    [SerializeField] GameObject levelSelectPanel;
    [SerializeField] internal Text diamondText;
    //[SerializeField] float[] yOffsetForCars;
    [SerializeField] float yOffsetForCars;
    [SerializeField] List<Button> raceButtons;
    [SerializeField] Transform lockImage;
    [SerializeField] CameraOrbit cameraOrbit;
    [SerializeField] Camera camera;
    [SerializeField] int smoothTime;
    bool cameraChange_1 = false;
    bool cameraChange_2 = false;
    [SerializeField] LayerMask cullGr;
    [SerializeField] LayerMask cullGr2;
    [SerializeField] Canvas mainCanvas;
    [SerializeField] Transform theThing;
    [SerializeField] Transform theThingMain;
    [SerializeField] GameObject postProcessForBlur;
    [SerializeField] Transform poss;
    Vector3 carsPosOriginal;
    Vector3 carsRotOriginal;
    Vector3 thingPosOriginal;
    Vector3 thingRotOriginal;
    Vector3 camPosOrj;
    Vector3 camRotOrj;
    [SerializeField] VolumeProfile volume;
    [SerializeField]DepthOfField dof;
    [SerializeField] Image[] images;
    [SerializeField] bool GD;
    [SerializeField] GameObject logo;
    private void Awake()
    {
        //volume = FindObjectOfType<Volume>()?.profile;
        //if (!volume) throw new System.NullReferenceException(nameof(UnityEngine.Rendering.VolumeProfile));

        if (!volume.TryGet(out dof)) throw new System.NullReferenceException(nameof(dof));

        
        PlayerPrefs.SetInt("race" + 0, 1);
        PlayerPrefs.SetInt("playercar" + 0, 1);
        if (!PlayerPrefs.HasKey("quality"))
            PlayerPrefs.SetInt("quality", 2);
        if (!PlayerPrefs.HasKey("music"))
            PlayerPrefs.SetInt("music", 1);
        if (!PlayerPrefs.HasKey("sounds"))
            PlayerPrefs.SetInt("sounds", 0);
        if (!PlayerPrefs.HasKey("fps"))
            PlayerPrefs.SetInt("fps", 1);

        //Debug.Log(PlayerPrefs.GetInt("race"));
    }
    private void Update()
    {
        if (cameraChange_1)
        {
            camera.fieldOfView -= Time.deltaTime * smoothTime;
            if (camera.fieldOfView <= 60)
            {
                camera.fieldOfView = 60;
                cameraOrbit.touchable = true;
                cameraChange_1 = false;

            }
               
        }
        if (cameraChange_2)
        {
            camera.fieldOfView += Time.deltaTime * smoothTime;
            if (cameraOrbit.distance > cameraOrbit.distanceMin)
                cameraOrbit.distance -= Time.deltaTime * smoothTime / 10;
            else
                cameraOrbit.distance = cameraOrbit.distanceMin;
          
            if (camera.fieldOfView >= 75)
            {
                camera.fieldOfView = 75;
                cameraChange_2 = false;
                
            }

        }
    }
    IEnumerator logoWait()
    {
        yield return new WaitForEndOfFrame();
        logo.SetActive(true);
        logo.transform.GetChild(0).localScale = Vector3.zero;
        yield return new WaitForSeconds(1.2f);
        LeanTween.scale(logo.transform.GetChild(0).gameObject, Vector3.one, .4f);
    }
    void Start()
    {
        StartCoroutine(logoWait());
        dof.active = false;
        AdsManager.instance.BannerSetter("Garage");
        carsPosOriginal = cars.transform.position;
        carsRotOriginal = cars.transform.eulerAngles;
        thingPosOriginal = theThing.localPosition;
        thingRotOriginal = theThing.localEulerAngles;
        camera = cameraOrbit.GetComponent<Camera>();
        camPosOrj = camera.transform.position;
        camRotOrj = camera.transform.eulerAngles;
        carCount = LoadingManager.instance.cars.Length;
        diamondCount = PlayerPrefs.GetInt("diamond");
        diamondText.text = diamondCount.ToString();
        for (int i = 0; i < raceButtons.Count; i++)
        {
            if(PlayerPrefs.GetInt("race" + i) == 1)
            {
                raceButtons[i].interactable = true;
            }
            else
            {
                raceButtons[i].interactable = false;
                Transform t = Instantiate(lockImage);
                t.SetParent(raceButtons[i].transform);
                t.GetComponent<Image>().rectTransform.anchoredPosition = Vector2.zero;
                Destroy(raceButtons[i].GetComponent<HoverEffectButton>());
            }
        }

    }

    public void SelectableButton(bool left)
    {
        StopCoroutine(CloseGameOject(notEnoughText));
        LeanTween.cancel(notEnoughText.gameObject);
        notEnoughText.transform.localScale = Vector3.zero;
        modeText.GetComponent<Button>().interactable = true;
        SoundManager.instance.PlaySoundOneShot("click", 1f);
        if (left)
        {
            if(pointer > 0)
            {
                pointer--;
                cars.transform.GetChild(pointer+1).gameObject.SetActive(false);
                cars.transform.GetChild(pointer).gameObject.SetActive(true);
                CarAnimation(pointer);
                notEnoughText.SetActive(false);
                if (PlayerPrefs.GetInt("playercar" + pointer) == 0)
                {
                    if (GD && (pointer == 2 || pointer == 8))
                    {
                        modeText.GetComponentInChildren<Text>().text = "Unlock Free";
                        modeText.GetComponentInChildren<Text>().color = SettingsManager.instance.activeColor;
                        modeText.GetComponentInChildren<Text>().fontSize = 35;
                    }
                    else
                    {
                        modeText.GetComponentInChildren<Text>().text = "BUY: " + carPrices[pointer];
                        modeText.GetComponentInChildren<Text>().color = SettingsManager.instance.passiveColor;
                        modeText.GetComponentInChildren<Text>().fontSize = 40;

                    }
                }
                else
                {
                    modeText.GetComponentInChildren<Text>().text = modeMainText;
                    modeText.GetComponentInChildren<Text>().color = SettingsManager.instance.passiveColor;
                    modeText.GetComponentInChildren<Text>().fontSize = 40;
                }


            }
            else
            {
                pointer = carCount - 1;
                cars.transform.GetChild(0).gameObject.SetActive(false);
                cars.transform.GetChild(pointer).gameObject.SetActive(true);
                CarAnimation(pointer);
                notEnoughText.SetActive(false);
                if (PlayerPrefs.GetInt("playercar" + pointer) == 0)
                {
                    if (GD && (pointer == 2 || pointer == 8))
                    {
                        modeText.GetComponentInChildren<Text>().text = "Unlock Free";
                        modeText.GetComponentInChildren<Text>().color = SettingsManager.instance.activeColor;
                        modeText.GetComponentInChildren<Text>().fontSize = 35;
                    }
                    else
                    {
                        modeText.GetComponentInChildren<Text>().text = "BUY: " + carPrices[pointer];
                        modeText.GetComponentInChildren<Text>().color = SettingsManager.instance.passiveColor;
                        modeText.GetComponentInChildren<Text>().fontSize = 40;
                    }
                }
                else
                {
                    modeText.GetComponentInChildren<Text>().text = modeMainText;
                    modeText.GetComponentInChildren<Text>().color = SettingsManager.instance.passiveColor;
                    modeText.GetComponentInChildren<Text>().fontSize = 40;
                }
            }
            
     
            

        }
        else
        {
            if (pointer < carCount-1)
            {
                pointer++;
                cars.transform.GetChild(pointer - 1).gameObject.SetActive(false);
                cars.transform.GetChild(pointer).gameObject.SetActive(true);
                CarAnimation(pointer);
                notEnoughText.SetActive(false);

                if (PlayerPrefs.GetInt("playercar" + pointer) == 0)
                {
                    if (GD && (pointer == 2 || pointer == 8))
                    {
                        modeText.GetComponentInChildren<Text>().text = "Unlock Free";
                        modeText.GetComponentInChildren<Text>().color = SettingsManager.instance.activeColor;
                        modeText.GetComponentInChildren<Text>().fontSize = 35;
                    }
                    else
                    {
                        modeText.GetComponentInChildren<Text>().text = "BUY: " + carPrices[pointer];
                        modeText.GetComponentInChildren<Text>().color = SettingsManager.instance.passiveColor;
                        modeText.GetComponentInChildren<Text>().fontSize = 40;

                    }


                }
                else
                {
                    modeText.GetComponentInChildren<Text>().text = modeMainText;
                    modeText.GetComponentInChildren<Text>().color = SettingsManager.instance.passiveColor;
                    modeText.GetComponentInChildren<Text>().fontSize = 40;
                }
            }
            else
            {
                pointer = 0;
                cars.transform.GetChild(carCount - 1).gameObject.SetActive(false);
                cars.transform.GetChild(pointer).gameObject.SetActive(true);
                CarAnimation(pointer);
                notEnoughText.SetActive(false);

                if (PlayerPrefs.GetInt("playercar" + pointer) == 0)
                {
                    if (GD && (pointer == 2 || pointer == 8))
                    {
                        modeText.GetComponentInChildren<Text>().text = "Unlock Free";
                        modeText.GetComponentInChildren<Text>().color = SettingsManager.instance.activeColor;
                        modeText.GetComponentInChildren<Text>().fontSize = 35;
                    }
                    else
                    {
                        modeText.GetComponentInChildren<Text>().text = "BUY: " + carPrices[pointer];
                        modeText.GetComponentInChildren<Text>().color = SettingsManager.instance.passiveColor;
                        modeText.GetComponentInChildren<Text>().fontSize = 40;
                    }


                }
                else
                {
                    modeText.GetComponentInChildren<Text>().text = modeMainText;
                    modeText.GetComponentInChildren<Text>().color = SettingsManager.instance.passiveColor;
                    modeText.GetComponentInChildren<Text>().fontSize = 40;
                }
            }
            
        }
    }
    public void ConfirmButton()
    {
        SoundManager.instance.PlaySoundOneShot("click", 1f);
        diamondCount = PlayerPrefs.GetInt("diamond");
        if (PlayerPrefs.GetInt("playercar" + pointer) == 1)
        {
            PlayerPrefs.SetInt("playercar", pointer);
            selectPanel.SetActive(false);
            if(PlayerPrefs.GetInt("mode") == 0)
            {
                postProcessForBlur.SetActive(true);
                levelSelectPanel.SetActive(true);
                modeText.GetComponentInChildren<Text>().text = modeMainText;

            }
            else
            {
                mainPanel.SetActive(false);
                modeText.GetComponentInChildren<Text>().text = modeMainText;
                SettingsManager.instance.blurEffect.SetActive(true);
                haiykut.LoadingManager.instance.LoadScene("scene_night");
            }     
        }
        else
        {
            if(GD && (pointer == 2 || pointer == 8))
            {
                //RewardManager.instance.freeCar = true;
                //RewardManager.instance.Reward();
                //AdsManager.instance.LoadRewardedAd();
                AdsManager.instance.freeCar = true;
                AdsManager.instance.ShowRewardedAd();
            }
            else
            {
                if (diamondCount >= carPrices[pointer])
                {
                    SoundManager.instance.PlaySoundOneShot("achievement", .75f);
                    diamondCount -= carPrices[pointer];
                    PlayerPrefs.SetInt("diamond", diamondCount);
                    PlayerPrefs.SetInt("playercar" + pointer, 1);
                    modeText.GetComponentInChildren<Text>().text = modeMainText;
                    //Debug.Log("opened");
                    diamondText.text = diamondCount.ToString();
                }
                else
                {
                    LeanTween.cancel(notEnoughText.gameObject);
                    notEnoughText.transform.localScale = Vector3.zero;
                    StopCoroutine(CloseGameOject(notEnoughText));
                    modeText.GetComponent<Button>().interactable = false;
                    //notEnoughText.GetComponent<Animator>().playbackTime = 0;
                    notEnoughText.SetActive(true);
                    int a = LeanTween.scale(notEnoughText.gameObject, Vector3.one, .6f).setOnComplete(() =>
                    {
                        //StopCoroutine(CloseGameOject(notEnoughText));
                        StartCoroutine(CloseGameOject(notEnoughText));
                    }).id;


                }
            }
        
        }    
    }


    void CarAnimation(int id)
    {
        Transform car = cars.transform.GetChild(id);
        car.localPosition = new Vector3(car.localPosition.x, yOffsetForCars, car.localPosition.z);
     
    }
    IEnumerator CloseGameOject(GameObject go)
    {
        yield return new WaitForSeconds(closeTime);
        go.SetActive(false);
        modeText.GetComponent<Button>().interactable = true;
    }

    void RaceButton()
    {
        SoundManager.instance.PlaySoundOneShot("click", 1f);
        AdsManager.instance.ShowInterstitialAd();
        modeMainText = "RACE";
        if (PlayerPrefs.GetInt("playercar" + pointer) == 0)
        {
            if (GD && (pointer == 2 || pointer == 8))
            {
                modeText.GetComponentInChildren<Text>().text = "Unlock Free";
                modeText.GetComponentInChildren<Text>().color = SettingsManager.instance.activeColor;
                modeText.GetComponentInChildren<Text>().fontSize = 35;
            }
            else
            {
                modeText.GetComponentInChildren<Text>().text = "BUY: " + carPrices[pointer];
                modeText.GetComponentInChildren<Text>().color = SettingsManager.instance.passiveColor;
                modeText.GetComponentInChildren<Text>().fontSize = 40;

            }
        }
        else
        {
            modeText.GetComponentInChildren<Text>().text = modeMainText;
            modeText.GetComponentInChildren<Text>().color = SettingsManager.instance.passiveColor;
            modeText.GetComponentInChildren<Text>().fontSize = 40;
        }
        //modeText.GetComponentInChildren<Text>().text = modeMainText;
        PlayerPrefs.SetInt("mode", 0);
        rccSettings.behaviorType = behaviorTypeNormal;
        ActionOne();
    }
    public void Reward()
    {
        AdsManager.instance.freeCar = false;
        AdsManager.instance.ShowRewardedAd();
        //RewardManager.instance.freeCar = false;
        //RewardManager.instance.Reward();

    }
    public void SelectRace(int id)
    {
        SoundManager.instance.PlaySoundOneShot("click", 1f);
        PlayerPrefs.SetInt("currentrace", id);
        levelSelectPanel.SetActive(false);
        SettingsManager.instance.blurEffect.SetActive(true);
        haiykut.LoadingManager.instance.LoadScene("scene_night");

    }
    public void FreeButton()
    {
        SoundManager.instance.PlaySoundOneShot("click", 1f);
        AdsManager.instance.ShowInterstitialAd();
        modeMainText = "BLAST";
        if (PlayerPrefs.GetInt("playercar" + pointer) == 0)
        {
            if (GD && (pointer == 2 || pointer == 8))
            {
                modeText.GetComponentInChildren<Text>().text = "Unlock Free";
                modeText.GetComponentInChildren<Text>().color = SettingsManager.instance.activeColor;
                modeText.GetComponentInChildren<Text>().fontSize = 35;
            }
            else
            {
                modeText.GetComponentInChildren<Text>().text = "BUY: " + carPrices[pointer];
                modeText.GetComponentInChildren<Text>().color = SettingsManager.instance.passiveColor;
                modeText.GetComponentInChildren<Text>().fontSize = 40;

            }
        }
        else
        {
            modeText.GetComponentInChildren<Text>().text = modeMainText;
            modeText.GetComponentInChildren<Text>().color = SettingsManager.instance.passiveColor;
            modeText.GetComponentInChildren<Text>().fontSize = 40;
        }
        //modeText.GetComponentInChildren<Text>().text = modeMainText;
        PlayerPrefs.SetInt("mode", 1);
        rccSettings.behaviorType = behaviorTypeNormal;
        ActionOne();
    }
    public void DriftButton()
    {
        SoundManager.instance.PlaySoundOneShot("click", 1f);
        AdsManager.instance.ShowInterstitialAd();
        modeMainText = "DRIFT";
        if (PlayerPrefs.GetInt("playercar" + pointer) == 0)
        {
            if (GD && (pointer == 2 || pointer == 8))
            {
                modeText.GetComponentInChildren<Text>().text = "Unlock Free";
                modeText.GetComponentInChildren<Text>().color = SettingsManager.instance.activeColor;
                modeText.GetComponentInChildren<Text>().fontSize = 35;
            }
            else
            {
                modeText.GetComponentInChildren<Text>().text = "BUY: " + carPrices[pointer];
                modeText.GetComponentInChildren<Text>().color = SettingsManager.instance.passiveColor;
                modeText.GetComponentInChildren<Text>().fontSize = 40;

            }
        }
        else
        {
            modeText.GetComponentInChildren<Text>().text = modeMainText;
            modeText.GetComponentInChildren<Text>().color = SettingsManager.instance.passiveColor;
            modeText.GetComponentInChildren<Text>().fontSize = 40;
        }
        //modeText.GetComponentInChildren<Text>().text = modeMainText;
        PlayerPrefs.SetInt("mode", 2);
        rccSettings.behaviorType = behaviorTypeDrift;
        ActionOne();
    }
    public void BackButton()
    {
        SoundManager.instance.PlaySoundOneShot("click", 1f);
        AdsManager.instance.ShowInterstitialAd();
        ActionTwo();
    }
    public void BackButton_2()
    {
        SoundManager.instance.PlaySoundOneShot("click", 1f);
        AdsManager.instance.ShowInterstitialAd();
        levelSelectPanel.SetActive(false);
        selectPanel.SetActive(true);
        postProcessForBlur.SetActive(false);
    }


    void ActionOne()
    {
        LeanTween.moveLocal(images[1].gameObject, new Vector3(images[1].rectTransform.localPosition.x + 500, images[1].rectTransform.localPosition.y - 500, images[1].rectTransform.localPosition.z), Time.fixedDeltaTime * 10).setOnComplete(() =>
        {
            //images[1].rectTransform.localPosition = new Vector3(49, -48, 0);
        });
        LeanTween.moveLocal(images[0].gameObject, new Vector3(images[0].rectTransform.localPosition.x - 500, images[0].rectTransform.localPosition.y + 500, images[0].rectTransform.localPosition.z), Time.fixedDeltaTime * 10).setOnComplete(() =>
        {
            //images[1].rectTransform.localPosition = new Vector3(-30, 24, 0);
            dof.active = true;
            theThing.localScale = new Vector3(0.85f, 0.85f, 1);
            theThing.localPosition = theThingMain.localPosition;
            cars.transform.position = new Vector3(poss.transform.position.x, poss.transform.position.y, poss.transform.position.z);
            camera.cullingMask = cullGr;
            //mainCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            cameraOrbit.enabled = true;
            cameraOrbit.touchable = true;
            cars.transform.GetChild(pointer).GetComponent<Rigidbody>().isKinematic = true;
            cars.transform.GetChild(pointer).localPosition = new Vector3(cars.transform.GetChild(pointer).localPosition.x, theThing.position.y + 2, cars.transform.GetChild(pointer).localPosition.z);
            cars.transform.GetChild(pointer).GetComponent<Rigidbody>().isKinematic = false;
            cameraChange_1 = false;
            cameraChange_2 = true;
            mainPanel.SetActive(false);
            //postProcessForBlur.SetActive(true);
            selectPanel.SetActive(true);
        });

    }

    void ActionTwo()
    {
        selectPanel.SetActive(false);
        mainPanel.GetComponentInChildren<Text>().transform.localScale = Vector3.one;
        mainPanel.SetActive(true);
        theThing.localScale = new Vector3(0.6f, 0.6f, 1);
        theThing.localPosition = thingPosOriginal;
        theThing.localEulerAngles = thingRotOriginal;
        cars.transform.GetChild(pointer).GetComponent<Rigidbody>().isKinematic = true;
        cars.transform.position = new Vector3(carsPosOriginal.x, carsPosOriginal.y + 3, carsPosOriginal.z);
        cars.transform.GetChild(pointer).GetComponent<Rigidbody>().isKinematic = false;
        cars.transform.eulerAngles = carsRotOriginal;
        camera.cullingMask = cullGr2;
        cameraOrbit.enabled = false;
        cameraOrbit.touchable = false;
        camera.transform.position = camPosOrj;
        camera.transform.eulerAngles = camRotOrj;
        cameraChange_1 = true;
        cameraChange_2 = false;
        postProcessForBlur.SetActive(false);
        LeanTween.moveLocal(images[0].gameObject, new Vector3(images[0].rectTransform.localPosition.x + 500, images[0].rectTransform.localPosition.y - 500, images[0].rectTransform.localPosition.z), Time.fixedDeltaTime * 10);
        LeanTween.moveLocal(images[1].gameObject, new Vector3(images[1].rectTransform.localPosition.x - 500, images[1].rectTransform.localPosition.y + 500, images[1].rectTransform.localPosition.z), Time.fixedDeltaTime * 10);
        //mainCanvas.renderMode = RenderMode.ScreenSpaceCamera;
        dof.active = false;
    }

    public void DevMode()
    {
        diamondCount += 10000000;
        PlayerPrefs.SetInt("diamond", diamondCount);
        diamondCount = PlayerPrefs.GetInt("diamond");
        diamondText.text = diamondCount.ToString();
        for (int i = 0; i < raceButtons.Count; i++)
        {
            raceButtons[i].interactable = true;
            //raceButtons[i].GetComponentInChildren<Image>().enabled = false;
            if (raceButtons[i].GetComponent<HoverEffectButton>() == null)
            {
                raceButtons[i].AddComponent<HoverEffectButton>();
                raceButtons[i].GetComponent<HoverEffectButton>().hoverColor = raceButtons[0].GetComponent<HoverEffectButton>().hoverColor;
                raceButtons[i].GetComponent<HoverEffectButton>().hoverOriginalColor = raceButtons[0].GetComponent<HoverEffectButton>().hoverOriginalColor;
            }
            if(i > 0)
            {
                if(raceButtons[i].transform.childCount >= 2)
                raceButtons[i].transform.GetChild(1).gameObject.SetActive(false);
            }
   
        }
    }

}

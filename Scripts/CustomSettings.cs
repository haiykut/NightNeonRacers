using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
namespace haiykut
{
    public class CustomSettings : MonoBehaviour
    {
        public static CustomSettings instance;
        RCC_CarControllerV3 car;
        [Header("Enable custom controller for mobile")]
        public bool customController;
        [SerializeField] VariableJoystick horizontalController;
        [SerializeField] VariableJoystick verticalController;
        [HideInInspector] public float horizontalInput;
        [HideInInspector]public bool isNossing;
        [HideInInspector]public bool isGassing;
        [HideInInspector]public bool isBraking;
        float originalGearShiftingDelay;
        [Header("Configure physics materials (Default: 1-Asphalt, 2-Sand, 3-Grass)")]
        public Material materialOne;
        public Material materialTwo;
        public Material materialThree;
        [Header("Set your master volume for player (It is recommended to set it initially)")]
        public float volume;
        bool isSceneLoaded;
        [Header("Additional Settings")]
        [SerializeField] bool changeSpritesForMobileController;
        [SerializeField] GameObject nosSprite;
        [SerializeField] GameObject brakeSprite;
        [SerializeField] GameObject reverseSprite;
        [SerializeField] Image nosImage;
        [SerializeField] Text nosText;
        [SerializeField] GameObject leftDirectionSprite;
        [SerializeField] GameObject rightDirectionSprite;
        [SerializeField]AudioSource nosSound;
        [Header("Custom other game plot settings")]
        [SerializeField] bool activateOthers;
        [SerializeField] Text speedText;
        int speed;
        [SerializeField] Color nosColor_2;
        private void Awake()
        {
            instance = this;
        }
        IEnumerator Start()
        {         
            yield return new WaitForEndOfFrame();
            isSceneLoaded = true;
            car = GameManager.instance.playerCar.GetComponent<RCC_CarControllerV3>();  //TODO: activate this after deveolopment.
            //Debug.Log(car.name);
            if (customController)
            verticalController.customType = true;
            if (car != null)
            originalGearShiftingDelay = car.gearShiftingDelay;
            if(car.useNOS)
            nosSound = car.NOSSound;
            if (!customController)
            {
                horizontalController.gameObject.SetActive(false);
                verticalController.gameObject.SetActive(false);
            }
            else
            {
                horizontalController.gameObject.SetActive(true);
                verticalController.gameObject.SetActive(true);            
            }
            if (activateOthers)
            {
                speedText.gameObject.SetActive(true);
            }
            else
            {
                speedText.gameObject.SetActive(false);
            }
            if (car.useNOS)
                nosSound.volume = nosSound.volume * volume;
        }

        void FixedUpdate()
        {
            if (isSceneLoaded)
            {              
                speed = (int)car.speed;
                speedText.text = speed.ToString("D3") + "<size=30> KMH </size>";
                float nosAmount = car.NoS / 100;
                nosImage.fillAmount = Mathf.Clamp(nosAmount, 0.11f, 1);
                if (nosAmount <= 0.20f)
                    nosImage.color = Color.red;
                else if (nosAmount > 0.20f && nosAmount < 0.6f)
                    nosImage.color = nosColor_2;
                else
                    nosImage.color = Color.cyan;

                int nos = (int)car.NoS;
                nosText.color = nosImage.color;
                if(nos <= 5)
                    nosText.text = "0" + "%";
                else
                nosText.text = nos.ToString() + "%";
            }
            horizontalInput = horizontalController.Horizontal;
            if (customController)
            {
                if (isSceneLoaded)
                {

                    if (verticalController.Vertical > 0.75 && car.direction == 1)
                    {
                        isNossing = true;
                        car.gearShiftingDelay = 0;
                        if (changeSpritesForMobileController)
                        {
                            nosSprite.SetActive(true);
                        }
                    }
                    else
                    {
                        isNossing = false;
                        car.gearShiftingDelay = originalGearShiftingDelay;
                        if (changeSpritesForMobileController)
                        {
                            nosSprite.SetActive(false);
                        }
                    }
                    if (verticalController.Vertical < -0.75)
                    {
                        isBraking = true;
                        if (changeSpritesForMobileController)
                        {
                            brakeSprite.SetActive(true);
                        }
                    }
                    else
                    {
                        isBraking = false;
                        if (changeSpritesForMobileController)
                        {
                            brakeSprite.SetActive(false);
                        }
                    }
                    if (horizontalController.Horizontal > 0.1f)
                    {
                        if (changeSpritesForMobileController)
                            rightDirectionSprite.SetActive(true);
                    }
                    else
                    {
                        if (changeSpritesForMobileController)
                            rightDirectionSprite.SetActive(false);
                    }
                    if (horizontalController.Horizontal < -0.1f)
                    {
                        if (changeSpritesForMobileController)
                            leftDirectionSprite.SetActive(true);
                    }
                    else
                    {
                        if (changeSpritesForMobileController)
                            leftDirectionSprite.SetActive(false);
                    }
                    if (car.direction == -1)
                    {
                        if (changeSpritesForMobileController)
                        {
                            reverseSprite.SetActive(true);
                            brakeSprite.SetActive(false);
                        }
                    }
                    else
                    {
                        if (changeSpritesForMobileController)
                            reverseSprite.SetActive(false);
                    }
                    if (verticalController.Vertical == 0)
                    {
                        isNossing = false;
                        isBraking = false;
                        if (changeSpritesForMobileController)
                        {
                            brakeSprite.SetActive(false);
                            nosSprite.SetActive(false);
                            reverseSprite.SetActive(false);
                        }
                    }
                    //if(car.NoS)
                   
                    
                }
            }
         
        }
    }
}

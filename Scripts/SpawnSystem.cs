using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using  UnityEngine.UI;
namespace haiykut
{
    public class SpawnSystem : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] bool mobile;
        [SerializeField] float rewindSpeed = 2;
        [SerializeField] float leanAnimationTimeInterval = 1;
        [SerializeField] float playerTransformRecorderTimeInterval = 1;
        [SerializeField] internal Transform player;
        //[SerializeField] GameObject rewindPanel;
        [SerializeField]   List<Vector3> lastPositions;
        [SerializeField] List<Quaternion> lastRotations;
        [SerializeField] List<Collider> playerColActual;
        internal Collider[] playerCols;
        // PRIVATE AND NON SERIALIZED FIELDS
        Button thisButton;
        Text buttonText;
        Image buttonSprite;
        bool timerActive;
        float time = 2;
        bool isReverseUsableForPc = false;
        RCC_CarControllerV3 car;
        internal int finishedAnimationCounter = 0;
        bool isBacking = false;
        int one;
        int two;
        bool cntrl;
        private void Awake()
        {
            mobile = CustomSettings.instance.customController;
            if (mobile)
            {
                thisButton = GetComponent<Button>();
                buttonText = GetComponentInChildren<Text>();
                buttonSprite = GetComponent<Image>();
                thisButton.image.enabled = false;
                thisButton.enabled = false;
        
                thisButton.transform.GetChild(0).gameObject.SetActive(false);
                EventTrigger eventTrigger = gameObject.AddComponent<EventTrigger>();
                EventTrigger.Entry onPointerDown = new EventTrigger.Entry();
                EventTrigger.Entry onPointerUp = new EventTrigger.Entry();
                EventTrigger.Entry onPointerCancel = new EventTrigger.Entry();
                onPointerDown.eventID = EventTriggerType.PointerDown;
                onPointerUp.eventID = EventTriggerType.PointerUp;
                onPointerCancel.eventID = EventTriggerType.Cancel;
                onPointerDown.callback.AddListener((pointerEventData) => { OnPointerDown((PointerEventData)pointerEventData); });
                onPointerUp.callback.AddListener((pointerEventData) => { OnPointerUp((PointerEventData)pointerEventData); });
                onPointerCancel.callback.AddListener((pointerEventData) => { OnPointerCancel((PointerEventData)pointerEventData); });
                eventTrigger.triggers.Add(onPointerDown);
                eventTrigger.triggers.Add(onPointerUp);


            }
            else
            {
                thisButton = GetComponent<Button>();
                buttonText = GetComponentInChildren<Text>();
                buttonSprite = GetComponent<Image>();
                thisButton.interactable = false;
                buttonText.enabled = false;
                buttonSprite.enabled = false;


            }
        }
   

        PointerEventData pointerEventData;
         void OnPointerCancel(PointerEventData eventData)
        {

            thisButton.interactable = false;
            buttonText.enabled = false;
            buttonSprite.enabled = false;
        }


        public void AutomaticRewindStart()
        {
            OnPointerDown(pointerEventData);
        }
         public void AutomaticRewindStop()
        {
            OnPointerUp(pointerEventData);
        }
         void OnPointerDown(PointerEventData eventData)
        {
            if (!isBacking)
            {
                ctrl = false;
                pointerEventData = eventData;
                GameManager.instance.rewindPanel.SetActive(true);
                GameManager.instance.ppr.SetActive(true);
                SoundManager.instance.PlaySound("rewindsound", true);
                for (int i = 0; i < playerColActual.Count; i++)
                {
                    playerColActual[i].enabled = false;
                }
                player.root.GetComponent<Rigidbody>().isKinematic = true;
                player.GetComponent<PlayerScript>().sounds.GetChild(0).gameObject.SetActive(false);
                player.GetComponent<RCC_CarControllerV3>().canControl = false;
                CancelInvoke();

                Time.timeScale = rewindSpeed;
                isBacking = true;
                StartCoroutine(AnimationStart());
            }
        }
         void OnPointerUp(PointerEventData eventData)
        {
            player.GetComponent<RCC_CarControllerV3>().isCollided = false;
            player.GetComponent<RCC_CarControllerV3>().isCollidedForDrift = false;
            pointerEventData = eventData;
            GameManager.instance.rewindPanel.SetActive(false);
            GameManager.instance.ppr.SetActive(false);
            if (PlayerPrefs.GetInt("music") == 1)
            SoundManager.instance.ReturnThemeMusic();
            isBacking = false;
            for (int i = 0; i < playerColActual.Count; i++)
            {
                playerColActual[i].enabled = true;
            }
            player.root.GetComponent<Rigidbody>().isKinematic = false;
            player.GetComponent<PlayerScript>().sounds.GetChild(0).gameObject.SetActive(true);
            player.GetComponent<RCC_CarControllerV3>().canControl = true;
            Time.timeScale = 1f;
            StopAllCoroutines();
            LeanTween.cancel(player.gameObject);
            if (lastPositions.Count > 0)
            {
                for (int i = lastPositions.Count - 1; i > lastPositions.Count - finishedAnimationCounter; i--)
                {
                    lastPositions.RemoveAt(i);
                    lastRotations.RemoveAt(i);
                    finishedAnimationCounter--;
                
                }             
            }
            InvokeRepeating(nameof(TransformRecorder), 2f, playerTransformRecorderTimeInterval);
        }

        bool isStarted;

        
        IEnumerator Start()
        {
         
            yield return new WaitForEndOfFrame();
        
            isStarted = true;
            GameManager.instance.rewindPanel.SetActive(false);
            GameManager.instance.ppr.SetActive(false);
            playerCols = player.GetComponentsInChildren<Collider>();
            car = player.GetComponent<RCC_CarControllerV3>();
            for (int i = 0; i < 9; i++)
            {
                lastPositions.Add(car.transform.position);
                lastRotations.Add(car.transform.rotation);
            }
            for (int i = 0; i < playerCols.Length; i++)
            {
                if (playerCols[i].GetComponent<WheelCollider>() == null)
                {
                    playerColActual.Add(playerCols[i]);
                }
            }

            InvokeRepeating(nameof(TransformRecorder), 0.01f, playerTransformRecorderTimeInterval);

        }



        internal bool ctrl = true;
        IEnumerator AnimationStart()
        {
            if (isBacking)
            {

                if (lastPositions.Count != 0)
                {
                    for (int i = lastPositions.Count - 1; i > 0; i--)
                    {
                        finishedAnimationCounter++;
                        
                        one = LeanTween.move(player.gameObject, lastPositions[i], leanAnimationTimeInterval).setEase(LeanTweenType.once).setOnComplete(() => { LeanTween.cancel(player.gameObject, one); }).id;
                        two = LeanTween.rotate(player.gameObject, lastRotations[i].eulerAngles, leanAnimationTimeInterval).setEase(LeanTweenType.once).id;
                        yield return new WaitForSeconds(leanAnimationTimeInterval);
                        if (finishedAnimationCounter == lastPositions.Count - 1)
                        {
                            ctrl = false;                        
                        }

                    }

                    if (!ctrl)
                    {
                        car.isCollided = false;
                        //car.isCollidedForDrift = false;
                        ctrl = true;
                        OnPointerUp(pointerEventData);
                        StopAllCoroutines();
                    }
                    
                }
                
            }
            else
            {
                yield break;
            }

        }

        void Update()
        {
            if (isStarted)
            {
                if (!mobile)
                {

                    if (lastPositions.Count > 1)
                    {
                        isReverseUsableForPc = true;
                    }
                    else
                    {
                        isReverseUsableForPc = false;
                    }
                    if (isReverseUsableForPc)
                    {
                        if (Input.GetKeyDown(KeyCode.R))
                        {
                            if (!isBacking)
                            {
                                isBacking = true;
                                player.GetComponent<PlayerScript>().sounds.GetChild(0).gameObject.SetActive(false);
                                player.GetComponent<RCC_CarControllerV3>().canControl = false;
                                for (int i = 0; i < playerColActual.Count; i++)
                                {
                                    playerColActual[i].enabled = false;
                                }

                                player.root.GetComponent<Rigidbody>().isKinematic = true;

                                CancelInvoke();

                                Time.timeScale = rewindSpeed;
                                SoundManager.instance.PlaySoundForAWhile("rewindsound", true);
                                GameManager.instance.rewindPanel.SetActive(true);
                                GameManager.instance.ppr.SetActive(true);
                                StartCoroutine(AnimationStart());
                            }
                        }
                        if (Input.GetKeyUp(KeyCode.R))
                        {
                            GameManager.instance.rewindPanel.SetActive(false);
                            GameManager.instance.ppr.SetActive(false);
                            if (PlayerPrefs.GetInt("music") == 1)
                                SoundManager.instance.ReturnThemeMusic();
                            else
                                SoundManager.instance.StopSound();
                            player.GetComponent<PlayerScript>().sounds.GetChild(0).gameObject.SetActive(true);
                            player.GetComponent<RCC_CarControllerV3>().canControl = true;
                            player.GetComponent<RCC_CarControllerV3>().isCollided = false;
                            player.GetComponent<RCC_CarControllerV3>().isCollidedForDrift = false;
                            isBacking = false;
                            for (int i = 0; i < playerColActual.Count; i++)
                            {
                                playerColActual[i].enabled = true;
                            }

                            player.root.GetComponent<Rigidbody>().isKinematic = false;
                            Time.timeScale = 1f;
                            StopAllCoroutines();
                            LeanTween.cancel(player.gameObject);
                            if (lastPositions.Count > 0)
                            {
                                for (int i = lastPositions.Count - 1; i > lastPositions.Count - finishedAnimationCounter; i--)
                                {
                                    lastPositions.RemoveAt(i);
                                    lastRotations.RemoveAt(i);
                                    finishedAnimationCounter--;
                                }
                            }

                            InvokeRepeating(nameof(TransformRecorder), 2f, playerTransformRecorderTimeInterval);
                        }
                    }

                }
                else
                {
                    if (timerActive)
                    {
                        time -= Time.deltaTime;
                        if (time <= 0)
                        {
                            if (car.speed > 30)
                            {
                                car.isCollided = false;
                                //car.isCollidedForDrift = false;
                                time = 2;
                                timerActive = false;
                            }
                            else
                            {
                                car.isCollided = true;
                                //car.isCollidedForDrift = false;
                                //time = 2;
                                //timerActive = false;
                            }


                        }
                    }
                    else
                    {
                        thisButton.image.enabled = false;
                        thisButton.enabled = false;
                        thisButton.transform.GetChild(0).gameObject.SetActive(false);
                    }
                    // if (lastPositions.Count > 1)
                    //{
                    //cntrl = true;
                    if (car.isCollided && lastPositions.Count > 1 && car.speed < 30)
                    {

                        thisButton.image.enabled = true;
                        thisButton.enabled = true;
                        thisButton.transform.GetChild(0).gameObject.SetActive(true);
                        timerActive = true;
                        //trigger.enabled = true;

                    }
                    else
                    {
                        thisButton.image.enabled = false;
                        thisButton.enabled = false;
                        thisButton.transform.GetChild(0).gameObject.SetActive(false);
                        //trigger.enabled = false;
                    }



                    //}

                    //if(lastPositions.Count == 1)
                    //{
                    //cntrl = false;
                    //else
                    //{
                    //    thisButton.image.enabled = false;
                    //    thisButton.enabled = false;
                    //    thisButton.transform.GetChild(0).gameObject.SetActive(false);
                    //    Debug.Log("asdf");
                    //}

                    //}


                }
            }
      
        

        }
        void TransformRecorder()
        {

            if (!isBacking && player.root.GetComponent<RCC_CarControllerV3>().speed > 30 && player.root.GetComponent<RCC_CarControllerV3>().isGrounded)
            {
                if (lastPositions.Count < 10)
                {
                    lastPositions.Add(player.position);
                    lastRotations.Add(player.localRotation);
                }
                else
                {
                    lastPositions.RemoveAt(0);
                    lastRotations.RemoveAt(0);
                    lastPositions.Add(player.position);
                    lastRotations.Add(player.localRotation);
                }


            }


        }

    }
}


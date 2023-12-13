using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace haiykut
{
    public class SettingsManager : MonoBehaviour
    {
        [SerializeField]GameObject settingsPanel;
        [SerializeField] GameObject mainPanel;
        public static SettingsManager instance;
        bool fpsLocked = false;
        [SerializeField] Button[] qualityButtons = new Button[3];
        [SerializeField] GameObject[] qualities = new GameObject[3];
        [SerializeField] internal Color activeColor;
        [SerializeField] internal Color passiveColor;
        internal int sounds;
        internal int music;
        int showFPS;
        int lockFPS;
        //[SerializeField] Text[] soundTexts;
        [SerializeField]
        GameObject[] soundTexts;
        [SerializeField] Text musicTexts;
        [SerializeField] GameObject fpsText;
        [SerializeField] Text showFpsText;
        [SerializeField] Text lockFPSText;
        [SerializeField] Canvas canvas;
        [SerializeField] internal GameObject blurEffect;
        private void Awake()
        {
            if(instance != null) {
                Destroy(this);
            }
            else
            {
                instance = this;
            }
         

        }
        int quality;
        IEnumerator Start()
        {
            yield return new WaitForEndOfFrame();
            //if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android || EditorUserBuildSettings.activeBuildTarget == BuildTarget.iOS)
               // Application.targetFrameRate = Screen.currentResolution.refreshRate;
            //else if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.WebGL)
                //Application.targetFrameRate = 60;
            //Debug.Log("Screen : " + Screen.currentResolution.refreshRate);
            quality = PlayerPrefs.GetInt("quality");
            QualitySettings.SetQualityLevel(quality);
            //Debug.Log(quality);
            for (int i = 0; i < 3; i++)
            {

                if (SettingsManager.instance.qualities[i] == SettingsManager.instance.qualities[quality])
                    SettingsManager.instance.qualities[i].SetActive(true); //= activeColor;
                else
                    SettingsManager.instance.qualities[i].SetActive(false); // = Color.white;

                //Debug.Log(SettingsManager.instance.qualityButtons[i].GetComponentInChildren<Text>().color);
            }
            sounds = PlayerPrefs.GetInt("sounds");
            music = PlayerPrefs.GetInt("music");
            showFPS = PlayerPrefs.GetInt("fps");
            lockFPS = PlayerPrefs.GetInt("lock");
            Application.targetFrameRate = Screen.currentResolution.refreshRate;
            lockFPSText.color = Color.white;
            
            //Debug.Log(Application.targetFrameRate);
            if (sounds == 0)
            {
                //Debug.Log("halil");
                AudioListener.pause = false;
                AudioListener.volume = 1;
                //musicTexts.rectTransform.parent.GetComponent<EventTrigger>().enabled = true;
                //Debug.Log(musicTexts.rectTransform.parent.GetComponent<Button>().gameObject.name);
                if (music == 0)
                {
                    musicTexts.rectTransform.parent.GetComponent<Button>().enabled = true;
                    musicTexts.rectTransform.parent.GetComponent<NewUiButton>().def = true;
                    musicTexts.color = Color.white;
                    SoundManager.instance.StopSound();
                }
                else
                {
                    musicTexts.rectTransform.parent.GetComponent<Button>().enabled = false;
                    musicTexts.rectTransform.parent.GetComponent<NewUiButton>().def = false;
                    musicTexts.color = activeColor;
                    SoundManager.instance.PlaySound("mainmusic", true);
                }
                soundTexts[0].SetActive(true); //= activeColor;
                soundTexts[1].SetActive(false); //color = Color.white;
            }
            else
            {
                //Debug.Log("ibrahim");
                AudioListener.pause = true;
                AudioListener.volume = 0;
                musicTexts.rectTransform.parent.GetComponent<Button>().enabled = false;
                musicTexts.rectTransform.parent.GetComponent<NewUiButton>().def = false;
                //musicTexts.rectTransform.parent.GetComponent<EventTrigger>().enabled= false;
                //Debug.Log(musicTexts.rectTransform.parent.GetComponent<Button>().gameObject.name);
                musicTexts.color = Color.red;
                soundTexts[0].SetActive(false); // = Color.white;
                soundTexts[1].SetActive(true); // = activeColor;

            }

          
            if (showFPS == 0)
            {
                fpsText.SetActive(true);
                showFpsText.color = activeColor;
                CalculateFPS();
            }
            else
            {
                fpsText.SetActive(false);
                showFpsText.color = Color.white;
                StopCalculateFPS();
            }
            


        }
        public void ShowFPS()
        {
            SoundManager.instance.PlaySoundOneShot("click", 0.25f);
            if (showFPS == 0)
            {
                PlayerPrefs.SetInt("fps", 1);
                showFPS = 1;
                showFpsText.color = Color.white;
                StopCalculateFPS();
                fpsText.SetActive(false);
            }
            else
            {
                PlayerPrefs.SetInt("fps", 0);
                showFPS = 0;
                showFpsText.color = activeColor;
                CalculateFPS();
                fpsText.SetActive(true);
            }
        }
        public void LockFPS()
        {
            if (lockFPS == 0)
            {
                PlayerPrefs.SetInt("lock", 1);
                lockFPS = 1;
                lockFPSText.color = Color.white;
                Application.targetFrameRate = Screen.currentResolution.refreshRate;
                Debug.Log(Application.targetFrameRate);
            }
            else
            {
                PlayerPrefs.SetInt("lock", 0);
                lockFPS = 0;
                lockFPSText.color = activeColor;
                Application.targetFrameRate = 60;
                Debug.Log(Application.targetFrameRate);
            }
        }
        public void Sounds(bool on)
        {
            SoundManager.instance.PlaySoundOneShot("click", 1f);
            if (on)
            {
              
                sounds = 1;
                PlayerPrefs.SetInt("sounds", sounds);
                soundTexts[1].SetActive(true); //= activeColor;
                soundTexts[0].SetActive(false); // = Color.white;
                AudioListener.pause = true;
                AudioListener.volume = 0;
                musicTexts.rectTransform.parent.GetComponent<Button>().enabled= false;
                musicTexts.rectTransform.parent.GetComponent<NewUiButton>().def = false;
                musicTexts.color = Color.red;

                on = true;
            }
            else
            {
          
                sounds = 0;
                PlayerPrefs.SetInt("sounds", sounds);
                soundTexts[0].SetActive(true);// = activeColor;
                soundTexts[1].SetActive(false); //= Color.white;
                AudioListener.pause = false;
                AudioListener.volume = 1;
                musicTexts.rectTransform.parent.GetComponent<Button>().enabled = true;
                musicTexts.rectTransform.parent.GetComponent<NewUiButton>().def = true;

                if (music == 0)
                {
                    musicTexts.color = Color.white;
                    SoundManager.instance.StopSound();
                }
                else
                {
                    musicTexts.color = activeColor;
                    SoundManager.instance.PlaySound("mainmusic", true);
                }
                on = false;
            }
            //Debug.Log("ahaao: " + sounds);
        }


        public void Music()
        {
            SoundManager.instance.PlaySoundOneShot("click", 1f);
            if (music == 1)
            {
                SoundManager.instance.StopSound();
                musicTexts.color = Color.white;
                PlayerPrefs.SetInt("music", 0);
                music = 0;

            }
            else
            {
                SoundManager.instance.PlaySound("mainmusic", true);
                musicTexts.color = activeColor;
                PlayerPrefs.SetInt("music", 1);
                music = 1;
            }
            //Debug.Log("ahaa22o: " + music);

        }
        public void CalculateFPS()
        {
            InvokeRepeating(nameof(GetFPS), 0.5f, 0.5f);
        }
        public void StopCalculateFPS()
        {
            CancelInvoke(nameof(GetFPS));
        }
        void GetFPS()
        {
            int fps = (int)(1 / Time.deltaTime);
            fpsText.GetComponent<Text>().text = fps.ToString() + " FPS";
        }
        public void SettingsPanel()
        {
            //canvas.planeDistance = 1;
            AdsManager.instance.isPausedGame = true;
            AdsManager.instance.ShowInterstitialAd();
            SoundManager.instance.PlaySoundOneShot("click", 1f);
            settingsPanel.SetActive(true);
            AudioListener.pause = true;
            AudioListener.volume = 0;
            //mainPanel.SetActive(false);
            blurEffect.SetActive(true);


        }
        public void QualitySetting(int quality)
        {
            SoundManager.instance.PlaySoundOneShot("click", 1f);
            QualitySettings.SetQualityLevel(quality);
            PlayerPrefs.SetInt("quality", quality);
            for(int i = 0; i < qualities.Length; i++)
            {
                if (qualities[i] == qualities[quality])
                    qualities[i].SetActive(true); // = activeColor;
                else
                    qualities[i].SetActive(false); // = Color.white;
            }
        }

        public void Back(int index)
        {
            Time.timeScale = 1;
            AdsManager.instance.isPausedGame = false;
            SoundManager.instance.PlaySoundOneShot("click", 1f);
            if (index == 0)
            {
                if(sounds == 0)
                {
                    AudioListener.pause = false;
                    AudioListener.volume = 1;
                }
                else
                {
                    AudioListener.pause = true;
                    AudioListener.volume = 0;
                }
                //canvas.planeDistance = 15.19f;
                settingsPanel.SetActive(false);
                mainPanel.SetActive(true);
                blurEffect.SetActive(false);

            }
        }
    }
}

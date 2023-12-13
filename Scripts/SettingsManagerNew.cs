using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using haiykut;
public class SettingsManagerNew : MonoBehaviour
{
    // Start is called before the first frame update
    public static SettingsManagerNew Instance;
    [SerializeField] internal float volume;
    [SerializeField] internal bool pause;
    [SerializeField] GameObject settingsPanel;
    [SerializeField] GameObject[] qualityTexts;
    [SerializeField] GameObject[] soundTexts;
    [SerializeField] Text musicTexts;
    [SerializeField] Color activeColor;
    [SerializeField] Camera mainCam;
    [SerializeField] Camera miniCam;
    [SerializeField] GameObject[] miniMapObjects;
    [SerializeField] Button miniMapButton;
    [SerializeField] GameObject miniMapNotAvailable;
    [SerializeField] GameObject fpsText;
    [SerializeField] Text showFpsText;
    [SerializeField] Text lockFPSText;
    [SerializeField] TSTrafficSpawner spawner;
    CameraCulling cameraCulling;
    int quality;
    internal int sounds;
    internal int music;
    int miniMapOpened; // 0 open 1 close
    int showFPS;
    int lockFPS;
    private void Awake()
    {
        Instance = this;
    }
    IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();

        cameraCulling = mainCam.GetComponent<CameraCulling>();
        quality = PlayerPrefs.GetInt("quality");
        miniMapOpened = PlayerPrefs.GetInt("minimap");
        showFPS = PlayerPrefs.GetInt("fps");
        //qualityTexts[quality].SetActive(true);// = activeColor;
        lockFPS = PlayerPrefs.GetInt("lock");
        QualitySettings.SetQualityLevel(quality);
        if (quality == 0)
        {
            mainCam.farClipPlane = 150;
            qualityTexts[0].SetActive(true);
            qualityTexts[1].SetActive(false);
            qualityTexts[2].SetActive(false);
        }
        else if (quality == 1)
        {
            mainCam.farClipPlane = 200;
            qualityTexts[0].SetActive(false);
            qualityTexts[1].SetActive(true);
            qualityTexts[2].SetActive(false);
        }
        else
        {
            mainCam.farClipPlane = 250;
            qualityTexts[0].SetActive(false);
            qualityTexts[1].SetActive(false);
            qualityTexts[2].SetActive(true);
        }
        spawner.maxDistance = mainCam.farClipPlane;
        RenderSettings.fogEndDistance = mainCam.farClipPlane;
        cameraCulling.defaultDistance = mainCam.farClipPlane;
        cameraCulling.LayerDistance1 = mainCam.farClipPlane - 50;
        cameraCulling.Init();
        if (miniMapOpened == 0)
        {
            foreach (GameObject go in miniMapObjects)
            {
                go.SetActive(true);
            }
        }
        else
        {
            foreach (GameObject go in miniMapObjects)
            {
                go.SetActive(false);
            }
        }
        IsMiniMapAvailable(quality);
        sounds = PlayerPrefs.GetInt("sounds");
        music = PlayerPrefs.GetInt("music");
        //for(int i = 0; i < soundTexts.Length; i++)
        //{
        //    if(i == sounds)
        //        soundTexts[sounds].SetActive(true); //= activeColor;
        //    else
        //        soundTexts[sounds].SetActive(false);
        //}

        if (sounds == 0)
        {
            soundTexts[0].SetActive(true);
            soundTexts[1].SetActive(false);
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
           // AudioListener.pause = false;
            //AudioListener.volume = 1;
        }
        else
        {
            soundTexts[0].SetActive(false);
            soundTexts[1].SetActive(true);
            musicTexts.rectTransform.parent.GetComponent<Button>().enabled = false;
            musicTexts.rectTransform.parent.GetComponent<NewUiButton>().def = false;
            musicTexts.color = Color.red;
            //AudioListener.pause = true;
           // AudioListener.volume = 0;
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
        if (lockFPS == 0)
        {
            Application.targetFrameRate = 60;
            lockFPSText.color = activeColor;

        }
        else
        {
            Application.targetFrameRate = Screen.currentResolution.refreshRate;
            lockFPSText.color = Color.white;
        }


    }
    void CalculateFPS()
    {
        InvokeRepeating(nameof(GetFPS), 0.5f, 0.5f);
    }
    void StopCalculateFPS()
    {
        CancelInvoke(nameof(GetFPS));
    }
    void GetFPS()
    {
        int fps = (int)(1 / Time.unscaledDeltaTime);
        fpsText.GetComponent<Text>().text = fps.ToString() + " FPS";
    }
    void IsMiniMapAvailable (int graphicIndex)
    {
     
        if(graphicIndex == 0) {
            miniMapButton.GetComponentInChildren<Text>().color = Color.red;
            //miniMapNotAvailable.SetActive(true);
            miniMapButton.interactable = false;
            miniMapButton.GetComponent<NewUiButton>().def = false;
            foreach (GameObject go in miniMapObjects)
            {
                go.SetActive(false);
            }
        }
        else
        {
            //miniMapNotAvailable.SetActive(false);
            miniMapButton.interactable = true;
            miniMapButton.GetComponent<NewUiButton>().def = true;
            if (miniMapOpened == 0)
            {
                miniMapButton.GetComponentInChildren<Text>().color = activeColor;
                foreach (GameObject go in miniMapObjects)
                {
                    go.SetActive(true);

                }
            }
            else
            {
                miniMapButton.GetComponentInChildren<Text>().color = Color.white;
                foreach (GameObject go in miniMapObjects)
                {
                    go.SetActive(false);

                }
            }
          
        }
    }
    public void MiniMap()
    {
        SoundManager.instance.PlaySoundOneShot("click", 1f);
        if (miniMapOpened == 0)
        {
            PlayerPrefs.SetInt("minimap", 1);
            miniMapOpened = PlayerPrefs.GetInt("minimap");
            foreach (GameObject go in miniMapObjects)
            {
                go.SetActive(false);
            }
        }
        else
        {
            PlayerPrefs.SetInt("minimap", 0);
            miniMapOpened = PlayerPrefs.GetInt("minimap");
            if(quality > 0)
            {
                foreach (GameObject go in miniMapObjects)
                {
                    go.SetActive(true);
                }
            }
          
        }
        IsMiniMapAvailable(quality);
    }
    public void ShowFPS()
    {
        SoundManager.instance.PlaySoundOneShot("click", 1f);
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
    public void SettingsPanel(bool opened)
    {
        SoundManager.instance.PlaySoundOneShot("click", 1f);
        AdsManager.instance.isPausedGame = true;
        AdsManager.instance.ShowInterstitialAd();
        if (!opened)
        {
            settingsPanel.SetActive(true);
            GameManager.instance.pausePanelMain.SetActive(false);
            opened = true;
        }
        else
        {
            settingsPanel.SetActive(false);
            GameManager.instance.pausePanelMain.SetActive(true);
            opened = false;
        }
    }
    public void QualitySetting(int qua)
    {
        SoundManager.instance.PlaySoundOneShot("click", 1f);
        quality = qua;
        QualitySettings.SetQualityLevel(quality);
        PlayerPrefs.SetInt("quality", quality);
        if (quality == 0)
        {
            mainCam.farClipPlane = 150;
        }
        else if (quality == 1)
        {
            mainCam.farClipPlane = 200;
        }
        else
        {
            mainCam.farClipPlane = 250;
        }
        spawner.maxDistance = mainCam.farClipPlane;
        RenderSettings.fogEndDistance = mainCam.farClipPlane;
        cameraCulling.defaultDistance = mainCam.farClipPlane;
        cameraCulling.LayerDistance1 = mainCam.farClipPlane - 50;
        cameraCulling.Init();
        IsMiniMapAvailable(quality);
        for (int i =0;i<qualityTexts.Length; i++)
        {
            qualityTexts[i].SetActive(false); //= Color.white;
        }
        qualityTexts[qua].SetActive(true); // = activeColor;
    }
    public void Sounds(bool on)
    {
        SoundManager.instance.PlaySoundOneShot("click", 1f);
        if (on)
        {
            pause = false;
            volume = 1;
            sounds = 0;
            PlayerPrefs.SetInt("sounds", sounds);
            soundTexts[0].SetActive(true); // = activeColor;
            soundTexts[1].SetActive(false); //= Color.white;

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
            on = true;
        }
        else
        {
            pause = true;
            volume = 0;
            sounds = 1;
            PlayerPrefs.SetInt("sounds", sounds);
            soundTexts[1].SetActive(true); //= activeColor;
            soundTexts[0].SetActive(false); // = Color.white;
            musicTexts.rectTransform.parent.GetComponent<Button>().enabled = false;
            musicTexts.rectTransform.parent.GetComponent<NewUiButton>().def = false;
            musicTexts.color = Color.red;
            on = false;
        }
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
            SoundManager.instance.PlaySound("mainmusic",true);
            musicTexts.color = activeColor;
            PlayerPrefs.SetInt("music", 1);
            music = 1;
        }
    }
}

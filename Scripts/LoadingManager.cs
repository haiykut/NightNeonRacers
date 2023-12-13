using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace haiykut
{
    public class LoadingManager : MonoBehaviour
    {
        //public Slider loadingBar;
        bool loader;
        public float amountValue;
        float progress;
        AsyncOperation operation;
        public GameObject loadingPanel;
        public GameObject loadingPanel_2;
        public Image bar;
        public static LoadingManager instance;
        public RCC_CarControllerV3[] cars = new RCC_CarControllerV3[10];
        [SerializeField] Text loadingProgressText;
        private void Awake()
        {
            //Application.targetFrameRate = 60;
            DontDestroyOnLoad(gameObject);
            if (instance == null)
                instance = this;
            else
                Destroy(gameObject);
        }
        private int SceneIndexFromName(string n)
        {

            for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
            {
                string pivotName = NameFromIndex(i);
                if (pivotName == n)
                {
                    return i;
                }
            }
            return -1;
        }
        public void LoadScene(string name)
        {
            //  AdsManager.instance.ShowAd();
            //YK.GameManager.Instance.isPauseGame = true;
            AdsManager.instance.LoadLoadInterstitialAd();
            loadingPanel.SetActive(true);
            loadingPanel_2.SetActive(true);
            operation = SceneManager.LoadSceneAsync(SceneIndexFromName(name));
            
            loader = true;
        }
        private static string NameFromIndex(int BuildIndex)
        {
            string path = SceneUtility.GetScenePathByBuildIndex(BuildIndex);
            int slash = path.LastIndexOf('/');
            string name = path.Substring(slash + 1);
            int dot = name.LastIndexOf('.');
            return name.Substring(0, dot);
        }

        private void Update()
        {

          
            if(loader)
            {
                if (!operation.isDone)
                {
                    bar.fillAmount = Mathf.SmoothStep(bar.fillAmount, operation.progress, Time.deltaTime * amountValue);
                    int progressVol = Mathf.RoundToInt(bar.fillAmount * 100);
                    loadingProgressText.text = progressVol.ToString() + "%" + " Loading...";
                    //progress = Time.deltaTime * amountValue;
                    //Debug.Log(progress);
                    //Debug.Log(bar.gameObject.name);
                    //bar.fillAmount = progress;
                }


                else
                {
                    loadingPanel.SetActive(false);
                    loadingPanel_2.SetActive(false);
                    if (SceneManager.GetActiveScene().name != "scene_night")
                    if (GameObject.FindObjectOfType<RCC_Camera>() != null)
                        Destroy(GameObject.FindObjectOfType<RCC_Camera>());
                    bar.fillAmount = 0;
                    loader = false;
                    operation = null;
                    AdsManager.instance.ShowInterstitialAd();
                }
            }
         
               
                  
                
               
                
            
            
        }
    }

}

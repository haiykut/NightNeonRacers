using haiykut;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class RewardManager : MonoBehaviour
{
    // Start is called before the first frame update
    public static RewardManager instance;

    internal int amount = 25;
    [SerializeField] GameObject info;
    [SerializeField] float time = 10;
    float vol;
    bool pause;
    internal bool freeCar = false;
    int sounds;
    private void Awake()
    {
        if (instance == null)
            instance = this;

        DontDestroyOnLoad(gameObject);
        amount = 50;
    }
   public void Reward()
    {
        sounds = PlayerPrefs.GetInt("sounds");
        AudioListener.pause = true;
        AudioListener.volume = 0;
        Debug.Log(AudioListener.volume);
        //Debug.Log("SHOWED");
    }
    public void Rewarded()
    {
        
        SoundManager.instance.PlaySoundOneShot("achievement", .75f);
        if (!freeCar)
        {

            int diamond = PlayerPrefs.GetInt("diamond");
            diamond += amount;
            PlayerPrefs.SetInt("diamond", diamond);
            if (SceneManager.GetActiveScene().name == "Garage")
            {
                GarageManager gm = FindObjectOfType<GarageManager>();
                gm.diamondText.text = diamond.ToString();
                if (SettingsManager.instance.sounds == 0)
                {
                    AudioListener.pause = false;
                    AudioListener.volume = 1;
                }
            }
            if (SceneManager.GetActiveScene().name == "scene_night")
            {
                GameManager.instance.pauseDiamondText.text = diamond.ToString();
                GameManager.instance.loseDiamondText.text = diamond.ToString();
                GameManager.instance.winDiamondText.text = diamond.ToString();
                if (SettingsManagerNew.Instance.sounds == 0)
                {
                    AudioListener.pause = false;
                    AudioListener.volume = 1;
                }

            }

        }
        else
        {
            if (SceneManager.GetActiveScene().name == "Garage")
            {
                GarageManager gm = FindObjectOfType<GarageManager>();
                PlayerPrefs.SetInt("playercar" + gm.pointer, 1);
                gm.modeText.GetComponentInChildren<Text>().text = gm.modeMainText;
                gm.modeText.GetComponentInChildren<Text>().fontSize = 40;
                gm.modeText.GetComponentInChildren<Text>().color = Color.white;
                freeCar = false;
                if(SettingsManager.instance.sounds == 0)
                {
                    AudioListener.pause = false;
                    AudioListener.volume =1;
                }
            }
        }

        

    }
    

    IEnumerator Stop()
    {
        yield return new WaitForSeconds(1);
        info.SetActive(false);
        info.transform.localScale = Vector3.zero;
    }
}

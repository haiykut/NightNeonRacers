using UnityEngine;
using UnityEngine.UI;
using haiykut;
public class MiniMapController : MonoBehaviour
{
    public bool activate;
    bool hasSet;
    Transform player;
    [Header("Set your minimap camera tag")]
    [SerializeField] string miniCamTag = "MiniCam";
    Transform miniCam;
    [Header("Set your objects for to set active your ui object (1) and to zoom effect (2)")]
    [SerializeField] GameObject miniMapObject;
    [SerializeField] Image[] maskSprites;
    [Header("Set it to close and open minimap within a certain time")]
    [SerializeField] bool useCloseAndOpen;
    [SerializeField] float time = 1.5f;
    [SerializeField]bool canZoom = true;
    float waitTime = 1.5f;
    [SerializeField]bool zoomed = false;
    bool pressing;
    [SerializeField]bool isClosed;
    float tmpTime;


    private void Start()
    {
        tmpTime = time;
        miniCam = GameObject.FindGameObjectWithTag(miniCamTag).transform;
        if (activate)
        {
            if(miniMapObject != null)
            {
                miniMapObject.SetActive(true);
            }
  
            player = GameManager.instance.playerCar.transform;
            miniCam.gameObject.SetActive(true);
            hasSet = true;
        }
        else
        {
            if (miniMapObject != null)
            {
                miniMapObject.SetActive(false);
            }
            miniCam.gameObject.SetActive(false);
            hasSet = false;
        }
    }
    void LateUpdate()
    {
        if(hasSet)
        {
            miniCam.position = new Vector3(player.position.x, miniCam.position.y, player.position.z);
            miniCam.eulerAngles = new Vector3(miniCam.eulerAngles.x, player.eulerAngles.y, player.eulerAngles.z);
        }
        if (useCloseAndOpen)
        {
            if (pressing)
            {

                tmpTime -= Time.deltaTime;
                if (tmpTime <= 0)
                {
                    if (!isClosed)
                    {
                        miniMapObject.SetActive(false);
                        canZoom = false;
                        pressing = false;
                        isClosed = true;
                        zoomed = false;
                        tmpTime = time;
                    }
                    else
                    {
                        miniMapObject.SetActive(true);
                        canZoom = false;
                        pressing = false;
                        isClosed = false;
                        tmpTime = time;
                    }

                }
            }
            else
            {
                tmpTime = time;
            }
            if (!canZoom && !isClosed)
            {
                waitTime -= Time.deltaTime;
                if(waitTime <= 0)
                {
                    canZoom = true;
                    waitTime = 1.5f;
                }   
            }
        }


    }

    void ZoomIt()
    {
        if (!isClosed)
        {
            if (canZoom)
            {
                if (!zoomed)
                {
                    for (int i = 0; i < maskSprites.Length; i++)
                    {
                        maskSprites[i].enabled = false;
                        //nonZoomedObject[i].GetComponent<Image>().enabled = false;
                    }
                    zoomed = true;
                }
                else
                {
                    for (int i = 0; i < maskSprites.Length; i++)
                    {
                        maskSprites[i].enabled = true;
                        //nonZoomedObject[i].GetComponent<Image>().enabled = false;
                    }
                    zoomed = false;
                }
            }
       
        }
        else
        {
            for (int i = 0; i < maskSprites.Length; i++)
            {
                maskSprites[i].enabled = true;
                //nonZoomedObject[i].GetComponent<Image>().enabled = false;
            }
            zoomed = false;
        }
        
        
    }


    void ZoomIt_2()
    {
        if (canZoom)
        {
            if (!zoomed)
            {
                for (int i = 0; i < maskSprites.Length; i++)
                {
                    maskSprites[i].enabled = false;
                    //nonZoomedObject[i].GetComponent<Image>().enabled = false;
                }
                zoomed = true;
            }
            else
            {
                for (int i = 0; i < maskSprites.Length; i++)
                {
                    maskSprites[i].enabled = true;
                    //nonZoomedObject[i].GetComponent<Image>().enabled = false;
                }
                zoomed = false;
            }
        }
    }
    public void Zoom()
    {
        Invoke(nameof(ZoomIt), 0f);
 
    }

    public void Close()
    {
        if(useCloseAndOpen)
        {
            pressing = true;

        }
    }
    public void Open()
    {
        if (useCloseAndOpen)
        {
            pressing = false;

        }
        //canZoom = false;
    }


   
}

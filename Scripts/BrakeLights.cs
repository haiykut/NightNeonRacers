using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class BrakeLights : MonoBehaviour
{
    // Start is called before the first frame update 
    RCC_CarControllerV3 thisCar;
    [Header("General Settings")]
    [SerializeField] Color brakeColor;
    [SerializeField] float intensity = 120;
    public int type;
    [Header("if type is 0, set the body and material index. if type is 1 dont set these.")]
    [SerializeField] MeshRenderer body;
    [SerializeField] int materialIndex;
    [Header("if type is 1 set the tag of your brake objects. if type is 0 dont set this")]
    [SerializeField] string tag;
    bool isLoaded;
    [Header("See your brake material(s)")]
    [SerializeField] List<Material> brakeMats;
    IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();
        thisCar = GetComponent<RCC_CarControllerV3>();
        if (type == 0)
        {
            brakeMats.Add(body.materials[materialIndex]);
        }
        if(type == 1)
        {
            foreach (Transform t in GetComponentsInChildren<Transform>())
            {
                if (t.CompareTag(tag))
                {
                    brakeMats.Add(t.GetComponent<Renderer>().material);
                }

            }
        }
        isLoaded = true;
    }
    void LateUpdate()
    {
        if (isLoaded)
        {
            if (thisCar.brakeInput >= .05f && thisCar.direction == 1)
            {
                if (thisCar.canControl)
                {
                    for (int i = 0; i < brakeMats.Count; i++)
                    {
                        brakeMats[i].SetColor("_EmissionColor", brakeColor * intensity);
                        brakeMats[i].EnableKeyword("_EMISSION");
                    }
                }                     
            }
            else
            {
                for (int i = 0; i < brakeMats.Count; i++)
                {
                   
                    brakeMats[i].DisableKeyword("_EMISSION");
                }
            }
        }                          
    }
}

using haiykut;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WaterSplash : MonoBehaviour
{
    bool hasSplashed = false;
    SpawnSystem spawnSystem;
    bool timer = false;
    float time = 3f;
    IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();   
        spawnSystem = GameManager.instance.spawn;
    }

    void Update()
    {
        if(timer)
        {
            time -= Time.deltaTime;
            spawnSystem.AutomaticRewindStart();
            if(time <= 0 )
            {
                spawnSystem.AutomaticRewindStop();
                timer = false;
                hasSplashed = false;
                time =3f;
            }
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (!hasSplashed)
        {
            timer = true;
            hasSplashed = true;
        }
    }
}

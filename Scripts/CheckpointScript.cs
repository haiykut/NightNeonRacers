using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using haiykut;
public class CheckpointScript : MonoBehaviour
{
    public int checkpointId;
    public bool finishLine;
    Collider thisColl;
    private void Start()
    {
        thisColl = GetComponent<Collider>();
    }
    private void OnTriggerEnter(Collider other)
    {
        
        if (other.transform.CompareTag("Collider"))
        {
            PlayerScript car = other.transform.root.GetComponent<PlayerScript>();
            //Debug.Log(car.gameObject.name);
            if (car != null)
            {
                if (car.chId == checkpointId)
                {
                    //Debug.Log(other.gameObject.name);
                    car.skor += 1000;
                    car.chId += 1;
                    //Aracýn geçtiði s
                    car.lastPosition = new Vector3(thisColl.bounds.center.x, thisColl.bounds.center.y + 5, thisColl.bounds.center.z);
                    car.lastRotation = transform.eulerAngles;
                    if (finishLine && !RaceManager.instance.hasRaceFinished)
                    {
                        //Debug.Log(car.id);
                        StartCoroutine(RaceManager.instance.RaceDone(car.id));
                       // for(int i = 0; i<GameManager.instance.cars.Count; i++)
                        //{
                            //GameManager.instance.cars[i].GetComponent<RCC_CarControllerV3>().canControl = false;
                            //GameManager.instance.cars[i].GetComponent<RCC_CarControllerV3>().brakeInput = 1;                          
                        //}
                        RaceManager.instance.hasRaceFinished = true;
                    }
                }
                else
                {
                    if (car.chId < checkpointId)
                    {
                        //RCC.Transport(car.GetComponent<RCC_CarControllerV3>(), RaceManager.instance.checkList[car.chId].transform.position + new Vector3(0, 3f, 0), Quaternion.identity);
                    }

                }
            }

        }
    }
}

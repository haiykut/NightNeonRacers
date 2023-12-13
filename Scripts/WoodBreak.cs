using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
[RequireComponent(typeof(Rigidbody))]
public class WoodBreak : MonoBehaviour
{
    Rigidbody body;
    bool hasCrashed = false;
    [SerializeField] float mass = 10;
    MeshCollider collider;
    int a = 5;
    bool thrw = false;
    float time = 2f;
    Transform car;
    Vector3 coll;
    int carDirection;
    int carSpeed;
    void Start()
    {
        mass = 10;
        time = 2;
        GameObject go = GameObject.Find("Leaf " + gameObject.name.Substring(a));
        //Debug.Log("Leaf" + gameObject.name.Substring(a));
        //Debug.Log(gameObject.name.Substring(a));
        if (go!=null)
        {
            //Debug.Log(go.name);
            go.transform.SetParent(transform);
        }
        gameObject.isStatic = false;
        collider = GetComponent<MeshCollider>();
        if(collider != null)
        collider.convex = true;
        body = GetComponent<Rigidbody>();
        body.isKinematic = true;
        body.mass = mass;
        hasCrashed = false;
    }
    private void FixedUpdate()
    {
        if (thrw)
        {
            time -= Time.fixedDeltaTime;
            //Debug.Log(coll.ToString());
            //body.AddForceAtPosition(car.position, coll, ForceMode.Force);
            if(carDirection == 1)
            body.AddForce(car.forward * 10 * carSpeed);
            else if(carDirection == -1)
                body.AddForce(-car.forward * 10* carSpeed);

            //body.AddForce(Vector3.up, ForceMode.Force);
            if (time <= 0)
            {
                time = 2f;
                Destroy(gameObject);
                thrw = false;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!hasCrashed && collision.transform.root.CompareTag("Player"))
        {
            transform.SetParent(null);
            SoundManager.instance.PlaySoundOneShot("wood", .75f);
            car = collision.transform;
            carDirection = car.root.GetComponent<RCC_CarControllerV3>().direction;
            carSpeed = (int)car.root.GetComponent<RCC_CarControllerV3>().speed;
            coll = transform.position;
            body.isKinematic = false;
            //body.
            thrw = true;
            hasCrashed = true;
        }
    }
}

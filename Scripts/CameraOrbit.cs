using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class CameraOrbit : MonoBehaviour
{
    [Header("General")]
    public Transform target;
    public bool autoRotateOn = false;
    public bool autoRotateReverse = false;
    public float autoRotateSpeed = 1f;
    float originalAutoRotateSpeed;
    public float autoRotateSpeedFast = 5f;
    public float autoRotateValue = 1;
    public float distance = 1.5f;
    public float distanceMin = 1f;
    public float distanceMax = 3f;

    public float xSpeed = 15.0f;
    public float ySpeed = 15.0f;
	public float yMinLimit = -20f;
	public float yMaxLimit = 80f;
	public float smoothTime = 2f;
	public float autoTimer = 5f;
	float rotationYAxis = 0.0f;
	float rotationXAxis = 0.0f;
	float velocityX = 0.0f;
	float velocityY = 0.0f;
	bool faster;
    [Header("PC")]
    public float speed = 1;
    private bool rkeyActive;
    float dis;
    float dis2;
    [Header("Mobile")]
    [SerializeField] bool mobile;
	[SerializeField] float xSpeedForMobile = 1f;
	[SerializeField] float ySpeedForMobile = 2f;
	public bool touchable = false;
	[Range(0,1)]
	[SerializeField] float touchPressure;
	bool canMove = false;
	[Range(0,5)]
	[SerializeField] float mobileSwipeSpeed;
    [SerializeField] bool mobileAutomatic;
    private float prevMagnitude = 0;
    private int touchCount = 0;

    void Start()
	{

        if (mobile)
        {
            if (mobileAutomatic)
            {
                mobileSwipeSpeed = 3;
                xSpeedForMobile = Screen.currentResolution.refreshRate * 3;
                ySpeedForMobile = Screen.currentResolution.refreshRate * 3;

            }
            // pinch gesture
            var touch0contact = new InputAction
            (
                type: InputActionType.Button,
                binding: "<Touchscreen>/touch0/press"
            );
            touch0contact.Enable();
            var touch1contact = new InputAction
            (
                type: InputActionType.Button,
                binding: "<Touchscreen>/touch1/press"
            );
            touch1contact.Enable();

            touch0contact.performed += _ => touchCount++;
            touch1contact.performed += _ => touchCount++;
            touch0contact.canceled += _ =>
            {
                touchCount--;
                prevMagnitude = 0;
            };
            touch1contact.canceled += _ =>
            {
                touchCount--;
                prevMagnitude = 0;
            };
            
            var touch0pos = new InputAction
            (
                type: InputActionType.Value,
                binding: "<Touchscreen>/touch0/position"
            );
            touch0pos.Enable();
            var touch1pos = new InputAction
            (
                type: InputActionType.Value,
                binding: "<Touchscreen>/touch1/position"
            );
            touch1pos.Enable();
            touch1pos.performed += _ =>
            {
                if (touchCount < 2)
                    return;
                var magnitude = (touch0pos.ReadValue<Vector2>() - touch1pos.ReadValue<Vector2>()).magnitude;
                if (prevMagnitude == 0)
                    prevMagnitude = magnitude;
                var difference = magnitude - prevMagnitude;
                prevMagnitude = magnitude;
      
                    CameraZoom(-difference * mobileSwipeSpeed / 1000);

            };
        }
     
        rkeyActive = autoRotateOn;
		autoRotateValue = 1;
		Vector3 angles = transform.eulerAngles;
		rotationYAxis = angles.y;
		rotationXAxis = angles.x;
		originalAutoRotateSpeed = autoRotateSpeed;
		if (GetComponent<Rigidbody>())
		{
			GetComponent<Rigidbody>().freezeRotation = true;
		}
	}
    void MoveCamera(float increment)
    {
     
    }
    void CameraZoom(float increment)
    {
        if (mobile)
        {
            if (distance <= distanceMax && distance >= distanceMin)
                distance += increment;
        }
        if (distance < distanceMin)
            distance = distanceMin;
        if (distance > distanceMax)
            distance = distanceMax;
    }
    
	private void Update()
	{
	 
        if (autoRotateOn)
		{
			velocityX += (autoRotateSpeed * autoRotateValue) * Time.deltaTime;
		}
		if (Input.GetKeyUp("r") && autoRotateOn == false)
		{
			autoRotateOn = true;
			rkeyActive = true;

		}
		else if (Input.GetKeyUp("r") && autoRotateOn == true)
		{
			autoRotateOn = false;
			rkeyActive = false;
		}

		if (Input.GetKeyDown(KeyCode.LeftShift) && (!faster))
		{
			faster = true;
			autoRotateSpeed = autoRotateSpeedFast;
			autoRotateOn = true;
		}

		if (Input.GetKeyUp(KeyCode.LeftShift) && (faster))
		{
			faster = false;
			autoRotateSpeed = originalAutoRotateSpeed;
			if (rkeyActive == false)
			{
				autoRotateOn = false;
			}
		}

		if (autoRotateReverse == true)
		{
			autoRotateValue = -1;
		}
		else
		{
			autoRotateValue = 1;
		}
	}

	void asdf()
    {
        /*
        if (mobile)
        {

            Touch[] touches = Input.touches;

            if (Input.touchCount == 1 && !canMove)
            {
                Touch touch = touches[0];
                if (touch.phase == TouchPhase.Stationary)
                {
                    dis = touches[0].position.x;
                }
                if (touch.phase == TouchPhase.Moved && !EventSystem.current.IsPointerOverGameObject())
                {
                    velocityX += xSpeedForMobile * touch.deltaPosition.x * speed / 10000;
                    velocityY += ySpeedForMobile * touch.deltaPosition.y / 10000;
                }
            }
            if (Input.touchCount > 1)
            {


                if (touches[1].phase == TouchPhase.Began)
                {
                    //firstPosForFinger1 = touches[0].position.x;
                    //firstPosForFinger2 = touches[1].position.x;#
                    dis2 = touches[1].position.x;
                    //dis = touches[0].position.x;
                }
                if ((touches[0].phase == TouchPhase.Stationary && touches[1].phase == TouchPhase.Stationary) )
                {
                    //firstPosForFinger1 = touches[0].position.x;
                    //firstPosForFinger2 = touches[1].position.x;

                    dis2 = touches[1].position.x;
                    dis = touches[0].position.x;
                    canMove = true;
                }
                if ((touches[0].phase == TouchPhase.Moved && touches[1].phase == TouchPhase.Moved) && canMove)
                {
                    txt_1.text = Mathf.Abs((touches[0].position.x - firstPosForFinger1)).ToString();
                    txt_2.text = Mathf.Abs((touches[1].position.x - firstPosForFinger2)).ToString();
                    //distance += Mathf.Lerp(distance, Mathf.Clamp(((dis - touches[0].position.x) + (dis2 - touches[1].position.x)) / 2, distanceMin, distanceMax), Time.deltaTime * smoothTimeMultiplierForMobile);
                    if (distance >= distanceMin && distance <= distanceMax)
                    {
                        distance += (((dis - touches[0].position.x) + (dis2 - touches[1].position.x) / 2) / 1000) * mobileSwipeSpeed;
                    }
                    if (distance < distanceMin)
                        distance = distanceMin;
                    if (distance > distanceMax)
                        distance = distanceMax;
                }
                if (touches[1].phase == TouchPhase.Ended || touches[0].phase == TouchPhase.Ended)
                {

                    dis = 0;
                    dis2 = 0;
                    canMove = false;
                    if (distance < distanceMin)
                        distance = distanceMin;
                    if (distance > distanceMax)
                        distance = distanceMax;
                }
            }

        }
       */
    }
	void FixedUpdate()
	{
		if (target != null)
		{
			if (touchable)
			{
                if (!mobile)
                {
                   
                        if (Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject())
                        {

                            velocityX += xSpeed * Input.GetAxis("Mouse X") * speed * 0.02f;
                            velocityY += ySpeed * Input.GetAxis("Mouse Y") * 0.02f;

                        }
                        if (!EventSystem.current.IsPointerOverGameObject())
                            distance = Mathf.Clamp(distance - Input.GetAxis("Mouse ScrollWheel") * 5, distanceMin, distanceMax);
                    
                }
                else
                {
                    if(Input.touchCount == 1)
                    {
                        velocityX += xSpeedForMobile * Input.touches[0].deltaPosition.x * speed * 0.00002f;
                        velocityY += ySpeedForMobile * Input.touches[0].deltaPosition.y * 0.00002f;
                    }
                }
            } 
		
   
		}
		else
		{
			Debug.LogWarning("Orbit Camera - No Target Set");
		}
        rotationYAxis += velocityX;
        rotationXAxis -= velocityY;
        rotationXAxis = ClampAngle(rotationXAxis, yMinLimit, yMaxLimit);
        Quaternion toRotation = Quaternion.Euler(rotationXAxis, rotationYAxis, 0);
        Quaternion rotation = toRotation;
        Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
        Vector3 position = rotation * negDistance + target.position;
        transform.rotation = rotation;
        transform.position = position;
        velocityX = Mathf.Lerp(velocityX, 0, Time.deltaTime * smoothTime);
        velocityY = Mathf.Lerp(velocityY, 0, Time.deltaTime * smoothTime);


    }
	public static float ClampAngle(float angle, float min, float max)
	{
		if (angle < -360F)
			angle += 360F;
		if (angle > 360F)
			angle -= 360F;
		return Mathf.Clamp(angle, min, max);
	}


}

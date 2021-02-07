using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlapManager : MonoBehaviour
{
    public Transform defaultPosition;
    public Transform dodgePosition;

    public GameObject slapColliderEast;
    public GameObject slapColliderWest;

    public bool isDodging = false;
    public bool isSlapped = false;

    public GameObject graphicsHolder;

    public float startingDodgeTime = 0.5f;
    public float currentDodgeTime;

    public float startingSlapTimer = 0.5f;
    public float currentSlapTimer;

    void Start()
    {
        currentDodgeTime = startingDodgeTime;
        currentSlapTimer = startingSlapTimer;
    }

    void Update()
    {
        // Dodge time countdown and reset.
        if (isDodging)
        {
            currentDodgeTime -= Time.deltaTime;
            if (currentDodgeTime <= 0)
            {
                graphicsHolder.transform.Rotate(50, 0, 0);
                currentDodgeTime = startingDodgeTime;
                isDodging = false;
                graphicsHolder.transform.position = defaultPosition.transform.position;
                graphicsHolder.transform.localEulerAngles = new Vector3(0, 0, 0);
            }
        }

        if (isSlapped)
        {
            currentSlapTimer -= Time.deltaTime;
            if (currentSlapTimer <= 0)
            {
                currentSlapTimer = startingSlapTimer;
                isSlapped = false;
                this.transform.localEulerAngles = new Vector3(0, 180, 0);
            }
        }
    }

    /*
    private void OnMouseEnter()
    {
        //Dodge();

        if (isDodging == false && isSlapped == false)
        {
            isSlapped = true;
            Debug.Log("POW!");
            this.GetComponent<HitPointsManager>().RegisterHit(1);
            this.transform.localEulerAngles = new Vector3(0, 230, 0);

        }
    }
    */

    private void Dodge()
    {
        isDodging = true;
        graphicsHolder.transform.position = dodgePosition.transform.position;
        graphicsHolder.transform.localEulerAngles = new Vector3(0, 0, 0);
    }

    public void GetSlapped(int yRotation)
    {
        if (isDodging == false && isSlapped == false)
        {
            isSlapped = true;
            Debug.Log("POW!");
            this.GetComponent<HitPointsManager>().RegisterHit(1);
            this.transform.localEulerAngles = new Vector3(0, yRotation, 0);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegisterSlap : MonoBehaviour
{
    public GameObject slappedCharacter;

    // slapRotationY should be different of the slap comes from the left or right.
    // Default 230 for the east collider, 130 for the west collider.
    public int slapRotationY;

    private void OnMouseEnter()
    {
        Debug.Log("Mouse enter " + this.name);
        slappedCharacter.GetComponent<SlapManager>().GetSlapped(slapRotationY);
    }
}

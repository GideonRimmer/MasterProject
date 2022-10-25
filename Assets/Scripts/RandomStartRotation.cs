using UnityEngine;

public class RandomStartRotation : MonoBehaviour
{
    public bool applyRandomRotation;
    [SerializeField] private float angleY;

    void Awake()
    {
        if (applyRandomRotation == true)
        {
            angleY = Random.Range(0, 360);
            transform.eulerAngles = new Vector3(transform.rotation.x, angleY, transform.rotation.z);
        }
    }
}

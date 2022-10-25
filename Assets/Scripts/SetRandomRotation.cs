using UnityEngine;

public class SetRandomRotation : MonoBehaviour
{
    [SerializeField] private GameObject[] objects;
    private float angleY;
    public string tagToRotate;


    void Awake()
    {
        objects = GameObject.FindGameObjectsWithTag(tagToRotate);
        RotaeObjectsY();
    }

    void RotaeObjectsY()
    {
        foreach (GameObject obj in objects)
        {
            angleY = Random.Range(0, 360);
            obj.transform.eulerAngles = new Vector3(transform.rotation.x, angleY, transform.rotation.z);
        }
    }
}

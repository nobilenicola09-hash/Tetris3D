using UnityEngine;

public class RotationGeometry : MonoBehaviour
{
    [SerializeField] float rotationSpeed = 50f;

    void Update()
    {
        // Ruota l'oggetto su se stesso ogni frame
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
        transform.Rotate(Vector3.right * (rotationSpeed * 0.5f) * Time.deltaTime);
    }
}
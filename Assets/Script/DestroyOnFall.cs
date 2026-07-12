using UnityEngine;

public class DestroyOnFall : MonoBehaviour
{
    void Update()
    {
        // Controlliamo se l'oggetto è caduto nel vuoto
        if (transform.position.y < -2f)
        {
            //  Cerchiamo il tavolo per togliere i punti
            CollisonDetected floor = Object.FindFirstObjectByType<CollisonDetected>();
            ShapeValue sv = GetComponent<ShapeValue>();

            if (floor != null && sv != null)
            {
                floor.RemovePoints(sv.Point);
            }

            // Comunichiamo al GameManager che abbiamo fatto cadere un ogetto su 3 
            if (GameManager.Instance != null)
            {
                GameManager.Instance.NotifyObjectFallen();
            }

            //  Distruggiamo l'oggetto
            Destroy(gameObject);
        }
    }

}
using UnityEngine;



public class DragWithMouse : MonoBehaviour
{
    private Vector3 screenPoint;
    private Vector3 offset;
    private float offsetZ; 

    private void OnMouseDown()
    {
        screenPoint = Camera.main.WorldToScreenPoint(gameObject.transform.position);
        Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
        Vector3 worldMousePos = Camera.main.ScreenToWorldPoint(mousePos);

        
        offset = gameObject.transform.position - worldMousePos;

        //  sottraiamo la Y del mouse nel mondo dalla Z dell'oggetto 
        //è necesario per evitare lo scatto
       
        offsetZ = gameObject.transform.position.z - worldMousePos.y;

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            
            if (!rb.isKinematic)//per evitare la forza di gravità 
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }

            
            rb.isKinematic = true;
        }
    }

    private void OnMouseDrag()
    {
        Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
        Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint);

        if (GameManager.Instance != null && GameManager.Instance.DragOnVerticalPlane)
        {
            // Modalità X, Y: usiamo l'offset normale
            transform.position = curPosition + offset;
        }
        else
        {
            // Modalità X, Z: mantieniamo la Y attuale e usa l'offset Z per la profondità
            float newZ = curPosition.y + offsetZ;
            transform.position = new Vector3(curPosition.x + offset.x, transform.position.y, newZ);
        }
    }

    private void OnMouseUp()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false; 

            
            if (!rb.isKinematic)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }

            rb.WakeUp();
            rb.AddForce(Vector3.down * 0.1f, ForceMode.Impulse);
        }

        Collider myCol = GetComponent<Collider>();
        if (myCol != null) myCol.enabled = true;
    }
}
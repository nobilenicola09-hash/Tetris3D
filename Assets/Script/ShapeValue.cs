using UnityEngine;

public class ShapeValue : MonoBehaviour
{
    [SerializeField] private int point;
    [SerializeField] private PrimitiveType type;
    [SerializeField] private int mergeCount = 0;
    [SerializeField] private AudioClip explode;

 
    public int Point => point;
    public PrimitiveType Type => type;
    public int MergeCount => mergeCount;
    public AudioClip Explode => explode;

    public bool IsDestroyedByMerge { get; private set; } = false;

    private Rigidbody rb;
    private MeshRenderer meshRenderer;
    private AudioSource audioSource;
    private Color originalColor;

    private bool isMerging = false;
    private bool isGray = false;
    private float timer = 0f;
    private float delay = 1.2f;

    public void SetExplosionSound(AudioClip clip) { explode = clip; }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer != null) originalColor = meshRenderer.material.color;
        audioSource = GetComponent<AudioSource>();
    }

    public void Setup(PrimitiveType t)
    {
        this.type = t;
        switch (t)
        {
            case PrimitiveType.Cube: point = 100; break;
            case PrimitiveType.Sphere: point = 200; break;
            case PrimitiveType.Capsule: point = 250; break;
            case PrimitiveType.Cylinder: point = 150; break;
        }
    }

    void Update()
    {
        if (rb == null || isMerging) return;

        if (rb.isKinematic)
        {
            ResetState();
            return;
        }

        if (rb.IsSleeping())
        {
            if (!isGray)
            {
                timer += Time.deltaTime;
                if (timer >= delay)
                {
                    meshRenderer.material.color = Color.gray;
                    isGray = true;
                }
            }
        }
        else
        {
            ResetState();
        }
    }

    void ResetState()
    {
        timer = 0f;
        if (isGray && meshRenderer != null)
        {
            meshRenderer.material.color = originalColor;
            isGray = false;
            if (rb != null) rb.WakeUp();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isMerging) return;

        ShapeValue other = collision.gameObject.GetComponent<ShapeValue>();

        if (other != null && !other.isMerging && other.Type == this.type && other.MergeCount == this.mergeCount)
        {
            if (gameObject.GetInstanceID() > collision.gameObject.GetInstanceID())//se il mio id è più grande ti mangio 
            {
                Merge(other);
            }
        }
    }

    void Merge(ShapeValue other)
    {
        isMerging = true;
        other.isMerging = true;
        other.IsDestroyedByMerge = true;



        if (rb != null)
        {
           
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

           
            rb.isKinematic = true;
        }

        
        Collider myCol = GetComponent<Collider>();
        if (myCol != null) myCol.enabled = false;

        Collider otherCol = other.GetComponent<Collider>();
        if (otherCol != null) otherCol.enabled = false;

        //Punteggio 
        CollisonDetected floor = Object.FindFirstObjectByType<CollisonDetected>();//cerchiamo il pavimento 
        if (floor != null) floor.AddPoints(other.Point);//aggiungiamo punti 

        this.point += other.Point;
        Vector3 puntoMedio = (transform.position + other.transform.position) / 2f;//punto dove nascerà la nuova forma calcolo punto medio 
        mergeCount++;
        Debug.Log($"<color=yellow>BONUS MERGE!Guadagnati {other.Point} punti. Punti Bonus Totali: {this.point}</color>");

        // esplosione 
        if (mergeCount > 2)
        {
            GameManager gm = Object.FindFirstObjectByType<GameManager>();
            if (gm != null && explode != null)
                gm.GetComponent<AudioSource>().PlayOneShot(explode);

            Destroy(other.gameObject);
            Destroy(this.gameObject);
            return; //non serve fare altro
        }

        // Crescita 
        transform.position = puntoMedio;
        transform.localScale += other.transform.localScale;
        transform.position += new Vector3(0, 0.05f, 0);

        Color nuovoColore = Random.ColorHSV(0f, 1f, 0.7f, 1f, 0.7f, 1f);
        if (meshRenderer != null)
        {
            meshRenderer.material.color = nuovoColore;
            originalColor = nuovoColore;
        }

        Destroy(other.gameObject);//"addio tensing"
        Invoke("ResetMerge", 0.2f);//aspetta un pò prima di chaiamre resetmerge 
    }

    void ResetMerge()
    {
        isMerging = false; // Permette nuove collisioni

        Collider myCol = GetComponent<Collider>();
        if (myCol != null) myCol.enabled = true; // Riaccende il collider

        if (rb != null)
        {
            rb.isKinematic = false; // Torna a subire la gravità
            rb.WakeUp();
            
            rb.AddForce(Vector3.down * 2f, ForceMode.Impulse);//una spinta in basso favorisce la lettura di collisioni 
        }
    }

    void OnDestroy()
    {
        CancelInvoke();
    }
}
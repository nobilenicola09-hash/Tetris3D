using UnityEngine;


public class GameManager : MonoBehaviour
{
    

    public static GameManager Instance { get; private set; }

    [SerializeField] private Vector3 nextShapePreviewPos = new Vector3(-20.48f, -0.77f, -1.69f);
    [SerializeField] private bool dragOnVerticalPlane = true;
    [SerializeField] private AudioClip spawn_geometry;
    [SerializeField] private AudioClip explosionSound;
    [SerializeField] private AudioClip Win;
    [SerializeField] private AudioClip Lose;

    private PrimitiveType primitiveToPlace;
    private GameObject priewObject;
    private AudioSource audioSource;
    private Texture metallicTexture;
    private int fallenObjects = 0;
    private bool isGameOver = false;


    public int FallenObjects => fallenObjects;//nel tavolo
    public bool DragOnVerticalPlane => dragOnVerticalPlane;//per capire dove muovere l ogetto 
    public AudioClip ExplosionSound => explosionSound;

    void Awake()
    {
       
        if (Instance == null) Instance = this;//una solo copia di game manager 
    }

    void Start()
    {
        Debug.Log("<color=purple>Modalità: CLICCA E PER CAMBIARE MODALITA'!!!</color>");
        audioSource = GetComponent<AudioSource>();
        audioSource.Play();
        metallicTexture = Resources.Load<Texture>("metalic_texture");

    }

    Color GetRandomConsistentColor()
    {
        Color randomColor = Random.ColorHSV();
        float H, S, V;
        Color.RGBToHSV(randomColor, out H, out S, out V);

       
        S = 0.8f;
        V = 0.8f;

        return Color.HSVToRGB(H, S, V);
    }
    void GenerateNextShape()
    {


        if (spawn_geometry != null)
            audioSource.PlayOneShot(spawn_geometry);

        switch (Random.Range(0, 4))
        {
            case 0: primitiveToPlace = PrimitiveType.Cube; break;
            case 1: primitiveToPlace = PrimitiveType.Sphere; break;
            case 2: primitiveToPlace = PrimitiveType.Capsule; break;
            case 3: primitiveToPlace = PrimitiveType.Cylinder; break;
            default: primitiveToPlace = PrimitiveType.Cube; break;
        }

        if (priewObject) Destroy(priewObject);

        priewObject = GameObject.CreatePrimitive(primitiveToPlace);
        priewObject.name = "Preview shape";
        priewObject.transform.position = nextShapePreviewPos;



        MeshRenderer mr = priewObject.GetComponent<MeshRenderer>();
        if (mr != null)
        {
         
            mr.material.color = GetRandomConsistentColor();
            
            mr.material.mainTexture = metallicTexture;
        }

        priewObject.AddComponent<RotationGeometry>();
       

        // togliamo il collider alla preview così non interagisce con nulla
        if (priewObject.GetComponent<Collider>())
            priewObject.GetComponent<Collider>().enabled = false;
    }


    
    public void EndGame(bool won)
    {
        if (isGameOver) return;

        
        isGameOver = true;
        float delay = 0f;

        if (won)
        {
            Debug.Log("<color=green><b>COMPLIMENTI! HAI VINTO!</b></color>");
            if (Win != null)
            {
                audioSource.PlayOneShot(Win);
                delay = Win.length;
              
            }
        }
        else
        {
            Debug.Log("<color=red><b>GAME OVER! HAI PERSO!</b></color>");
            if (Lose != null)
            {
                audioSource.PlayOneShot(Lose);
                delay = Lose.length; 
            }
        }


        Invoke("CloseApplication", delay);
    }

 
    private void CloseApplication()
    {
       
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }


    public void NotifyObjectFallen()
    {
        fallenObjects++;
        Debug.Log($"<color=orange>Oggetto caduto! Totale: {fallenObjects}/3</color>");

        
        CollisonDetected tavolo = Object.FindFirstObjectByType<CollisonDetected>();
        if (tavolo != null)
        {
            tavolo.AggiornaTesto();
        }

        if (fallenObjects >= 3)
        {
            EndGame(false);
        }
    }


    void Update()
    {


        if (isGameOver) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            dragOnVerticalPlane = !dragOnVerticalPlane;

            if (dragOnVerticalPlane)
                Debug.Log("<color=cyan>MODALITÀ: Base (X, Y)</color>");
            else
                Debug.Log("<color=blue>MODALITÀ: Riposizionamento (X, Z)</color>");
        }



        if (Input.GetMouseButtonDown(1))
        {
            Ray ray=Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;

            if(Physics.Raycast(ray,out hit, 100))
            {
                GameObject go = GameObject.CreatePrimitive(primitiveToPlace);
                ShapeValue sv = go.AddComponent<ShapeValue>();
                sv.Setup(primitiveToPlace);
                sv.SetExplosionSound(explosionSound); 

                go.GetComponent<MeshRenderer>().material.color = GetRandomConsistentColor();
                go.transform.localScale = Vector3.one * 0.3f;
                go.transform.position = hit.point + new Vector3(0, 1f, 0);
                go.transform.rotation = Random.rotation;

                go.AddComponent<Rigidbody>();


                //Control color randomness
                MeshRenderer mr = go.GetComponent<MeshRenderer>();
                mr.material.color = GetRandomConsistentColor();
                mr.material.mainTexture = metallicTexture;





               


                go.AddComponent<DestroyOnFall>();

                go.AddComponent<DragWithMouse>();

              

                
                GenerateNextShape();



            }
        }





    }
}

using UnityEngine;
using TMPro;



public class CollisonDetected : MonoBehaviour
{
    [SerializeField] private TextMeshPro scoreText;
    private int totalscore = 0;
    

    private void Start()
    {
        if (scoreText == null) scoreText = GetComponentInChildren<TextMeshPro>();
        totalscore = 0;
       
        AggiornaTesto();
    }

    public void AddPoints(int points)
    {
        totalscore += points;
        AggiornaTesto();

        // CONDIZIONE DI VITTORIA
        if (totalscore >= 20000)
        {
            GameManager.Instance.EndGame(true);
        }
    }

    public void RemovePoints(int points)
    {
        totalscore -= points;
        if (totalscore < 0) totalscore = 0; // il punteggio non va sotto zero
        AggiornaTesto();
    }
    private void OnCollisionEnter(Collision collision)
    {
        ShapeValue sv = collision.gameObject.GetComponent<ShapeValue>();
        if (sv != null)
        {
            AddPoints(sv.Point);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        ShapeValue sv = collision.gameObject.GetComponent<ShapeValue>();

        if (sv != null)
        {
            if (sv.IsDestroyedByMerge) return;

           
        }
    }

    public void AggiornaTesto()
    {
        
        if (scoreText != null && GameManager.Instance != null)
        {
            int cadutiReal = GameManager.Instance.FallenObjects;
            scoreText.text = "Punteggio: " + totalscore + "\nCaduti: " + cadutiReal + "/3";
        }
    }
}
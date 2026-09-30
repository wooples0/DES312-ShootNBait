using UnityEngine;
using TMPro;

public class CountLabel : MonoBehaviour
{
    [SerializeField] private Color baseColour;
    [SerializeField] private Color secondaryColour;

    [SerializeField] private TMP_Text labelText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetColour(int i)
    {
        switch (i)
        {
            case 0:
                labelText.color = baseColour; 
                break;
            case 1:
                labelText.color = secondaryColour; 
                break;
        }
    }
}

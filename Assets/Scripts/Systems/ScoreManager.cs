using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;
    public int bulletsUsed;
    public int baitUsed;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void IncrementBulletsUsed(int i)
    {
        bulletsUsed += i;
    }

    public void IncrementBaitUsed(int i)
    {
        baitUsed += i;
    }

    public void ResetAllValues()
    {
        bulletsUsed = 0;
        baitUsed = 0;
    }
}

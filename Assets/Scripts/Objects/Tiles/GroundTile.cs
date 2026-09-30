using System.Reflection.Emit;
using UnityEngine;
using UnityEngine.UIElements;

public class GroundTile : Tile
{
    [SerializeField] private Color baseColour, offsetColour;
    public override void Init(int x, int y)
    {
        positionLabel.text = ($"{x.ToString()},{y.ToString()}");
        var isOffset = (x + y) % 2 == 1;
        renderer.color = isOffset ? offsetColour : baseColour;
    }
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

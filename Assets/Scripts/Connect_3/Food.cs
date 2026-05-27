using UnityEngine;

public class Food : MonoBehaviour
{
    public  FoodType foodType; 
    
    public int xIndex;
    public int yIndex;

    public bool isMatched;
    private Vector2 currentPosition;
    private Vector2 targetPosition;
    
    public bool isMoving;
    
    // public Food(int _x, int _y)
    // {
    //     xIndex = _x;
    //     yIndex = _y;
    // }

    public void SetIndecies(int _xIndex, int _yIndex)
    {
        xIndex = _xIndex;
        yIndex = _yIndex;
    }
}
public enum FoodType
{
    Arroz,
    Bolacha,
    Pao
}
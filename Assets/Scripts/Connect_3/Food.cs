using System.Collections;
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
    
    // MoveToTarget
    public void MoveToTarget(Vector2 _targetPos)
    {
        StartCoroutine(MoveCoroutine(_targetPos));
    }
    
    // MoveCoroutine
    private IEnumerator MoveCoroutine(Vector2 _targetPos)
    {
        isMoving = true;
        float duration = 0.2f;
        
        Vector2 startPosition = transform.position;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            transform.position = Vector2.Lerp(startPosition, _targetPos, t);
            
            elapsedTime += Time.deltaTime;

            yield return null;
        }
        
        transform.position = _targetPos;
        isMoving = false;
    }
}
public enum FoodType
{
    Arroz,
    Bolacha,
    Pao,
    Pizza
}
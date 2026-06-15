using UnityEngine;

public class FoodSpawner : MonoBehaviour
{
    public GameObject[] foods;

    public RectTransform canvas;

    public float interval = 1f;

    void Start()
    {
        InvokeRepeating(
            nameof(SpawnFood),
            1,
            interval
        );
    }

    void SpawnFood()
    {
        int index = Random.Range(0, foods.Length);

        GameObject food = Instantiate(foods[index], canvas);

        RectTransform rt = food.GetComponent<RectTransform>();

        rt.anchoredPosition = new Vector2(
            Random.Range(-850, 850),
            650
        );

        // Coloca o alimento logo acima do background
        food.transform.SetSiblingIndex(1);
    }
}
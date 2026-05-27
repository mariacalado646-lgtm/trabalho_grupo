using UnityEngine;
using UnityEngine.Serialization;

public class FoodBoard : MonoBehaviour
{
    // define size of the board
    public int width = 5;
    public int height = 5;
    
    // define some spacing of the board
    public float spacingX;
    public float spacingY;
    
    // get a reference to our food prefabs
    public GameObject[] foodPrefabs;
    
    // get a reference to the collection nodes foodBoard + GameObject
    private Node[,] foodBoard;
    public GameObject foodBoardGameObject;
    
    // layout array
    // missing scripts
    
    //public static of FoodBoard
    public static FoodBoard Instance;

    public void Awake()
    {
        Instance = this;
    }
    
    void Start()
    {
        InitializeBoard();
    }

    void InitializeBoard()
    {
        foodBoard = new Node[width, height];
        
        spacingX = (float)(width  - 1)/2;
        spacingY = (float)(height - 1)/2;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Vector2 position = new Vector2(x - spacingX, y - spacingY);
                
                int randomIndex =  Random.Range(0, foodPrefabs.Length);
                
                GameObject food = Instantiate(foodPrefabs[randomIndex], position, Quaternion.identity);
                food.GetComponent<Food>().SetIndecies(x, y);
                foodBoard[x, y] = new Node(true, food);
            }
        }
    }
}

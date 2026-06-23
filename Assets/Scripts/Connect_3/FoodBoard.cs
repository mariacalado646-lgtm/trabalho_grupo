using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class FoodBoard : MonoBehaviour
{
    // define size of the board
    [SerializeField] private int boardDimension = 5;
    public int width => boardDimension;
    public int height => boardDimension;
    
    // define some spacing of the board
    public float spacingX;
    public float spacingY;
    
    // get a reference to our food prefabs
    public GameObject[] foodPrefabs;
    
    // get a reference to the collection nodes foodBoard + GameObject
    private Node[,] foodBoard;
    public GameObject foodBoardGameObject;
    
    public List<GameObject> foodsToDestroy = new();
    public GameObject foodParent;
    
    [SerializeField] private Food selectedFood = null;

    [SerializeField]
    private bool isProcessingMove;

    [SerializeField]
    List<Food> foodsToRemove = new();
    // layout array
    // missing scripts
    
    //public static of FoodBoard
    public static FoodBoard Instance;
    
    [Header("Board Area")]
    private readonly Vector2 boardCenter = new Vector2(0f, -0.8f);
    [SerializeField] private float boardSize = 7f;
    private float cellSize;

    // Sounds and stuff
    [Header("Sound Effects")]
    private AudioSource audioSource;
    public AudioClip selectSound;
    // public AudioClip swapSound;
    public AudioClip matchSound;
    
    
    // helper: convert grid index to world position
    private Vector3 GridToWorldPos(int x, int y, float z = 0f)
    {
        return new Vector3(
            x * cellSize - spacingX + boardCenter.x,
            y * cellSize - spacingY + boardCenter.y,
            z
        );
    }

    // helper: scale sprite to fit inside a cell
    private void ScaleToFitCell(GameObject food)
    {
        SpriteRenderer sr = food.GetComponent<SpriteRenderer>();
        float originalWidth = sr.bounds.size.x / food.transform.localScale.x;
        float originalHeight = sr.bounds.size.y / food.transform.localScale.y;
        float largestSide = Mathf.Max(originalWidth, originalHeight);
        float scale = cellSize / largestSide * 0.85f;
        food.transform.localScale = Vector3.one * scale;
    }

    public void Awake()
    {
        Instance = this;
    }
    
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        InitializeBoard();
    }


    void Update()
    {
        if (GameManagerConnect3.instance.isGameEnded) return;
        if (GameManagerConnect3.instance.isGamePaused) return;
        
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();
            Ray ray = Camera.main.ScreenPointToRay(mousePos);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

            if (hit.collider != null && hit.collider.gameObject.GetComponentInParent<Food>())
            {
                audioSource.PlayOneShot(selectSound);
                if (isProcessingMove) return;
                Food food = hit.collider.gameObject.GetComponent<Food>();
                Debug.Log($"Clicked food: {food.foodType}");
                SelectFood(food);
            }
        }
    }

    void InitializeBoard()
    {
        DestroyFoods();
        
        foodBoard = new Node[width, height];
        
        cellSize = boardSize / width;
        spacingX = (width  - 1) * cellSize / 2f;
        spacingY = (height - 1) * cellSize / 2f;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Vector2 position = GridToWorldPos(x, y);
                
                int randomIndex =  Random.Range(0, foodPrefabs.Length);
                
                GameObject food = Instantiate(foodPrefabs[randomIndex], position, Quaternion.identity);
                ScaleToFitCell(food);
                food.transform.SetParent(foodParent.transform);
                food.GetComponent<Food>().SetIndecies(x, y);
                foodBoard[x, y] = new Node(true, food);
                foodsToDestroy.Add(food);
            }
        }

        if (CheckBoard())
        {
            Debug.Log("Board initialized with matches, reinitializing");
            InitializeBoard();
        }
        else Debug.Log("Board initialized with no matches, keeping board");
    }

    private void DestroyFoods()
    {
        if (foodsToDestroy != null)
        {
            foreach (GameObject food in foodsToDestroy)
            {
                Destroy(food);
            }
            foodsToDestroy.Clear();
        }
    }
    
     public bool CheckBoard()
    {
        Debug.Log("Checking the Board");
        bool hasMatched = false;

        foodsToRemove.Clear();

        foreach (Node nodeFood in foodBoard)
        {
            if (nodeFood.food != null)
            {
                nodeFood.food.GetComponent<Food>().isMatched = false;
            }
        }

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // get food class in node 
                if (foodBoard[x, y].food == null) continue;
                Food food = foodBoard[x, y].food.GetComponent<Food>();
                
                // ensure it's not matched
                if (!food.isMatched)
                {
                    //run matching logic
                    
                    MatchResult matchedFoods = IsConnected(food);

                    if (matchedFoods.connectedFoods.Count >= 3)
                    {
                        MatchResult superMatchedFoods =  SuperMatch(matchedFoods);
                        
                        foodsToRemove.AddRange(superMatchedFoods.connectedFoods);

                        foreach (Food f in superMatchedFoods .connectedFoods)
                            f.isMatched = true;
                        hasMatched = true;
                    }
                }
                    
            }
        }
        
        return hasMatched;
    }

    public IEnumerator ProcessTurnOnMatchBoard()
    {
        audioSource.PlayOneShot(matchSound);
        foreach (Food f in foodsToRemove)
        {
            f.isMatched = false;    
        }
            
        RemoveAndRefill(foodsToRemove);
        GameManagerConnect3.instance.ProcessTurn(foodsToRemove.Count - 2);
        yield return new WaitForSeconds(1f);
        if (GameManagerConnect3.instance.isGameEnded) yield break;
        if (CheckBoard())
        {
            StartCoroutine(ProcessTurnOnMatchBoard());
        }
    }
    

    private void RemoveAndRefill(List<Food> _foodsToRemove)
    {
        // Removing the food and clearing the board at that location
        foreach (Food food in _foodsToRemove)
        {
            // getting it'sx and y indecies and storing them
            int _xIndex = food.xIndex;
            int _yIndex = food.yIndex;
            
            // Destroy the food
            Destroy(food.gameObject);
            
            // Create a blank node on the destroyed food position
            foodBoard[_xIndex, _yIndex] = new Node(true, null);
        }

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (foodBoard[x, y].food == null)
                {
                    Debug.Log($"The location [{x}, {y}] is empty, attempting to refill");
                    RefillFood(x, y);
                }
            }
        }
    }

    #region Cascading Foods
    
    // RefilFoods
    private void RefillFood(int x, int y)
    {
        // y offset
        int yOffset = 1;
        
        // While the cell above is null and we're below the height of the board, increment y offset
        while (y + yOffset < height && foodBoard[x, y + yOffset].food == null)
        {
            Debug.Log($"The potion above me is null, but I'm not at the top of the board yet, so add to my yOffset and try again. Current offset is: {yOffset}. Will add 1");
            yOffset++;
        }
        // We've either hit a food or the top of the board

        if (y + yOffset < height && foodBoard[x, y + yOffset].food != null)
        {
            // We've hit a food
            Food foodAbove = foodBoard[x, y + yOffset].food.GetComponent<Food>();
            
            // Move it to the correct location
            Vector3 targetPos = GridToWorldPos(x, y, foodAbove.transform.position.z);
            Debug.Log($"Food found when refilling he board, it was at [{x}, {y + yOffset}], we have moved it to the location [{x}, {y}]");
            
            // Move to location
            foodAbove.MoveToTarget(targetPos);
            
            // update indecies
            foodAbove.SetIndecies(x, y);
            foodBoard[x, y] = foodBoard[x, y + yOffset];
            
            // Set original food location to null
            foodBoard[x, y + yOffset] = new Node(true, null);
        }
        // if we've hit the top of the board without finding a food
        if (y + yOffset == height)
        {
            Debug.Log("Reached top of the Board without finding any food");
            SpawnFoodsAtTop(x);
        }
    }
        
    // SpawnFoodsAtTop()
    private void SpawnFoodsAtTop(int x)
    {
        int index = FindIndexOfLowestNull(x);
        Debug.Log($"Spawning foods at [{x}, {index}]");
        
        int randomIndex = Random.Range(0, foodPrefabs.Length);
        GameObject newFood = Instantiate(foodPrefabs[randomIndex], GridToWorldPos(x, height), Quaternion.identity);
        ScaleToFitCell(newFood);
        newFood.transform.SetParent(foodParent.transform);
        
        // Set Indecies
        newFood.GetComponent<Food>().SetIndecies(x, index);
        
        // Set it on the board
        foodBoard[x, index] = new Node(true, newFood);
        
        // Move it to that location
        Vector3 targetPosition = GridToWorldPos(x, index, newFood.transform.position.z);
        newFood.GetComponent<Food>().MoveToTarget(targetPosition);
    }
    
    // FindIndexOfLowestNull
    private int FindIndexOfLowestNull(int x)
    {
        int lowestNull = 99;
        for (int y = height - 1; y >= 0; y--)
        {
            if (foodBoard[x, y].food == null)
            {
                lowestNull = y;
            }
        }
        return lowestNull;
    }
    #endregion

    #region Matching Logic
    
    private MatchResult SuperMatch(MatchResult _matchedResults)
    {
        // if horizontal or long horizontal match
        if (_matchedResults.direction == MatchDirection.Horizontal ||
            _matchedResults.direction == MatchDirection.LongHorizontal)
        {
            foreach (Food f in _matchedResults.connectedFoods)
            {
                List<Food> extraConnectedFoods = new();
                
                CheckDirection(f, new Vector2Int(0, 1), extraConnectedFoods);
                CheckDirection(f, new Vector2Int(0, -1), extraConnectedFoods);

                if (extraConnectedFoods.Count >= 2) 
                {
                    Debug.Log($"Super Horizontal match found at: [{f.xIndex}, {f.yIndex}]");
                    extraConnectedFoods.AddRange(_matchedResults.connectedFoods);

                    return new MatchResult
                    {
                        connectedFoods = extraConnectedFoods,
                        direction = MatchDirection.Super
                    };
                }
            }

            return new MatchResult
            {
                connectedFoods = _matchedResults.connectedFoods,
                direction = _matchedResults.direction
            };   
        }
        // if vertical or long vertical match
        else if (_matchedResults.direction == MatchDirection.Vertical ||
            _matchedResults.direction == MatchDirection.LongVertical)
        {
            foreach (Food f in _matchedResults.connectedFoods)
            {
                List<Food> extraConnectedFoods = new();
            
                CheckDirection(f, new Vector2Int(-1, 0), extraConnectedFoods);
                CheckDirection(f, new Vector2Int(1 , 0), extraConnectedFoods);

                if (extraConnectedFoods.Count >= 2) 
                {
                    Debug.Log($"Super Vertical match found at: [{f.xIndex}, {f.yIndex}]");
                    extraConnectedFoods.AddRange(_matchedResults.connectedFoods);

                    return new MatchResult
                    {
                        connectedFoods = extraConnectedFoods,
                        direction = MatchDirection.Super
                    };
                }
            }

            return new MatchResult
            {
                connectedFoods = _matchedResults.connectedFoods,
                direction = _matchedResults.direction
            };   
        }
        return null;
    }
    
    MatchResult IsConnected(Food food)
    {
        List<Food> connectedFoods = new();
        FoodType foodType = food.foodType;
        
        connectedFoods.Add(food);
        
        // check right
        CheckDirection(food, new Vector2Int(1,0), connectedFoods);
        // check left
        CheckDirection(food, new Vector2Int(-1, 0), connectedFoods);
        // have we made a 3 match (Horizontal match)
        if (connectedFoods.Count == 3)
        {
            Debug.Log($"Normal Horizontal match at [{connectedFoods[0].xIndex},{connectedFoods[0].yIndex}], food matched is: {connectedFoods[0].foodType}");

            return new MatchResult()
            {
                connectedFoods = connectedFoods,
                direction = MatchDirection.Horizontal
            };
        }
        // checking more than 3 (Long Horizontal match)
        else if (connectedFoods.Count > 3)
        {
            Debug.Log($"Long Horizontal match at [{connectedFoods[0].xIndex},{connectedFoods[0].yIndex}], food matched is: {connectedFoods[0].foodType}");

            return new MatchResult()
            {
                connectedFoods = connectedFoods,
                direction = MatchDirection.LongHorizontal
            };
        }
        
        // clear out connectedFoods
        connectedFoods.Clear();
        // read our initial food
        connectedFoods.Add(food);
        
        // check up
        CheckDirection(food, new Vector2Int(0, 1), connectedFoods);
        //check down
        CheckDirection(food, new Vector2Int(0, -1), connectedFoods);
        // have we made a 3 match (Vertical match)
        if (connectedFoods.Count == 3)
        {
            Debug.Log($"Normal Vertical match at [{connectedFoods[0].xIndex},{connectedFoods[0].yIndex}], food matched is: {connectedFoods[0].foodType}");

            return new MatchResult()
            {
                connectedFoods = connectedFoods,
                direction = MatchDirection.Vertical
            };
        }
        // checking more than 3 (Long Vertical match)
        else if (connectedFoods.Count > 3)
        {
            Debug.Log($"Long Vertical match at [{connectedFoods[0].xIndex},{connectedFoods[0].yIndex}], food matched is: {connectedFoods[0].foodType}");

            return new MatchResult()
            {
                connectedFoods = connectedFoods,
                direction = MatchDirection.LongVertical
            };
        }
        else
        {
            return new MatchResult
            {
                connectedFoods = connectedFoods,
                direction = MatchDirection.None
            }; 
        }
    }

    void CheckDirection(Food food, Vector2Int direction, List<Food> connectedFoods)
    {
        FoodType foodType = food.foodType;
        int x = food.xIndex + direction.x;
        int y = food.yIndex + direction.y;
        
        // check we're within boundaries
        while (x >= 0 && x < width && y >= 0 && y < height)
        {
            if (foodBoard[x, y].isUsable)
            {
                Food neighbourFood = foodBoard[x, y].food.GetComponent<Food>();
            
            
                // does food type match? must also not be matched
                if (!neighbourFood.isMatched && neighbourFood.foodType == foodType)
                {
                    connectedFoods.Add(neighbourFood);
                    
                    x += direction.x;
                    y += direction.y; 
                }
                else break;
            }
            else break;
        }
    }

    public class MatchResult
    {
        public List<Food> connectedFoods;
        public MatchDirection direction; 
    }

    public enum MatchDirection
    {
        Vertical,
        Horizontal,
        LongVertical,
        LongHorizontal,
        Super,
        None
    }
    
    #endregion
    
    #region Swapping Foods
    
    // select food
    public void SelectFood(Food _targetFood)
    {
        if (selectedFood == null)
        {
            selectedFood = _targetFood;
            selectedFood.Select();
            Debug.Log($"Selected Food: {_targetFood}");
        }
        else if (selectedFood == _targetFood)
        {
            selectedFood.Deselect();
            selectedFood = null;
        }
        else if (!IsAdjacent(selectedFood, _targetFood))
        {
            selectedFood.Deselect();
            selectedFood = _targetFood;
            selectedFood.Select();
            Debug.Log($"Selected Food: {_targetFood}");
        }
        else if (selectedFood != _targetFood)
        {
            selectedFood.Deselect();
            SwapFood(selectedFood, _targetFood);
            selectedFood = null;
        }
    }
    
    // swap food logic
    private void SwapFood(Food _currentFood, Food _targetFood)
    {
        if (!IsAdjacent(_currentFood, _targetFood))
        {
            return;
        }

        DoSwap(_currentFood, _targetFood);
        
        isProcessingMove = true;

        StartCoroutine(ProcessMatches(_currentFood, _targetFood));
    }
    
    // do the swap
    private void DoSwap(Food _currentFood, Food _targetFood)
    {
        GameObject tmp = foodBoard[_currentFood.xIndex, _currentFood.yIndex].food;
        
        foodBoard[_currentFood.xIndex, _currentFood.yIndex].food = foodBoard[_targetFood.xIndex, _targetFood.yIndex].food; 
        foodBoard[_targetFood.xIndex, _targetFood.yIndex].food = tmp;
        
        // update indecies
        int tmpXIndex = _currentFood.xIndex;
        int tmpYIndex = _currentFood.yIndex;
        _currentFood.xIndex = _targetFood.xIndex;
        _currentFood.yIndex = _targetFood.yIndex;
        _targetFood.xIndex = tmpXIndex;
        _targetFood.yIndex = tmpYIndex;
        
        _currentFood.MoveToTarget(foodBoard[_targetFood.xIndex, _targetFood.yIndex].food.transform.position);
        _targetFood.MoveToTarget(foodBoard[_currentFood.xIndex, _currentFood.yIndex].food.transform.position);
    }

    private IEnumerator ProcessMatches(Food _currentFood, Food _targetFood)
    {
        yield return new WaitForSeconds(0.2f);

        if (GameManagerConnect3.instance.isGameEnded)
        {
            isProcessingMove = false;
            yield break;
        }
        
        bool hasMatch = CheckBoard();

        if (CheckBoard())
        {
            // start a coroutine that is going to process out matches in our turn
            StartCoroutine(ProcessTurnOnMatchBoard());
        }
        else DoSwap(_currentFood, _targetFood);
        
        isProcessingMove = false;
    }
    private bool IsAdjacent(Food _currentFood, Food _targetFood)
    {
        return Mathf.Abs(_currentFood.xIndex - _targetFood.xIndex) + Mathf.Abs(_currentFood.yIndex-_targetFood.yIndex) == 1;
    }
    
    
    //ProcessMatches
    #endregion
}

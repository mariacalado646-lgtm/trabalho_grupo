using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;

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
    
    public List<GameObject> foodsToDestroy = new();
    public GameObject foodParent;
    
    [SerializeField] private Food selectedFood = null;

    [SerializeField]
    private bool isProcessingMove;
    
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


    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();
            Ray ray = Camera.main.ScreenPointToRay(mousePos);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

            if (hit.collider != null && hit.collider.gameObject.GetComponentInParent<Food>())
            {
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
        
        spacingX = (float)(width  - 1)/2;
        spacingY = (float)(height - 1)/2;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Vector2 position = new Vector2(x - spacingX, y - spacingY);
                
                int randomIndex =  Random.Range(0, foodPrefabs.Length);
                
                GameObject food = Instantiate(foodPrefabs[randomIndex], position, Quaternion.identity);
                food.transform.SetParent(foodParent.transform);
                food.GetComponent<Food>().SetIndecies(x, y);
                foodBoard[x, y] = new Node(true, food);
                foodsToDestroy.Add(food);
            }
        }

        if (CheckBoard(false))
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
    
     public bool CheckBoard(bool _takeAction)
    {
        Debug.Log("Checking the Board");
        bool hasMatched = false;

        List<Food> foodsToRemove = new();

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

        if (_takeAction)
        {
            foreach (Food f in foodsToRemove)
            {
                f.isMatched = false;    
            }
            
            RemoveAndRefill(foodsToRemove);
             
            if (CheckBoard(false))
            {
                CheckBoard(true);
            }
        }
        //check for a brand new match
        
        return hasMatched;
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
            Vector3 targetPos = new Vector3(x - spacingX, y - spacingY, foodAbove.transform.position.z);
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
        int locatioToMoveTo = height - index;
        Debug.Log($"Spawning foods at [{x}, {index}]");
        
        int randomIndex = Random.Range(0, foodPrefabs.Length);
        GameObject newFood = Instantiate(foodPrefabs[randomIndex], new Vector2(x - spacingX, height - spacingY), Quaternion.identity);
        newFood.transform.SetParent(foodParent.transform);
        
        // Set Indecies
        newFood.GetComponent<Food>().SetIndecies(x, index);
        
        // Set it on the board
        foodBoard[x, index] = new Node(true, newFood);
        
        // Move it to that location
        Vector3 targetPosition = new Vector3(x - spacingX, index - spacingY, newFood.transform.position.z);
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
                        direction = MatchDirection.Horizontal
                    };
                }
            }

            return new MatchResult
            {
                connectedFoods = _matchedResults.connectedFoods,
                direction = _matchedResults.direction
            };   
        }
        
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
            // loop through foods in match
                // create a new list of foods "extra matches"
            // CheckDirection up
            // CheckDirection down
            // do we have 2 or more matches?
                // we've made a super match, return a new matchresult of type super
            // return extra matches
            
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
            // loop through foods in match
            // create a new list of foods "extra matches"
            // CheckDirection up
            // CheckDirection down
            // do we have 2 or more matches?
            // we've made a super match, return a new matchresult of type super
            // return extra matches
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
        
        // return new MatchResult(); // suppress error, to be deleted
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
        // if we don't have a food currently selected, then set food clicked to selected food
        if (selectedFood == null)
        {
            selectedFood = _targetFood;
            Debug.Log($"Selected Food: {_targetFood}");
        }
        // if we select the same food twice, unselect
        else if (selectedFood == _targetFood)
        {
            selectedFood = null;
        }
        else if (!IsAdjacent(selectedFood, _targetFood))
        {
            selectedFood = _targetFood;
            Debug.Log($"Selected Food: {_targetFood}");
        }
        // if selected food != mull and is not the current food, attempt swap
        else if (selectedFood != _targetFood)
        {
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

        bool hasMatch = CheckBoard(true);
        if (!hasMatch)
        {
            DoSwap(_currentFood, _targetFood);
        }
        isProcessingMove = false;
    }
    private bool IsAdjacent(Food _currentFood, Food _targetFood)
    {
        return Mathf.Abs(_currentFood.xIndex - _targetFood.xIndex) + Mathf.Abs(_currentFood.yIndex-_targetFood.yIndex) == 1;
    }
    
    
    //ProcessMatches
    #endregion
}
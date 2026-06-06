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

        List<Food> foodsToRemove = new();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // get food class in node
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

                        foreach (Food f in superMatchedFoods .connectedFoods) f.isMatched = true;
                        hasMatched = true;
                    }
                }
                    
            }
        }
        
        return hasMatched;
    }

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
    }
    
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

        bool hasMath = CheckBoard();
        if (!hasMath)
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
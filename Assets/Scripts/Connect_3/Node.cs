using UnityEngine;

public class Node
{
    // determine if the space can be used (might not be implemented)
    public bool isUsable;

    public GameObject food;

    public Node(bool _isUsable, GameObject _food)
    {
        isUsable = _isUsable;
        food = _food;
    }
}

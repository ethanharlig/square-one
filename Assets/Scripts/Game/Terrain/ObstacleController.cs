using UnityEngine;

public class ObstacleController : MonoBehaviour
{
    public void LogFromObstacle()
    {
        Debug.Log("You're calling an obstacle here!");
    }

    public void SetName(string name)
    {
        gameObject.name = name;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
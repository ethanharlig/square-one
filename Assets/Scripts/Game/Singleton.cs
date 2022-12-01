using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour
{
    // SINGLETON

    public static Singleton<T> Instance
    {
        get;
        private set;
    }
#pragma warning disable IDE0051
    void Awake()
    {
        // If there is an instance and it's not me, delete myself.
        Debug.LogFormat("Spawning instance of {0}", GetType().ToString());

        if (Instance != null && Instance != this)
        {
            Debug.LogFormat("destroy {0}", GetType().ToString());
            Destroy(this);
        }
        else
        {
            Instance = this;
            // don't destroy on load if we want to persist player instance throughout game
        }
    }
#pragma warning restore IDE0051
}

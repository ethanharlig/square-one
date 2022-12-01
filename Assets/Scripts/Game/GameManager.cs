using UnityEngine;

public class GameManager : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod]
    public static void InstantiatePrefabs()
    {
        // put any singletons we want in every scene here
        GameObject audio = (GameObject)Resources.Load("Prefabs/Sounds");
        GameObject music = (GameObject)Resources.Load("Prefabs/Music");

        Instantiate(audio);
        Instantiate(music);
    }
}

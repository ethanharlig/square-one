using UnityEngine;

public class ResetPlaneController : Singleton<ResetPlaneController>
{
#pragma warning disable IDE0051
    void OnTriggerEnter(Collider other)
    {
        Debug.LogFormat("Something hit me!! {0}", other.gameObject.transform.position);
        other.gameObject.GetComponent<PlayerController>().ResetPosition();
    }
#pragma warning restore IDE0051
}

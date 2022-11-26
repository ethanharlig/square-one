using UnityEngine;

public class WaypointController : MonoBehaviour
{
    public delegate void OnTriggeredAction();
    public event OnTriggeredAction OnTriggered;

#pragma warning disable IDE0051
    void OnTriggerEnter(Collider other)
    {
        if (!PlayerController.IsColliderPlayer(other)) return;
        gameObject.SetActive(false);

        OnTriggered?.Invoke();
    }
#pragma warning restore IDE0051
}

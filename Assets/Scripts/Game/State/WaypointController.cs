using UnityEngine;

public class WaypointController : MonoBehaviour
{
    public delegate void OnTriggeredAction();
    public event OnTriggeredAction OnTriggered;

    public bool EnableAudio { get; set; } = true;

#pragma warning disable IDE0051
    void OnTriggerEnter(Collider other)
    {
        if (!PlayerController.IsColliderPlayer(other)) return;
        gameObject.SetActive(false);

        if (EnableAudio)
        {
            AudioController.Instance.PlayWaypointAudio();
        }
        OnTriggered?.Invoke();
    }
#pragma warning restore IDE0051

    public void SetSize(float size)
    {
        transform.localScale = new Vector3(size, size, size);
    }

    public void SetColor(Color color)
    {
        GetComponent<MeshRenderer>().material.color = color;
    }
}

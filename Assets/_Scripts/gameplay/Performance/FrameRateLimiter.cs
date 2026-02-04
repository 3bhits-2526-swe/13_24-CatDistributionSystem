using UnityEngine;

public class FrameRateLimiter : MonoBehaviour
{
    [SerializeField] private int targetFrameRate = 30;

    private void Awake() =>
        Application.targetFrameRate = targetFrameRate;
}

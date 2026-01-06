using UnityEngine;

public class ChangeCameraBackGroundColorStart : MonoBehaviour
{
    [SerializeField] private Color startBackGroundColor = new Color();

    private void Start() => GetComponent<Camera>().backgroundColor = startBackGroundColor;
}

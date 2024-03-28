using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Camera _mainCamera;

    private void Awake()
    {
        _mainCamera = Camera.main;
    }

    private void LateUpdate()
    {
        LookAtCamera();
    }

    public void LookAtCamera()
    {
        transform.LookAt(transform.position + _mainCamera.transform.forward);
    }
}

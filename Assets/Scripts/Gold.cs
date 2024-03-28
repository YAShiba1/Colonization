using UnityEngine;

public class Gold : MonoBehaviour
{
    public bool IsReserved { get; private set; } = false;

    public void Reserve()
    {
        IsReserved = true;
    }
}
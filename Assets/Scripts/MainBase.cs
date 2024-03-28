using UnityEngine;

public class MainBase : Base
{
    private void Start()
    {
        SpawnStarterBots();
    }

    protected override void TrySelectBase()
    {
        if (Physics.Raycast(Ray, out RaycastHit hit, Mathf.Infinity, LayerMask))
        {
            if (hit.collider.gameObject.TryGetComponent(out MainBase mainBase) == this)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    IsBaseSelected = true;
                    return;
                }
            }
        }

        base.TrySelectBase();
    }

    private void SpawnStarterBots()
    {
        int amountOfStartingBots = 3;

        for (int i = 0; i < amountOfStartingBots; i++)
        {
            SpawnBot();
        }
    }
}
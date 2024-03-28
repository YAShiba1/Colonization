using UnityEngine;

public class ColonyBase : Base
{
    private GameObject _lastClickedObject;

    public void TakeBot(Bot bot)
    {
        bot.SetParentBase(this);
        Bots.Add(bot);
    }

    protected override void TrySelectBase()
    {
        if (Physics.Raycast(Ray, out RaycastHit hit, Mathf.Infinity, LayerMask))
        {
            SetLastClickedObject(hit.collider.gameObject);

            if (hit.collider.gameObject.TryGetComponent(out ColonyBase colonyBase))
            {
                if (Input.GetMouseButtonDown(0) && _lastClickedObject == gameObject)
                {
                    IsBaseSelected = true;
                    return;
                }
            }
        }

        base.TrySelectBase();
    }

    private void SetLastClickedObject(GameObject gameObject)
    {
        if (Input.GetMouseButtonDown(0))
        {
            _lastClickedObject = gameObject;
        }
    }
}

using TMPro;
using UnityEngine;

public class GoldViewer : MonoBehaviour
{
    [SerializeField] private TMP_Text _score;
    [SerializeField] private Base _mainBase;

    private void OnEnable()
    {
        _mainBase.GoldChanged += OnGoldChanged;
    }

    private void OnDisable()
    {
        _mainBase.GoldChanged -= OnGoldChanged;
    }

    public void OnGoldChanged(int value)
    {
        _score.text = value.ToString("F0");
    }
}

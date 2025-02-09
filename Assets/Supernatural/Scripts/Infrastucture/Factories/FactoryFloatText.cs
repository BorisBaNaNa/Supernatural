using UnityEngine;

public class FactoryFloatText : IService
{
    private readonly FloatingText _floatingText;
    private Transform _parent;

    public FactoryFloatText(FloatingText floatingText)
    {
        _floatingText = floatingText;
    }

    public FloatingText BuildFloatingText(Vector3 at, string text, Color color)
    {
        var _position = Camera.main.WorldToScreenPoint(at);
        FloatingText floatingText = Object.Instantiate(_floatingText);

        floatingText.transform.SetParent(_parent, false);
        floatingText.transform.position = _position;
        floatingText.SetText(text, color);
        return floatingText;
    }

    public void SetTextParent(Transform parent) => _parent = parent;
}

using System;
using _CodeBase.Extensions;
using TMPro;
using UnityEngine;

namespace _CodeBase.List
{
  public class ListItem : MonoBehaviour
  {
    public event Action<ListItem> BecameInvisible;

    public int Number { get; private set; }
    public bool IsVisible => _textRectTransform.WorldSpaceOverlaps(_parent);
    public float Width => _rectTransform.rect.width;

    [SerializeField] private TextMeshProUGUI _textField;

    private RectTransform _textRectTransform;
    private RectTransform _rectTransform;
    private RectTransform _parent;
    private bool _isVisible;

    private void Awake()
    {
      _rectTransform = GetComponent<RectTransform>();
      _textRectTransform = _textField.GetComponent<RectTransform>();
    }

    private void Update()
    {
      if (IsVisible == false) 
        BecameInvisible?.Invoke(this);
    }

    public void Initialize(RectTransform parent) => _parent = parent;

    public void SetNumber(int number)
    {
      Number = number;
      _textField.text = number.ToString();
    }
  }
}
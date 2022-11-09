using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace _CodeBase.List
{
  public class ListGenerator : MonoBehaviour
  {
    [SerializeField] private int _maxItemsCount;
    [SerializeField] private int _spawnedItemsCount;
    [Space(10)]
    [SerializeField] private Transform _parent;
    [SerializeField] private RectTransform _rectParent;
    [SerializeField] private ScrollRect _scroll;
    [SerializeField] private Scrollbar _scrollbar;
    [SerializeField] private RectTransform _content;
    [Space(10)]
    [SerializeField] private ListItem _horizontalItemPrefab;
    [SerializeField] private ListItem _verticalItemPrefab;

    private readonly List<ListItem> _items = new List<ListItem>();
    private ListItem _currentVisibleItem;
    private int _scrollbarMoveDirection;
    private float? _lastScrollbarValue;

    private void Start() => Initialize();

    private void Update() => UpdateScrollbarMoveDirection();

    private void OnApplicationFocus(bool hasFocus)
    {
      if(hasFocus == false)
        PlayerPrefs.SetInt(Keys.CurrentListItemNumber, _currentVisibleItem.Number);
    }

    private void Initialize()
    {
      int savedListItemNumber = PlayerPrefs.GetInt(Keys.CurrentListItemNumber, 1);
      ListItem item;

      if (PlayerPrefs.HasKey(Keys.CurrentListItemNumber) && savedListItemNumber > _spawnedItemsCount)
      {
        for (int i = 0; i < _spawnedItemsCount; i++)
        {
          int spawnItemNumber = Mathf.RoundToInt(savedListItemNumber - _spawnedItemsCount / 2) + i;
          SpawnItem(spawnItemNumber);
        }

        _scrollbar.value = 0.5f;
        item = _items[Mathf.CeilToInt((float)_spawnedItemsCount / 2)];
      }
      else
      {
        for (int i = 0; i < _spawnedItemsCount; i++)
          SpawnItem(i + 1);

        item = _items[1];
      }

      _currentVisibleItem = item;
      _currentVisibleItem.BecameInvisible += OnCurrentVisibleItemBecomeInvisible;
      item.BecameInvisible += OnFirstVisibleItemBecomeInvisible;
    }

    private void UpdateScrollbarMoveDirection()
    {
      float handledScrollbarValue = Mathf.Clamp(_scrollbar.value, 0, 1);
      
      if (_lastScrollbarValue == null || _lastScrollbarValue.Value == handledScrollbarValue)
        _scrollbarMoveDirection = 0;
      else if (_lastScrollbarValue > handledScrollbarValue)
        _scrollbarMoveDirection = -1;
      else if (_lastScrollbarValue < handledScrollbarValue)
        _scrollbarMoveDirection = 1;

      _lastScrollbarValue = handledScrollbarValue;
    }

    private void OnCurrentVisibleItemBecomeInvisible(ListItem listItem)
    {
      if(_scrollbarMoveDirection == 0) return;

      listItem.BecameInvisible -= OnCurrentVisibleItemBecomeInvisible;

      int newCurrentItemIndex =
        _scrollbarMoveDirection == 1 ? _items.IndexOf(listItem) + 1 : _items.IndexOf(listItem) - 1;
      newCurrentItemIndex = Mathf.Clamp(newCurrentItemIndex, 0, _items.Count - 1);

      ListItem newCurrentItem = _items[newCurrentItemIndex];
      ListItem previousItem = GetPreviousItem(listItem);

      _currentVisibleItem = newCurrentItem;
      _currentVisibleItem.BecameInvisible += OnCurrentVisibleItemBecomeInvisible;
      
      ListItem firstItem = _items.First();

      if (firstItem.Number > 1 && _scrollbarMoveDirection == -1 && (previousItem == null || previousItem.IsVisible))
        SpawnItem(firstItem.Number - 1, true);
    }
    
    private void OnFirstVisibleItemBecomeInvisible(ListItem listItem)
    {
      if (_items.Count >= _maxItemsCount)
      {
        listItem.BecameInvisible -= OnFirstVisibleItemBecomeInvisible;
        return;
      }

      ListItem nextItem = GetNextItem(listItem);
      if (nextItem.IsVisible == false) return;

      listItem.BecameInvisible -= OnFirstVisibleItemBecomeInvisible;
      UpdateFirstVisibleItem(listItem);
      SpawnItem(_items.Last().Number + 1);
    }

    private void UpdateFirstVisibleItem(ListItem listItem)
    {
      ListItem newFirstItem = GetNextItem(listItem);
      newFirstItem.BecameInvisible += OnFirstVisibleItemBecomeInvisible;
    }
    
    private void SpawnItem(int number, bool atBeginning = false)
    {
      ListItem prefab = number % 2 == 0 ? _verticalItemPrefab : _horizontalItemPrefab;
      ListItem item = Instantiate(prefab, _parent);
      item.Initialize(_rectParent);
      item.SetNumber(number);

      if (atBeginning)
      {
        _scrollbar.enabled = false;
        item.transform.SetSiblingIndex(0);
        _items.Insert(0, item);
        _content.anchoredPosition -= new Vector2(item.Width + _scroll.horizontalScrollbarSpacing, 0);
        _scrollbar.enabled = true;
      }
      else
        _items.Add(item);
    }

    private ListItem GetNextItem(ListItem listItem)
    {
      int index = _items.IndexOf(listItem) + 1;
      return index < _items.Count ? _items[index] : null;
    }
    
    private ListItem GetPreviousItem(ListItem listItem)
    {
      int index = _items.IndexOf(listItem) - 1;
      return index >= 0 ? _items[index] : null;
    }
  }
}

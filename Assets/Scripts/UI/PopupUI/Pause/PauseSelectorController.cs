using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

public class PauseSelectorController : MonoBehaviour
{
    [SerializeField] RectTransform _selector;
    [SerializeField] RectTransform _selectorParent;
    [SerializeField] float _selectorMoveDuration = 0.3f;

    List<RectTransform> _selectorList = new List<RectTransform>();

    int _currentSelectorIndex = -1;
    
    private Coroutine _moveSelectorCoroutine;
    
    [SerializeField] Vector2 _originSelectorAnchorPosition = new Vector2(-400, 340);
    [SerializeField] Vector2 _firstItemSelectorAnchorPosition = new Vector2(0, 340);



    void Start()
    {
        Initialize();
    }

    void Initialize()
    {
        foreach (var selector in _selectorList)
        {
            DestroyImmediate(selector);
        }
        _selectorList.Clear();

        foreach (Transform child in _selectorParent)
        {
            _selectorList.Add(child.GetComponent<RectTransform>());
        }

        _currentSelectorIndex = -1;
        _selector.anchoredPosition = _originSelectorAnchorPosition;
    }

    void Update()
    {
        if(GetSelectedIndex() != _currentSelectorIndex && GetSelectedIndex() != -1)
        {
            _currentSelectorIndex = GetSelectedIndex();
            //_selector.anchoredPosition = _selectorList[_currentSelectorIndex].anchoredPosition;
            if(_moveSelectorCoroutine != null)
            {
                StopCoroutine(_moveSelectorCoroutine);
                _moveSelectorCoroutine = null;
            }
            _moveSelectorCoroutine = StartCoroutine(MoveSelector(_currentSelectorIndex));
        }
    }

    IEnumerator MoveSelector(int index){
        Vector2 targetPos;
        if(index < 0){
            targetPos = _originSelectorAnchorPosition;
        }
        else{
            targetPos = _firstItemSelectorAnchorPosition + new Vector2(0, _selectorList[index].anchoredPosition.y);
        }

        Vector2 startPos = _selector.anchoredPosition;

        float elapsedTime = 0;
        while(elapsedTime < _selectorMoveDuration){
            elapsedTime += Time.unscaledDeltaTime;
            _selector.anchoredPosition = Vector2.Lerp(startPos, targetPos, elapsedTime / _selectorMoveDuration);
            yield return null;
        }
        _selector.anchoredPosition = targetPos;
    }

    int GetSelectedIndex()
    {
        int ret = -1;
        if(EventSystem.current.currentSelectedGameObject == null)
        {
            return ret;
        }
        EventSystem.current.currentSelectedGameObject.TryGetComponent(out RectTransform selectedRectTransform);

        for (int i = 0; i < _selectorList.Count; i++)
        {
            if (_selectorList[i] == selectedRectTransform)
            {
                ret = i;
                break;
            }
        }

        return ret;
    }
}

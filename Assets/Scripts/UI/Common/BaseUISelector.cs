using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

public class BaseUISelector : MonoBehaviour
{
    /// <summary>
    /// Image형태로 초기 위치를 수동으로 잡아주고 할당
    /// </summary>
    [SerializeField] RectTransform _selector;

    /// <summary>
    /// VerticalLayoutGroup이나 HorizontalLayoutGroup을 사용시 해당 부모를 선택 하위의 아이템들을 찾아서 위치를 찾기 위함
    /// </summary>
    [SerializeField] RectTransform _selectorParent;
    [SerializeField] float _selectorMoveDuration = 0.15f;

    List<RectTransform> _selectorList = new List<RectTransform>();

    int _currentSelectorIndex = -1;
    
    Coroutine _moveSelectorCoroutine;
    
    //수동으로 맞춘 Selector초기 위치값을 초기화때 받아와서 사용
    Vector2 _originSelectorAnchorPosition;
    [SerializeField] bool _usingVerticalLayoutGroup = true;
    [SerializeField] bool _usingHorizontalLayoutGroup = true;
    
    void Start()
    {
        Initialize();
    }

    void Initialize()
    {
        _selectorList.Clear();

        foreach (Transform child in _selectorParent)
        {
            _selectorList.Add(child.GetComponent<RectTransform>());
        }

        _currentSelectorIndex = -1;
        _selector.gameObject.SetActive(false);
        _originSelectorAnchorPosition = _selector.anchoredPosition;
    }

    void Update()
    {
        if(GetSelectedIndex() != _currentSelectorIndex && GetSelectedIndex() != -1)
        {   
            _currentSelectorIndex = GetSelectedIndex();

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
            _selector.gameObject.SetActive(false);
            yield break;
        }
        else{
            targetPos = _originSelectorAnchorPosition + new Vector2(_usingHorizontalLayoutGroup ? _selectorList[index].anchoredPosition.x : 0, _usingVerticalLayoutGroup ? _selectorList[index].anchoredPosition.y : 0)
             - new Vector2(_usingHorizontalLayoutGroup ? _selectorList[0].anchoredPosition.x : 0, _usingVerticalLayoutGroup ? _selectorList[0].anchoredPosition.y : 0);
            
            if(!_selector.gameObject.activeSelf)
            {
                _selector.gameObject.SetActive(true);
                _selector.anchoredPosition = targetPos;
                yield break;
            }
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

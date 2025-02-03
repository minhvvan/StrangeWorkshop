using System;
using System.Collections.Generic;
using System.Linq;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BarrierUIController : MonoBehaviour, IGameUI
{
    private BarrierController _barrierController;
    private RectTransform _barrierFlatPrefab;
    private RectTransform _barrierCornerPrefab;
    private RectTransform _root;

    [SerializeField] private RectTransform _minimapRect;
    private readonly Dictionary<Barrier, BarrierUI> _barrierUIs = new();
    private MapBounds _mapBounds;

    [Header("UI Elements")]
    [SerializeField] private Slider _totalHealthBar;
    [SerializeField] private TMP_Text _currentHealthText;
    [SerializeField] private TMP_Text _maxHealthText;
    
    [Header("UI Settings")]
    [SerializeField] private int _gridWidth;            // 가로 그리드 수
    [SerializeField] private int _gridHeight;           // 세로 그리드 수
    [SerializeField] private float _borderSpacing;      // 테두리 간격
    [SerializeField] private float _cornerOverlap;      // 코너 겹침 정도
    [SerializeField] private float _barrierThickness;   // 두께

    private void Awake()
    {
        _barrierFlatPrefab = DataManager.Instance.LoadUIPrefab(Addresses.Prefabs.UI.BARRIER_FLAT);
        _barrierCornerPrefab = DataManager.Instance.LoadUIPrefab(Addresses.Prefabs.UI.BARRIER_CORNER);

        _root = transform.GetComponent<RectTransform>();
    }

    public void SetBarrierController(BarrierController newBarrierController)
    {
        if (newBarrierController == null) return;
        CleanUp();
        
        _barrierController = newBarrierController;
        _barrierController.OnBarrierDamagedAction += UpdateBarrierHealth;
        
        Initialize();
        ShowUI();
    }

    public void ShowUI()
    {
        UIAnimationUtility.SlideInRight(_root);   
    }

    public void HideUI()
    {
        UIAnimationUtility.SlideOutRight(_root);
    }

    public void CleanUp()
    {
        //Controller 초기화
        _barrierController = null;
        
        //BarrierUI 삭제
        foreach (var barrierUI in _barrierUIs.Values)
        {
            Destroy(barrierUI.gameObject);
        }
        
        _barrierUIs.Clear();
    }

    public void Initialize()
    {
        if (!_barrierController) return;
        
        _gridWidth = _barrierController.Barriers.Max(barrier => barrier.GridPos.x + 1);
        _gridHeight = _barrierController.Barriers.Max(barrier => barrier.GridPos.y + 1);
        
        _maxHealthText.text = $"{_barrierController.MaxHealth}";
        UpdateBarrierTotalHealth();
        CalculateMapBounds();

        foreach (var barrier in _barrierController.Barriers)
        {
            CreateBarrierUI(barrier);
            UpdateBarrierHealth(barrier);
        }
    }

    private void CreateBarrierUI(Barrier barrier)
    {
        var prefab = barrier.BarrierType >= BarrierType.CornerTopLeft ? _barrierCornerPrefab : _barrierFlatPrefab;
        var uiInstance = Instantiate(prefab, _minimapRect);
        
        Vector2 normalizedPos = GridToUIPosition(barrier.GridPos);
        uiInstance.anchorMin = uiInstance.anchorMax = normalizedPos;
        uiInstance.anchoredPosition = Vector2.zero;
        UpdateBarrierSize(barrier, uiInstance);

        var rotation = GetCornerRotation(barrier.BarrierType);
        uiInstance.localRotation = Quaternion.Euler(0, 0, rotation);
        uiInstance.localPosition += GetCornerAdjustPosition(barrier.BarrierType);

        _barrierUIs[barrier] = uiInstance.GetComponent<BarrierUI>();
    }
    
    private void UpdateBarrierTotalHealth()
    {
        // 총 체력 UI 업데이트
        _currentHealthText.text = $"{_barrierController.TotalHeath}";
        _totalHealthBar.value = _barrierController.TotalHeath / _barrierController.MaxHealth;
    }

    private void UpdateBarrierHealth(Barrier barrier)
    {
        //전달받은 barrierUI 업데이트
        if (_barrierUIs.TryGetValue(barrier, out var barrierUI))
        {
            float percentage = barrier.BarrierStat.health / barrier.BarrierStat.maxHealth;
            barrierUI.UpdateHealthColor(percentage);
        }
    }
    
    private Vector2 GridToUIPosition(GridPosition gridPos)
    {
        // 전체 영역에 대한 비율로 여백 계산
        float paddingRatio = _borderSpacing; // 전체 크기의 10%를 여백으로 설정
        float padding = Mathf.Min(_minimapRect.rect.width, _minimapRect.rect.height) * paddingRatio;
    
        // 실제 사용 가능 영역 계산
        float usableWidth = _minimapRect.rect.width - (2 * padding);
        float usableHeight = _minimapRect.rect.height - (2 * padding);

        // 그리드 셀 크기 계산
        float cellWidth = usableWidth / (_gridWidth - 1);
        float cellHeight = usableHeight / (_gridHeight - 1);

        return new Vector2(
            (padding + gridPos.x * cellWidth) / _minimapRect.rect.width,
            (padding + gridPos.y * cellHeight) / _minimapRect.rect.height
        );
    }
    
    private void UpdateBarrierSize(Barrier barrier, RectTransform uiInstance)
    {
        float paddingRatio = _borderSpacing;
        float padding = Mathf.Min(_minimapRect.rect.width, _minimapRect.rect.height) * paddingRatio;
   
        float usableWidth = _minimapRect.rect.width - (2 * padding);
        float usableHeight = _minimapRect.rect.height - (2 * padding);
   
        float cellSize = Mathf.Min(usableWidth, usableHeight) / (_gridWidth - 1);

        uiInstance.sizeDelta = barrier.BarrierType switch
        {
            BarrierType.Horizontal => new Vector2(cellSize, _barrierThickness),
            BarrierType.Vertical => new Vector2(_barrierThickness, cellSize),
            _ => uiInstance.sizeDelta
        };
    }

    private void CalculateMapBounds() 
    {
        _mapBounds = new MapBounds();
        var positions = _barrierController.Barriers.Select(b => b.transform.position).ToList();
        
        _mapBounds.minX = positions.Min(p => p.x);
        _mapBounds.maxX = positions.Max(p => p.x);
        _mapBounds.minZ = positions.Min(p => p.z);
        _mapBounds.maxZ = positions.Max(p => p.z);

        // 여유 공간 추가
        float paddingX = (_mapBounds.maxX - _mapBounds.minX) * 0.1f;
        float paddingZ = (_mapBounds.maxZ - _mapBounds.minZ) * 0.1f;

        _mapBounds.minX -= paddingX;
        _mapBounds.maxX += paddingX;
        _mapBounds.minZ -= paddingZ;
        _mapBounds.maxZ += paddingZ;
    }
    
    private float GetCornerRotation(BarrierType type)
    {
        return type switch
        {
            BarrierType.CornerTopLeft => -90f,
            BarrierType.CornerTopRight => 180f,
            BarrierType.CornerBottomRight => 90f,
            BarrierType.TurnLeft => 90f,
            BarrierType.TurnRight => -90f,
            _ => 0f
        };
    }
    
    private Vector3 GetCornerAdjustPosition(BarrierType type)
    {
        return type switch
        {
            BarrierType.CornerTopLeft => new Vector3(_cornerOverlap, -_cornerOverlap, 0f),
            BarrierType.CornerBottomLeft => new Vector3(_cornerOverlap, _cornerOverlap, 0f),
            BarrierType.CornerTopRight => new Vector3(-_cornerOverlap, -_cornerOverlap, 0f),
            BarrierType.CornerBottomRight => new Vector3(-_cornerOverlap, _cornerOverlap, 0f),
            BarrierType.TurnLeft => new Vector3(-_cornerOverlap, _cornerOverlap, 0f),
            BarrierType.TurnRight => new Vector3(_cornerOverlap, -_cornerOverlap, 0f),
            _ => Vector3.zero
        };
    }
}

public class MapBounds
{
    public float minX;
    public float maxX;
    public float minZ;
    public float maxZ;
}
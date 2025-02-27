using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using UnityEngine;
using UnityEngine.UI;

public class RecipePartController : MonoBehaviour
{
    [SerializeField] private SerializedDictionary<string, Image> objectImages;
    [SerializeField] private CraftRecipeSO _recipeSO;

    private Dictionary<string, int> _objectRequirements;
    private Dictionary<string, int> _currentObjectNum;
    
    private Color _originalColor;
    private float _originalAlpha;
    private Color _changedColor;

    void Awake()
    {
        InitObjectNum();
        
        _originalColor = objectImages.Values.FirstOrDefault().color;
        _originalAlpha = _originalColor.a;
        _changedColor = Color.green;
        _changedColor.a = _originalAlpha;
    }

    private void InitObjectNum()
    {
        _objectRequirements = new Dictionary<string, int>();
        _currentObjectNum = new Dictionary<string, int>();
        foreach (HoldableObjectSO objectSO in _recipeSO.inputs)
        {
            if (_objectRequirements.TryGetValue(objectSO.objectName, out _))
            {
                _objectRequirements[objectSO.objectName] += 1;
            }
            else
            {
                _objectRequirements[objectSO.objectName] = 1;
            }
            _currentObjectNum[objectSO.objectName] = 0;
        }
    }
    
    public void ActivateColor(List<string> objectNames)
    {
        foreach (string objectName in objectNames)
        {
            _currentObjectNum[objectName] = 0;
        }
        foreach (var objectName in objectNames)
        {
            if (objectImages.TryGetValue(objectName, out Image image))
            {
                _currentObjectNum[objectName] += 1;
                if (_currentObjectNum[objectName] >= _objectRequirements[objectName])
                {
                    image.color = _changedColor;
                }
            }
        }
    }

    public void DeactivateColor()
    {
        foreach (Image image in objectImages.Values)
        {
            image.color = _originalColor;
        }
    }
}

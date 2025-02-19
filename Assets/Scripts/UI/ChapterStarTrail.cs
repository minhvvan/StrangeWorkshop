using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Splines;
using Random = UnityEngine.Random;

public class ChapterStarTrail : MonoBehaviour
{
    [SerializeField] private List<Color> _starColor;
    [SerializeField] private Material _glowMaterial;
    [SerializeField] private Material _lockedMaterial;

    private SplineContainer _spline;
    private SpriteRenderer _spriteRenderer;
    
    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        
        _spline = FindObjectOfType<SplineContainer>();
        SetColor();
        
        float rand = Random.Range(5f, 13f);
        transform.localScale = new Vector3(rand, rand, rand);
    }

    private void SetColor()
    {
        if (transform.position.x >= _spline.Spline[SaveData.CurrentChapter].Position.x)
        {
            _spriteRenderer.color = _starColor[1];
            _spriteRenderer.material = _lockedMaterial;
        }
        else
        {
            _spriteRenderer.color = _starColor[0];
            _spriteRenderer.material = _glowMaterial;
        }
    }
}

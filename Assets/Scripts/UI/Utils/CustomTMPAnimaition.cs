using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class CustomTMPAnimaition : MonoBehaviour
{
    TextMeshProUGUI textMeshPro;
    
    [SerializeField] float _firstDelay = 0.5f;
    [SerializeField] float _enterDuration = 0.5f;
    [SerializeField] float _jumpHeight = 50f;
    [SerializeField] float _jumpDuration = 0.1f;
    [SerializeField] float _waveSpeed = 2f;
    [SerializeField] float _waveHeight = 5f;
    [SerializeField] float _fadeDuration = 0.5f;

    //InitializeText 함수에서 사용할 변수
    [SerializeField] bool _startWithHide = false;
    [SerializeField] bool _startWithFadeIn = false;
    [SerializeField] bool _startWithFadeOut = false;
    [SerializeField] bool _startWithJump = false;
    [SerializeField] bool _startWithWave = false;
    

    public bool isAnimating = true;

    TMP_TextInfo _textInfo;
    Vector3[][] _originalVertices;

    async void Start()
    {
        textMeshPro = GetComponent<TextMeshProUGUI>();

        InitializeText();

        await Task.Delay((int)(_firstDelay * 1000));
        await PlayAnimation();
    }

    void OnDisable()
    {
        isAnimating = false;
    }

    void InitializeText()
    {
        textMeshPro.ForceMeshUpdate();
        _textInfo = textMeshPro.textInfo;

        _originalVertices = new Vector3[_textInfo.meshInfo.Length][];
        for (int i = 0; i < _textInfo.meshInfo.Length; i++)
        {
            _originalVertices[i] = new Vector3[_textInfo.meshInfo[i].vertices.Length];
            _textInfo.meshInfo[i].vertices.CopyTo(_originalVertices[i], 0);
        }
        
        if(_startWithHide){
            HideTextUsingAlpha();
        }
    }

    async Task PlayAnimation(){
        if(_startWithHide){
            //ShowTextUsingAlpha();
            await AnimateTextFadeIn();
        }

        //await AnimateTextEntry();
        //await AnimateWaveEffect();

        await AnimateTextJump();
    }

    void HideTextUsingAlpha(){
        for (int i = 0; i < _textInfo.characterCount; i++)
        {
            if (!_textInfo.characterInfo[i].isVisible)
                continue;

            int vertexIndex = _textInfo.characterInfo[i].vertexIndex;
            int materialIndex = _textInfo.characterInfo[i].materialReferenceIndex;
            
            for (int j = 0; j < 4; j++)
            {
                Color32 c = _textInfo.meshInfo[materialIndex].colors32[vertexIndex + j];
                c.a = 0;
                _textInfo.meshInfo[materialIndex].colors32[vertexIndex + j] = c;
            }
        }

        for (int i = 0; i < _textInfo.meshInfo.Length; i++)
        {
            textMeshPro.mesh.vertices = _textInfo.meshInfo[i].vertices;
            textMeshPro.mesh.colors32 = _textInfo.meshInfo[i].colors32;
            textMeshPro.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices);
        }
    }

    void ShowTextUsingAlpha(){
        for (int i = 0; i < _textInfo.characterCount; i++)
        {
            if (!_textInfo.characterInfo[i].isVisible)
                continue;

            int vertexIndex = _textInfo.characterInfo[i].vertexIndex;
            int materialIndex = _textInfo.characterInfo[i].materialReferenceIndex;
            
            for (int j = 0; j < 4; j++)
            {
                Color32 c = _textInfo.meshInfo[materialIndex].colors32[vertexIndex + j];
                c.a = 255;
                _textInfo.meshInfo[materialIndex].colors32[vertexIndex + j] = c;
            }
        }

        for (int i = 0; i < _textInfo.meshInfo.Length; i++)
        {
            textMeshPro.mesh.vertices = _textInfo.meshInfo[i].vertices;
            textMeshPro.mesh.colors32 = _textInfo.meshInfo[i].colors32;
            textMeshPro.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices);
        }
    }

    async Task AnimateTextEntry()
    {
        List<Task> tasks = new List<Task>();
        
        for (int i = 0; i < _textInfo.characterCount; i++)
        {
            if (!_textInfo.characterInfo[i].isVisible)
                continue;

            int vertexIndex = _textInfo.characterInfo[i].vertexIndex;
            int materialIndex = _textInfo.characterInfo[i].materialReferenceIndex;
            Vector3[] vertices = _textInfo.meshInfo[materialIndex].vertices;

            //Get Screen Height
            float screenHeight = Screen.height;

            Vector3 offset = new Vector3(0, -screenHeight, 0); // 왼쪽 화면 밖
            for (int j = 0; j < 4; j++)
            {
                vertices[vertexIndex + j] += offset;
            }

            _ = MoveCharacterToPosition(i, 0);
        }
        
        for (int i = 0; i < _textInfo.characterCount; i++)
        {
            if (!_textInfo.characterInfo[i].isVisible)
                continue;

            int vertexIndex = _textInfo.characterInfo[i].vertexIndex;
            int materialIndex = _textInfo.characterInfo[i].materialReferenceIndex;
            Vector3[] vertices = _textInfo.meshInfo[materialIndex].vertices;

            float screenHeight = Screen.height;

            Vector3 offset = new Vector3(0, -screenHeight, 0);
            for (int j = 0; j < 4; j++)
            {
                vertices[vertexIndex + j] += offset;
            }

            tasks.Add(MoveCharacterToPosition(i, _enterDuration));

            await Task.Delay(300);
        }
        
        await Task.WhenAll(tasks);
    }
    
    async Task MoveCharacterToPosition(int charIndex, float duration)
    {
        int vertexIndex = _textInfo.characterInfo[charIndex].vertexIndex;
        int materialIndex = _textInfo.characterInfo[charIndex].materialReferenceIndex;
        Vector3[] vertices = _textInfo.meshInfo[materialIndex].vertices;

        float elapsedTime = 0;
        Vector3[] startVertices = new Vector3[4];
        Vector3[] targetVertices = new Vector3[4];

        for (int j = 0; j < 4; j++)
        {
            startVertices[j] = vertices[vertexIndex + j];
            targetVertices[j] = _originalVertices[materialIndex][vertexIndex + j];
        }

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);

            for (int j = 0; j < 4; j++)
            {
                vertices[vertexIndex + j] = Vector3.Lerp(startVertices[j], targetVertices[j], t);
            }
            if (textMeshPro != null)
            {
                textMeshPro.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices);
            }

            await Task.Yield();
        }
    }

    private async Task AnimateWaveEffect()
    {
        while (isAnimating)
        {
            textMeshPro.ForceMeshUpdate();
            _textInfo = textMeshPro.textInfo;

            for (int i = 0; i < _textInfo.characterCount; i++)
            {
                if (!_textInfo.characterInfo[i].isVisible)
                    continue;

                int vertexIndex = _textInfo.characterInfo[i].vertexIndex;
                int materialIndex = _textInfo.characterInfo[i].materialReferenceIndex;
                Vector3[] vertices = _textInfo.meshInfo[materialIndex].vertices;

                float waveOffset = Mathf.Sin(Time.time * _waveSpeed + i) * _waveHeight;
                for (int j = 0; j < 4; j++)
                {
                    vertices[vertexIndex + j] = _originalVertices[materialIndex][vertexIndex + j] + new Vector3(0, waveOffset, 0);
                }
            }

            for (int i = 0; i < _textInfo.meshInfo.Length; i++)
            {
                textMeshPro.mesh.vertices = _textInfo.meshInfo[i].vertices;
                textMeshPro.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices);
            }

            await Task.Yield();
        }
    }

    async Task AnimateTextJump()
    {
        List<Task> tasks = new List<Task>();

        for (int i = 0; i < _textInfo.characterCount; i++)
        {
            if (!_textInfo.characterInfo[i].isVisible)
                continue;

            int vertexIndex = _textInfo.characterInfo[i].vertexIndex;
            int materialIndex = _textInfo.characterInfo[i].materialReferenceIndex;
            Vector3[] vertices = _textInfo.meshInfo[materialIndex].vertices;

            //Get Screen Height
            float screenHeight = Screen.height;

            Vector3 offset = new Vector3(0, -screenHeight, 0);
            for (int j = 0; j < 4; j++)
            {
                vertices[vertexIndex + j] += offset;
            }

            _ = MoveCharacterToPosition(i, 0);
        }


        for (int i = 0; i < _textInfo.characterCount; i++)
        {
            if (!_textInfo.characterInfo[i].isVisible)
                continue;

            tasks.Add(MoveCharacterJump(i, _enterDuration, _jumpHeight));

            await Task.Delay((int)(_jumpDuration * 1000));
        }

        await Task.WhenAll(tasks);
    }

    async Task MoveCharacterJump(int charIndex, float duration, float jumpHeight)
    {
        int vertexIndex = _textInfo.characterInfo[charIndex].vertexIndex;
        int materialIndex = _textInfo.characterInfo[charIndex].materialReferenceIndex;
        Vector3[] vertices = _textInfo.meshInfo[materialIndex].vertices;

        float elapsedTime = 0;
        Vector3[] startVertices = new Vector3[4];
        Vector3[] targetVertices = new Vector3[4];
   
        for (int j = 0; j < 4; j++)
        {
            startVertices[j] = vertices[vertexIndex + j];
            targetVertices[j] = _originalVertices[materialIndex][vertexIndex + j];
        }

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);

            float jumpOffset = Mathf.Sin(t * Mathf.PI) * jumpHeight;


            for (int j = 0; j < 4; j++)
            {
                vertices[vertexIndex + j] = Vector3.Lerp(startVertices[j], targetVertices[j] + new Vector3(0, jumpOffset, 0), t);
            }
            
            if (textMeshPro != null)
            {
                textMeshPro.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices);    
            }
            
            await Task.Yield();
        }

        if(jumpHeight > 5){
            await MoveCharacterJump(charIndex, duration, jumpHeight / 5);
        }
    }

    async Task AnimateTextFadeIn()
    {
        float elapsedTime = 0;
        while (elapsedTime < _fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / _fadeDuration);

            for (int i = 0; i < _textInfo.characterCount; i++)
            {
                if (!_textInfo.characterInfo[i].isVisible)
                    continue;

                int vertexIndex = _textInfo.characterInfo[i].vertexIndex;
                int materialIndex = _textInfo.characterInfo[i].materialReferenceIndex;

                for (int j = 0; j < 4; j++)
                {
                    Color32 c = _textInfo.meshInfo[materialIndex].colors32[vertexIndex + j];
                    c.a = (byte)(t * 255);
                    _textInfo.meshInfo[materialIndex].colors32[vertexIndex + j] = c;
                }
            }

            for (int i = 0; i < _textInfo.meshInfo.Length; i++)
            {
                textMeshPro.mesh.vertices = _textInfo.meshInfo[i].vertices;
                textMeshPro.mesh.colors32 = _textInfo.meshInfo[i].colors32;
            }

            await Task.Yield();
        }
    }

    async Task AnimateTextFadeOut()
    {
        float elapsedTime = 0;
        while (elapsedTime < _fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / _fadeDuration);

            for (int i = 0; i < _textInfo.characterCount; i++)
            {
                if (!_textInfo.characterInfo[i].isVisible)
                    continue;

                int vertexIndex = _textInfo.characterInfo[i].vertexIndex;
                int materialIndex = _textInfo.characterInfo[i].materialReferenceIndex;

                for (int j = 0; j < 4; j++)
                {
                    Color32 c = _textInfo.meshInfo[materialIndex].colors32[vertexIndex + j];
                    c.a = (byte)((1 - t) * 255);
                    _textInfo.meshInfo[materialIndex].colors32[vertexIndex + j] = c;
                }
            }

            for (int i = 0; i < _textInfo.meshInfo.Length; i++)
            {
                textMeshPro.mesh.vertices = _textInfo.meshInfo[i].vertices;
                textMeshPro.mesh.colors32 = _textInfo.meshInfo[i].colors32;
            }

            await Task.Yield();
        }
    }
}
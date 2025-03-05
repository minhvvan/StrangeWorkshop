using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class CustomTMPAnimaition : MonoBehaviour
{
    public enum TMPAnimationType
    {
        None,
        Entry,
        Wave,
        EntryAndJump,
        FadeIn,
        FadeOut
    }

    TextMeshProUGUI textMeshPro;
    
    //[SerializeField] float _enterDuration = 0.5f;
    //[SerializeField] float _jumpHeight = 600;
    //[SerializeField] float _jumpDuration = 0.1f;
    //[SerializeField] float _waveSpeed = 2f;
    //[SerializeField] float _waveHeight = 5f;
    //[SerializeField] float _fadeDuration = 0.1f;

    //InitializeText 함수에서 사용할 변수
    [SerializeField] bool _startWithHide = false; //처음에 숨길지 여부
    

    public bool isAnimating = true;

    TMP_TextInfo _textInfo;
    Vector3[][] _originalVertices;

    void Start()
    {
        textMeshPro = GetComponent<TextMeshProUGUI>();

        InitializeText();
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

        if (_startWithHide)
        {
            HideTextUsingAlpha();
        }
    }

    public async void PlayAnimation(TMPAnimationType type, Action callback = null, params object[] args){
        switch(type){
            case TMPAnimationType.Entry:
                if (_startWithHide) ShowTextUsingAlpha();
                if(args.Length == 1){
                    await AnimateTextEntry((float)args[0]);
                }else{
                    await AnimateTextEntry();
                }
                break;
            case TMPAnimationType.Wave:
                if (_startWithHide) ShowTextUsingAlpha();
                if(args.Length == 2){
                    await AnimateWaveEffect((float)args[0], (float)args[1]);
                }else{
                    await AnimateWaveEffect();
                }
                
                break;
            case TMPAnimationType.EntryAndJump:
                if (_startWithHide) ShowTextUsingAlpha();
                if(args.Length == 3){
                    await AnimateTextEntryAndJump((float)args[0], (float)args[1], (float)args[2]);
                }else{
                    await AnimateTextEntryAndJump();
                }
                break;
            case TMPAnimationType.FadeIn:
                if(args.Length == 1){
                    await AnimateTextFadeIn((float)args[0]);
                }else{
                    await AnimateTextFadeIn();
                }
                
                break;
            case TMPAnimationType.FadeOut:
                if(args.Length == 1){
                    await AnimateTextFadeOut((float)args[0]);
                }
                else{
                    await AnimateTextFadeOut();
                }
                break;
        }

        callback?.Invoke();
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

    async Task AnimateTextEntry(float enterDuration = 0.5f)
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

            tasks.Add(MoveCharacterToPosition(i, enterDuration));

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
            elapsedTime += Time.unscaledDeltaTime;
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

    private async Task AnimateWaveEffect(float waveSpeed = 2f, float waveHeight = 5f)
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

                float waveOffset = Mathf.Sin(Time.time * waveSpeed + i) * waveHeight;
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

    async Task AnimateTextEntryAndJump(float enterDuration = 0.5f, float jumpDuration = 0.1f, float jumpHeight = 600)
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

            tasks.Add(MoveCharacterJump(i, enterDuration, jumpHeight));

            await Task.Delay((int)(jumpDuration * 1000));
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
            elapsedTime += Time.unscaledDeltaTime;
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
            await MoveCharacterJump(charIndex, duration / 2, jumpHeight / 5);
        }
    }

    async Task AnimateTextFadeIn(float fadeDuration = 0.2f)
    {
        float elapsedTime = 0;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsedTime / fadeDuration);

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

            if (textMeshPro != null)
            {
                textMeshPro.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices);    
            }

            await Task.Yield();
        }
    }

    async Task AnimateTextFadeOut(float fadeDuration = 0.2f)
    {
        float elapsedTime = 0;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsedTime / fadeDuration);

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

            if (textMeshPro != null)
            {
                textMeshPro.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices);    
            }

            await Task.Yield();
        }
    }
}
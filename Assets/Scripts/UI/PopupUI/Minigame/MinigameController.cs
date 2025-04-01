using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class MinigameController : MonoBehaviour
{
    [SerializeField] RectTransform gaugeBackground;
    [SerializeField] RectTransform gaugeFill;
    [SerializeField] RectTransform gaugeHandle;

    [SerializeField] float gaugeFillSpeed = 1f;
    [SerializeField] float gaugeFillMax = 1f;    

    private float gaugeValue = 0f;

    public enum MinigameType
    {
        MeterTiming,
        Flurry,
        Shake,
        CommandRush,
        
    }    

    public enum MinigameState{
        Idle,
        Playing,
        Success,
        Fail
    }

    public MinigameType minigameType;
    public MinigameState minigameState;

    public Action OnSuccess;
    public Action OnFail;

    //Minigame color set for gradient
    [Header("Color Set")]
    [SerializeField] Color warningColor = Color.red;
    [SerializeField] Color normalColor = Color.blue;
    [SerializeField] Color successColor = Color.green;


    
    float minigameTimer = 0f;

    //Flurry
    [Header("Flurry")]
    float flurryFillValue = 0f;
    [SerializeField] float flurryForce = 0.1f;
    [SerializeField] float flurryReduceSpeed = 0.3f;
    [SerializeField] float flurryFailTime = 4f;


    //Shake
    [Header("Shake")]
    [SerializeField] float shakeForce = 0.1f;
    [SerializeField] float shakeReduceSpeed = 0.3f;
    [SerializeField] float shakeFailTime = 4f;



    //MeterTiming
    [Header("MeterTiming")]
    [SerializeField] int meterTimingMaxFailCount = 2;
    int _meterTimingfailCount = 0;
    float _meterTimingSuccessRange = 0.1f;
    float _meterTimingSuccessValue = 0.5f;
    bool _meterTimingIsPositiveDirection = true;
    

    [Header("CommandRush")]
    [SerializeField] float commandRushReduceSpeed = 0.3f;
    
    void Success()
    {
        Debug.Log("Success");
    }

    void Fail()
    {
        Debug.Log("Fail");
    }

    void Start()
    {
    }

    void OnEnable()
    {
        OnSuccess += Success;
        OnFail += Fail;
        //TEST Code
        Initialize(minigameType);
    }

    public void Initialize(MinigameType type){
        minigameType = type;
        switch(minigameType)
        {
            case MinigameType.MeterTiming:
                //random range
                _meterTimingSuccessRange = 0.1f; //diff control
                _meterTimingSuccessValue = Random.Range(0.3f, 0.7f); //success value
                _meterTimingIsPositiveDirection = true;
                SetFillColor(Color.green);
                SetHandleColor(Color.red);
                SetGauge(_meterTimingSuccessValue, _meterTimingSuccessValue + _meterTimingSuccessRange);
                
                gaugeValue = 0f;
                gaugeFill.sizeDelta = new Vector2(0f, gaugeFill.sizeDelta.y);
                gaugeHandle.anchoredPosition = new Vector2(gaugeHandle.anchoredPosition.x, 0f);
                break;
            case MinigameType.Flurry:
                gaugeValue = 0f;
                gaugeFill.sizeDelta = new Vector2(0f, gaugeFill.sizeDelta.y);
                SetGauge(0f, 0f);
                gaugeHandle.anchoredPosition = new Vector2(gaugeHandle.anchoredPosition.x, 0f);
                break;
            case MinigameType.Shake:
                gaugeValue = 0f;
                _shakeDirection = 0;
                gaugeFill.sizeDelta = new Vector2(0f, gaugeFill.sizeDelta.y);
                SetGauge(0f, 0f);
                gaugeHandle.anchoredPosition = new Vector2(gaugeHandle.anchoredPosition.x, 0f);
                break;
        }
    }

    void OnValidate()
    {
        Initialize(minigameType);
    }


    void Update()
    {
        switch(minigameType)
        {
            case MinigameType.MeterTiming:
                UpdateMeterTiming();
                break;
            case MinigameType.Flurry:
                UpdateFlurry();
                break;
            case MinigameType.Shake:
                UpdateShake();
                break;
        }
    }

    public void SetMinigameType(int type)
    {
        minigameType = (MinigameType)type;
    }



    public void UpdateMeterTiming()
    {
        //성공 조건 - 정확한 범위내에서 스페이스바를 누를 경우 성공
        //패배 조건 - 2번 실패할 경우 패배
        SetGauge(_meterTimingSuccessValue, _meterTimingSuccessValue + _meterTimingSuccessRange);

        if(Input.GetKeyDown(KeyCode.Space))
        {
            gaugeHandle.DOPunchScale(Vector3.one * 1.2f, 0.1f);

            if(IsMeterTimingSuccess(gaugeValue))
            {
                //성공
                SetHandle(gaugeValue);
                OnSuccess?.Invoke();
                return;
            }
            else
            {
                //실패
                _meterTimingfailCount++;
                if(_meterTimingfailCount >= meterTimingMaxFailCount)
                {
                    OnFail?.Invoke();                    
                    return;
                }
            }
        }

       gaugeValue = Mathf.Clamp(gaugeValue + (_meterTimingIsPositiveDirection ? 1 : -1)  * gaugeFillSpeed * Time.deltaTime, 0f, gaugeFillMax);
       if(gaugeValue >= gaugeFillMax)
       {
           _meterTimingIsPositiveDirection = false;
       }
       else if(gaugeValue <= 0)
       {
           _meterTimingIsPositiveDirection = true;
       }
       SetHandle(gaugeValue);        
    }

    bool IsMeterTimingSuccess(float value)
    {
        return value >= _meterTimingSuccessValue - _meterTimingSuccessRange && value <= _meterTimingSuccessValue + _meterTimingSuccessRange;
    }

    void UpdateFlurry()
    {
        //성공 조건 - 스페이스바를 지정한 시간내에 충분히 누르면 성공        
        //패배 조건 - 지정한 시간내에 충분히 못누르면 패배
        if(gaugeValue >= gaugeFillMax) return;
        if(minigameTimer >= flurryFailTime) return;

        minigameTimer += Time.deltaTime;
        if(minigameTimer >= flurryFailTime)
        {
            OnFail?.Invoke();
            return;
        }

        if(Input.GetKeyDown(KeyCode.Space))
        {
            if(buttonInteraction(flurryForce))
            {
                return;
            }
        }

        if (gaugeValue > 0)
        {
            gaugeValue -= flurryReduceSpeed * Time.deltaTime;
            SetGaugeAndHandle(gaugeValue);
        }
    }

    int _shakeDirection = 0;

    void UpdateShake(){
        //성공 조건 - 좌우방향의 입력을 번갈아가면서 지정한 시간내에 충분히 누르면 성공        
        //패배 조건 - 지정한 시간내에 충분히 못누르면 패배
        if(gaugeValue >= gaugeFillMax) return;
        if(minigameTimer >= shakeFailTime) return;

        minigameTimer += Time.deltaTime;
        if(minigameTimer >= shakeFailTime)
        {
            OnFail?.Invoke();
            return;
        }

        int direction = Input.GetAxis("Horizontal") > 0 ? Mathf.CeilToInt(Input.GetAxis("Horizontal")) : Mathf.FloorToInt(Input.GetAxis("Horizontal"));

        if(direction != 0)
        {
            if(direction != _shakeDirection)
            {
                _shakeDirection = direction;
                if(buttonInteraction(shakeForce))
                {
                    return;
                }
            }
        }

        if (gaugeValue > 0)
        {
            gaugeValue -= shakeReduceSpeed * Time.deltaTime;
            SetGaugeAndHandle(gaugeValue);
        }
    }

    bool buttonInteraction(float force){
        gaugeHandle.DOPunchScale(Vector3.one * 1.2f, 0.1f).OnComplete(() => gaugeHandle.DOScale(Vector3.one, 0.1f));

        gaugeValue = Mathf.Clamp(gaugeValue + force, 0f, gaugeFillMax);

        if (gaugeValue >= gaugeFillMax)
        {
            SetGaugeAndHandle(gaugeValue);
            OnSuccess?.Invoke();
            return true;
        }

        return false;
    }


    void SetGaugeAndHandle(float value)
    {
        SetGauge(value);
        //SetHandle(value);
        SetFillColorGradient(value);
        //SetHandleColorGradient(value);
    }


    void SetGauge(float value)
    {
        gaugeFill.sizeDelta = new Vector2(value * gaugeBackground.sizeDelta.x, gaugeFill.sizeDelta.y);
    }

    void SetGauge(float start, float end)
    {
        gaugeFill.anchoredPosition = new Vector2(start * gaugeBackground.sizeDelta.x, gaugeFill.anchoredPosition.y);
        gaugeFill.sizeDelta = new Vector2((end - start) * gaugeBackground.sizeDelta.x, gaugeFill.sizeDelta.y);
    }

    void SetHandle(float value)
    {
        //핸들 위치때문에 오차 생기는 부분이 있을거로 보임
        gaugeHandle.anchoredPosition = new Vector2(value * gaugeBackground.sizeDelta.x + (0.5f - value) * gaugeHandle.sizeDelta.x, gaugeHandle.anchoredPosition.y);
    }

    void SetFillColor(Color color)
    {
        gaugeFill.GetComponent<Image>().color = color;
    }

    void SetFillColorGradient(float value)
    {
        gaugeFill.GetComponent<Image>().color = Color.Lerp(warningColor, successColor, value);
    }

    void SetHandleColor(Color color)
    {
        gaugeHandle.GetComponent<Image>().color = color;
    }

    void SetHandleColorGradient(float value)
    {
        gaugeHandle.GetComponent<Image>().color = Color.Lerp(Color.red, Color.blue, value);
    }

    
}
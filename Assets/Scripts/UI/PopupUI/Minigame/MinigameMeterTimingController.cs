using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using Cysharp.Threading.Tasks.Triggers;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using Random = UnityEngine.Random;

public class MinigameMeterTimingController: MinigameBaseController
{
    [Header("Note")]
    [SerializeField ]List<MinigameNoteSO> _minigameNoteList = new List<MinigameNoteSO>();
    [SerializeField] RectTransform noteParent;
    [SerializeField] RectTransform notePrefab;

    List<(RectTransform, float, MinigameNoteSO)> _noteList = new List<(RectTransform, float, MinigameNoteSO)>();
    List<GameObject> _rangeList = new List<GameObject>();

    [Header("UI")]
    [SerializeField] RectTransform gaugeHandleHolder;
    [SerializeField] RectTransform gaugeHandle;
    [SerializeField] float gaugeFillSpeed = 1f;
    [SerializeField] float gaugeDirection = 1f;
    [SerializeField] float gaugeFillMin = 0f;
    [SerializeField] float gaugeFillMax = 1f;

    [Header("UI Effect")]
    [SerializeField] RectTransform commonKeyImage;
    [SerializeField] TextMeshProUGUI commonChanceCountText;
    [SerializeField] RectTransform commonChanceGroup;
    [SerializeField] RectTransform commonSuccessGroup;
    [SerializeField] RectTransform commonSuccessNoteParent;

    [SerializeField] RectTransform commonFailGroup;
    

    private float gaugeValue = 0f;

    //Game Logic
    [Header("MeterTiming")]
    [SerializeField] int meterTimingMaxFailCount = 2;
    int _meterTimingfailCount = 0;

    [Header("Result")]
    public Action<MinigameNoteSO> OnCustomSuccess;
    public Action OnCustomFail;

    void OnDisable()
    {
        ClearTargetNotes();
    }

    public void Initialize()
    {   
        //UI Init
        commonChanceGroup.gameObject.SetActive(false);
        commonSuccessGroup.gameObject.SetActive(false);
        commonFailGroup.gameObject.SetActive(false);
        
        _meterTimingfailCount = 0;
        commonChanceCountText.text = meterTimingMaxFailCount.ToString();     
        gaugeValue = 0f;
        
        SetHandle(0f);
        SetTargetNotes();

        OnCustomSuccess = null;
        OnCustomSuccess += (note) =>
        {
            ChangeState(MinigameState.Success);

            var data =_noteList.Find(x => x.Item3 == note);
            var noteHolderRect = data.Item1;
            
            _noteList.Remove(data);
            Destroy(data.Item1.gameObject);

            RectTransform noteHolderRectSuccess = Instantiate(notePrefab, commonSuccessNoteParent);
            Image image = noteHolderRectSuccess.GetComponentInChildren<Image>();
            image.sprite = note.sprite;

            DOVirtual.DelayedCall(1f, () => {
                //gameObject.SetActive(false); // Hide로 교체 할 것 
                Dispose();
            });

            OnSuccess?.Invoke();
        };

        OnCustomFail = null;
        OnCustomFail += () =>
        {
            FailMiniGame();
            ChangeState(MinigameState.Fail);
            Debug.Log("Fail");
            
            DOVirtual.DelayedCall(1f, () => {
                //gameObject.SetActive(false); // Hide로 교체 할 것 
                Dispose();
            });
        };
    }
    
    protected override void ChangeState(MinigameState state)
    {
        if(state == minigameState) return;       

        //exit
        switch(minigameState)
        {
            case MinigameState.Idle:
                break;
            case MinigameState.Playing:
                commonChanceGroup.gameObject.SetActive(false);
                break;
            case MinigameState.Success:    
                commonSuccessGroup.gameObject.SetActive(false);
                foreach (Transform child in commonSuccessNoteParent)
                {
                    Destroy(child.gameObject);
                }
                break;
            case MinigameState.Fail:                
                commonFailGroup.gameObject.SetActive(false);
                break;
        }

        //enter
        switch(state)
        {
            case MinigameState.Idle:
                IdleMinigame();
                break;
            case MinigameState.Playing:
                StartMinigame();
                break;
            case MinigameState.Success:
                SuccessMiniGame();
                break;
            case MinigameState.Fail:
                FailMiniGame();
                break;
        }

        minigameState = state;
    }


    void SetTargetNotes()
    {
        _rangeList.ForEach(x => Destroy(x));
        _rangeList.Clear();
        int errorCount = 0;

        foreach (var note in _minigameNoteList)
        {
            if (_noteList.Exists(x => x.Item3 == note)) continue;
            float randomAngle = Random.Range(0, 360);            
            
            retry:
            foreach (var targetNote in _noteList)
            {
                List<Vector2> range = GetRange(randomAngle / 360, note);

                if(_noteList.Exists((x) => {
                    var rangeX = GetRange(x.Item2, x.Item3);
                    if(rangeX.Exists(y => range.Exists(z => IsOverlapping(y, z))))
                    {
                        return true;
                    }
                    return false;
                })){
                    randomAngle = Random.Range(0, 360);
                    errorCount++;

                    if(errorCount > 10)
                    {
                        Debug.LogError("Error");
                        continue;
                    }
                    goto retry;
                }else{
                    errorCount = 0;
                }
            }
            

            RectTransform noteHolderRect = Instantiate(notePrefab, noteParent);
            
            noteHolderRect.DORotate(new Vector3(0, 0, randomAngle), 0.2f, RotateMode.Fast);
            Image image = noteHolderRect.GetComponentInChildren<Image>();
            image.sprite = note.sprite;
            
            _noteList.Add((noteHolderRect, randomAngle / 360, note));
        }
    }

    void ClearTargetNotes()
    {
        foreach (var note in _noteList)
        {
            Destroy(note.Item1.gameObject);
        }
        _noteList.Clear();
    }

    void Update()
    {
        switch(minigameState)
        {
            case MinigameState.Idle:
                if(Input.GetKeyDown(KeyCode.Space))
                {
                    ChangeState(MinigameState.Translating); //애니메이션 동작 중 키입력을 막기 위한 중간 상태
                    ChangeState(MinigameState.Playing);
                }        
                break;
            case MinigameState.Playing:
                UpdateMeterTiming();
                break;
            case MinigameState.Success:
                if(Input.GetKeyDown(KeyCode.Space))
                {
                    ChangeState(MinigameState.Translating); //애니메이션 동작 중 키입력을 막기 위한 중간 상태
                    ChangeState(MinigameState.Idle);
                }
                break;
            case MinigameState.Fail:
                if(Input.GetKeyDown(KeyCode.Space))
                {
                    ChangeState(MinigameState.Translating); //애니메이션 동작 중 키입력을 막기 위한 중간 상태
                    ChangeState(MinigameState.Idle);
                }
                break;
        }

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            OnFail?.Invoke();
            Dispose();
        }
    }

    void IdleMinigame()
    {
        Initialize();
    }

    void StartMinigame()
    {
        commonChanceGroup.gameObject.SetActive(true);
        commonChanceGroup.transform.DOScale(0, 0).OnComplete(() => commonChanceGroup.transform.DOScale(Vector3.one, 0.3f));
        commonKeyImage.DOLocalMoveY(-80, 0.5f).OnComplete(() => {
            
            minigameState = MinigameState.Playing;
            });
    }

    void SuccessMiniGame()
    {
        commonKeyImage.DOKill(true);
        commonSuccessGroup.gameObject.SetActive(true);
        //commonSuccessGroup.transform.DOScale(0, 0).OnComplete(() => commonSuccessGroup.transform.DOScale(Vector3.one, 0.1f));
        commonSuccessGroup.transform.DOPunchScale(Vector3.one * 1.1f, 0.1f).OnComplete(() => commonSuccessGroup.transform.DOScale(Vector3.one, 0.2f));
    }

    void FailMiniGame()
    {
        commonKeyImage.DOKill(true);
        commonFailGroup.gameObject.SetActive(true);
        commonFailGroup.transform.DOScale(0, 0).OnComplete(() => commonFailGroup.transform.DOScale(Vector3.one, 0.3f));
    }


    void UpdateMeterTiming()
    {
        //성공 조건 - 정확한 범위내에서 스페이스바를 누를 경우 성공
        //패배 조건 - N번 실패할 경우 패배
        if(Input.GetKeyDown(KeyCode.Space))
        {
            ButtonInteraction();
            var note =IsMeterTimingSuccess(gaugeValue);
            if(note != null)
            {
                //성공
                SetHandle(gaugeValue);
                OnCustomSuccess?.Invoke(note);
                return;
            }
            else
            {
                Debug.Log("Interact Fail Detected");
                //실패
                _meterTimingfailCount++;
                commonChanceCountText.text = (meterTimingMaxFailCount - _meterTimingfailCount).ToString();
                commonChanceCountText.transform.DOPunchScale(Vector3.one * 1.1f, 0.1f).OnComplete(() => commonChanceCountText.transform.DOScale(Vector3.one, 0.1f));
                
                if(_meterTimingfailCount >= meterTimingMaxFailCount)
                {
                    OnFail?.Invoke();                    
                    return;
                }
            }
        }

        gaugeValue += gaugeFillSpeed * Time.deltaTime * gaugeDirection;
        if(gaugeValue > 1)
        {
            gaugeValue -= (int)gaugeValue;
        }
        else if(gaugeValue < 0)
        {
            gaugeValue += (int)(gaugeValue + 1);
        }
    
       SetHandle(gaugeValue);
    }

    bool ButtonInteraction()
    { 
        gaugeHandle.DOPunchScale(Vector3.one * 1.5f, 0.1f).OnComplete(() => gaugeHandle.DOScale(Vector3.one, 0.1f));

        return true;
    }


    MinigameNoteSO IsMeterTimingSuccess(float value)
    {
        foreach (var note in _noteList)
        {
            var noteRange = GetRange(note.Item2, note.Item3);            
            if(noteRange.Exists(x => value >= x.x && value <= x.y))
            {
                return note.Item3;
            }
        }

        return null;
    }


    List<Vector2> GetRange(float value, MinigameNoteSO note)
    {  
        List<Vector2> range = new List<Vector2>();


        float min = value - note.interactionRange;
        float max = value + note.interactionRange;

        if(min < 0)
        {
            range.Add(new Vector2(1 + min, 1));
            range.Add(new Vector2(0, max));
        }
        else if(max > 1)
        {
            range.Add(new Vector2(min, 1));
            range.Add(new Vector2(0, max - 1));
        }
        else
        {
            range.Add(new Vector2(min, max));
        }


        return range;
    }

    bool IsOverlapping(Vector2 rangeA, Vector2 rangeB)
    {
        Debug.Log(rangeA + " " + rangeB);

        return rangeA.y >= rangeB.x && rangeB.y >= rangeA.x;
    }

    void SetHandle(float value)
    {
        gaugeHandleHolder.localRotation = Quaternion.Euler(0, 0, value * 360f * gaugeDirection);
    }
}

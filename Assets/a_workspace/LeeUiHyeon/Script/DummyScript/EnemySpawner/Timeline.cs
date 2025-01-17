using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class Timeline//미완성 클래스 - 수정중
{
    //Timeline은 정해진 시간(타이밍)에 원하는 기능이 실행되도록 하는 클래스입니다.
    //우선은 적 스폰을 위해서만 만들었지만 이벤트 발생시에도 사용할 수 있습니다.
    
    //시간대를 List로 저장합니다.
    private List<float> _gameTimeline;

    //설정한 타임라인을 받습니다.
    // public void SetTimelines(List<float> timelines)
    // {
    //     _gameTimeline.AddRange(timelines);
    //     
    // }
    
    public List<float> AddTimeline(float time)
    {
        List<float> timeline = new List<float>();
        timeline.Add(time);
        return timeline;
    }

    // //타임라인을 모두 지웁니다.
    // public void ClearTimelines()
    // {
    //     _gameTimeline.Clear();
    // }

    //각 타임라인별로 실행할 함수를 List로 받습니다.
    //0, 10, 30
    public async void StartTimelines(List<float> _gameTimeline, List<UnityAction> function)
    {
        float delaytime;
        for (int i = 0; i < _gameTimeline.Count; i++)
        {
            delaytime = (i == 0) ? _gameTimeline[i] : _gameTimeline[i] - _gameTimeline[i - 1];
            await UniTask.Delay((int)(1000 * delaytime));
            RunTask(function[i]);
        }
    }

    //받은 함수가 null이 아닌지 확인하고 실행합니다.
    public void RunTask(UnityAction function)
    {
        function?.Invoke();
    }
}
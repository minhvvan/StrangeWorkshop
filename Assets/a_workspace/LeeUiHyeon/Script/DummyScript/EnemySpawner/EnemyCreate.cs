using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public interface IEnemyCreate
{

}

[RequireComponent(typeof(EnemySpawner))]
public class EnemyCreate : MonoBehaviour
{
    //Dictionary<시간,발동될액션들>
    Dictionary<float, List<UnityAction>> actionTimeline = new Dictionary<float, List<UnityAction>>();
    EnemySpawner _spawner;
    Timeline _timeline;

    void Awake()
    {
        _timeline = new Timeline();
        _spawner = GetComponent<EnemySpawner>();
    }

    void Start()
    {
        
    }
    
    //타임라인 Task생성해주는 함수 만들기?
}

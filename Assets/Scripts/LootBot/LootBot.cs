using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootBot : MonoBehaviour
{
    private LootBotBlackBoard _blackBoard;

    private void Awake()
    {
        _blackBoard = GetComponent<LootBotBlackBoard>();
        _blackBoard.OnBotPowerdown += HandlePowerDown;
    }

    private void HandlePowerDown()
    {
        //TODO: Anim play
        //Anim이 종료되면 삭제 -> 플레이어로 컨트롤 전환
        Destroy(gameObject);
        Debug.Log("Power down");
        CameraManager.Instance.ResetFollowTarget();
        InputManager.Instance.ReturnToPlayerControl();
    }

    void OnTriggerEnter(Collider other)
    {
        //TODO: 재화 수집(Gold 오브젝트에서 값 가져와야 함)
        _blackBoard.AddGold(10);
        Destroy(other.gameObject);
    }
}

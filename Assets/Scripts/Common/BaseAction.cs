using UnityEngine;

public abstract class BaseAction : MonoBehaviour
{

    /// <summary>
    /// 컨트롤러에 액션을 등록하는 함수
    /// OnActions 에 등록할 경우 Update 함수에서 지속 호출
    /// 다른 액션은 선언 후 등록해서 사용하면 될듯 OnInteract 관련 예제 확인
    /// </summary>
    /// <returns></returns>
    public abstract bool RegistAction();

    /// <summary>
    /// 등록한 액션을 해제하는 함수
    /// RegistAction 에서 등록한 액션을 해제
    /// </summary>
    public abstract void UnregistAction();

}
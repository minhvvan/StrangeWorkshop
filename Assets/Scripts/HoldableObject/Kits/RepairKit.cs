using Cysharp.Threading.Tasks;
using Managers;
using UnityEngine;

public class RepairKit : BaseKit, IInteractKits
{
    private async void Awake()
    {
        await Initialize();
    }
    
    public async UniTask Initialize()
    {
        //Setup
        IsNoCost = false;
        OnExcute = Excute;
        
        //SO 받기
        var repairKitSO = await DataManager.Instance
            .LoadDataAsync<KitInfoSO>(Addresses.Data.Kits.REPAIR);
        
        //Set Data
        SetKitInfo(repairKitSO);
        SetKit();
    }
    
    public void Excute(SampleCharacterController player = null, int? cost = null)
    {
        cost = cost ?? 1;
        var clearCounter = (ClearCounter)player.GetSelectedInteractableObject();
        
        //cost 체크
        if (KitRemainingCost - cost.Value < 0)
        {
            Debug.LogError("REPAIR: COST가 부족합니다.");
            return;
        }
        
        //수리동작 수행
        if (!clearCounter.RepairCounter(kitValue))
        {
            Debug.LogError("REPAIR: 이미 내구도가 최대 입니다!");
            return;
        }
        
        //연산처리
        KitRemainingCost -= cost.Value;
        
        //VFX
        VFXManager.Instance.TriggerVFX(VFXType.COUNTERREPAIR, 
            clearCounter.transform.position + new Vector3(0,2f,0));
    }
}
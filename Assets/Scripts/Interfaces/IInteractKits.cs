public interface IInteractKits
{
    void SetKitInfo(KitInfoSO kitInfo);
    void SetKit(KitObject kit);
    void UpgradeKit(int? upgradeValue = null);
    void Excute(KitObject kit, SampleCharacterController player = null, int? cost = null);
}
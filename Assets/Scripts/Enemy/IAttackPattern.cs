using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Managers;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public abstract class IAttackPattern : MonoBehaviour
{
    protected Barrier TargetBarrier;
    protected BlackboardEnemy EnemyBb;
    //protected bool _bCanAttack;
    //protected bool ActionCompleted;
    protected bool IsBurserkOn = false;

    protected float AtkDamage;
    protected float AtkFirstDelay;
    protected float AtkRange;
    protected float AtkCooldown;
    protected float MotionSpeed = 1f;
    
    public abstract UniTask RunPattern(BlackboardEnemy enemyBlackboard);
    public abstract UniTask DelayAction(Action action, float atkFirstDelay, float atkCooldown);
}

public class MeleeNormal : IAttackPattern
{
    public override async UniTask RunPattern(BlackboardEnemy enemyBlackboard)
    {
        EnemyBb = enemyBlackboard;
        TargetBarrier = EnemyBb.targetCollider.GetComponent<Barrier>();
        
        //설정값 초기화.
        AtkFirstDelay = EnemyBb.enemyStatus.animFirstDelay;
        AtkCooldown = EnemyBb.enemyStatus.attackSpeed;
        AtkDamage = EnemyBb.enemyStatus.attackDamage;
        MotionSpeed = 1f;
        
        //우선순위 최하위로 낮춘다.
        EnemyBb.priorityStack = EnemyBb.agent.avoidancePriority = 1;
        
        //공격 판정 동작.
        await NormalAttack();
        
        EnemyBb.bCanPattern = false;
    }
    
    private void GiveDamage(float damage)
    {
        TargetBarrier.TakeDamage(damage);
    }
    
    private async UniTask NormalAttack()
    {
        AtkFirstDelay = EnemyBb.enemyStatus.animFirstDelay;
        AtkCooldown = EnemyBb.enemyStatus.attackSpeed;
        EnemyBb.AnimAttack();
        EnemyBb.AnimSetSpeed(MotionSpeed);
        await DelayAction(
            () => GiveDamage(AtkDamage),
            AtkFirstDelay/MotionSpeed,
            AtkCooldown/MotionSpeed);
        EnemyBb.AnimSetSpeed(1f);
    }
    

    public override async UniTask DelayAction(Action action, float atkFirstDelay, float atkCooldown)
    {
        //공격 모션에 맞게 판정이 나가도록 선딜레이 적용
        await UniTask.Delay(
            (int)(1000*atkFirstDelay),
            cancellationToken: EnemyBb.cts.Token);
        if (!EnemyBb.bCanPattern) return;
        
        //공격 명령
        action?.Invoke();
        
        //공격 대기시간
        await UniTask.Delay( 
            //공격 대기시간에 모션 선딜레이시간을 빼서 딜레이 맞추기.
            (int)(1000*(atkCooldown - atkFirstDelay)), 
            //공격 도중 죽으면 도중에 취소해야한다.
            cancellationToken: EnemyBb.cts.Token);
    }
}

public class MeleeBruiser : IAttackPattern
{
    public override async UniTask RunPattern(BlackboardEnemy enemyBlackboard)
    {
        EnemyBb = enemyBlackboard;
        TargetBarrier = EnemyBb.targetCollider.GetComponent<Barrier>();
        
        //설정값 초기화.
        AtkFirstDelay = EnemyBb.enemyStatus.animFirstDelay;
        AtkCooldown = EnemyBb.enemyStatus.attackSpeed;
        AtkDamage = EnemyBb.enemyStatus.attackDamage;
        MotionSpeed = 2f;
        
        //우선순위 최하위로 낮춘다.
        EnemyBb.priorityStack = EnemyBb.agent.avoidancePriority = 1;
        
        //공격 판정 동작.
        await NormalAttack();
        
        EnemyBb.bCanPattern = false;
    }
    
    private void GiveDamage(float damage)
    {
        TargetBarrier.TakeDamage(damage);
    }
    
    private async UniTask NormalAttack()
    {
        AtkFirstDelay = EnemyBb.enemyStatus.animFirstDelay;
        AtkCooldown = EnemyBb.enemyStatus.attackSpeed;
        EnemyBb.AnimAttack();
        EnemyBb.AnimSetSpeed(MotionSpeed);
        await DelayAction(
            () => GiveDamage(AtkDamage),
            AtkFirstDelay/MotionSpeed,
            AtkCooldown/MotionSpeed);
        EnemyBb.AnimSetSpeed(1f);
    }
    

    public override async UniTask DelayAction(Action action, float atkFirstDelay, float atkCooldown)
    {
        //공격 모션에 맞게 판정이 나가도록 선딜레이 적용
        await UniTask.Delay(
            (int)(1000*atkFirstDelay),
            cancellationToken: EnemyBb.cts.Token);
        if (!EnemyBb.bCanPattern) return;
        
        //공격 명령
        action?.Invoke();
        
        //공격 대기시간
        await UniTask.Delay( 
            //공격 대기시간에 모션 선딜레이시간을 빼서 딜레이 맞추기.
            (int)(1000*(atkCooldown - atkFirstDelay)), 
            //공격 도중 죽으면 도중에 취소해야한다.
            cancellationToken: EnemyBb.cts.Token);
    }
}

public class MeleeTanker : IAttackPattern
{
    public override async UniTask RunPattern(BlackboardEnemy enemyBlackboard)
    {
        EnemyBb = enemyBlackboard;
        TargetBarrier = EnemyBb.targetCollider.GetComponent<Barrier>();
        
        //설정값 초기화.
        AtkFirstDelay = EnemyBb.enemyStatus.animFirstDelay;
        AtkCooldown = EnemyBb.enemyStatus.attackSpeed;
        AtkDamage = EnemyBb.enemyStatus.attackDamage;
        MotionSpeed = 1f;
        
        //우선순위 최하위로 낮춘다.
        EnemyBb.priorityStack = EnemyBb.agent.avoidancePriority = 1;
        
        //공격 판정 동작.
        await ChargeAttack();
        
        EnemyBb.bCanPattern = false;
    }
    
    private void GiveDamage(float damage)
    {
        TargetBarrier.TakeDamage(damage);
    }
    
    private async UniTask ChargeAttack()
    {
        AtkFirstDelay = EnemyBb.enemyStatus.animFirstDelay;
        AtkCooldown = EnemyBb.enemyStatus.attackSpeed;
        EnemyBb.AnimCrossFade("ChargeAttack");
        EnemyBb.AnimSetSpeed(MotionSpeed);
        await DelayAction(
            () => GiveDamage(AtkDamage),
            AtkFirstDelay/MotionSpeed,
            AtkCooldown/MotionSpeed);
        EnemyBb.AnimSetSpeed(1f);
    }
    

    public override async UniTask DelayAction(Action action, float atkFirstDelay, float atkCooldown)
    {
        //공격 모션에 맞게 판정이 나가도록 선딜레이 적용
        await UniTask.Delay(
            (int)(1000*atkFirstDelay),
            cancellationToken: EnemyBb.cts.Token);
        if (!EnemyBb.bCanPattern) return;
        
        //공격 명령
        action?.Invoke();
        
        //공격 대기시간
        await UniTask.Delay( 
            //공격 대기시간에 모션 선딜레이시간을 빼서 딜레이 맞추기.
            (int)(1000*(atkCooldown - atkFirstDelay)), 
            //공격 도중 죽으면 도중에 취소해야한다.
            cancellationToken: EnemyBb.cts.Token);
    }
}

public class MeleeFlanker : IAttackPattern
{
    public override async UniTask RunPattern(BlackboardEnemy enemyBlackboard)
    {
        EnemyBb = enemyBlackboard;
        TargetBarrier = EnemyBb.targetCollider.GetComponent<Barrier>();
        
        //설정값 초기화.
        AtkFirstDelay = EnemyBb.enemyStatus.animFirstDelay;
        AtkCooldown = EnemyBb.enemyStatus.attackSpeed;
        AtkDamage = EnemyBb.enemyStatus.attackDamage;
        MotionSpeed = 1f;
        
        //우선순위 최하위로 낮춘다.
        EnemyBb.priorityStack = EnemyBb.agent.avoidancePriority = 1;
        
        //공격 판정 동작.
        await NormalAttack();
        
        EnemyBb.bCanPattern = false;
    }
    
    private void GiveDamage(float damage)
    {
        TargetBarrier.TakeDamage(damage);
    }
    
    private async UniTask NormalAttack()
    {
        //AtkFirstDelay = EnemyBb.enemyStatus.animFirstDelay;
        AtkFirstDelay = 0f;
        AtkCooldown = EnemyBb.enemyStatus.attackSpeed;
        //EnemyBb.AnimAttack();
        //EnemyBb.AnimSetSpeed(MotionSpeed);
        await DelayAction(
            () => GiveDamage(AtkDamage),
            AtkFirstDelay/MotionSpeed,
            AtkCooldown/MotionSpeed);
        //EnemyBb.AnimSetSpeed(1f);
    }
    

    public override async UniTask DelayAction(Action action, float atkFirstDelay, float atkCooldown)
    {
        //공격 모션에 맞게 판정이 나가도록 선딜레이 적용
        await UniTask.Delay(
            (int)(1000*atkFirstDelay),
            cancellationToken: EnemyBb.cts.Token);
        if (!EnemyBb.bCanPattern) return;
        
        //공격 명령
        action?.Invoke();
        
        //공격 대기시간
        await UniTask.Delay( 
            //공격 대기시간에 모션 선딜레이시간을 빼서 딜레이 맞추기.
            (int)(1000*(atkCooldown - atkFirstDelay)), 
            //공격 도중 죽으면 도중에 취소해야한다.
            cancellationToken: EnemyBb.cts.Token);
    }
}

public class MeleeHider : IAttackPattern
{
    public override async UniTask RunPattern(BlackboardEnemy enemyBlackboard)
    {
        EnemyBb = enemyBlackboard;
        TargetBarrier = EnemyBb.targetCollider.GetComponent<Barrier>();
        
        //설정값 초기화.
        AtkFirstDelay = EnemyBb.enemyStatus.animFirstDelay;
        AtkCooldown = EnemyBb.enemyStatus.attackSpeed;
        AtkDamage = EnemyBb.enemyStatus.attackDamage;
        MotionSpeed = 1f;
        
        //우선순위 최하위로 낮춘다.
        EnemyBb.priorityStack = EnemyBb.agent.avoidancePriority = 1;
        
        //공격 판정 동작.
        await NormalAttack();
        
        EnemyBb.bCanPattern = false;
    }
    
    private void GiveDamage(float damage)
    {
        TargetBarrier.TakeDamage(damage);
    }
    
    private async UniTask NormalAttack()
    {
        //AtkFirstDelay = EnemyBb.enemyStatus.animFirstDelay;
        AtkFirstDelay = 0f;
        AtkCooldown = EnemyBb.enemyStatus.attackSpeed;
        //EnemyBb.AnimAttack();
        //EnemyBb.AnimSetSpeed(MotionSpeed);
        await DelayAction(
            () => GiveDamage(AtkDamage),
            AtkFirstDelay/MotionSpeed,
            AtkCooldown/MotionSpeed);
        //EnemyBb.AnimSetSpeed(1f);
    }
    

    public override async UniTask DelayAction(Action action, float atkFirstDelay, float atkCooldown)
    {
        //공격 모션에 맞게 판정이 나가도록 선딜레이 적용
        await UniTask.Delay(
            (int)(1000*atkFirstDelay),
            cancellationToken: EnemyBb.cts.Token);
        if (!EnemyBb.bCanPattern) return;
        
        //공격 명령
        action?.Invoke();
        
        //공격 대기시간
        await UniTask.Delay( 
            //공격 대기시간에 모션 선딜레이시간을 빼서 딜레이 맞추기.
            (int)(1000*(atkCooldown - atkFirstDelay)), 
            //공격 도중 죽으면 도중에 취소해야한다.
            cancellationToken: EnemyBb.cts.Token);
    }
}

public class RangeNormal : IAttackPattern
{
    private GameObject swordPrefab;
    private Transform swordTransform;
    
    public RangeNormal()
    {
        LoadItem().Forget();
    }

    private async UniTask LoadItem()
    {
        swordPrefab = await DataManager.Instance.LoadPrefabAsync(Addresses.Prefabs.Enemy.THROW_SWORD);
    }
    
    public override async UniTask RunPattern(BlackboardEnemy enemyBlackboard)
    {
        EnemyBb = enemyBlackboard;
        TargetBarrier = EnemyBb.targetCollider.GetComponent<Barrier>();

        if (swordTransform == null)
        {
            swordTransform = EnemyBb.transform.GetComponentsInChildren<Transform>()
                .FirstOrDefault(t => t.name == "MiddleFinger4_R");
        }
        
        
        //설정값 초기화.
        AtkFirstDelay = EnemyBb.enemyStatus.animFirstDelay;
        AtkRange = EnemyBb.enemyStatus.attackRange;
        AtkCooldown = EnemyBb.enemyStatus.attackSpeed;
        AtkDamage = EnemyBb.enemyStatus.attackDamage;
        MotionSpeed = 1f;
        
        //우선순위 최하위로 낮춘다.
        EnemyBb.priorityStack = EnemyBb.agent.avoidancePriority = 1;
        //공격 판정 동작.
        await NormalRangeAttack();
        
        EnemyBb.bCanPattern = false;
    }
    
    private void ThrowSword(float damage)
    {
        var obj = Instantiate(swordPrefab, swordTransform.position, Quaternion.identity);
        var newTargetPos = TargetBarrier.transform.position;
        var targetDirection = newTargetPos - swordTransform.position;
        obj.transform.forward = targetDirection;
        var getOrder = obj.GetComponent<KnightSword>();
        getOrder.throwType = KnightSword.ThrowType.RangeAttack;
        getOrder.OnAction(() =>
        {
            obj.transform.DOMove(newTargetPos, 0.5f);
            TargetBarrier.TakeDamage(damage);
        });
    }
    
    private async UniTask NormalRangeAttack()
    {
        AtkFirstDelay = EnemyBb.enemyStatus.animFirstDelay;
        AtkCooldown = EnemyBb.enemyStatus.attackSpeed;
        EnemyBb.AnimCrossFade("Throw");
        EnemyBb.AnimSetSpeed(MotionSpeed);
        await DelayAction(
            () => ThrowSword(AtkDamage),
            AtkFirstDelay/MotionSpeed,
            AtkCooldown/MotionSpeed);
        EnemyBb.AnimSetSpeed(1f);
    }
    
    public override async UniTask DelayAction(Action action, float atkFirstDelay, float atkCooldown)
    {
        //공격 모션에 맞게 판정이 나가도록 선딜레이 적용
        await UniTask.Delay(
            (int)(1000*atkFirstDelay),
            cancellationToken: EnemyBb.cts.Token);
        if (!EnemyBb.bCanPattern) return;
        
        //공격 명령
        action?.Invoke();
        
        //공격 대기시간
        await UniTask.Delay( 
            //공격 대기시간에 모션 선딜레이시간을 빼서 딜레이 맞추기.
            (int)(1000*(atkCooldown - atkFirstDelay)), 
            //공격 도중 죽으면 도중에 취소해야한다.
            cancellationToken: EnemyBb.cts.Token);
    }
}

public class RangeMage : IAttackPattern
{
    private GameObject spellPrefab;
    private Transform spellTransform;
    
    public RangeMage()
    {
        LoadItem().Forget();
    }

    private async UniTask LoadItem()
    {
        spellPrefab = await DataManager.Instance.LoadPrefabAsync(Addresses.Prefabs.Enemy.THROW_SWORD);
    }
    
    public override async UniTask RunPattern(BlackboardEnemy enemyBlackboard)
    {
        EnemyBb = enemyBlackboard;
        TargetBarrier = EnemyBb.targetCollider.GetComponent<Barrier>();

        if (spellTransform == null)
        {
            // spellTransform = EnemyBb.transform.GetComponentsInChildren<Transform>()
            //     .FirstOrDefault(t => t.name == "MiddleFinger4_R");
            
            //더미모델 사용중 임시 Transform
            spellTransform = EnemyBb.transform.GetComponentInChildren<Transform>();
        }
        
        
        //설정값 초기화.
        AtkFirstDelay = EnemyBb.enemyStatus.animFirstDelay;
        AtkRange = EnemyBb.enemyStatus.attackRange;
        AtkCooldown = EnemyBb.enemyStatus.attackSpeed;
        AtkDamage = EnemyBb.enemyStatus.attackDamage;
        MotionSpeed = 1f;
        
        //우선순위 최하위로 낮춘다.
        EnemyBb.priorityStack = EnemyBb.agent.avoidancePriority = 1;
        //공격 판정 동작.
        await CastingZone();
        
        EnemyBb.bCanPattern = false;
    }
    
    private void ThrowSword(float damage)
    {
        var obj = Instantiate(spellPrefab, spellTransform.position, Quaternion.identity);
        var newTargetPos = TargetBarrier.transform.position;
        var targetDirection = newTargetPos - spellTransform.position;
        obj.transform.forward = targetDirection;
        var getOrder = obj.GetComponent<KnightSword>();
        getOrder.throwType = KnightSword.ThrowType.RangeAttack;
        getOrder.OnAction(() =>
        {
            obj.transform.DOMove(newTargetPos, 0.5f);
            TargetBarrier.TakeDamage(damage);
        });
    }
    
    private async UniTask CastingZone()
    {
        //AtkFirstDelay = EnemyBb.enemyStatus.animFirstDelay;
        AtkFirstDelay = 0;
        AtkCooldown = EnemyBb.enemyStatus.attackSpeed;
        //EnemyBb.AnimCrossFade("Throw");
        //EnemyBb.AnimSetSpeed(MotionSpeed);
        await DelayAction(
            () => ThrowSword(AtkDamage),
            AtkFirstDelay/MotionSpeed,
            AtkCooldown/MotionSpeed);
        //EnemyBb.AnimSetSpeed(1f);
    }
    
    public override async UniTask DelayAction(Action action, float atkFirstDelay, float atkCooldown)
    {
        //공격 모션에 맞게 판정이 나가도록 선딜레이 적용
        await UniTask.Delay(
            (int)(1000*atkFirstDelay),
            cancellationToken: EnemyBb.cts.Token);
        if (!EnemyBb.bCanPattern) return;
        
        //공격 명령
        action?.Invoke();
        
        //공격 대기시간
        await UniTask.Delay( 
            //공격 대기시간에 모션 선딜레이시간을 빼서 딜레이 맞추기.
            (int)(1000*(atkCooldown - atkFirstDelay)), 
            //공격 도중 죽으면 도중에 취소해야한다.
            cancellationToken: EnemyBb.cts.Token);
    }
}

public class Chapter1Boss : IAttackPattern
{
    private SampleCharacterController _player;
    private Barrier _targetBarrier;
    private List<Vector3> grids;
    private GameObject swordPrefab;
    
    public Chapter1Boss()
    {
        LoadItem().Forget();
        
        var barrierPoints = EnemyPathfinder.instance.pbBarrierPoints;
        grids = new List<Vector3>();
        
        //배리어 위치를 grid로 삼는다.
        foreach (var barriers in barrierPoints)
        {
            grids.Add(barriers.position);
        }
    }

    private async UniTask LoadItem()
    {
        swordPrefab = await DataManager.Instance.LoadPrefabAsync(Addresses.Prefabs.Enemy.THROW_SWORD);
    }

    public override async UniTask RunPattern(BlackboardEnemy enemyBlackboard)
    {
        EnemyBb = enemyBlackboard;
        _targetBarrier = EnemyBb.targetCollider.GetComponent<Barrier>();
        _player = EnemyBb.player;
        
        //설정값 초기화.
        AtkFirstDelay = EnemyBb.enemyStatus.animFirstDelay;
        AtkCooldown = EnemyBb.enemyStatus.attackSpeed;
        AtkDamage = EnemyBb.enemyStatus.attackDamage;
        MotionSpeed = 1f;
        
        //공격하려는데 그 순간 방벽의 체력이 0이면 중단.
        // if (_targetBarrier.BarrierStat.health <= 0)
        // {
        //     _enemyBb.bCanPattern = false;
        //     return;
        // }
        
        if (Mathf.Approximately(EnemyBb.enemyStatus.hp, EnemyBb.enemyStatus.maxHp) ||
            EnemyBb.enemyStatus.hp >= EnemyBb.enemyStatus.maxHp * 0.8f)
        {
            await RunPhase_One();
        }
        else if (EnemyBb.enemyStatus.hp < EnemyBb.enemyStatus.maxHp * 0.8f &&
                 EnemyBb.enemyStatus.hp >= EnemyBb.enemyStatus.maxHp * 0.3f)
        {
            await RunPhase_Two();
            EnemyBb.RandomResearchTarget();
            EnemyBb.agent.speed = EnemyBb.enemyStatus.moveSpeed *= 5f;
        }
        else if (EnemyBb.enemyStatus.hp < EnemyBb.enemyStatus.maxHp * 0.3f &&
                 EnemyBb.enemyStatus.hp > 0)
        {
            if (!IsBurserkOn)
            {
                //광폭화 On
                IsBurserkOn = true;
                MotionSpeed = 2f;
            }
            await RunPhase_Three();
        }

        EnemyBb.bCanPattern = false;
        await UniTask.CompletedTask;
    }
    
    private async UniTask RunPhase_One()
    {
        //공격 판정 동작
        for (int i = 0; i < 3; i++)
        {
            //방벽체력 0이면 공격모션 중단.
            // if (_targetBarrier.BarrierStat.health <= 0) return;

            if (i < 2)
            {
                await NormalAttack();
            }
            else if(i == 2)
            {
                await ChargeAttack();
            }
        }
    }
    
    private async UniTask RunPhase_Two()
    {
        //공격 판정 동작
        for (int i = 0; i < 5; i++)
        {
            //방벽체력 0이면 공격모션 중단.
            // if (_targetBarrier.BarrierStat.health <= 0) return;
            
            if (i < 2)
            {
                await NormalAttack();
            }
            else if(i == 2)
            {
                MotionSpeed = 2f;
                await ChargeAttack();
                MotionSpeed = 1f;
            }
            else if (i == 3)
            {
                await StunningShout();
            }
            else if(i == 4)
            {
                await ThrowSword(grids);
            }
            
        }
    }
    
    private async UniTask RunPhase_Three()
    {
        //공격 판정 동작
        for (int i = 0; i < 4; i++)
        {
            //방벽체력 0이면 공격모션 중단.
            // if (_targetBarrier.BarrierStat.health <= 0) return;
            
            if (i < 2)
            {
                await ChargeAttack();
            }
            else if(i == 2)
            {
                await StunningShout();
            }
            else if (i == 3)
            {
                await ThrowSword(grids);
            }
        }
    }

    private async UniTask NormalAttack()
    {
        AtkFirstDelay = EnemyBb.enemyStatus.animFirstDelay;
        AtkCooldown = EnemyBb.enemyStatus.attackSpeed;
        EnemyBb.AnimAttack();
        EnemyBb.AnimSetSpeed(MotionSpeed);
        await DelayAction(
            () => GiveDamage(AtkDamage),
            AtkFirstDelay/MotionSpeed,
            AtkCooldown/MotionSpeed);
        EnemyBb.AnimSetSpeed(1f);
        EnemyBb.AnimIdle();
    }

    private async UniTask ChargeAttack()
    {
        int repeat = 0;
        while (repeat < 2)
        {
            AtkFirstDelay = 2.6f;
            AtkCooldown = 5f; //4.75
            EnemyBb.AnimCrossFade("ChargeAttack");
                    
            EnemyBb.AnimSetSpeed(MotionSpeed);
            await DelayAction(
                () => GiveDamage(AtkDamage * 2f),
                AtkFirstDelay/MotionSpeed,
                AtkCooldown/MotionSpeed);
            EnemyBb.AnimSetSpeed(1f);
            EnemyBb.AnimIdle();
            repeat++;
        }
    }
    
    private async UniTask StunningShout()
    {
        var vfxOffset = new Vector3(EnemyBb.transform.position.x, EnemyBb.transform.position.y + 6f, EnemyBb.transform.position.z);
        VFXManager.Instance.TriggerVFX(VFXType.WARNINGSIGN, vfxOffset);
        MotionSpeed = 0.5f;
        AtkFirstDelay = 1f;
        AtkCooldown = 3f;
        EnemyBb.AnimCrossFade("Shout");
        EnemyBb.AnimSetSpeed(MotionSpeed);
        await DelayAction(
            ()=> Stunning().Forget(),
            AtkFirstDelay/MotionSpeed,
            AtkCooldown/MotionSpeed);
        MotionSpeed = 1f;
        EnemyBb.AnimSetSpeed(1f);
    }

    private async UniTask ThrowSword(List<Vector3> grids)
    {
        int swordLevel = 6;
        int repeatCount = 3;
        
        if (IsBurserkOn)
        {
            repeatCount = 5;
        }
        
        //칼 생성
        for (int i = 0; i < repeatCount; i++)
        {
            var randomIndex = new HashSet<int>();
            while (randomIndex.Count < grids.Count/6)
            {
                randomIndex.Add(Random.Range(0, grids.Count));
            }
            
            List<int> indexList = randomIndex.ToList();
            List<GameObject> throwObjects = new List<GameObject>();
            
            for (int j = 0; j < randomIndex.Count; j++)
            {
                var gridOffset = new Vector3(grids[indexList[j]].x, grids[indexList[j]].y + 2f, grids[indexList[j]].z);
                var obj = Instantiate(swordPrefab, gridOffset, Quaternion.identity);
                var newTargetPos = new Vector3(_player.transform.position.x, _player.transform.position.y + 1f, _player.transform.position.z);
                var targetDirection = newTargetPos - obj.transform.position;
                obj.transform.forward = targetDirection;
                obj.transform.DOScale(new Vector3(3,3,3), 0f);
                obj.GetComponent<KnightSword>().throwType = KnightSword.ThrowType.BossPattern;
                throwObjects.Add(obj);
                
                var vfxOffset = new Vector3(obj.transform.position.x, obj.transform.position.y + 1f, obj.transform.position.z);
                VFXManager.Instance.TriggerVFX(VFXType.WARNINGSIGN, vfxOffset);
            }
            
            //생성 후 1초 대기
            await UniTask.Delay(1000);

            foreach (var obj in throwObjects)
            {
                var newTargetPos = new Vector3(_player.transform.position.x, _player.transform.position.y + 1f, _player.transform.position.z);
                var targetDirection = newTargetPos - obj.transform.position;
                obj.transform.forward = targetDirection;
                obj.transform.DOBlendableMoveBy(targetDirection * 2f, 2f).OnComplete(() =>
                {
                    obj.SetActive(false);
                    Destroy(obj, 4f);
                    throwObjects.Remove(obj);
                });
            }
            
            await UniTask.Delay(3000);
        }
    }
    
    //방벽에 피해 가하기
    private void GiveDamage(float damage)
    {
        _targetBarrier.TakeDamage(damage);
    }

    //플레이어 기절 (물건 떨굼, 2초 이동불가)
    private async UniTask Stunning()
    {
        try
        {
            _player.walkSpeed = 0f;
            //_player.ClearHoldableObject(); //물건떨구기 임시 비활성화
            await UniTask.Delay(2000,cancellationToken: EnemyBb.cts.Token);
            _player.walkSpeed = 15f;
        }
        finally
        {
            _player.walkSpeed = 15f;
        }
    }

    public override async UniTask DelayAction(Action action, float atkFirstDelay, float atkCooldown)
    {
        //공격 모션에 맞게 판정이 나가도록 선딜레이 적용
        await UniTask.Delay(
            (int)(1000 * atkFirstDelay),
            cancellationToken: EnemyBb.cts.Token);
        
        if (!EnemyBb.bCanPattern) return;

        //공격 명령
        action?.Invoke();

        //공격 대기시간
        await UniTask.Delay(
            //공격 대기시간에 모션 선딜레이시간을 빼서 딜레이 맞추기.
            (int)(1000 * (atkCooldown - atkFirstDelay)),
            //공격 도중 죽으면 도중에 취소해야한다.
            cancellationToken: EnemyBb.cts.Token);
    }
}

public static class PatternHandler
{
    private static readonly Dictionary<EnemyType, Func<IAttackPattern>> Patterns =
        new Dictionary<EnemyType, Func<IAttackPattern>>
        {
            {EnemyType.MeleeNormal,() => new MeleeNormal()},
            {EnemyType.MeleeBruiser,() => new MeleeBruiser()},
            {EnemyType.MeleeFlanker,() => new MeleeFlanker()},
            {EnemyType.MeleeTanker,() => new MeleeTanker()},
            {EnemyType.MeleeHider,() => new MeleeHider()},
            {EnemyType.RangeNormal,() => new RangeNormal()},
            {EnemyType.RangeMage,() => new RangeMage()},
            {EnemyType.Chapter1Boss,() => new Chapter1Boss()}
        };
    
    public static IAttackPattern CreatePattern(EnemyType etype)
    {
        if (Patterns.TryGetValue(etype, out Func<IAttackPattern> pattern))
        {
            return pattern();
        }
        else
        {
            Debug.Log("Cannot find pattern");
            return null;
        }
    }
}
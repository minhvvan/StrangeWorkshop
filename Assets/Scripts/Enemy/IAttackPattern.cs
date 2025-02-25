using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Managers;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public abstract class IAttackPattern : MonoBehaviour
{
    public abstract UniTask RunPattern(BlackboardEnemy enemyBlackboard);
    public abstract UniTask DelayAction(Action action, float atkFirstDelay, float atkCooldown);
}

public class MeleeAttack : IAttackPattern
{
    private Barrier _targetBarrier;
    private BlackboardEnemy _enemyBb;
    private bool _bCanAttack;
    private bool _actionCompleted;
    private bool _isBurserkOn;

    private float _atkDamage;
    private float _atkFirstDelay;
    private float _atkCooldown;
    private float _motionSpeed;
    
    public override async UniTask RunPattern(BlackboardEnemy enemyBlackboard)
    {
        _enemyBb = enemyBlackboard;
        _targetBarrier = _enemyBb.targetCollider.GetComponent<Barrier>();
        
        //설정값 초기화.
        _atkFirstDelay = _enemyBb.enemyStatus.animFirstDelay;
        _atkCooldown = _enemyBb.enemyStatus.attackSpeed;
        _atkDamage = _enemyBb.enemyStatus.attackDamage;
        _motionSpeed = 1f;
        
        //공격하려는데 그 순간 방벽의 체력이 0이면 중단.
        if (_enemyBb.targetCollider.GetComponent<Barrier>().BarrierStat.health <= 0)
        {
            _enemyBb.bCanPattern = false;
            return;
        }
        //우선순위 최하위로 낮춘다.
        _enemyBb.priorityStack = _enemyBb.agent.avoidancePriority = 1;
        
        //공격 판정 동작.
        await NormalAttack();
        
        _enemyBb.bCanPattern = false;
    }
    
    private void GiveDamage(float damage)
    {
        if (_targetBarrier.BarrierStat.health > 0)
        {
            _targetBarrier.TakeDamage(damage);
        }
    }
    
    private async UniTask NormalAttack()
    {
        _atkFirstDelay = _enemyBb.enemyStatus.animFirstDelay;
        _atkCooldown = _enemyBb.enemyStatus.attackSpeed;
        _enemyBb.AnimAttack();
        _enemyBb.AnimSetSpeed(_motionSpeed);
        await DelayAction(
            () => GiveDamage(_atkDamage),
            _atkFirstDelay/_motionSpeed,
            _atkCooldown/_motionSpeed);
        _enemyBb.AnimSetSpeed(1f);
    }
    

    public override async UniTask DelayAction(Action action, float atkFirstDelay, float atkCooldown)
    {
        //공격 모션에 맞게 판정이 나가도록 선딜레이 적용
        await UniTask.Delay(
            (int)(1000*atkFirstDelay),
            cancellationToken: _enemyBb.cts.Token);
        if (!_enemyBb.bCanPattern) return;
        
        //공격 명령
        action?.Invoke();
        
        //공격 대기시간
        await UniTask.Delay( 
            //공격 대기시간에 모션 선딜레이시간을 빼서 딜레이 맞추기.
            (int)(1000*(atkCooldown - atkFirstDelay)), 
            //공격 도중 죽으면 도중에 취소해야한다.
            cancellationToken: _enemyBb.cts.Token);
    }
}

public class RangeAttack : IAttackPattern
{
    private Barrier _targetBarrier;
    private BlackboardEnemy _enemyBb;
    private bool _bCanAttack;
    
    private float _atkFirstDelay;
    private float _atkCooldown;
    
    public override async UniTask RunPattern(BlackboardEnemy enemyBlackboard)
    {
        _enemyBb = enemyBlackboard;
        _targetBarrier = _enemyBb.targetCollider.GetComponent<Barrier>();
        _atkFirstDelay = _enemyBb.enemyStatus.animFirstDelay;
        _atkCooldown = _enemyBb.enemyStatus.attackSpeed;
        
        
        _targetBarrier.TakeDamage(_enemyBb.enemyStatus.attackDamage);
        
        
        _enemyBb.bCanPattern = false;
        await UniTask.CompletedTask;
    }
    
    
    public override async UniTask DelayAction(Action action, float atkFirstDelay, float atkCooldown)
    {
        //공격 모션에 맞게 판정이 나가도록 선딜레이 적용
        await UniTask.Delay(
            (int)(1000*atkFirstDelay),
            cancellationToken: _enemyBb.cts.Token);
        if (!_enemyBb.bCanPattern) return;
        
        //공격 명령
        action?.Invoke();
        
        //공격 대기시간
        await UniTask.Delay( 
            //공격 대기시간에 모션 선딜레이시간을 빼서 딜레이 맞추기.
            (int)(1000*(atkCooldown - atkFirstDelay)), 
            //공격 도중 죽으면 도중에 취소해야한다.
            cancellationToken: _enemyBb.cts.Token);
    }
}

public class Chapter1Boss : IAttackPattern
{
    private SampleCharacterController _player;
    private Barrier _targetBarrier;
    private List<Vector3> grids;
    private BlackboardEnemy _enemyBb;
    private GameObject swordPrefab;
    private bool _bCanAttack;
    private bool _actionCompleted;
    private bool _isBurserkOn;

    private float _atkDamage;
    private float _atkFirstDelay;
    private float _atkCooldown;
    private float _motionSpeed;

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
        _enemyBb = enemyBlackboard;
        _targetBarrier = _enemyBb.targetCollider.GetComponent<Barrier>();
        _player = _enemyBb.player;
        
        //설정값 초기화.
        _atkFirstDelay = _enemyBb.enemyStatus.animFirstDelay;
        _atkCooldown = _enemyBb.enemyStatus.attackSpeed;
        _atkDamage = _enemyBb.enemyStatus.attackDamage;
        _motionSpeed = 1f;
        
        //공격하려는데 그 순간 방벽의 체력이 0이면 중단.
        if (_targetBarrier.BarrierStat.health <= 0)
        {
            _enemyBb.bCanPattern = false;
            return;
        }
        
        if (Mathf.Approximately(_enemyBb.enemyStatus.hp, _enemyBb.enemyStatus.maxHp) ||
            _enemyBb.enemyStatus.hp >= _enemyBb.enemyStatus.maxHp * 0.8f)
        {
            await RunPhase_One();
        }
        else if (_enemyBb.enemyStatus.hp < _enemyBb.enemyStatus.maxHp * 0.8f &&
                 _enemyBb.enemyStatus.hp >= _enemyBb.enemyStatus.maxHp * 0.3f)
        {
            await RunPhase_Two();
        }
        else if (_enemyBb.enemyStatus.hp < _enemyBb.enemyStatus.maxHp * 0.3f &&
                 _enemyBb.enemyStatus.hp > 0)
        {
            if (!_isBurserkOn)
            {
                //광폭화 On
                _isBurserkOn = true;
                _motionSpeed = 2f;
            }
            await RunPhase_Three();
        }

        _enemyBb.bCanPattern = false;
        await UniTask.CompletedTask;
    }
    
    private async UniTask RunPhase_One()
    {
        //공격 판정 동작
        for (int i = 0; i < 3; i++)
        {
            //방벽체력 0이면 공격모션 중단.
            if (_targetBarrier.BarrierStat.health <= 0) return;

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
        for (int i = 0; i < 4; i++)
        {
            //방벽체력 0이면 공격모션 중단.
            if (_targetBarrier.BarrierStat.health <= 0) return;
            for (int j = 0; j < 3; j++)
            {
                if (j != 2)
                {
                    if (i < 2)
                    {
                        await NormalAttack();
                    }
                    else if(i == 2)
                    {
                        _motionSpeed = 2f;
                        await ChargeAttack();
                        _motionSpeed = 1f;
                    }
                    else if (i == 3)
                    {
                        await StunningShout();
                    }
                }
                else
                {
                    await ThrowSword(grids);
                }
            }
        }
    }
    
    private async UniTask RunPhase_Three()
    {
        //공격 판정 동작
        for (int i = 0; i < 4; i++)
        {
            //방벽체력 0이면 공격모션 중단.
            if (_targetBarrier.BarrierStat.health <= 0) return;
            
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
        _atkFirstDelay = _enemyBb.enemyStatus.animFirstDelay;
        _atkCooldown = _enemyBb.enemyStatus.attackSpeed;
        _enemyBb.AnimAttack();
        _enemyBb.AnimSetSpeed(_motionSpeed);
        await DelayAction(
            () => GiveDamage(_atkDamage),
            _atkFirstDelay/_motionSpeed,
            _atkCooldown/_motionSpeed);
        _enemyBb.AnimSetSpeed(1f);
    }

    private async UniTask ChargeAttack()
    {
        int repeat = 0;
        while (repeat < 2)
        {
            _atkFirstDelay = 2.6f;
            _atkCooldown = 5f; //4.75
            _enemyBb.AnimCrossFade("ChargeAttack");
                    
            _enemyBb.AnimSetSpeed(_motionSpeed);
            await DelayAction(
                () => GiveDamage(_atkDamage * 2f),
                _atkFirstDelay/_motionSpeed,
                _atkCooldown/_motionSpeed);
            _enemyBb.AnimSetSpeed(1f);
            repeat++;
        }
    }
    
    private async UniTask StunningShout()
    {
        _atkFirstDelay = 1;
        _atkCooldown = 3;
        _enemyBb.AnimCrossFade("Shout");
        _enemyBb.AnimSetSpeed(_motionSpeed);
        await DelayAction(
            ()=> Stunning().Forget(),
            _atkFirstDelay/_motionSpeed,
            _atkCooldown/_motionSpeed);
        _enemyBb.AnimSetSpeed(1f);
    }

    private async UniTask ThrowSword(List<Vector3> grids)
    {
        int swordLevel = 6;
        int repeatCount = 3;
        
        if (_isBurserkOn)
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
        if (_targetBarrier.BarrierStat.health > 0)
        {
            _targetBarrier.TakeDamage(damage);
        }
    }

    //플레이어 기절 (물건 떨굼, 2초 이동불가)
    private async UniTask Stunning()
    {
        try
        {
            _player.walkSpeed = 0f;
            //_player.ClearHoldableObject(); //물건떨구기 임시 비활성화
            await UniTask.Delay(2000,cancellationToken: _enemyBb.cts.Token);
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
            cancellationToken: _enemyBb.cts.Token);
        
        if (!_enemyBb.bCanPattern) return;

        //공격 명령
        action?.Invoke();

        //공격 대기시간
        await UniTask.Delay(
            //공격 대기시간에 모션 선딜레이시간을 빼서 딜레이 맞추기.
            (int)(1000 * (atkCooldown - atkFirstDelay)),
            //공격 도중 죽으면 도중에 취소해야한다.
            cancellationToken: _enemyBb.cts.Token);
    }
}

public class OtherAttack : IAttackPattern
{
    private Barrier _targetBarrier;
    private BlackboardEnemy _enemyBb;
    private bool _bCanAttack;
    
    //판정 적용 선딜레이, 재공격 대기시간.
    private float _atkFirstDelay;
    private float _atkCooldown;
    
    public override async UniTask RunPattern(BlackboardEnemy enemyBlackboard)
    {
        _enemyBb = enemyBlackboard;
        _targetBarrier = _enemyBb.targetCollider.GetComponent<Barrier>();
        _atkFirstDelay = _enemyBb.enemyStatus.animFirstDelay;
        _atkCooldown = _enemyBb.enemyStatus.attackSpeed;
        
        _targetBarrier.TakeDamage(_enemyBb.enemyStatus.attackDamage);
        
        _enemyBb.bCanPattern = false;
        await UniTask.CompletedTask;
    }
    
    public override async UniTask DelayAction(Action action, float atkFirstDelay, float atkCooldown)
    {
        
        
        //공격 모션에 맞게 판정이 나가도록 선딜레이 적용
        await UniTask.Delay(
            (int)(1000*atkFirstDelay),
            cancellationToken: _enemyBb.cts.Token);
        if (!_enemyBb.bCanPattern) return;
        
        //공격 명령
        action?.Invoke();
        
        //공격 대기시간
        await UniTask.Delay( 
            //공격 대기시간에 모션 선딜레이시간을 빼서 딜레이 맞추기.
            (int)(1000*(atkCooldown - atkFirstDelay)), 
            //공격 도중 죽으면 도중에 취소해야한다.
            cancellationToken: _enemyBb.cts.Token);
    }
}

public static class PatternHandler
{
    private static readonly Dictionary<EnemyType, Func<IAttackPattern>> Patterns =
        new Dictionary<EnemyType, Func<IAttackPattern>>
        {
            {EnemyType.MeleeNormal,() => new MeleeAttack()},
            {EnemyType.MeleeHeavy,() => new OtherAttack()},
            {EnemyType.LongRangeNormal,() => new RangeAttack()},
            {EnemyType.Chapter1Boss,() => new Chapter1Boss()},
            {(EnemyType)3,() => new OtherAttack()}
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
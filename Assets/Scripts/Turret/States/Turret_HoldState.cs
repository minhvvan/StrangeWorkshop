// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
//
// public class Turret_HoldState : BaseState<Turret>
// {
//     public Turret_HoldState(Turret controller) : base(controller){ }
//
//     public override void Enter()
//     {
//
//     }
//
//     public override void UpdateState()
//     {
//         ChangeState();
//     }
//
//     public override void Exit()
//     {
//     }
//     
//     private void ChangeState()
//     {
//         /*
//          state 변경 여부 체크 순서:
//          turret이 counter에 있는가? -> turret이 고장났는가? -> turret이 총알이 없는가? -> 적이 있는가?
//          e.g. turret이 고장난 상태에서 플레이어가 turret을 들면 무조건 holdstate가 된다.
//          */
//         if (_controller.turretData.isOnCounter)
//         {
//             if (_controller.turretData.isCrashed)
//             {
//                 _controller.SetState(_controller.crashState);
//             }
//             else if (_controller.turretData.currentBulletNum <= 0)
//             {
//                 _controller.SetState(_controller.emptyState);
//             }
//             else if (_controller.turretData.target is not null)
//             {
//                 _controller.SetState(_controller.attackState);
//             }
//             else
//             {
//                 _controller.SetState(_controller.idleState);
//             }
//         }
//     }
// }
// using System;
// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
//
// public static class VFXEventManager
// {
//     public static event Action<string, Vector3> OnVFXTriggeredWithPosition;
//     public static event Action<string, Vector3, Quaternion> OnVFXTriggeredWithPosRot;
//     public static event Action<string, Transform> OnVFXTriggeredWithTransform;
//
//     public static void TriggerVFX(string vfxName, Vector3 position)
//     {
//         OnVFXTriggeredWithPosition?.Invoke(vfxName, position);
//     }
//     
//     public static void TriggerVFX(string vfxName, Vector3 position, Quaternion rotation)
//     {
//         OnVFXTriggeredWithPosRot?.Invoke(vfxName, position, rotation);
//     }
//
//     public static void TriggerVFX(string vfxName, Transform transform)
//     {
//         OnVFXTriggeredWithTransform?.Invoke(vfxName, transform);
//     }
// }
//
// public class VFXEventListener : MonoBehaviour
// {
//     private void OnEnable()
//     {
//         VFXEventManager.OnVFXTriggeredWithPosition += PlayVFX;
//         VFXEventManager.OnVFXTriggeredWithPosRot += PlayVFX;
//         VFXEventManager.OnVFXTriggeredWithTransform += PlayVFX;
//     }
//
//     private void OnDisable()
//     {
//         VFXEventManager.OnVFXTriggeredWithPosition -= PlayVFX;
//         VFXEventManager.OnVFXTriggeredWithPosRot -= PlayVFX;
//         VFXEventManager.OnVFXTriggeredWithTransform -= PlayVFX;
//     }
//
//     private void PlayVFX(string vfxName, Vector3 position)
//     {
//         VFXManager.Instance.GetVFX(vfxName, position, Quaternion.identity);
//     }
//
//     private void PlayVFX(string vfxName, Vector3 position, Quaternion rotation)
//     {
//         VFXManager.Instance.GetVFX(vfxName, position, rotation);
//     }
//
//     private void PlayVFX(string vfxName, Transform transform)
//     {
//         VFXManager.Instance.GetVFX(vfxName, transform);
//     }
// }
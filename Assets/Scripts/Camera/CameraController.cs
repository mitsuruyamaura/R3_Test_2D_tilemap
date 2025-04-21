using Unity.Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public CinemachineCamera focusCamera;
    public CinemachineCamera defaultCamera;

    /// <summary>
    /// カーソルの追従
    /// </summary>
    /// <param name="targetTran"></param>
    public void SetDefaultCameraFollow(Transform targetTran) {
        defaultCamera.Follow = targetTran;
    }

    /// <summary>
    /// ユニットの追従
    /// </summary>
    /// <param name="unitTransform"></param>
    public void FocusOnUnit(Transform unitTransform) {
        // フォーカスカメラを選択ユニットに切り替える
        focusCamera.Follow = unitTransform;
        //focusCamera.LookAt = unitTransform;

        // フォーカスカメラの優先度を上げる
        focusCamera.Priority = 10;
        defaultCamera.Priority = 0;
    }

    public void ResetCamera() {
        // デフォルトカメラに戻す
        focusCamera.Priority = 0;
        defaultCamera.Priority = 10;
    }
}

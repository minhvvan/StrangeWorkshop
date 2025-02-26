using UnityEngine;
using UnityEditor;
using System.IO;

public class TransparentSpriteCapture : EditorWindow
{
    private Camera captureCamera; // 캡처할 카메라
    private string savePath = "Assets/Resources/Sprites/CapturedSprite.png"; // 저장 경로
    private int imageSize = 512; // 기본 해상도

    [MenuItem("Tools/Transparent Sprite Capture")]
    public static void ShowWindow()
    {
        GetWindow<TransparentSpriteCapture>("Transparent Sprite Capture");
    }

    private void OnGUI()
    {
        GUILayout.Label("3D Object to Transparent 2D Sprite", EditorStyles.boldLabel);

        // 카메라 선택 필드
        captureCamera = (Camera)EditorGUILayout.ObjectField("Capture Camera", captureCamera, typeof(Camera), true);

        // 저장 경로 입력
        savePath = EditorGUILayout.TextField("Save Path", savePath);

        // 이미지 크기 선택
        imageSize = EditorGUILayout.IntSlider("Image Size", imageSize, 128, 1024);

        if (GUILayout.Button("Capture Transparent Sprite"))
        {
            if (captureCamera != null)
            {
                CaptureTransparent();
            }
            else
            {
                Debug.LogError("Camera is not assigned!");
            }
        }
    }

    private void CaptureTransparent()
    {
        // 기존 카메라 설정 저장
        Color originalBackground = captureCamera.backgroundColor;
        CameraClearFlags originalClearFlags = captureCamera.clearFlags;

        // 투명한 배경 설정
        captureCamera.backgroundColor = new Color(0, 0, 0, 0); // 완전 투명한 색
        captureCamera.clearFlags = CameraClearFlags.SolidColor;

        // 렌더링할 RenderTexture 생성
        RenderTexture rt = new RenderTexture(imageSize, imageSize, 24, RenderTextureFormat.ARGB32);
        captureCamera.targetTexture = rt;
        captureCamera.Render();

        // ReadPixels로 텍스처 데이터를 가져옴
        RenderTexture.active = rt;
        Texture2D tex = new Texture2D(imageSize, imageSize, TextureFormat.RGBA32, false);
        tex.ReadPixels(new Rect(0, 0, imageSize, imageSize), 0, 0);
        tex.Apply();

        // PNG로 저장
        byte[] bytes = tex.EncodeToPNG();
        File.WriteAllBytes(savePath, bytes);
        Debug.Log($"Transparent Sprite saved to {savePath}");

        // 유니티 에셋으로 변환
        AssetDatabase.Refresh();
        TextureImporter importer = AssetImporter.GetAtPath(savePath) as TextureImporter;
        if (importer != null)
        {
            importer.textureType = TextureImporterType.Sprite; // 스프라이트로 설정
            importer.alphaIsTransparency = true; // 알파 채널을 투명도로 사용
            importer.SaveAndReimport();
        }

        // 원래 카메라 설정 복구
        captureCamera.targetTexture = null;
        captureCamera.backgroundColor = originalBackground;
        captureCamera.clearFlags = originalClearFlags;
        RenderTexture.active = null;
        DestroyImmediate(rt);
    }
}
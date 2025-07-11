using UnityEngine;
using System.IO;

public class SaveImageToDesktop : MonoBehaviour
{
    public Texture2D textureToSave; // 저장할 이미지 (Inspector에서 지정)
    // Commit Test
    // test

    void Start()
    {
        // Texture2D uncompressed = CopyTexture(textureToSave);
        // SaveAsJPG(uncompressed, "MySavedImage.jpg");
    }

    Texture2D CopyTexture(Texture2D source)
    {
        // 압축되지 않은 텍스처로 복사 (RGBA32 포맷)
        Texture2D newTex = new Texture2D(source.width, source.height, TextureFormat.RGBA32, false);
        newTex.SetPixels(source.GetPixels());
        newTex.Apply();
        return newTex;
    }

    void SaveAsJPG(Texture2D texture, string fileName)
    {
        // 1. 바탕화면 경로
        string desktopPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop);

        // 2. 바이트 배열로 인코딩 (JPG 형식)
        byte[] jpgBytes = texture.EncodeToJPG();

        // 3. 파일 저장
        string fullPath = Path.Combine(desktopPath, fileName);
        File.WriteAllBytes(fullPath, jpgBytes);

        Debug.Log($"이미지가 저장되었습니다: {fullPath}");
    }
}

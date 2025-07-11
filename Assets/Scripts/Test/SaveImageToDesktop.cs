using UnityEngine;
using System.IO;

public class SaveImageToDesktop : MonoBehaviour
{
    public Texture2D textureToSave; // ������ �̹��� (Inspector���� ����)
    // Commit Test
    // test

    void Start()
    {
        // Texture2D uncompressed = CopyTexture(textureToSave);
        // SaveAsJPG(uncompressed, "MySavedImage.jpg");
    }

    Texture2D CopyTexture(Texture2D source)
    {
        // ������� ���� �ؽ�ó�� ���� (RGBA32 ����)
        Texture2D newTex = new Texture2D(source.width, source.height, TextureFormat.RGBA32, false);
        newTex.SetPixels(source.GetPixels());
        newTex.Apply();
        return newTex;
    }

    void SaveAsJPG(Texture2D texture, string fileName)
    {
        // 1. ����ȭ�� ���
        string desktopPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop);

        // 2. ����Ʈ �迭�� ���ڵ� (JPG ����)
        byte[] jpgBytes = texture.EncodeToJPG();

        // 3. ���� ����
        string fullPath = Path.Combine(desktopPath, fileName);
        File.WriteAllBytes(fullPath, jpgBytes);

        Debug.Log($"�̹����� ����Ǿ����ϴ�: {fullPath}");
    }
}

using UnityEngine;

public class IconCreator : MonoBehaviour
{
    public RenderTexture renderTexture;
    public string fileName = "icon.png";

    [ContextMenu("Save to PNG")]
    public void Save()
    {
        RenderTexture.active = renderTexture;
        Texture2D tex = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);
        tex.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        tex.Apply();
        byte[] bytes = tex.EncodeToPNG();
        System.IO.File.WriteAllBytes(Application.dataPath + "/" + fileName, bytes);
        Debug.Log("Saved to: " + Application.dataPath + "/" + fileName);
    }
}
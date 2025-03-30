using UnityEngine;

public class PlayerColorManager : MonoBehaviour
{
    private Renderer[] renderers;
    private Color[] originalColors;
    private void GetRendererColors(){
        // 子オブジェクト内のすべての Renderer を取得する
        renderers = GetComponentsInChildren<Renderer>();
        // 各Rendererの元の色を保存するための配列
        originalColors = new Color[renderers.Length];

        for (int i = 0; i < renderers.Length; i++)
        {
            // 元の色を保存する
            originalColors[i] = renderers[i].material.color;
            Debug.Log("original color: " + i + originalColors[i]);

            // URP Litの場合、_Surfaceプロパティを1に設定するとTransparentモードになる
            renderers[i].material.SetFloat("_Surface", 1);
            Debug.Log("renderer: " + i + renderers[i].material.name);
        }
    }
    private void resetRendererColors(){
        renderers = null;
        originalColors = null;
    }
    // 妨害時
    public void ChangeColorRed()
    {
        GetRendererColors();
        for (int i = 0; i < renderers.Length; i++)
        {
            // まず色を赤にして不透明（alpha=1）に設定する
            Color newColor = Color.red;
            newColor.a = 1f;
            renderers[i].material.color = newColor;
        }
    }
    public void ChangeColorOriginal()
    {
        for (int i = 0; i < renderers.Length; i++)
        {
            // 元の色に戻す
            renderers[i].material.color = originalColors[i];
        }
        resetRendererColors();
    }
    // 死んだ時
    public void ChangeColorBlack()
    {
        GetRendererColors();
        for (int i = 0; i < renderers.Length; i++)
        {
            // 色を黒にして不透明（alpha=1）に設定する
            Color newColor = Color.black;
            newColor.a = 1f;
            renderers[i].material.color = newColor;
        }
    }
}

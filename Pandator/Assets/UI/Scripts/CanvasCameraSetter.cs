using UnityEngine;

public class CanvasCameraSetter : MonoBehaviour
{
    public static CanvasCameraSetter Instance;
    private void Start()
    {
        
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetCanvasCamera()
    {
        // Canvasコンポーネントを取得
        Canvas canvas = GetComponent<Canvas>();
        if (canvas != null)
        {
            // "MainCamera"というタグが付いたカメラを探す
            GameObject cameraObject = GameObject.FindWithTag("MainCamera");
            if (cameraObject != null)
            {
                Camera camera = cameraObject.GetComponent<Camera>();
                if (camera != null)
                {
                    // CanvasのRender Cameraを設定
                    canvas.worldCamera = camera;
                }
                else
                {
                    Debug.LogError("指定されたオブジェクトにCameraコンポーネントが見つかりません。");
                }
            }
            else
            {
                Debug.LogError("タグ 'MainCamera' を持つオブジェクトが見つかりません。");
            }
        }
        else
        {
            Debug.LogError("Canvasコンポーネントがアタッチされていません。");
        }
    }

    public void SetCanvasSortingLayer()
    {
        // Canvasコンポーネントを取得
        Canvas canvas = GetComponent<Canvas>();
        if (canvas != null)
        {
            // 指定されたSortingLayerを設定
            canvas.sortingLayerName = "FrontUI";
            canvas.planeDistance = 1f;
            canvas.sortingOrder = 1;
        }
        else
        {
            Debug.LogError("Canvasコンポーネントがアタッチされていません。");
        }
    }
}
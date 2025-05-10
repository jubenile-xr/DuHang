using System;
using UnityEngine;
using UnityEngine.UI; // UIコンポーネントを使用するために必要

public class KilledImagedAttach : MonoBehaviour
{
    [SerializeField] private GameObject FirstKilledImage; // インスペクターでUI画像を指定
    [SerializeField] private GameObject SecondKilledImage; // インスペクターでUI画像を指定
    [SerializeField] private GameObject ThirdKilledImage; // インスペクターでUI画像を指定

    private bool isFirstCharacterSet = false;
    private bool isSecondCharacterSet = false;
    private bool isThirdCharacterSet = false;
    private bool isFirstPlayerDead = false;
    private bool isSecondPlayerDead = false;
    private bool isThirdPlayerDead = false;
    private static string firstCharacter = null;
    private static string secondCharacter = null;
    private static string thirdCharacter = null;
    [SerializeField] private GameObject canvas; // インスペクターでCanvasを指定
    [SerializeField] private Texture2D RabbitImage;
    [SerializeField] private Texture2D BirdImage;
    [SerializeField] private Texture2D MouseImage;
    GameObject FirstImageObject;
    GameObject SecondImageObject;
    GameObject ThirdImageObject;
    RawImage FirstImage;
    RawImage SecondImage;
    RawImage ThirdImage;

    private int characterNum = 0;
    [SerializeField] private GameObject FrameImage;

    private static bool isFirstCharacterIsMe = false;
    private static bool isSecondCharacterIsMe = false;
    private static bool isThirdCharacterIsMe = false;


    private void Start()
    {
        FirstImageObject = new GameObject("FirstAnimalImage");
        FirstImage = FirstImageObject.AddComponent<RawImage>();
        SecondImageObject = new GameObject("SecondAnimalImage");
        SecondImage = SecondImageObject.AddComponent<RawImage>();
        ThirdImageObject = new GameObject("ThirdAnimalImage");
        ThirdImage = ThirdImageObject.AddComponent<RawImage>();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            if (RabbitImage != null)
            {
                // テクスチャをスプライトに変換して設定
                FirstImage.texture = RabbitImage;
                setFirstPosition();
            }
        }

        if (!isFirstCharacterSet && firstCharacter != null)
        {
            changeCharacterNameToImage(firstCharacter);
            isFirstCharacterSet = true;
        }

        if (!isSecondCharacterSet && secondCharacter != null)
        {
            changeCharacterNameToImage(secondCharacter);
            isSecondCharacterSet = true;
        }

        if (!isThirdCharacterSet && thirdCharacter != null)
        {
            changeCharacterNameToImage(thirdCharacter);
            isThirdCharacterSet = true;
        }
    }

    //下のコードなんやけど多分これであってると思う→長張へ
    //これでテキスチャーを決定する感じ
    private void changeCharacterNameToImage(string characterName)
    {
        RawImage img;
        characterNum++;
        switch (characterNum)
        {
            case 1:
                img = FirstImage;
                break;
            case 2:
                img = SecondImage;
                break;
            case 3:
                img = ThirdImage;
                break;
            default:
                return;
        }

        if (characterName.Contains("MOUSE"))
        {
            img.texture = MouseImage;
        }
        else if (characterName.Contains("BIRD"))
        {
            img.texture = BirdImage;
        }
        else if (characterName.Contains("RABBIT"))
        {
            img.texture = RabbitImage;
        }

        switch (characterNum)
        {
            case 1:
                setFirstPosition();
                break;
            case 2:
                setSecondPosition();
                break;
            case 3:
                setThirdPosition();
                break;
            default:
                return;
        }
    }

    //以下は画像をセットしてくれるものです。
    public void setFirstPosition()
    {
        // Canvasに追加
        FirstImageObject.transform.SetParent(canvas.transform, false);

        // RectTransformの設定
        RectTransform setimage = FirstImageObject.GetComponent<RectTransform>();
        setimage.sizeDelta = new Vector2(100f, 100f); // サイズ
        setimage.anchoredPosition = new Vector2(350f, 200f); // 位置
        // FrameImageの設定
        if (isFirstCharacterIsMe)
        {
            FrameImage.SetActive(true);
            RectTransform setFrame = FrameImage.GetComponent<RectTransform>();
            setFrame.anchoredPosition = new Vector2(350f, 200f); // 位置
        }
    }
    public void setSecondPosition()
    {
        // Canvasに追加
        SecondImageObject.transform.SetParent(canvas.transform, false);

        // RectTransformの設定
        RectTransform setimage = SecondImageObject.GetComponent<RectTransform>();
        setimage.sizeDelta = new Vector2(100f, 100f); // サイズ
        setimage.anchoredPosition = new Vector2(210f, 200f); // 位置
        // FrameImageの設定
        if (isSecondCharacterIsMe)
        {
            FrameImage.SetActive(true);
            RectTransform setFrame = FrameImage.GetComponent<RectTransform>();
            setFrame.anchoredPosition = new Vector2(210f, 200f); // 位置
        }
    }
    public void setThirdPosition()
    {
        // Canvasに追加
        ThirdImageObject.transform.SetParent(canvas.transform, false);

        // RectTransformの設定
        RectTransform setimage = ThirdImageObject.GetComponent<RectTransform>();
        setimage.sizeDelta = new Vector2(100f, 100f); // サイズ
        setimage.anchoredPosition = new Vector2(70f, 200f); // 位置
        // FrameImageの設定
        if (isThirdCharacterIsMe)
        {
            FrameImage.SetActive(true);
            RectTransform setFrame = FrameImage.GetComponent<RectTransform>();
            setFrame.anchoredPosition = new Vector2(70f, 200f); // 位置
        }
    }

    public static void SetFirstCharacter(string character, bool isMine)
    {
        firstCharacter = character;
        isFirstCharacterIsMe = isMine;
    }
    public static void SetSecondCharacter(string character, bool isMine)
    {
        secondCharacter = character;
        isSecondCharacterIsMe = isMine;
    }
    public static void SetThirdCharacter(string character, bool isMine)
    {
        thirdCharacter = character;
        isThirdCharacterIsMe = isMine;
    }

    public void SetFirstPlayerDead()
    {
        FirstKilledImage.SetActive(true);
    }
    public void SetSecondPlayerDead()
    {
        SecondKilledImage.SetActive(true);
    }
    public void SetThirdPlayerDead()
    {
        ThirdKilledImage.SetActive(true);
    }
}


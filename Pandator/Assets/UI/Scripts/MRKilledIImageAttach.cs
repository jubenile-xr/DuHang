using UnityEngine;
using UnityEngine.UI; // UIコンポーネントを使用するために必要

public class MRKilleImagedAttach : MonoBehaviour
{
    [SerializeField] private GameObject FirstKilledImage; // インスペクターでUI画像を指定
    [SerializeField] private GameObject SecondKilledImage; // インスペクターでUI画像を指定
    [SerializeField] private GameObject ThirdKilledImage; // インスペクターでUI画像を指定

    private bool isFirstPlayerDead = false;
    private bool isSecondPlayerDead = false;
    private bool isThirdPlayerDead = false;
    private InitializeManager.GameCharacter firstCharacter;
    private InitializeManager.GameCharacter secondCharacter;
    private InitializeManager.GameCharacter thirdCharacter;
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
        if (Input.GetKeyDown(KeyCode.I))
        {
            isFirstPlayerDead = true;
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            isSecondPlayerDead = true;
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            isThirdPlayerDead = true;
        }
        FirstKilledImage.SetActive(isFirstPlayerDead);
        ThirdKilledImage.SetActive(isSecondPlayerDead);
        SecondKilledImage.SetActive(isThirdPlayerDead);

        if (Input.GetKeyDown(KeyCode.L))
        {
            if (RabbitImage != null)
            {
                // テクスチャをスプライトに変換して設定
                FirstImage.texture = RabbitImage;
                setFirstPosition();
            }
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            changeCharacterNameToImage(InitializeManager.GameCharacter.RABBIT);
        }
    }

    public void setIsFirstPlayerDead(bool isDead)
    {
        isFirstPlayerDead = isDead;
    }
    public void setIsSecondPlayerDead(bool isDead)
    {
        isSecondPlayerDead = isDead;
    }
    public void setIsThirdPlayerDead(bool isDead)
    {
        isThirdPlayerDead = isDead;
    }

    public void setFirstCharacterName(InitializeManager.GameCharacter characterName)
    {
        firstCharacter = characterName;
    }

    public void setSecondCharacterName(InitializeManager.GameCharacter characterName)
    {
        secondCharacter = characterName;
    }
    public void setThirdCharacterName(InitializeManager.GameCharacter characterName)
    {
        thirdCharacter = characterName;
    }

    //下のコードなんやけど多分これであってると思う→長張へ
    //これでテキスチャーを決定する感じ
    private void changeCharacterNameToImage(InitializeManager.GameCharacter characterName)
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
        switch (characterName)
        {
            case InitializeManager.GameCharacter.RABBIT:
                img.texture = RabbitImage;
                break;
            case InitializeManager.GameCharacter.BIRD:
                img.texture = BirdImage;
                break;
            case InitializeManager.GameCharacter.MOUSE:
                img.texture = MouseImage;
                break;
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
    private void setFirstPosition()
    {
        // Canvasに追加
        FirstImageObject.transform.SetParent(canvas.transform, false);

        // RectTransformの設定
        RectTransform setimage = FirstImageObject.GetComponent<RectTransform>();
        setimage.sizeDelta = new Vector2(100f, 100f); // サイズ
        setimage.anchoredPosition = new Vector2(350f, 200f); // 位置
    }
    private void setSecondPosition()
    {
        // Canvasに追加
        SecondImageObject.transform.SetParent(canvas.transform, false);

        // RectTransformの設定
        RectTransform setimage = SecondImageObject.GetComponent<RectTransform>();
        setimage.sizeDelta = new Vector2(100f, 100f); // サイズ
        setimage.anchoredPosition = new Vector2(210f, 200f); // 位置
    }
    private void setThirdPosition()
    {
        // Canvasに追加
        ThirdImageObject.transform.SetParent(canvas.transform, false);

        // RectTransformの設定
        RectTransform setimage = ThirdImageObject.GetComponent<RectTransform>();
        setimage.sizeDelta = new Vector2(100f, 100f); // サイズ
        setimage.anchoredPosition = new Vector2(70f, 200f); // 位置
    }
}

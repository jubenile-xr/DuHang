using UnityEngine;
using UnityEngine.UI; // UIコンポーネントを使用するために必要
using TMPro; // TextMeshProを使うのに必要

public class VRKilleImagedAttach : MonoBehaviour
{
    [SerializeField] private GameObject RabbitImage; // インスペクターでUI画像を指定
    [SerializeField] private GameObject BirdImage; // インスペクターでUI画像を指定
    [SerializeField] private GameObject MouseImage; // インスペクターでUI画像を指定
    [SerializeField] private GameObject Rabbit; // インスペクターでUI画像を指定
    [SerializeField] private GameObject Bird; // インスペクターでUI画像を指定
    [SerializeField] private GameObject Mouse; // インスペクターでUI画像を指定
    [SerializeField] private TextMeshProUGUI hitText;//選択キャラのヒットカウントを数える

    [SerializeField] private GameObject initialize;//自身のキャラクタ―情報持ってくるやつ

    private InitializeManager.GameCharacter character;
    private bool PlayerAlive = true;

    void Start()
    {
        InitializeManager initmanager = initialize.GetComponent<InitializeManager>();
        character = initmanager.GetGameCharacter(); // フィールドに値を代入

        // 初期設定
        switch (character)
        {
            case InitializeManager.GameCharacter.RABBIT:
                SwapPositions(Rabbit, Bird);
                SwapPositions(RabbitImage, BirdImage);
                Rabbit.SetActive(false);
                break;
            case InitializeManager.GameCharacter.BIRD:
                Bird.SetActive(false);
                break;
            case InitializeManager.GameCharacter.MOUSE:
                SwapPositions(Mouse, Bird);
                SwapPositions(MouseImage, BirdImage);
                Mouse.SetActive(false);
                break;
        }

        // 最初は全ての画像を非表示
        RabbitImage.SetActive(false);
        BirdImage.SetActive(false);
        MouseImage.SetActive(false);
        hitText.gameObject.SetActive(false);
    }

    void Update()
    {
        // 自身のキャラクターが死んだときに、全部を消して、ヒットを出す
        switch (character)
        {
            case InitializeManager.GameCharacter.RABBIT when RabbitKill():
                BirdImage.SetActive(false);
                Bird.SetActive(false);
                MouseImage.SetActive(false);
                Mouse.SetActive(false);
                hitText.gameObject.SetActive(true);
                PlayerAlive = false;
                break;
            case InitializeManager.GameCharacter.BIRD when BirdKill():
                RabbitImage.SetActive(false);
                Rabbit.SetActive(false);
                MouseImage.SetActive(false);
                Mouse.SetActive(false);
                hitText.gameObject.SetActive(true);
                PlayerAlive = false;
                break;
            case InitializeManager.GameCharacter.MOUSE when MouseKill():
                RabbitImage.SetActive(false);
                Rabbit.SetActive(false);
                BirdImage.SetActive(false);
                Bird.SetActive(false);
                hitText.gameObject.SetActive(true);
                PlayerAlive = false;
                break;
        }

        if (PlayerAlive)//プレイヤ―が死んだ時には実行しない
        {
            // 画像の表示条件（非選択キャラが死んだとき）
            if (RabbitKill() && character != InitializeManager.GameCharacter.RABBIT)
            {
                RabbitImage.SetActive(true); // UI画像を表示
            }
            else if (MouseKill() && character != InitializeManager.GameCharacter.MOUSE)
            {
                MouseImage.SetActive(true); // UI画像を表示
            }
            else if (BirdKill() && character != InitializeManager.GameCharacter.BIRD)
            {
                BirdImage.SetActive(true); // UI画像を表示
            }
        }
    }


    bool RabbitKill()
    {
        return Input.GetKeyDown(KeyCode.U);
    }

    bool BirdKill()
    {
        return Input.GetKeyDown(KeyCode.B);
    }

    bool MouseKill()
    {
        return Input.GetKeyDown(KeyCode.M);
    }

    void SwapPositions(GameObject obj1, GameObject obj2)
    {
        // Transformを使用して位置を交換
        Transform transform1 = obj1.transform;
        Transform transform2 = obj2.transform;

        if (transform1 != null && transform2 != null)
        {
            Vector3 tempPosition = transform1.position; // ワールド空間の位置を取得
            transform1.position = transform2.position; // obj2の位置をobj1に設定
            transform2.position = tempPosition; // obj1の位置をobj2に設定
        }
    }
}
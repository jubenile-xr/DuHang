using UnityEngine;
using UnityEngine.UI; // UIコンポーネントを使用するために必要

public class MRKilleImagedAttach : MonoBehaviour
{
    [SerializeField] private GameObject RabbitImage; // インスペクターでUI画像を指定
    [SerializeField] private GameObject BirdImage; // インスペクターでUI画像を指定
    [SerializeField] private GameObject MouseImage; // インスペクターでUI画像を指定

    void Update()
    {
        if (RabbitKill())
        {
            if (RabbitImage != null)
            {
                RabbitImage.SetActive(true); // UI画像を表示
            }
        }
        else if (MouseKill())
        {
            if (MouseImage != null)
            {
                MouseImage.SetActive(true); // UI画像を表示
            }
        }
        else if (BirdKill())
        {
            if (BirdImage != null)
            {
                BirdImage.SetActive(true); // UI画像を表示
            }
        }
    }

    //仮に置いているだけなので、ステートマネージャー次第で消しといて
    bool RabbitKill()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            return true;
        }
        return false;
    }

    bool BirdKill()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            return true;
        }
        return false;
    }
    bool MouseKill()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            return true;
        }
        return false;
    }
}

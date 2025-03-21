using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ImageSpawner : MonoBehaviour
{
    public GameObject AmmoImage;
    public Transform parentPanel;
    public int imageCount = 8; 

    private List<GameObject> images = new List<GameObject>();

    void Start()
    {
        SpawnImages(imageCount);
    }

    public void SpawnImages(int count)
    {
        // CURRENT IMAGE DELETE
        foreach (var img in images)
        {
            Destroy(img);
        }
        images.Clear();

        // NEW IMAGE SPAWN
        for (int i = 0; i < count; i++)
        {
            GameObject newImage = Instantiate(AmmoImage, parentPanel);
            images.Add(newImage);
        }
    }

    public void SetImageCount(int newCount)
    {
        imageCount = newCount;
        SpawnImages(imageCount);
    }
}

using UnityEngine;

public class Mouse : SmallAnimal
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        base.Start();
        name = "Mouse";
        moveSpeed = 5.0f;
    }

    // Update is called once per frame
    protected void Update()
    {
        base.Update();
    }
}

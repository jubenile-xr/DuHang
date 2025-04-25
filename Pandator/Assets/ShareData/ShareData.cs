using UnityEngine;

public static class ShareData
{
    private static string winner = "SMALLANIMAL";
    
    public static string GetWinner()
    {
        return winner;
    }
    
    public static void SetWinner(string animal)
    {
        winner = animal;
    }
}

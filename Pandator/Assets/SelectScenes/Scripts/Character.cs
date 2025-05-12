public static class Character
{
    // キャラクターの種類を列挙型で定義
    public enum GameCharacters
    {
        BIRD,
        RABBIT,
        MOUSE,
        PANDA,
        GOD
    }
    private static GameCharacters selectedAnimal = GameCharacters.GOD; // デフォルトはgod
    public static GameCharacters GetSelectedAnimal()
    {
        return selectedAnimal;
    }
    public static void SetSelectedAnimal(GameCharacters animal)
    {
        selectedAnimal = animal;
    }

    private static string myName = "initial";
    public static string GetMyName()
    {
        return myName;
    }
    public static void SetMyName(string name)
    {
        myName = name;
    }
}
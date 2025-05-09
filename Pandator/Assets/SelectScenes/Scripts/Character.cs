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
    private static string myName = "";
    public static GameCharacters GetSelectedAnimal()
    {
        return selectedAnimal;
    }
    public static void SetSelectedAnimal(GameCharacters animal)
    {
        selectedAnimal = animal;
    }
    public static void SetMyName(string name) => myName = name;
    public static string GetMyName() => myName;
}
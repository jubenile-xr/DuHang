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
    private static GameCharacters selectedAnimal = GameCharacters.BIRD; // デフォルトはgod

    public static GameCharacters GetSelectedAnimal()
    {
        return selectedAnimal;
    }
    public static void SetSelectedAnimal(GameCharacters animal)
    {
        selectedAnimal = animal;
    }
}
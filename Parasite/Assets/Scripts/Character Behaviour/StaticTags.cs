
// Crear strings en C# es costoso, por lo que es mejor cachearlos en una clase estática. Tambien podria hacer con un scriptable
// Que se pueda editar desde el inspector pero por cuestiones de tiempo he optado por esta alternativa
static class StaticTags
{
    public const string PlayerTag = "Player";
    public const string EnemyTag = "Enemy";
    public const string AttackTag = "AttackTag";
    public const string MapTag = "MapTag";
    public const string InvisibleBounds = "InvisibleBounds";
    public const string MainCameraTag = "MainCamera";
}
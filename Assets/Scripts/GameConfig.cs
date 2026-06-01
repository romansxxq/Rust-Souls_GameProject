using UnityEngine;

[CreateAssetMenu(fileName = "GameConfig", menuName = "Game/GameConfig")]
public class GameConfig : ScriptableObject
{
    [Header("Гравець")]
    public float moveSpeed = 5f;
    public float dashSpeed = 10f;
    public float jumpForce = 15f;
    public int maxHealth = 100;

    [Header("Система життів")]
    public int startingLives = 3;

    [Header("Рівень")]
    public int levelNumber = 1;
    public string nextSceneName = "";
    public string menuSceneName = "MainMenu";

    [Header("Ворог")]
    public float enemySpeed = 3f;
    public int enemyDamage = 10;

    [Header("Умова перемоги")]
    public bool killAllEnemies = true;
    public int rustToWin = 0;  // 0 = не потрібно
    public int soulsToWin = 0; // 0 = не потрібно
}

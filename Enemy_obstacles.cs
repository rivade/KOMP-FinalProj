using System;
using Raylib_cs;
using System.Numerics;

public class Obstacle
{
    public Obstacle nextObstacle;
    public List<Rectangle> spikes = new();
    public List<Rectangle> enemies = new();
    private int enemyDirection = 1;
    private int enemySpeed = 2;
    private int enemyDistance = 0;
    private int enemyMaxDistance = 300;
    public Texture2D spikeTexture = Raylib.LoadTexture("Textures/spike.png");
    public Texture2D enemyTexture = Raylib.LoadTexture("Sprites/enemy.png");
    public Obstacle(Vector2[] spike, Vector2[] enemy) //Vector2 används för att ange spikens och fiendernas koordinater
    {
        for (var i = 0; i < spike.Length; i++)
        {
            spikes.Add(new Rectangle((spike[i].X + 5), spike[i].Y, 40, 65)); //Skapar en spike rektangel för varje vector2 man skapar i program.cs när man skapar klassinstansen (lägger in dem i listan spikes också)
        }
        for (var i = 0; i < enemy.Length; i++)
        {
            enemies.Add(new Rectangle(enemy[i].X, enemy[i].Y, 96, 69)); //Samma som spikes
        }
    }

    public void DrawObstacles()
    {
        foreach (var spike in spikes)
        {
            Raylib.DrawTexture(spikeTexture, ((int)spike.x - 5), (int)spike.y, Color.WHITE); //Ritar ut spike texturen för varje spike i listan spikes
        }
        foreach (var enemy in enemies)
        {
            Raylib.DrawTexture(enemyTexture, (int)enemy.x, (int)enemy.y, Color.WHITE); //Samma som spikes
        }
    }

    public void MoveEnemy()
    {
        for (int i = 0; i < enemies.Count; i++)
        {
            Rectangle enemy = enemies[i];
            enemy.x += enemyDirection * enemySpeed;
            enemyDistance += enemySpeed;
            if (enemyDistance == enemyMaxDistance)
            {
                enemyDirection *= -1;
                enemyDistance = 0;
            }
            enemies[i] = enemy;
        }
    }
}
using System;
using Raylib_cs;
using System.Numerics;

public class Global //Globala variabler som ska kunna användas i flera klasser utan att skapa en instans av klassen
{

    public static Level currentLevel;
    public static string currentscene = "start";
    public const int screenwidth = 1024;
    public const int screenheight = 768;
    public static string levelDied = "";
    public static Music music = Raylib.LoadMusicStream("Sounds/bgmusic.mp3");
    public static Sound jumpSound = Raylib.LoadSound("Sounds/jump.mp3");
    public static Sound deathSound = Raylib.LoadSound("Sounds/death.mp3");
    public static Sound coinSound = Raylib.LoadSound("Sounds/coin.mp3");
    public static Sound winSound = Raylib.LoadSound("Sounds/win.mp3");
    public static Sound cheer = Raylib.LoadSound("Sounds/cheer.mp3");
    public static Texture2D invCoinTexture = Raylib.LoadTexture("Textures/invcoin.png");
    public static Texture2D infoSign = Raylib.LoadTexture("Textures/infosign.png");
    public static Texture2D deathScreen = Raylib.LoadTexture("Textures/deathscreen.png");
    public static Texture2D winScreen = Raylib.LoadTexture("Textures/winscreen.png");
    public static void SoundInitialization()
    {
        Raylib.SetSoundVolume(jumpSound, 0.5f);
        Raylib.SetSoundVolume(coinSound, 0.5f);
        Raylib.SetSoundVolume(cheer, 0.6f);
    }
    public static void RespawnLogic() //Avgör var man ska respawna ifall man dör
    {
        if (currentscene != "death" && currentscene != "start" && currentscene != "win")
        {
            levelDied = currentscene;
        }
    }
    public static void DrawWin(Player p)
    {
        Raylib.DrawTexture(Global.winScreen, 0, 0, Color.WHITE);
        Raylib.DrawText("Congratulations, you won!", 450, 100, 30, Color.BLACK);
        Raylib.DrawText($"You finished with {p.hearts} lives remaining", 450, 150, 30, Color.BLACK);
        Raylib.DrawText($"You collected {p.coins} coins.", 450, 200, 30, Color.BLACK);
        if (p.coins == 25)
        {
            Raylib.DrawText("Thats all of them, good job!", 450, 250, 30, Color.BLACK);
        }
        Raylib.DrawText("Press enter to restart", 425, 450, 40, Color.BLACK);
        Raylib.DrawText("Press ESC to exit", 425, 500, 40, Color.BLACK);
    }
}

public class UI //Klass för startskärm som man interagerar med
{
    public Rectangle button = new((Global.screenwidth / 2) - 100, (Global.screenheight / 2) + 50, 200, 75);
    public Color buttoncolor = new(91, 14, 179, 255);
    public Texture2D startBG = Raylib.LoadTexture("Textures/titlescreen.png");
    public bool begin = false;
    public void StartButton(string level, Level startMenu) //Gör så att man kommer till level one när man trycker på knappen
    {
        Vector2 mouse = Raylib.GetMousePosition(); //Skapar en vektor med musens position
        if (Raylib.CheckCollisionPointRec(mouse, button))
        {
            buttoncolor.r = (byte)29; buttoncolor.g = (byte)1; buttoncolor.b = (byte)61; // Gör så att knappens färg blir mörkare när man hoverar med musen på knappen.
            if (Raylib.IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT))
            {
                begin = true;
            }
        }
        else
        {
            buttoncolor.r = (byte)91; buttoncolor.g = (byte)14; buttoncolor.b = (byte)179; //Återställer knappfärgen när musen slutar hovera på knappen
        }
        if (begin)
        {
            startMenu.NextLevel(level, true);
            Raylib.PlayMusicStream(Global.music);
        }
    }
}

public class Reset
{
    public static void ResetGame(Player p, Level levelOne, Level levelTwo, Level levelThree, Level startMenu, UI u)
    {
        if (Raylib.IsKeyPressed(KeyboardKey.KEY_ENTER))
        {
            Raylib.StopMusicStream(Global.music);
            Global.currentscene = "start";
            u.begin = false;
            startMenu.alpha.a = 0;
            p.rect.x = 0;
            p.rect.y = 526;
            p.hearts = 3;
            p.coins = 0;
            p.verticalVelocity = 0;
            levelOne.wonLevel = false;
            levelTwo.wonLevel = false;
            levelThree.wonLevel = false;
            levelOne.ResetCoins();
            levelTwo.ResetCoins();
            levelThree.ResetCoins();
            levelOne.alpha.a = 254;
        }
    }
}
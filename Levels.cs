using System;
using Raylib_cs;
using System.Numerics;

public static class Death
{
    public static int deathTimer = 180;
    public static void DeathHandler(Player p, Level levelOne, Level levelTwo, Level levelThree, Level startMenu, UI u)
    {
        if (p.hearts > 0)
        {
            if (deathTimer > 0)
            {
                deathTimer--;
            }
            if (deathTimer == 0)
            {
                p.rect.x = 0;
                p.rect.y = 526;
                p.verticalVelocity = 0f;
                Global.currentscene = Global.levelDied;
                deathTimer = 180;
                Raylib.ResumeMusicStream(Global.music);
            }
        }
        else
        {
            Reset.ResetGame(p, levelOne, levelTwo, levelThree, startMenu, u);
        }
    }
    public static void Draw(Player p)
    {
        Raylib.DrawTexture(Global.deathScreen, 0, 0, Color.WHITE);
        if (p.hearts > 0)
        {
            Raylib.DrawText("Tip: Try to avoid spikes, slimes, or falling down!", 20, 50, 40, Color.WHITE);
            Raylib.DrawText("You died!", 670, 400, 40, Color.WHITE);
            Raylib.DrawText($"Respawning in: {Death.deathTimer / 60 + 1}", 600, 500, 40, Color.WHITE);
        }
        else
        {
            Raylib.DrawText("You lost!", 670, 400, 40, Color.WHITE);
            Raylib.DrawText("Press enter to restart!", 600, 500, 30, Color.WHITE);
        }
    }
}

public class Level
{

    public Level nextLevel; //Används för att den ska veta vilken level som är näst (Defineras när instans av level skapas)
    public Player p; //Samma som i kameraklassen
    public Color alpha = new(0, 0, 0, 0); //Används för svart skärm som fadear vid levelbyte

    public List<Rectangle> floors = new();
    public List<Texture2D> backgrounds = new();
    public List<Texture2D> floorTextures = new();
    public List<Texture2D> assetTextures = new();
    public List<Rectangle> coins = new();
    public Rectangle gate;
    public Texture2D coinTexture = Raylib.LoadTexture("Textures/coin.png");
    public int frame = 1;
    public float elapsed = 0;
    public bool wonLevel = false;
    private Vector2[] coinpositions;
    public Level(int count, int gateX, bool resetAlpha, Player pExtern, Vector2[] coinpos)
    {
        Rectangle gatecreator = new(gateX, 490, 110, 110); //Skapar en gate rektangel där man anger vart gaten ska ligga (konstant y, men x bestäms när instansen av klassen skapas)
        for (var i = 0; i < count; i++)
        {
            floors.Add(new Rectangle(i * 1024, 600, Global.screenwidth, 1)); //Lägger till en golvrektangel för antal golv man anger när man skapar instans
        }
        for (var i = 0; i < coinpos.Length; i++)
        {
            coins.Add(new Rectangle(coinpos[i].X, coinpos[i].Y, 30, 30)); //Lägger till en coinrektangel med position man anger för varje coin man vill skapa
        }
        backgrounds.Add(Raylib.LoadTexture("Textures/titlescreen.png"));
        backgrounds.Add(Raylib.LoadTexture("Textures/Wall.png"));
        backgrounds.Add(Raylib.LoadTexture("Textures/black.png"));
        floorTextures.Add(Raylib.LoadTexture("Textures/floor.png"));
        assetTextures.Add(Raylib.LoadTexture("Textures/gate.png"));
        gate = gatecreator; //Gör så att klassens gate rektangel får de värden man anger
        p = pExtern;
        coinpositions = coinpos;
        if (resetAlpha) //Gör så att den återställer alphavärdet på svarta skärmen ifall banan har bytts.
        {
            alpha.a = 254;
            resetAlpha = false;
        }
    }
    public void DrawTextures() //Ritar ut bakgrunder och golv
    {
        for (var i = 0; i < 20; i++)
        {
            Raylib.DrawTexture(backgrounds[1], (i * 384), 0, Color.WHITE);
            Raylib.DrawTexture(backgrounds[1], (i * 384), 384, Color.WHITE);
        }
        for (var i = 0; i < floors.Count; i++)
        {
            Raylib.DrawTexture(floorTextures[0], (int)floors[i].x, (int)floors[i].y, Color.WHITE);
        }
        Raylib.DrawTexture(assetTextures[0], (int)gate.x, (int)gate.y, Color.WHITE);
    }

    public void NextLevel(string next, bool instantNext) //Ändrar alpha på svarta skärmen och gör så att man går vidare till nästa level när man trycker på enter vid gaten
    {
        if (Raylib.CheckCollisionRecs(p.rect, gate) && Raylib.IsKeyPressed(KeyboardKey.KEY_ENTER))
        {
            Raylib.PlaySound(Global.winSound);
            Raylib.PauseMusicStream(Global.music);
            wonLevel = true;
        }
        if (wonLevel || instantNext)
        {
            if (alpha.a < 255)
            {
                alpha.a += 2;
            }
            if (alpha.a == 254)
            {
                p.rect.x = 0;
                p.rect.y = 526;
                Raylib.ResumeMusicStream(Global.music);
                Global.currentscene = next;
                if (next == "win")
                {
                    Raylib.PlaySound(Global.cheer);
                }
            }
        }
    }
    public void AlphaReset() //Återställer sakta alphavärdet till 0 på svarta skärmen vilket gör den osynlig igen
    {
        if (alpha.a > 0 && !wonLevel)
        {
            alpha.a -= 2;
        }
    }

    public void CoinCollection() //Kollar ifall spelaren plockar upp en coin
    {
        for (var i = 0; i < coins.Count; i++)
        {
            if (Raylib.CheckCollisionRecs(p.rect, coins[i]))
            {
                p.coins++; //Lägger till en coin till hur många spelaren plockat upp
                coins.Remove(coins[i]); //Tar bort coinen från leveln så att man ej kan plocka upp samma coin flera gånger
                Raylib.PlaySound(Global.coinSound);
            }
        }
    }

    public int CoinFrameLogic(ref int frame, ref float elapsed) //Samma som när spelarens running animation ska ritas bara med längre frame duration
    {
        const float frameDuration = 0.15f;
        elapsed += Raylib.GetFrameTime();
        if (elapsed >= frameDuration)
        {
            frame++;
            elapsed -= frameDuration;
        }
        frame %= 8;
        return frame;
    }

    public void DrawCoin(ref int frame, ref float timer)
    {
        frame = CoinFrameLogic(ref frame, ref timer);
        Rectangle sourcerec = new Rectangle(30 * frame, 0, 30, 30);
        foreach (var coin in coins)
        {
            Raylib.DrawTextureRec(coinTexture, sourcerec, new Vector2(coin.x, coin.y), Color.WHITE);
        }
    }

    public void ResetCoins()
    {
        coins.Clear();
        for (var i = 0; i < coinpositions.Length; i++)
        {
            coins.Add(new Rectangle(coinpositions[i].X, coinpositions[i].Y, 30, 30)); //Lägger tillbaka coinsen i leveln när man restartat spelet
        }
    }

    public void LogicHandler(Camera c, Player p, Obstacle currentObstacle, Level currentLevel, string next)
    {
        AlphaReset();
        c.CameraBounds();
        if (!wonLevel && (alpha.a < 20)) { p.lastleft = p.Movement(p.lastleft, currentLevel); } //Gör så att man kan röra sig ifall man inte klarat leveln och skärmen fadeat tillbaka från svart
        p.DeathCheck(currentObstacle); //Kollar ifall spelaren dött
        CoinCollection(); //Kollar ifall spelaren plockar upp en coin
        currentObstacle.MoveEnemy();
        NextLevel(next, false); //Definerar vilken level som currentscene ska uppdateras till när man vinner
    }

    public virtual void Draw(Camera camera, Player player, Obstacle obstacle)
    {
    }
    public virtual void DrawMenu(UI u)
    {
    }
}


//Drawing metoder för levels
public class Level1 : Level
{
    public Level1(int count, int gateX, bool resetAlpha, Player pExtern, Vector2[] coinpos) : base(count, gateX, resetAlpha, pExtern, coinpos)
    {
    }

    public override void Draw(Camera camera, Player player, Obstacle obstacle)
    {
        AlphaReset();
        Raylib.BeginMode2D(camera.c);
        DrawTextures(); //Ritar ut golv och bakgrund
        Raylib.DrawText("Welcome to Jumpman!", 30, 375, 40, Color.BLACK);
        Raylib.DrawText("Move using A/D/SPACE or the Arrow Keys", 30, 200, 40, Color.BLACK);
        Raylib.DrawText("Get to the gate at the end of the level to win", 700, 375, 40, Color.BLACK);
        Raylib.DrawText("Press enter when at", 2100, 400, 40, Color.GREEN);
        Raylib.DrawText("the gate to continue", 2100, 450, 40, Color.GREEN);
        Raylib.DrawText("Don't fall down!", 2100, 600, 40, Color.GREEN);
        player.DrawCharacter(ref player.frame, ref player.elapsed, player.rect, player.lastleft); //Ritar ut spelaren
        DrawCoin(ref frame, ref elapsed); //Ritar ut coins
        Raylib.EndMode2D();
        Raylib.DrawTexture(Global.infoSign, 0, 0, Color.WHITE);
        Raylib.DrawText("Level: 1", 25, 10, 30, Color.RED);
        Raylib.DrawTexture(Global.invCoinTexture, 35, 50, Color.WHITE);
        Raylib.DrawText($": {player.coins}", 75, 50, 35, Color.BLACK);
        player.DrawHearts();
        Raylib.DrawTexture(backgrounds[2], 0, 0, alpha);
    }
}
public class Level2 : Level
{
    public Level2(int count, int gateX, bool resetAlpha, Player pExtern, Vector2[] coinpos) : base(count, gateX, resetAlpha, pExtern, coinpos)
    {
    }

    public override void Draw(Camera camera, Player player, Obstacle obstacle)
    {
        Raylib.BeginMode2D(camera.c);
        DrawTextures();
        obstacle.DrawObstacles();
        Raylib.DrawText("Watch out for spikes!", 405, 375, 40, Color.BLACK);
        player.DrawCharacter(ref player.frame, ref player.elapsed, player.rect, player.lastleft);
        DrawCoin(ref frame, ref elapsed);
        Raylib.EndMode2D();
        Raylib.DrawTexture(Global.infoSign, 0, 0, Color.WHITE);
        Raylib.DrawText("Level: 2", 25, 10, 30, Color.RED);
        Raylib.DrawTexture(Global.invCoinTexture, 35, 50, Color.WHITE);
        Raylib.DrawText($": {player.coins}", 75, 50, 35, Color.BLACK);
        player.DrawHearts();
        Raylib.DrawTexture(backgrounds[2], 0, 0, alpha);
    }
}

public class Level3 : Level
{
    public Level3(int count, int gateX, bool resetAlpha, Player pExtern, Vector2[] coinpos) : base(count, gateX, resetAlpha, pExtern, coinpos)
    {
    }

    public override void Draw(Camera camera, Player player, Obstacle obstacle)
    {
        Raylib.BeginMode2D(camera.c);
        DrawTextures();
        obstacle.DrawObstacles();
        Raylib.DrawText("Watch out for slimes!", 1700, 375, 40, Color.BLACK);
        Raylib.DrawText("Great job!", 2555, 375, 40, Color.BLACK);
        player.DrawCharacter(ref player.frame, ref player.elapsed, player.rect, player.lastleft);
        DrawCoin(ref frame, ref elapsed);
        Raylib.EndMode2D();
        Raylib.DrawTexture(Global.infoSign, 0, 0, Color.WHITE);
        Raylib.DrawText("Level: 3", 25, 10, 30, Color.RED);
        Raylib.DrawTexture(Global.invCoinTexture, 35, 50, Color.WHITE);
        Raylib.DrawText($": {player.coins}", 75, 50, 35, Color.BLACK);
        player.DrawHearts();
        Raylib.DrawTexture(backgrounds[2], 0, 0, alpha);
    }
}

public class Menu : Level
{
    public Menu(int count, int gateX, bool resetAlpha, Player pExtern, Vector2[] coinpos) : base(count, gateX, resetAlpha, pExtern, coinpos)
    {
    }

    public override void DrawMenu(UI u)
    {
        Raylib.DrawTexture(u.startBG, 0, 0, Color.WHITE);
        Raylib.DrawText("Jumpman!", 350, 250, 75, Color.RED);
        Raylib.DrawRectangle((Global.screenwidth / 2) - 110, (Global.screenheight / 2) + 40, 220, 95, Color.BLACK);
        Raylib.DrawRectangleRec(u.button, u.buttoncolor);
        Raylib.DrawText("START", 442, 453, 40, Color.RED);
        Raylib.DrawTexture(backgrounds[2], 0, 0, alpha);
    }
}
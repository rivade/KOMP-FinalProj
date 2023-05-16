using System;
using Raylib_cs;
using System.Numerics;
public class Player
{
    Random generator = new();
    public List<Texture2D> sprites = new();
    public Rectangle rect = new Rectangle(0, 526, 50, 75);
    public bool collidesWithFloor = true;
    public bool lastleft = false; //Lastleft används för att veta åt vilket håll den ska rita ut spriten
    public float verticalVelocity = 0f; //Spelarens gravitation
    public int frame = 1;
    public int hearts = 3;
    public float elapsed = 0;
    public int coins = 0;
    public Texture2D heart = Raylib.LoadTexture("Textures/heart.png");
    public Texture2D emptyheart = Raylib.LoadTexture("Textures/emptyheart.png");
    public Player()
    {
        sprites.Add(Raylib.LoadTexture("Sprites/character.png"));
        sprites.Add(Raylib.LoadTexture("Sprites/running.png"));
        sprites.Add(Raylib.LoadTexture("Sprites/air.png"));
    }

    public bool Movement(bool lastleft, Level currentLevel) //currentlevel gör så att man kallar den instans av level som används just då i program.cs
    {
        if ((Raylib.IsKeyDown(KeyboardKey.KEY_A) || Raylib.IsKeyDown(KeyboardKey.KEY_LEFT)) && rect.x > 0) //Gör så att spelaren går vänster men inte kan gå ut ur skärmen
        {
            rect.x -= 5;
            lastleft = true;
        }
        if (Raylib.IsKeyDown(KeyboardKey.KEY_D) || Raylib.IsKeyDown(KeyboardKey.KEY_RIGHT))
        {
            rect.x += 5;
            lastleft = false;
        }

        collidesWithFloor = false; //Sätter collideswithfloor till default false
        foreach (Rectangle f in currentLevel.floors)
        {
            if (Raylib.CheckCollisionRecs(rect, f)) //Kollar kollisioner för spelaren och varje golv i banan
            {
                collidesWithFloor = true;
                if (rect.y + 75 > 600) //Återställer spelarens position till y värdet precis vid golvet för att förhindra att man faller igenom golvet.
                {
                    rect.y = 526;
                }

                if (Raylib.IsKeyPressed(KeyboardKey.KEY_SPACE) || Raylib.IsKeyPressed(KeyboardKey.KEY_UP)) //Gör gravitationen "negativ" när man trycker på space/uppåtpil vilket gör att man hoppar
                {
                    verticalVelocity = 10f;
                    Raylib.PlaySound(Global.jumpSound);
                }
                else
                {
                    verticalVelocity = 0f; //Gör gravitationen till 0 när man står på marken
                }
            }
        }
        if (!collidesWithFloor && verticalVelocity > -15)
        {
            verticalVelocity -= 0.3f; //Ökar gravitationen mer och mer när man är i luften för ett smooth fallande
        }
        rect.y -= (int)verticalVelocity; //Gör så att spelarens position påverkas av gravitationen

        return lastleft;
    }

    private int RunningLogic(ref int frame, ref float elapsed) //Används för att veta vilken frame av spritesheeten som ska ritas när spelaren springer
    {
        const float frameDuration = 0.07f; //Hur länge varje frame ska visas
        elapsed += Raylib.GetFrameTime(); //Hur lång tid som gått sen framen byttes
        if (elapsed >= frameDuration)
        {
            frame++; //Ökar framen av spritesheeten när den visats så länge som frameduration anger
            elapsed -= frameDuration; //Återställer visningstiden till nästa frame
        }
        frame %= 12; //Eftersom spritesheeten är 12 frames lång förhindrar detta frame från att överstiga 12
        return frame; // Returnerar modulusen av frame vilket avgör om den ska gå vidare till nästa frame eller inte
    }

    public void DrawCharacter(ref int frame, ref float timer, Rectangle player, bool lastLeft) //Ritar ut spelaren
    {
        Rectangle sourcerec = new Rectangle(); //Används för att programmet ska veta vilken del av spritesheeten som ska ritas
        if ((Raylib.IsKeyDown(KeyboardKey.KEY_D) || Raylib.IsKeyDown(KeyboardKey.KEY_RIGHT)) && collidesWithFloor) //Gör så att animationen bara visas när man rör sig och nuddar golvet
        {
            frame = RunningLogic(ref frame, ref timer);
            sourcerec = new Rectangle(50 * frame, 0, 50, 75); //Definerar sourcerec baserat på frame
            Raylib.DrawTextureRec(sprites[1], sourcerec, new Vector2(player.x, player.y), Color.WHITE);
        }
        else if ((Raylib.IsKeyDown(KeyboardKey.KEY_A) || Raylib.IsKeyDown(KeyboardKey.KEY_LEFT)) && collidesWithFloor)
        {
            frame = RunningLogic(ref frame, ref timer);
            sourcerec = new Rectangle(50 * frame, 0, -50, 75); //-50 bredd gör att spriten ritas ut spegelvänt när spelaren går åt andra hållet
            Raylib.DrawTextureRec(sprites[1], sourcerec, new Vector2(player.x, player.y), Color.WHITE);
        }
        else
        {
            if (lastLeft)
            {
                sourcerec = new Rectangle(0, 0, -50, 75);
                if (collidesWithFloor)
                {
                    Raylib.DrawTextureRec(sprites[0], sourcerec, new Vector2(player.x, player.y), Color.WHITE);
                }
                else
                {
                    Raylib.DrawTextureRec(sprites[2], sourcerec, new Vector2(player.x, player.y), Color.WHITE);
                }
            }
            else
            {
                sourcerec = new Rectangle(0, 0, 50, 75);
                if (collidesWithFloor)
                {
                    Raylib.DrawTextureRec(sprites[0], sourcerec, new Vector2(player.x, player.y), Color.WHITE);
                }
                else
                {
                    Raylib.DrawTextureRec(sprites[2], sourcerec, new Vector2(player.x, player.y), Color.WHITE);
                }
            }
        }
    }

    public void DeathCheck(Obstacle levelObstacles)
    {
        if (rect.y > Global.screenheight)
        {
            Global.currentscene = "death"; //Gör så att currentscene blir death när man faller ut från skärmen
            Raylib.PlaySound(Global.deathSound);
            hearts--;
        }
        foreach (var spike in levelObstacles.spikes) //Kollar collisions mellan spelaren och varje spike i leveln.
        {
            if (Raylib.CheckCollisionRecs(rect, spike))
            {
                Global.currentscene = "death";
                Raylib.PlaySound(Global.deathSound);
                hearts--;
                break;
            }
        }
        foreach (var enemy in levelObstacles.enemies)
        {
            if (Raylib.CheckCollisionRecs(rect, enemy))
            {
                Global.currentscene = "death";
                Raylib.PlaySound(Global.deathSound);
                hearts--;
                break;
            }
        }
    }

    public void DrawHearts()
    {
        for (var i = 0; i < 3; i++)
        {
            Raylib.DrawTexture(emptyheart, (i * 63 + 835), 5, Color.WHITE);
        }
        for (var i = 0; i < hearts; i++) //Gör så att den ritar ut ett hjärta ovanpå den tomma texturen för varje liv man har
        {
            Raylib.DrawTexture(heart, (i * 63 + 835), 5, Color.WHITE);
        }
    }
}

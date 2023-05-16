using System;
using Raylib_cs;
using System.Numerics;

public class Camera
{
    Player p;
    public Camera(Player pExtern)
    {
        p = pExtern; //Gör så att instansen av player blir samma som i program.cs
    }
    public Camera2D c = new();
    public void InitializeCamera()
    {
        c.zoom = 1;
        c.rotation = 0;
        c.offset = new Vector2(Global.screenwidth / 2, Global.screenheight / 2);
    }
    public void CameraBounds() //Gör så att kameran bara följer efter spelaren efter den passerat en viss punkt vilket gör det snyggare
    {
        if (p.rect.x >= 265)
        {
            c.target = new Vector2((p.rect.x + 250), (Global.screenheight / 2));
        }
        else
        {
            c.target = new Vector2((Global.screenwidth / 2), (Global.screenheight / 2));
        }
    }
}
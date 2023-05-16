using Raylib_cs;
using System.Numerics;

Raylib.InitWindow(Global.screenwidth, Global.screenheight, "Jumpman!");
Raylib.InitAudioDevice();
Raylib.SetTargetFPS(60);
Raylib.SetMasterVolume(1);

Player p = new Player();
UI u = new UI();
Camera c = new Camera(p); //Skapar en instans av kameraklassen med instansen av Player som program.cs använder

Level startMenu = new Menu(0, 0, false, p, new Vector2[] {});

//När en level läses in används en array med vektorer för att avgöra vart levelns coins ska vara. Jag valde en array då det finns ett fast antal coins per level, och det inte kan ändras beroende på andra faktorer
Level levelOne = new Level1(2, 1938, true, p, new Vector2[] //Skapar en instans av Level klassen med valdra presets för antal golv, etc.
{new Vector2(1030, 550), new Vector2(1130, 550), new Vector2(1230, 550)}); //Coin positioner

Level levelTwo = new Level2(3, 1938, true, p, new Vector2[]
{new Vector2(2650, 550), new Vector2(2700, 550), new Vector2(2750, 550), new Vector2(2650, 500), new Vector2(2700, 500), new Vector2(2750, 500)});

Level levelThree = new Level3(3, 2962, true, p, new Vector2[]
{ new Vector2(410, 450), new Vector2(460, 450), new Vector2(510, 450), new Vector2(560, 450), new Vector2(710, 450), new Vector2(760, 450), new Vector2(810, 450), new Vector2(860, 450), new Vector2(1010, 450), new Vector2(1060, 450), new Vector2(1110, 450), new Vector2(1160, 450), new Vector2(1310, 450), new Vector2(1360, 450), new Vector2(1410, 450), new Vector2(1460, 450) });

Obstacle obsOne = new Obstacle(new Vector2[] { }, new Vector2[] { });
Obstacle obsTwo = new Obstacle(new Vector2[] { new Vector2(400, 535), new Vector2(600, 535), new Vector2(800, 535), new Vector2(1200, 535), new Vector2(1250, 535), new Vector2(1300, 535), new Vector2(2200, 535), new Vector2(2250, 535), new Vector2(2300, 535), new Vector2(2450, 535), new Vector2(2500, 535), new Vector2(2550, 535), new Vector2(3022, 535), new Vector2(2972, 535), new Vector2(2922, 535), new Vector2(2872, 535), new Vector2(2822, 535) }, new Vector2[] { }); //Positioner för taggar i level 2
Obstacle obsThree = new Obstacle(new Vector2[] { new Vector2(400, 535), new Vector2(450, 535), new Vector2(500, 535), new Vector2(550, 535), new Vector2(700, 535), new Vector2(750, 535), new Vector2(800, 535), new Vector2(850, 535), new Vector2(1000, 535), new Vector2(1050, 535), new Vector2(1100, 535), new Vector2(1150, 535), new Vector2(1300, 535), new Vector2(1350, 535), new Vector2(1400, 535), new Vector2(1450, 535) },
new Vector2[] { new Vector2(1700, 540), new Vector2(1950, 540), new Vector2(2200, 540) }); //Enemy positioner

levelOne.nextLevel = levelTwo; //Definerar vilken instans av level som ska användas när man klarar en level
levelTwo.nextLevel = levelThree;
obsOne.nextObstacle = obsTwo; //Definerar vilken instans av obstacle som ska användas när man klarar en level
obsTwo.nextObstacle = obsThree;

c.InitializeCamera(); //Initierar kamerainställningar i klassen camera
Global.SoundInitialization();

while (!Raylib.WindowShouldClose())
{
    //Logik
    Raylib.UpdateMusicStream(Global.music);
    Global.RespawnLogic();

    switch (Global.currentscene)
    {
        case "start":
            u.StartButton("levelOne", startMenu);
            break;

        case "death":
            Raylib.PauseMusicStream(Global.music);
            Death.DeathHandler(p, levelOne, levelTwo, levelThree, startMenu, u);
            break;
        
        case "win":
            Raylib.PauseMusicStream(Global.music);
            Reset.ResetGame(p, levelOne, levelTwo, levelThree, startMenu, u);
            break;

        case "levelOne":
            levelOne.LogicHandler(c, p, obsOne, levelOne, "levelTwo");
            break;

        case "levelTwo":
            levelTwo.LogicHandler(c, p, obsTwo, levelTwo, "levelThree");
            break;

        case "levelThree":
            levelThree.LogicHandler(c, p, obsThree, levelThree, "win");
            break;
    }

    //Grafik
    Raylib.BeginDrawing();
    Raylib.ClearBackground(Color.WHITE);
    switch (Global.currentscene)
    {
        case "start":
            startMenu.DrawMenu(u);
            break;

        case "death":
            Death.Draw(p);
            break;
        
        case "win":
            Global.DrawWin(p);
            break;

        case "levelOne":
            levelOne.Draw(c, p, null);
            break;

        case "levelTwo":
            levelTwo.Draw(c, p, obsTwo);
            break;

        case "levelThree":
            levelThree.Draw(c, p, obsThree);
            break;
    }
    Raylib.EndDrawing();
}
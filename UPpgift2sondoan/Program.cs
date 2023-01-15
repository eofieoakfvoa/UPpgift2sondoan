using System;
using System.Numerics;
using Raylib_cs;

using System.Collections.Generic;
int ScreenWidth = 1280;
int ScreenHeight = 720;
Raylib.InitWindow(ScreenWidth, ScreenHeight, "game");
Raylib.SetTargetFPS(60);
bool menuopen = false;
Char Character = new Char();
Rectangle CharRect = new Rectangle(0, 200, 80, 80);
Rectangle DamageGive = new Rectangle(-350, 300, 80, 80);
Rectangle DamageTake = new Rectangle(-350, 500, 80, 80);
Rectangle HealthGive = new Rectangle(-350, 700, 80, 80);



Ray CheckCollisionRay = new Ray();


double Deg2Rad = Math.PI / 180;

Rectangle menu = new Rectangle (0,0, ScreenWidth, ScreenHeight);

Camera2D camera;
camera.zoom = 0.5f;
camera.rotation = 0;
camera.offset = new Vector2(ScreenWidth / 2, ScreenHeight / 2);
bool debug = false;

//Hud
int CurrentScreenHeight = 720;
int CurrentScreenWidth = 1280;

//Health Vars
Texture2D BotMenu = Raylib.LoadTexture("MenuNoHealth.png");
Texture2D Health = Raylib.LoadTexture("MenuHealth.png");
int HealthDefaultwidth = Health.width;
float HealthPercantage;
double HealthWidth = 100;

//Movement vars
int CharXSpeed = 7;
int CharYSpeed = 7;
bool walking = false;
bool ObstacleAvoidance = false;
double angle = 0;
Vector2 Target = new Vector2(0, 0);
Vector2 targetorig = new Vector2(0, 0);
Ray PointView = new Ray();
Ray CharView = new Ray();
int TargetIndex = 0;
bool debounce = false;
bool WallPoints = false;

//lists
List<Rectangle> walls = new List<Rectangle>();
walls.Add(new Rectangle(20, -60, 150, 150));
walls.Add(new Rectangle(700, -60, 150, 150));
walls.Add(new Rectangle(1400, -60, 150, 150));
walls.Add(new Rectangle(1550, -60, 150, 150));
List<BoundingBox> Wallcollisions = new List<BoundingBox>();
List<Vector2> p = new List<Vector2>();
List<Vector2> targets = new();

void togglefullscreen(int windowwidth, int windowheight)
{
    if (!Raylib.IsWindowFullscreen())
    {
        int monitor = Raylib.GetCurrentMonitor();
        Raylib.SetWindowSize(Raylib.GetMonitorWidth(monitor), Raylib.GetMonitorWidth(monitor));
        camera.offset = new Vector2(Raylib.GetMonitorWidth(monitor) / 2, Raylib.GetMonitorHeight(monitor) / 2);
        CurrentScreenHeight = Raylib.GetMonitorHeight(monitor);
        CurrentScreenWidth = Raylib.GetMonitorWidth(monitor);
        Raylib.ToggleFullscreen();
    }
    else
    {
        Raylib.ToggleFullscreen();
        Raylib.SetWindowSize(windowwidth, windowheight);
        camera.offset = new Vector2(windowwidth / 2, windowheight / 2);
        CurrentScreenHeight = 720;
        CurrentScreenWidth = 1280;
    }
}


foreach (Rectangle wall in walls)
{
    Wallcollisions.Add(new BoundingBox(new Vector3(wall.x, wall.y, 0), new Vector3(wall.x + wall.width, wall.y + wall.height, 0)));

}
Rectangle Ability1Hitbox = new Rectangle(0,0, 10, 40);
//functions
static double distanceCalc(Vector2 Origin, Vector2 comparison)
{
    return Math.Sqrt(Math.Pow(Origin.X - comparison.X, 2) + Math.Pow(Origin.Y - comparison.Y, 2));
}
static void GetTarget(List<Vector2> p, List<Vector2> targets, int skip, int skip2, Ray PointView, Vector2 closest, float MoveAngleCenterx, float MoveAngleCenterY, BoundingBox wall, Vector2 characterPos, Vector2 Target)
{
    Vector2 nextClosest = new Vector2(0, 0);
    for (int i = 0; i < p.Count; i++)
    {
        if (i == skip2 || i == skip)
        {
            continue;
        }
        else
        {
            double angle55 = Math.Atan2(closest.Y - p[i].Y, closest.X - p[i].X);
            float TargAngle1 = (float)Math.Sin(angle55); //y
            float TargAngle2 = (float)Math.Cos(angle55); //x
            PointView.position = new Vector3(closest.X + (MoveAngleCenterx * 20), (int)(closest.Y + (MoveAngleCenterY * 20)), 0);
            PointView.direction = new Vector3(-TargAngle2, -TargAngle1, 0);
            if (Raylib.GetRayCollisionBox(PointView, wall).hit == true)
            {   
                System.Console.WriteLine("count" + p[i]);
                continue;
            }

             if (distanceCalc(characterPos, p[i]) + distanceCalc(p[i], Target) < distanceCalc(characterPos, nextClosest) + distanceCalc(nextClosest, Target))
            {
                nextClosest = p[i];
                System.Console.WriteLine("added" + p[i]);
                System.Console.WriteLine("hi");
                targets.Add(new Vector2(nextClosest.X, nextClosest.Y));
            }
        }
    }
}

static void Ability1Indicator(KeyboardKey ButtonUsed){
    
}
static void Ability1(KeyboardKey ButtonUsed){
    
}
//Logic
while (Raylib.WindowShouldClose() == false)
{
    float DeltaTime = Raylib.GetFrameTime();
    // Vars
    Vector2 characterPos = new Vector2(CharRect.x, CharRect.y);
    camera.target = characterPos;
    int mouseY = Raylib.GetMouseY();
    int mouseX = Raylib.GetMouseX();
    Vector2 screenmousePos = new Vector2(mouseX, mouseY);
    Vector2 WorldMousePos = Raylib.GetScreenToWorld2D(screenmousePos, camera);
    Vector2 ScreenCharPos = Raylib.GetWorldToScreen2D(characterPos, camera);

    //keybinds

    if (Raylib.IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT))
    {
        targetorig = new Vector2(WorldMousePos.X, WorldMousePos.Y);
        Target = targetorig;
        angle = Math.Atan2(ScreenCharPos.Y - mouseY, ScreenCharPos.X - mouseX) * (180 / Math.PI); // Ifall BegindMode2D inte är på Replace ScreenCharPos.X and Y with Character.x and y
        walking = true;
        TargetIndex = 0;
        p.Clear();
        WallPoints = false;
        ObstacleAvoidance = false;
        targets.Clear();
        debounce = false;
    }

    if (Raylib.IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_RIGHT))
    {

    }
    if (Raylib.IsKeyPressed(KeyboardKey.KEY_S))
    {
        walking = false;
    }

    if (Raylib.IsKeyDown(KeyboardKey.KEY_Q)){ 
        Ability1Indicator(KeyboardKey.KEY_Q);
    }
    else if (Raylib.IsKeyReleased(KeyboardKey.KEY_Q)){ 
        Ability1(KeyboardKey.KEY_Q);
    }
    if (Raylib.IsKeyPressed(KeyboardKey.KEY_W)){

    }
    if (Raylib.IsKeyPressed(KeyboardKey.KEY_E)){

    }
    if (Raylib.IsKeyPressed(KeyboardKey.KEY_R)){

    }
    if (Raylib.IsKeyPressed(KeyboardKey.KEY_D))
    {
        if (debug == false)
        {
            debug = true;
        }
        else
        {
            debug = false;
        }
    }
    if (Raylib.IsKeyPressed(KeyboardKey.KEY_C))
    {
        menuopen = true;
    }
    if (Raylib.IsKeyPressed(KeyboardKey.KEY_F))
    {
        togglefullscreen(ScreenWidth, ScreenHeight);
    }


    //Movement
    if (walking == true)
    {
        double CharTargDist = distanceCalc(Target, characterPos);
        if (CharTargDist > 5)
        {

            float angle1 = (float)Math.Sin(Deg2Rad * angle);
            float angle2 = (float)Math.Cos(Deg2Rad * angle);
            CharView.position = new Vector3(characterPos.X, characterPos.Y, 0);
            CharView.direction = new Vector3(-angle2, -angle1, 0);


            foreach (BoundingBox wall in Wallcollisions)
            {
                if (Raylib.GetRayCollisionBox(CharView, wall).hit == true)
                {
                    double charwaldist = distanceCalc(new Vector2((wall.min.X + wall.max.X) / 2, (wall.min.Y + wall.max.Y) / 2), characterPos);
                    if (charwaldist < 150)
                    {
                        if (debounce == false)
                        {
                            debounce = true;

                            if (WallPoints == false)
                            {
                                WallPoints = true;
                                p = new List<Vector2>()
                            {
                                new Vector2(wall.min.X, wall.min.Y), // top left p[0]
                                new Vector2(wall.max.X, wall.min.Y), // top right p[1]
                                new Vector2(wall.max.X, wall.max.Y), // bot right p[2]
                                new Vector2(wall.min.X, wall.max.Y), // bot left p[3]
                                new Vector2((wall.min.X + wall.max.X) / 2, (wall.min.X + wall.max.X) / 2) // center p[4]
                            };

                            }

                            Vector2 closest = p[0];
                            Vector2 center = p[4];

                            for (int i = 1; i < p.Count; i++)
                            {
                                int skip2;
                                int skip = (p.IndexOf(center));
                                if (i == skip)
                                {
                                    continue;
                                }
                                if ((characterPos - p[i]).Length() < (characterPos - closest).Length())
                                {
                                    closest = p[i];
                                }
                                if (i == p.Count - 2)
                                {
                                    targets.Add(new Vector2(closest.X, closest.Y));
                                    double anglecenter = Math.Atan2(closest.Y - center.Y, closest.X - center.X);
                                    float MoveAngleCenterY = (float)Math.Sin(anglecenter); //y
                                    float MoveAngleCenterx = (float)Math.Cos(anglecenter); //x
                                    double RayAngle = Math.Atan2(closest.Y - Target.Y, closest.X - Target.X);
                                    //float RayAngle = Convert.ToSingle(AngleCalc(closest, Target));
                                    float Rayangle1 = (float)Math.Sin(RayAngle);
                                    float Rayangle2 = (float)Math.Cos(RayAngle);
                                    ObstacleAvoidance = true;

                                    CheckCollisionRay.position = new Vector3(closest.X + (MoveAngleCenterx * 20), (int)(closest.Y + (MoveAngleCenterY * 20)), 0);
                                    //CheckCollisionRay.position = new Vector3(closest.X, closest.Y, 0);
                                    CheckCollisionRay.direction = new Vector3(-Rayangle2, -Rayangle1, 0);
                                    skip2 = (p.IndexOf(closest));
                                    if (Raylib.GetRayCollisionBox(CheckCollisionRay, wall).hit == true)
                                    {
                                        GetTarget(p, targets, skip, skip2, PointView, closest, MoveAngleCenterx, MoveAngleCenterY, wall, characterPos, Target);

                                    }
                                    targets.Add(Target);
                                    foreach (var item in targets)
                                    {
                                        System.Console.WriteLine(item);
                                    }

                                }
                            }


                        }
                    }
                }
            }
            if (ObstacleAvoidance == true)
            {
                if (TargetIndex < targets.Count)
                {
                    Target = targets[TargetIndex];
                    if (distanceCalc(characterPos, Target) < 30)
                    {
                        TargetIndex++;
                    }
                }
                if (TargetIndex == targets.Count)
                {
                    TargetIndex = 0;
                    p.Clear();
                    ObstacleAvoidance = false;
                    targets.Clear();
                    walking = false;
                    debounce = false;
                }
                // ObstacleAvoidance = false;
                // WallPoints = false;
                // p.Clear();
                // debounce = true;
                // targets.Clear();
            }
            // Movement / https://stackoverflow.com/a/49503918
            double dx = Target.X - CharRect.x;
            double dy = Target.Y - CharRect.y;
            double Len = Math.Sqrt(dx * dx + dy * dy);
            float normaldx = Convert.ToSingle(dx / Len);
            float normaldy = Convert.ToSingle(dy / Len);
            if (CharRect.x > Target.X)
            {
                CharRect.x += CharXSpeed * normaldx;

            }
            if (CharRect.x < Target.X)
            {
                CharRect.x += CharXSpeed * normaldx;

            }
            if (CharRect.y > Target.Y)
            {
                CharRect.y += CharYSpeed * normaldy;

            }
            if (CharRect.y < Target.Y)
            {
                CharRect.y += CharYSpeed * normaldy;

            }
        }

    }
    //Health
    if (Character.Health >= 0)
    {
        HealthPercantage = (float)Character.Health / Character.MaxHealth;
        HealthWidth = HealthDefaultwidth * HealthPercantage;
        Health.width = (int)HealthWidth;
    }
    else
    {
        Character.Health = 0;
    }
    if (Raylib.CheckCollisionRecs(CharRect, DamageGive))
    {
        Character.Health -= 10;
    }
    if (Raylib.CheckCollisionRecs(CharRect, HealthGive))
    {
        Character.Health += 10;
    }
    if (Character.Health > Character.MaxHealth)
    {
        Character.Health = Character.MaxHealth;
    }



    //*****************************************************************************************************************************************************************  
    //Rendering

    Raylib.BeginDrawing();
    Raylib.ClearBackground(Color.WHITE);
    Raylib.BeginMode2D(camera);
    Raylib.DrawRectangleRec(DamageGive, Color.RED);
    Raylib.DrawRectangleRec(DamageTake, Color.BLUE);
    Raylib.DrawRectangleRec(HealthGive, Color.GREEN);
    foreach (Rectangle wall in walls)
    {
        Raylib.DrawRectangleRec(wall, Color.RED);
    }
    foreach (BoundingBox wall in Wallcollisions)
    {
        Raylib.DrawBoundingBox(wall, Color.BLACK);
    }

    Raylib.DrawTexturePro(Character.CharacterImage, new Rectangle(0, 0, 80, 80), CharRect, new Vector2(40, 40), Convert.ToSingle(angle), Color.WHITE); // Origin är mitten av texturen, så en 80x80 bild har origin 40, 40
    if (debug == true)
    {
        Raylib.DrawRay(CharView, Color.BLACK);
        Raylib.DrawRectangle((int)Target.X, (int)Target.Y, 10, 10, Color.RED);
        Raylib.DrawRectangle((int)targetorig.X, (int)targetorig.Y, 10, 10, Color.GREEN);
        Raylib.DrawRay(CheckCollisionRay, Color.BLACK);
        Raylib.DrawRay(PointView, Color.RED);
        //Raylib.DrawTextEx(Raylib.LoadFont("resources/fonts/alagard.png"), Character.Health + "/" + Character.MaxHealth, new Vector2(camera.target.X - BotMenu.width / 2, camera.target.Y + ScreenHeight / 2 / camera.zoom - BotMenu.height), 25, 5, Color.BLACK);


    }
    Raylib.DrawTextureEx(BotMenu, new Vector2(camera.target.X - BotMenu.width / 2, camera.target.Y + CurrentScreenHeight / 2 / camera.zoom - BotMenu.height), 0, 1, Color.WHITE);
    Raylib.DrawTextureEx(Health, new Vector2(camera.target.X - BotMenu.width / 2 / (float)(HealthDefaultwidth / HealthWidth), camera.target.Y + CurrentScreenHeight / 2 / camera.zoom - BotMenu.height), 0, 1, Color.WHITE);
    if (menuopen == true)
    {
        menu.width = CurrentScreenWidth;
        menu.height = CurrentScreenHeight;
        menu.x = (characterPos.X - CurrentScreenWidth/2);
        menu.y = (characterPos.Y - CurrentScreenHeight/2);
        Raylib.DrawRectanglePro(menu, new Vector2(0,0), 0, Color.BLACK);
    }
    Raylib.EndDrawing();
}

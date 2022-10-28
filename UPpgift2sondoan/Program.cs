using System.Numerics;
using Raylib_cs;

Raylib.InitWindow(1080, 1080, "game");
Raylib.SetTargetFPS(60);

Char Character = new Char();

Camera2D camera;
camera.zoom = 0.5f;
camera.rotation = 0;
camera.offset = new Vector2(1080/2, 1080/2);
bool walking = false;

List<Enemy> enemies = new List<Enemy>();
enemies.Add(new Enemy());
enemies.Add(new Enemy());
enemies.Add(new Enemy());
enemies[1].rect.y = 200;
enemies[2].rect.y = 400;

List<Rectangle> walls = new List<Rectangle>();
walls.Add(new Rectangle(20, 20, 150, 150));
walls.Add(new Rectangle(700, 20, 32, 32));
walls.Add(new Rectangle(14000, 20, 32, 32));
List<BoundingBox> Wallcollisions = new List<BoundingBox>();
Wallcollisions.Add(new BoundingBox(new Vector3(walls[0].x,walls[0].y,0), new Vector3(walls[0].x+walls[0].width,walls[0].y-walls[0].height,0)));
Wallcollisions.Add(new BoundingBox(new Vector3(walls[1].x,walls[1].y,0), new Vector3(walls[1].x+walls[1].width,walls[1].y-walls[1].height,0)));


Rectangle CharRect = new Rectangle(0, 200, 80, 80);
Vector2 Target = new Vector2 (0,0);
Ray CharView = new Ray();
static void WalkToDestination(Vector2 Target, Vector2 ScreenCharPos, Char Character, Rectangle CharRect){
   CharRect.x += 1000;
}


while (Raylib.WindowShouldClose() == false){
  float DeltaTime = Raylib.GetFrameTime();
//Logic
// Vars
  Vector2 characterPos = new Vector2(CharRect.x, CharRect.y);
  camera.target = characterPos;
  int mouseY = Raylib.GetMouseY(); 
  int mouseX = Raylib.GetMouseX();
  Vector2 screenmousePos = new Vector2(mouseX, mouseY);
  Vector2 WorldMousePos = Raylib.GetScreenToWorld2D(screenmousePos, camera);
  Vector2 ScreenCharPos = Raylib.GetWorldToScreen2D(characterPos, camera);
  double angle = Math.Atan2(ScreenCharPos.Y - mouseY, ScreenCharPos.X - mouseX) * (180 / Math.PI); // Ifall BegindMode2D inte är på Replace ScreenCharPos.X and Y with Character.x and y
//Movement
    if (Raylib.IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT)){
      Target = new Vector2(WorldMousePos.X,WorldMousePos.Y);
      walking = true;
    } 
  if (walking == true){
    double Distancce = Math.Sqrt(Math.Pow(Target.X - CharRect.x, 2) + Math.Pow(Target.Y - CharRect.y, 2) );
    if (Distancce > 30){
      double Deg2Rad = Math.PI/180;
      float angle1 = (float)Math.Sin(Deg2Rad * angle);
      float angle2 = (float)Math.Cos(Deg2Rad * angle);
      CharView.position = new Vector3(characterPos.X, characterPos.Y, 0);
      CharView.direction = new Vector3(-angle2,-angle1,0);
      
      RayCollision checkcollision = new RayCollision();   
      
      checkcollision.hit = false;
      checkcollision.distance = 10f;
      checkcollision.normal = new Vector3 (characterPos.X, characterPos.Y, 0);
      checkcollision.point = new Vector3 (characterPos.X, characterPos.Y, 0);
      foreach(BoundingBox wall in Wallcollisions){
      if (Raylib.GetRayCollisionBox(CharView, wall).hit == true){
        Wallcollisions[0].GetType();
        Console.WriteLine(Wallcollisions[0].GetType());

      }
        
        
      }
      if (CharRect.x > Target.X){
          CharRect.x -= 5;

    }
      if (CharRect.x < Target.X){
          CharRect.x += 5;

    }
      if (CharRect.y > Target.Y){
          CharRect.y -= 5;

    }
      if (CharRect.y < Target.Y){
          CharRect.y += 5;
      }
    }
    else{
      walking = false;
    }
  }
//Drawing

  Raylib.BeginDrawing();
  Raylib.ClearBackground(Color.WHITE);
  Raylib.BeginMode2D(camera);
    foreach(Rectangle wall in walls)
    {
    Raylib.DrawRectangleRec(wall, Color.RED);

    }
      Raylib.DrawTexturePro(Character.CharacterImage ,CharRect, CharRect, new Vector2(40,40), Convert.ToSingle(angle), Color.WHITE); // Origin är mitten av texturen, så en 80x80 bild har origin 40, 40
            Raylib.DrawRay(CharView, Color.BLACK);
    foreach(BoundingBox wall in Wallcollisions){
      Raylib.DrawBoundingBox(wall, Color.BLACK);
    }
  Raylib.EndDrawing();
}
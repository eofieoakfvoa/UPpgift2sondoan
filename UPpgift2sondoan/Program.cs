/// Notes for Myself
/// A* patfinding. Ray cast
//
using System.Numerics;
using Raylib_cs;

Raylib.InitWindow(1080, 1080, "game");
Raylib.SetTargetFPS(60);

Char Character = new Char();
Rectangle CharRect = new Rectangle(0, 200, 80, 80);
bool walking = false;
bool ObstacleAvoidance = false;
Vector2 targetorig = new Vector2(0,0);
double Deg2Rad = Math.PI/180;
double angle = 0;
Camera2D camera;
camera.zoom = 0.5f;
camera.rotation = 0;
camera.offset = new Vector2(1080/2, 1080/2);
bool debug = false;
int CharXSpeed = 10;
int CharYSpeed = 10;
Vector2 CharDirection = new Vector2 (0,0);
Vector2 Target = new Vector2 (0,0);
Ray CharView = new Ray();

//lists
List<Enemy> enemies = new List<Enemy>();
enemies.Add(new Enemy());
enemies.Add(new Enemy());
enemies.Add(new Enemy());
enemies[1].rect.y = 200;
enemies[2].rect.y = 400;


List<Rectangle> walls = new List<Rectangle>();
walls.Add(new Rectangle(20, 20, 150, 150));
walls.Add(new Rectangle(700, 20, 150, 150));
walls.Add(new Rectangle(1400, 20, 150, 150));

List<BoundingBox> Wallcollisions = new List<BoundingBox>();
foreach(Rectangle wall in walls){
Wallcollisions.Add(new BoundingBox(new Vector3(wall.x,wall.y,0), new Vector3(wall.x+wall.width,wall.y+wall.height,0)));

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
//keybinds
    if (Raylib.IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT)){
      Target = new Vector2(WorldMousePos.X,WorldMousePos.Y);
      angle = Math.Atan2(ScreenCharPos.Y - mouseY, ScreenCharPos.X - mouseX) * (180 / Math.PI); // Ifall BegindMode2D inte är på Replace ScreenCharPos.X and Y with Character.x and y
      walking = true;
      Vector2 diff = screenmousePos - characterPos;
      CharDirection = Vector2.Normalize(diff);
    } 
    if (Raylib.IsKeyPressed(KeyboardKey.KEY_S)){
      walking = false;
    }
    if (Raylib.IsKeyPressed(KeyboardKey.KEY_D)){
      System.Console.WriteLine(debug);
      if (debug == false){
        debug = true;
      }
      else{
        debug = false;
      }
    }


//Movement
  if (walking == true){
    double Distancce = Math.Sqrt(Math.Pow(Target.X - CharRect.x, 2) + Math.Pow(Target.Y - CharRect.y, 2) );
    if (Distancce > 5){
      float angle1 = (float)Math.Sin(Deg2Rad * angle);
      float angle2 = (float)Math.Cos(Deg2Rad * angle);
      CharView.position = new Vector3(characterPos.X, characterPos.Y, 0);
      CharView.direction = new Vector3(-angle2,-angle1,0);
      
      //ray
      RayCollision checkcollision = new RayCollision();   
      checkcollision.hit = false;
      checkcollision.distance = 10f;
      checkcollision.normal = new Vector3 (characterPos.X, characterPos.Y, 0);
      checkcollision.point = new Vector3 (characterPos.X, characterPos.Y, 0);

      foreach(BoundingBox wall in Wallcollisions){
      if (Raylib.GetRayCollisionBox(CharView, wall).hit == true){
        Vector2 p1 = new Vector2(wall.min.X,wall.min.Y); // top left
        Vector2 p2 = new Vector2(wall.max.X,wall.min.Y); // top right
        Vector2 p3 = new Vector2(wall.max.X,wall.max.Y); // bot right
        Vector2 p4 = new Vector2(wall.min.X,wall.max.Y); // bot left
        List<Vector2> p = new List<Vector2>();
        p.Add(new Vector2(wall.min.X,wall.min.Y)); // top left p[0]
        p.Add(new Vector2(wall.max.X,wall.min.Y)); // top right p[1]
        p.Add(new Vector2(wall.max.X,wall.max.Y)); // bot right p[2]
        p.Add(new Vector2(wall.min.X,wall.max.Y)); // bot left p[3]
        foreach (Vector2 point in p){
        double RayDistance = Math.Sqrt(Math.Pow(point.X - CharRect.x, 2) + Math.Pow(point.Y - CharRect.y, 2) );
        if (RayDistance < 80){
            //check all distancces
            System.Console.WriteLine("up");
            System.Console.WriteLine(point.X);
            System.Console.WriteLine(wall.min.X);
            if (point.X == wall.min.X && point.Y == wall.min.Y){
              System.Console.WriteLine("botleft");
            }

            Target = new Vector2(point.X+50, point.Y+50);
          if (ObstacleAvoidance == false){
            targetorig = Target;
            ObstacleAvoidance = true;
          }
          if (ObstacleAvoidance == true){
            if (Distancce > 5){ 
              Target = targetorig;
              ObstacleAvoidance = false;
            }
          // else if (angle >= -45 && angle <= 45){
          //   Target = new Vector2(point.X+50, point.Y+50);
          //   System.Console.WriteLine("left");
          // }
          // else if (angle <= -135 && angle <= 135){
          //   Target = new Vector2(point.X-70, point.Y-70);
          //   System.Console.WriteLine("right");
          // }
          // else if (angle >= -135 && angle <= -45){
          //   Target = new Vector2(point.X+75, point.Y-50);
          //   System.Console.WriteLine("down");
          // }
          }
        }
        }
      }  
      }
      
      System.Console.WriteLine(characterPos);
      // Movement / https://stackoverflow.com/a/49503918
      double dx = Target.X - CharRect.x;
      double dy = Target.Y - CharRect.y;
      double Len = Math.Sqrt(dx*dx + dy*dy);
      float normaldx  = Convert.ToSingle(dx / Len);
      float normaldy = Convert.ToSingle( dy / Len);

      if (CharRect.x > Target.X){
          CharRect.x += CharXSpeed * normaldx;

      }
      if (CharRect.x < Target.X){
          CharRect.x += CharXSpeed * normaldx;

      }
      if (CharRect.y > Target.Y){
          CharRect.y += CharYSpeed * normaldy;

      }
      if (CharRect.y < Target.Y){
          CharRect.y += CharYSpeed * normaldy;
      
      }
    
    }
    else{
      walking = false;
    }

  }
//*****************************************************************************************************************************************************************  
//Rendering
  Raylib.BeginDrawing();
  Raylib.ClearBackground(Color.WHITE);
  Raylib.BeginMode2D(camera);
    foreach(Rectangle wall in walls)
    {
    Raylib.DrawRectangleRec(wall, Color.RED);
    }
    foreach(BoundingBox wall in Wallcollisions){
      Raylib.DrawBoundingBox(wall, Color.BLACK);
    }
    
    Raylib.DrawTexturePro(Character.CharacterImage ,CharRect, CharRect, new Vector2(40,40), Convert.ToSingle(angle), Color.WHITE); // Origin är mitten av texturen, så en 80x80 bild har origin 40, 40
 if (debug == true){
    Raylib.DrawRay(CharView, Color.BLACK);

 }
  Raylib.EndDrawing();
}
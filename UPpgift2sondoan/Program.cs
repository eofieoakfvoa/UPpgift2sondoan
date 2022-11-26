//
// min code är inte cleaned den har bara alla mina ideer utan att tagit bort allt som inte fungera, kommer fixa
//
using System.Numerics;
using Raylib_cs;
using System.Collections.Generic;
Raylib.InitWindow(1080, 1080, "game");
Raylib.SetTargetFPS(60);
        int TargetIndex = 0;

Char Character = new Char();
Rectangle CharRect = new Rectangle(0, 200, 80, 80);
bool walking = false;
bool ObstacleAvoidance = false;
Vector2 targetorig = new Vector2(0,0);
double Deg2Rad = Math.PI/180;
double angle = 0;
Camera2D camera;
List<Vector2> p = new List<Vector2>();
        List<Vector2> targets = new();
camera.zoom = 0.5f;
camera.rotation = 0;
camera.offset = new Vector2(1080/2, 1080/2);
bool debug = false;
int CharXSpeed = 10;
int CharYSpeed = 10;
Vector2 CharDirection = new Vector2 (0,0);
Vector2 Target = new Vector2 (0,0);
Ray CharView = new Ray();
bool debounce = false;
bool TargetReached = false;

bool listmade = false;
//lists
List<Enemy> enemies = new List<Enemy>();
enemies.Add(new Enemy());
enemies.Add(new Enemy());
enemies.Add(new Enemy());
enemies.Add(new Enemy());
enemies[1].rect.y = 200;
enemies[2].rect.y = 400;
        Vector2 nextClosest = new Vector2(0,0);


List<Rectangle> walls = new List<Rectangle>();
walls.Add(new Rectangle(20, 20, 150, 150));
walls.Add(new Rectangle(700, 20, 150, 150));
walls.Add(new Rectangle(1400, 20, 150, 150));
walls.Add(new Rectangle(1550, 20, 150, 150));

List<BoundingBox> Wallcollisions = new List<BoundingBox>();
foreach(Rectangle wall in walls){
Wallcollisions.Add(new BoundingBox(new Vector3(wall.x,wall.y,0), new Vector3(wall.x+wall.width,wall.y+wall.height,0)));

}

//functions
static double AngleCalc(Vector2 Origin, Vector2 comparison){
  return Math.Atan2(Origin.Y - comparison.Y, Origin.X - comparison.X) * (180 / Math.PI);
}
static double distanceCalc(Vector2 Origin, Vector2 comparison){
  return Math.Sqrt(Math.Pow(Origin.X - comparison.X, 2) + Math.Pow(Origin.Y - comparison.Y, 2) );
}
static void MovementClear(List<Vector2> p, bool debounce)
{
  p.Clear();
  debounce = false;
  
}

//Logic
while (Raylib.WindowShouldClose() == false){
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

    if (Raylib.IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT)){
      targetorig = new Vector2(WorldMousePos.X,WorldMousePos.Y);
      Target = targetorig;
      angle = Math.Atan2(ScreenCharPos.Y - mouseY, ScreenCharPos.X - mouseX) * (180 / Math.PI); // Ifall BegindMode2D inte är på Replace ScreenCharPos.X and Y with Character.x and y
      walking = true;
      Vector2 diff = screenmousePos - characterPos;
      CharDirection = Vector2.Normalize(diff);
      MovementClear(p, debounce);
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
    double CharTargDist = distanceCalc(Target,characterPos);
    if (CharTargDist > 5){

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
        p.Add(new Vector2(wall.min.X,wall.min.Y)); // top left p[0]
        p.Add(new Vector2(wall.max.X,wall.min.Y)); // top right p[1]
        p.Add(new Vector2(wall.max.X,wall.max.Y)); // bot right p[2]
        p.Add(new Vector2(wall.min.X,wall.max.Y)); // bot left p[3]
        p.Add(new Vector2((wall.min.X + wall.max.X)/2,(wall.min.X + wall.max.X)/2 )); // center p[4]¨
        
        Vector2 closest = p[0];
        Vector2 center = p[4];

        // find closest point to player
        for (int i = 1; i < p.Count; i++)
        {
          if ((characterPos - p[i]).Length() < (characterPos - closest).Length())
          {
            closest = p[i];
          }
        }
  
        double angle23 = Math.Atan2(closest.Y - p[4].Y, closest.X - p[4].X) * (180 / Math.PI);



        double RayDistance = distanceCalc(closest, characterPos);
        
        if (RayDistance < 80){
        if (debounce == false)
        {
          debounce = true;  
        

        foreach (Vector2 point in p){
        double PointDistancce = distanceCalc(point,characterPos);

        if (PointDistancce < 100){
        
        for (int i = 0; i < p.Count; i++)
        {
          int skip = (p.IndexOf(closest));
          int skip2 = (p.IndexOf(center));
          if (i == skip2 || i == skip){
            break;
          }
           if (distanceCalc(characterPos, p[i]) + distanceCalc(p[i], Target) < distanceCalc(characterPos, nextClosest) +  distanceCalc(nextClosest, Target))
          {
            nextClosest = p[i];
          }
        }
        }
        }
        //Make Target List
        if (listmade == false){
                  ObstacleAvoidance = true;
          targets.Add(new Vector2(closest.X, closest.Y));
          targets.Add(new Vector2(nextClosest.X, nextClosest.Y));
          targets.Add(new Vector2(targetorig.X, targetorig.Y));
        }
        }
        }
      }  
      }
      //Movement To Target IF Obstacle
      if (ObstacleAvoidance == true){
        listmade = true;
          if (TargetIndex < targets.Count){
          Target = targets[TargetIndex];
          if(distanceCalc(characterPos,Target) < 30){
            TargetIndex++;
          }
          }
          if (TargetIndex == targets.Count){
            TargetIndex = 0;
            ObstacleAvoidance = false;
            targets.Clear(); 
            walking = false;
            debounce = false;
            listmade = false;
          }


      }
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

    }


  
  
//*****************************************************************************************************************************************************************  
//Rendering

  Raylib.BeginDrawing();
  Raylib.ClearBackground(Color.WHITE);
  Raylib.BeginMode2D(camera);
    Raylib.DrawRectangle(-100,-100, 32, 32, Color.RED);
    foreach(Rectangle wall in walls)
    {
    Raylib.DrawRectangleRec(wall, Color.RED);
    }
    foreach(BoundingBox wall in Wallcollisions){
      Raylib.DrawBoundingBox(wall, Color.BLACK);
    }
    
    Raylib.DrawTexturePro(Character.CharacterImage ,new Rectangle(0,0, 80, 80), CharRect, new Vector2(40,40), Convert.ToSingle(angle), Color.WHITE); // Origin är mitten av texturen, så en 80x80 bild har origin 40, 40
 if (debug == true){
    Raylib.DrawRectangle((int)Target.X, (int)Target.Y, 10, 10, Color.BLACK);
    Raylib.DrawRectangle((int)targetorig.X, (int)targetorig.Y, 10, 10, Color.BLACK);
    
 }
  Raylib.EndDrawing();
}




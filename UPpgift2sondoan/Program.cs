//
// min code är inte cleaned den har bara alla mina ideer utan att tagit bort allt som inte fungera, kommer fixa
//
using System.Numerics;
using Raylib_cs;
using System.Collections.Generic;
Raylib.InitWindow(1080, 1080, "game");
Raylib.SetTargetFPS(60);
        int TargetIndex = 0;
        bool firstpoint = true;
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
bool lastpoint = false;
bool listmade = false;
//lists
List<Enemy> enemies = new List<Enemy>();
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

List<BoundingBox> Wallcollisions = new List<BoundingBox>();
foreach(Rectangle wall in walls){
Wallcollisions.Add(new BoundingBox(new Vector3(wall.x,wall.y,0), new Vector3(wall.x+wall.width,wall.y+wall.height,0)));

}

//functions
static double AngleCalc(Vector2 Origin, Vector2 comparison){
    double angle = Math.Atan2(Origin.Y - comparison.Y, Origin.X - comparison.X) * (180 / Math.PI);
    return angle;
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
    // System.Console.WriteLine(p.Count);
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
        p.Add(new Vector2(wall.min.X,wall.min.Y)); // top left p[0]
        p.Add(new Vector2(wall.max.X,wall.min.Y)); // top right p[1]
        p.Add(new Vector2(wall.max.X,wall.max.Y)); // bot right p[2]
        p.Add(new Vector2(wall.min.X,wall.max.Y)); // bot left p[3]
        p.Add(new Vector2((wall.min.X + wall.max.X)/2,(wall.min.X + wall.max.X)/2 )); // center p[4]¨
        Vector2 closest = p[0];
        // find closest
        for (int i = 1; i < p.Count; i++)
        {
          if ((characterPos - p[i]).Length() < (characterPos - closest).Length())
          {
            closest = p[i];
          }
        }
  
        double angle23 = Math.Atan2(closest.Y - p[4].Y, closest.X - p[4].X) * (180 / Math.PI);
        Vector2 center = p[4];

        double RayDistance = Math.Sqrt(Math.Pow(closest.X - CharRect.x, 2) + Math.Pow(closest.Y - CharRect.y, 2) );


        if (RayDistance < 80){
        if (debounce == false)
        {
          debounce = true;  
        

        foreach (Vector2 point in p){
        
        double RayDistance2 = Math.Sqrt(Math.Pow(point.X - CharRect.x, 2) + Math.Pow(point.Y - CharRect.y, 2) );
        if (RayDistance2 < 100){
        
        for (int i = 0; i < p.Count; i++)
        {
          int skip = (p.IndexOf(closest));
          int skip2 = (p.IndexOf(center));
          if (i == skip2){
            continue;
          }

           if (distanceCalc(characterPos, p[i]) + distanceCalc(p[i], Target) < distanceCalc(characterPos, nextClosest) + distanceCalc(nextClosest, Target))
          {
            nextClosest = p[i];
            //  System.Console.WriteLine("p0 is" +  p[0]);
            //  System.Console.WriteLine("p1 is" +  p[1]);
            //  System.Console.WriteLine("p2 is" +  p[2]);
            //  System.Console.WriteLine("p3 is" +  p[3]);
        
          }
        }
        }
        }
        if (listmade == false){
                  ObstacleAvoidance = true;
          targets.Add(new Vector2(closest.X, closest.Y));
          targets.Add(new Vector2(nextClosest.X, nextClosest.Y));
          targets.Add(new Vector2(targetorig.X, targetorig.Y));
          System.Console.WriteLine(nextClosest);
        }
        }
        }
      }  
      }
      if (ObstacleAvoidance == true){
        listmade = true;
        System.Console.WriteLine(TargetIndex);
        System.Console.WriteLine("target" + targets.Count);
          if (TargetIndex < targets.Count){
            System.Console.WriteLine(TargetIndex);
          Target = targets[TargetIndex];
          if(distanceCalc(characterPos,Target) < 30){
            TargetIndex++;
          }
          }
          if (TargetIndex == targets.Count){
            System.Console.WriteLine("hiwqif");
            TargetIndex = 0;
            Target = targetorig;
            ObstacleAvoidance = false;
            targets.Clear();
            walking = false;
            listmade = false;
          }
        // for (var i = 0; i < targets.Count;)
        // {
        //   Target = targets[i];
        //   if(distanceCalc(characterPos,Target) < 20){
        //     i++;
        //   }
        //   else{
        //   }
        //   if (i == targets.Count){
        //     Target = targetorig;
        //     ObstacleAvoidance = false;
        //   }
        // }
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
    else{
      debounce = false;
      firstpoint = true;
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
    
    Raylib.DrawTexturePro(Character.CharacterImage ,CharRect, CharRect, new Vector2(40,40), Convert.ToSingle(angle), Color.WHITE); // Origin är mitten av texturen, så en 80x80 bild har origin 40, 40
 if (debug == true){
    Raylib.DrawRay(CharView, Color.BLACK);
    Raylib.DrawRectangle((int)Target.X, (int)Target.Y, 10, 10, Color.BLACK);
    Raylib.DrawRectangle((int)targetorig.X, (int)targetorig.Y, 10, 10, Color.BLACK);

 }
  Raylib.EndDrawing();
}




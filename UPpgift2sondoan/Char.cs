using System.Numerics;
using Raylib_cs;
public class Character
{
    public Rectangle rect = new Rectangle(100, 100, 80, 80);
    public Texture2D CharacterImage = Raylib.LoadTexture("TempCharFront.png");
    public int speed = 275;

}

public class Char{
    public Rectangle rect = new Rectangle(300, 300, 80, 80);
    public Texture2D CharacterImage = Raylib.LoadTexture("Avatar.png");
    public int speed = 50;
    public int Health = 1000;
    public int MaxHealth = 1000;

}
using EPainter.Core;
using Godot;
using System;

public partial class Rayitas : TextureRect
{
	[Export] int gridsize = 64;
	Godot.Color Color = new Godot.Color(0, 0, 0, 0.1f);
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
    public override void _Draw()
    {
		DrawLines();
    }

	void DrawLines()
	{
		float space = Size.X / gridsize;

		for (var i = 0; i < gridsize; i++)
		{
			float c = i * space;
            DrawLine(new Vector2(0, c), new Vector2(Size.X, c), Color, 1f );
            DrawLine(new Vector2(c, 0), new Vector2(c, Size.Y), Color, 1f);
		}
	}
}

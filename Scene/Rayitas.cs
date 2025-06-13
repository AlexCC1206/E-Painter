using EPainter.Core;
using Godot;
using System;

public partial class Rayitas : TextureRect
{
	[Export] int gridsize = 64;
	Godot.Color Color = new Godot.Color(0, 0, 0, 0.1f);
	float space = 0;
	public Canvas Canvas;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Canvas = new Canvas(gridsize);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	public override void _Draw()
	{
		DrawCanvas(Canvas);
		DrawLines();
	}
	void DrawCanvas(Canvas canvas)
	{
		string[,] Rosalia = canvas.Pixels;

		for (var i = 0; i < Rosalia.GetLength(0); i++)
		{
			for (var j = 0; j < Rosalia.GetLength(1); j++)
			{
				Color color = CheckColor(Rosalia[i, j]);
				Rect2 rect = new Rect2(i * space, j * space, space, space);
				DrawRect(rect, color);
			}
		}
	}
	void DrawLines()
	{
		space = Size.X / gridsize;

		for (var i = 0; i < gridsize; i++)
		{
			float c = i * space;
			DrawLine(new Vector2(0, c), new Vector2(Size.X, c), Color, 1f);
			DrawLine(new Vector2(c, 0), new Vector2(c, Size.Y), Color, 1f);
		}
	}
	Godot.Color CheckColor(string color)
	{
		switch (color)
		{
			case "Red": return new Color(1, 0, 0);
			case "Blue": return new Color(0, 0, 1);
			case "Green": return new Color(0, 1, 0);
			case "Yellow": return new Color(1, 1, 0);
			case "Orange": return new Color(1, 0.647f, 0);
			case "Purple": return new Color(0.627f, 0.125f, 0.941f);
			case "Black": return new Color(0, 0, 0);
			case "White": return new Color(1, 1, 1);

			default: return new Color(0, 0, 0, 0); // Transparent
		}
	}
}

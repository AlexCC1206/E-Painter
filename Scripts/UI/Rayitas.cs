using EPainter.Core;
using Godot;
using System;

public partial class Rayitas : TextureRect
{
	[Export] int gridsize = 32;
	Color Color = new Color(0, 0, 0, 0.1f);
	float space = 0;
	public Canvas Canvas;
	
	public override void _Ready()
	{
		Canvas = new Canvas(gridsize);
		
		CustomMinimumSize = new Vector2(400, 400);
		
		var newTexture = new ImageTexture();
		var image = Image.CreateEmpty(gridsize, gridsize, false, Image.Format.Rgba8);
		newTexture.SetImage(image);
		Texture = newTexture;
		
		space = CustomMinimumSize.X / gridsize;
		
		GD.Print($"Canvas initialized with size {gridsize}x{gridsize}, space = {space}");
	}

	public override void _Process(double delta)
	{
	}
	
	public void ResizeCanvas(int newSize)
	{
		gridsize = newSize;
		Canvas = new Canvas(gridsize);
		
		var newTexture = new ImageTexture();
		var image = Image.CreateEmpty(gridsize, gridsize, false, Image.Format.Rgba8);
		
		newTexture.SetImage(image);
		Texture = newTexture;
		
		CustomMinimumSize = new Vector2(400, 400);
		
		space = CustomMinimumSize.X / gridsize;
		
		QueueRedraw();
		
		GD.Print($"Canvas resized to {gridsize}x{gridsize}, with space = {space}");
	}
	
	public override void _Draw()
	{
		float effectiveWidth = Mathf.Max(Size.X, CustomMinimumSize.X);
		space = effectiveWidth / gridsize;
		
		DrawCanvas(Canvas);
		DrawLines();
	}
	void DrawCanvas(Canvas canvas)
	{
		string[,] Rosalia = canvas.Pixels;
		
		if (space <= 0)
		{
			space = Mathf.Max(Size.X, CustomMinimumSize.X) / gridsize;
		}

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
		float effectiveWidth = Mathf.Max(Size.X, CustomMinimumSize.X);
		float effectiveHeight = Mathf.Max(Size.Y, CustomMinimumSize.Y);
		space = effectiveWidth / gridsize;

		for (var i = 0; i <= gridsize; i++)
		{
			float c = i * space;
			DrawLine(new Vector2(0, c), new Vector2(effectiveWidth, c), Color, 1f);
			DrawLine(new Vector2(c, 0), new Vector2(c, effectiveHeight), Color, 1f);
		}
	}
	
	Color CheckColor(string color)
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

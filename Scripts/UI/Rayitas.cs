using EPainter.Core;
using Godot;
using System;

public partial class Rayitas : TextureRect
{
	[Export] int gridSize = 32;
	[Export] Color gridColor = new Color(0, 0, 0, 0.1f);

	public Canvas Canvas { get; private set; }
	private float cellSize = 0;
	
	
	public override void _Ready()
	{
		InitializeCanvas();
	}

    public void ResizeCanvas(int newSize)
	{
		gridSize = newSize;
		InitializeCanvas();
		GD.Print($"Canvas resized to {gridSize}x{gridSize}, with cellSize = {cellSize}");
	}

	private void InitializeCanvas()
	{
		Canvas = new Canvas(gridSize);

		CustomMinimumSize = new Vector2(400, 400);

		var newTexture = new ImageTexture();
		var image = Image.CreateEmpty(gridSize, gridSize, false, Image.Format.Rgba8);
		newTexture.SetImage(image);
		Texture = newTexture;

		RecalculateCellSize();
		QueueRedraw();
    }

    private void RecalculateCellSize()
    {
        float effectiveWidth = Mathf.Max(Size.X, CustomMinimumSize.X);
        cellSize = effectiveWidth / gridSize;
    }

    public override void _Draw()
	{
		RecalculateCellSize();
		DrawCanvas(Canvas);
		DrawGrid();
	}
	
	void DrawCanvas(Canvas canvas)
	{
		string[,] pixels = canvas.Pixels;
		
		if (cellSize <= 0)
		{
			cellSize = Mathf.Max(Size.X, CustomMinimumSize.X) / gridSize;
		}

		for (var i = 0; i < pixels.GetLength(0); i++)
		{
			for (var j = 0; j < pixels.GetLength(1); j++)
			{
				Color color = ConvertStringToColor(pixels[i, j]);
				Rect2 rect = new Rect2(i * cellSize, j * cellSize, cellSize, cellSize);
				DrawRect(rect, color);
			}
		}
	}
	
	void DrawGrid()
	{
		float effectiveWidth = Mathf.Max(Size.X, CustomMinimumSize.X);
            float effectiveHeight = Mathf.Max(Size.Y, CustomMinimumSize.Y);

            for (var i = 0; i <= gridSize; i++)
            {
                float position = i * cellSize;
                DrawLine(new Vector2(0, position), new Vector2(effectiveWidth, position), gridColor, 1f);
                DrawLine(new Vector2(position, 0), new Vector2(position, effectiveHeight), gridColor, 1f);
            }
	}
	
	private Color ConvertStringToColor(string color)
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

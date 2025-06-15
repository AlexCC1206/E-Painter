using EPainter.Core;
using Godot;
using System;

public partial class Rayitas : TextureRect
{
	[Export] int gridsize = 32;
	Color Color = new Color(0, 0, 0, 0.1f);
	float space = 0;
	public Canvas Canvas;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// Crear el canvas inicial
		Canvas = new Canvas(gridsize);
		
		// Establecer un tamaño mínimo para el TextureRect
		CustomMinimumSize = new Vector2(400, 400);
		
		// Crear una textura inicial
		var newTexture = new ImageTexture();
		var image = Image.CreateEmpty(gridsize, gridsize, false, Image.Format.Rgba8);
		newTexture.SetImage(image);
		Texture = newTexture;
		
		// Calcular el espaciado inicial
		space = CustomMinimumSize.X / gridsize;
		
		GD.Print($"Canvas initialized with size {gridsize}x{gridsize}, space = {space}");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	public void ResizeCanvas(int newSize)
	{
		gridsize = newSize;
		Canvas = new Canvas(gridsize);
		
		// Crear una textura nueva con el tamaño adecuado
		var newTexture = new ImageTexture();
		var image = Image.CreateEmpty(gridsize, gridsize, false, Image.Format.Rgba8);
		
		// Establecer la textura
		newTexture.SetImage(image);
		Texture = newTexture;
		
		// Forzar un tamaño mínimo para el TextureRect
		CustomMinimumSize = new Vector2(400, 400);
		
		// Actualizar el cálculo de espaciado
		space = CustomMinimumSize.X / gridsize;
		
		// Actualizar el dibujo
		QueueRedraw();
		
		GD.Print($"Canvas resized to {gridsize}x{gridsize}, with space = {space}");
	}
	
	public override void _Draw()
	{
		// Calcular el espacio antes de dibujar
		float effectiveWidth = Mathf.Max(Size.X, CustomMinimumSize.X);
		space = effectiveWidth / gridsize;
		
		// Dibujar el canvas y las líneas
		DrawCanvas(Canvas);
		DrawLines();
	}
	void DrawCanvas(Canvas canvas)
	{
		string[,] Rosalia = canvas.Pixels;
		
		// Asegurar que el espacio está calculado correctamente
		if (space <= 0)
		{
			space = Mathf.Max(Size.X, CustomMinimumSize.X) / gridsize;
		}

		// Dibujar cada celda del canvas
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
		// Calcular el espacio basado en el tamaño actual o mínimo
		float effectiveWidth = Mathf.Max(Size.X, CustomMinimumSize.X);
		float effectiveHeight = Mathf.Max(Size.Y, CustomMinimumSize.Y);
		space = effectiveWidth / gridsize;

		// Dibujar líneas horizontales y verticales
		for (var i = 0; i <= gridsize; i++)
		{
			float c = i * space;
			DrawLine(new Vector2(0, c), new Vector2(effectiveWidth, c), Color, 1f);
			DrawLine(new Vector2(c, 0), new Vector2(c, effectiveHeight), Color, 1f);
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

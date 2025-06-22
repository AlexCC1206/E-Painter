using EPainter.Core;
using Godot;

namespace EPainter.UI
{
	/// <summary>
	/// Clase que representa el componente visual del lienzo de dibujo.
	/// Maneja la renderización del grid y los píxeles coloreados.
	/// </summary>
	public partial class Rayitas : TextureRect
	{
		#region Campos y Propiedades
		/// <summary>
		/// El tamaño de la cuadrícula del lienzo (ancho y alto en número de celdas).
		/// Puede ser configurado desde el editor de Godot.
		/// </summary>
		[Export] int gridSize = 32;

		/// <summary>
		/// El color de las líneas de la cuadrícula.
		/// Puede ser configurado desde el editor de Godot.
		/// </summary>
		[Export] Color gridColor = new Color(0, 0, 0, 0.1f);

		/// <summary>
		/// El lienzo que contiene la información de los píxeles.
		/// Proporciona acceso al modelo de datos subyacente del lienzo.
		/// </summary>
		public Canvas Canvas { get; private set; }

		/// <summary>
		/// Tamaño calculado de cada celda en píxeles.
		/// Se recalcula automáticamente cuando cambia el tamaño del control.
		/// </summary>
		private float cellSize = 0;
		#endregion

		#region Inicialización y Configuración
		/// <summary>
		/// Se llama cuando el nodo entra en el árbol de escena.
		/// Inicializa el lienzo con el tamaño predeterminado.
		/// </summary>
		public override void _Ready()
		{
			InitializeCanvas();
		}

		/// <summary>
		/// Cambia el tamaño del lienzo al valor especificado.
		/// Recrea el lienzo y actualiza la visualización.
		/// </summary>
		/// <param name="newSize">El nuevo tamaño del lienzo (ancho y alto en píxeles).</param>
		public void ResizeCanvas(int newSize)
		{
			gridSize = newSize;
			InitializeCanvas();
			GD.Print($"Canvas resized to {gridSize}x{gridSize}, with cellSize = {cellSize}");
		}

		/// <summary>
		/// Inicializa el lienzo con el tamaño actual.
		/// Crea una nueva textura vacía y recalcula el tamaño de las celdas.
		/// </summary>
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

		/// <summary>
		/// Recalcula el tamaño de cada celda del lienzo basado en el tamaño actual del control.
		/// </summary>
		private void RecalculateCellSize()
		{
			float effectiveWidth = Mathf.Max(Size.X, CustomMinimumSize.X);
			cellSize = effectiveWidth / gridSize;
		}
		#endregion

		#region Renderizado
		/// <summary>
		/// Se llama cuando el nodo necesita ser redibujado.
		/// Dibuja el lienzo y la cuadrícula.
		/// </summary>
		public override void _Draw()
		{
			RecalculateCellSize();
			DrawCanvas(Canvas);
			DrawGrid();
		}

		/// <summary>
		/// Dibuja el contenido del lienzo, coloreando cada celda según el valor en la matriz de píxeles.
		/// </summary>
		/// <param name="canvas">El objeto lienzo que contiene la información de los píxeles.</param>
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

		/// <summary>
		/// Dibuja la cuadrícula del lienzo como líneas que separan las celdas.
		/// </summary>
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
		#endregion

		#region Utilidades
		/// <summary>
		/// Convierte una cadena que representa un color en un objeto Color de Godot.
		/// </summary>
		/// <param name="color">La cadena que representa el color (ej. "Red", "Blue", etc.).</param>
		/// <returns>El objeto Color correspondiente, o transparente si no se reconoce el color.</returns>
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
		#endregion
	}
}

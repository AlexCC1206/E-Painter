using System;

namespace EPainter.Core
{
    /// <summary>
    /// Representa el lienzo de dibujo para E-Painter.
    /// </summary>
    public class Canvas
    {
        /// <summary>
        /// Tamaño del lienzo (ancho y alto).
        /// </summary>
        public int Size;
        
        /// <summary>
        /// Matriz bidimensional que almacena los colores de los píxeles.
        /// </summary>
        public string[,] Pixels;

        /// <summary>
        /// Inicializa una nueva instancia de la clase Canvas.
        /// </summary>
        /// <param name="size">El tamaño del lienzo (ancho y alto).</param>
        public Canvas(int size)
        {
            Size = size;
            Pixels = new string[size, size];
            Clear();
        }

        /// <summary>
        /// Limpia el lienzo estableciendo todos los píxeles en blanco.
        /// </summary>
        private void Clear()
        {
            for (var y = 0; y < Size; y++)
            {
                for (var x = 0; x < Size; x++)
                {
                    Pixels[x, y] = "White";
                }
            }
        }

        /// <summary>
        /// Verifica si una posición está dentro de los límites del lienzo.
        /// </summary>
        /// <param name="x">La coordenada X a verificar.</param>
        /// <param name="y">La coordenada Y a verificar.</param>
        /// <returns>True si la posición es válida, False si está fuera de los límites.</returns>
        public bool IsValidPosition(int x, int y)
        {
            return x >= 0 && x < Size && y >= 0 && y < Size;
        }

        /// <summary>
        /// Establece el color de un píxel en el lienzo.
        /// </summary>
        /// <param name="x">La coordenada X del píxel.</param>
        /// <param name="y">La coordenada Y del píxel.</param>
        /// <param name="color">El color a establecer.</param>
        public void SetPixel(int x, int y, string color)
        {
            if (IsValidPosition(x, y) && color != "Transparent")
            {
                Pixels[x, y] = color;
            }
        }

        /// <summary>
        /// Obtiene el color de un píxel específico del lienzo.
        /// </summary>
        /// <param name="x">La coordenada X del píxel.</param>
        /// <param name="y">La coordenada Y del píxel.</param>
        /// <returns>El color del píxel o null si la posición no es válida.</returns>
        public string GetPixel(int x, int y)
        {
            return IsValidPosition(x, y) ? Pixels[x, y] : null;
        }
    }
}
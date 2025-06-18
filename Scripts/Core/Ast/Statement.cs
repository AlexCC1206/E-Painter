using System.Linq.Expressions;

namespace EPainter.Core
{
    /// <summary>
    /// Clase base abstracta para todas las sentencias en el lenguaje E-Painter.
    /// </summary>
    public abstract class Stmt
    {
        /// <summary>
        /// Método abstracto que implementa el patrón Visitor para procesar sentencias.
        /// </summary>
        /// <typeparam name="T">El tipo de retorno del visitante.</typeparam>
        /// <param name="visitor">El visitante que procesará esta sentencia.</param>
        /// <returns>Un resultado de tipo T producido por el visitante.</returns>
        public abstract T Accept<T>(IStmtVisitor<T> visitor);

        /// <summary>
        /// Interfaz para implementar el patrón Visitor para las sentencias.
        /// </summary>
        /// <typeparam name="T">El tipo de retorno del visitante.</typeparam>
        public interface IStmtVisitor<T>
        {
            /// <summary>
            /// Visita una sentencia de asignación.
            /// </summary>
            /// <param name="stmt">La sentencia de asignación a visitar.</param>
            /// <returns>El resultado de procesar la sentencia de asignación.</returns>
            T VisitAssignment(Assignment stmt);
            
            /// <summary>
            /// Visita una sentencia spawn para establecer la posición inicial.
            /// </summary>
            /// <param name="stmt">La sentencia spawn a visitar.</param>
            /// <returns>El resultado de procesar la sentencia spawn.</returns>
            T VisitSpawn(Spawn stmt);
            
            /// <summary>
            /// Visita una sentencia para cambiar el color de dibujo.
            /// </summary>
            /// <param name="stmt">La sentencia de cambio de color a visitar.</param>
            /// <returns>El resultado de procesar la sentencia de cambio de color.</returns>
            T VisitColor(Color stmt);
            
            /// <summary>
            /// Visita una sentencia para cambiar el tamaño del pincel.
            /// </summary>
            /// <param name="stmt">La sentencia de cambio de tamaño a visitar.</param>
            /// <returns>El resultado de procesar la sentencia de cambio de tamaño.</returns>
            T VisitSize(Size stmt);
            
            /// <summary>
            /// Visita una sentencia para dibujar una línea.
            /// </summary>
            /// <param name="stmt">La sentencia de dibujo de línea a visitar.</param>
            /// <returns>El resultado de procesar la sentencia de dibujo de línea.</returns>
            T VisitDrawLine(DrawLine stmt);
            
            /// <summary>
            /// Visita una sentencia para dibujar un círculo.
            /// </summary>
            /// <param name="stmt">La sentencia de dibujo de círculo a visitar.</param>
            /// <returns>El resultado de procesar la sentencia de dibujo de círculo.</returns>
            T VisitDrawCircle(DrawCircle stmt);
            
            /// <summary>
            /// Visita una sentencia para dibujar un rectángulo.
            /// </summary>
            /// <param name="stmt">La sentencia de dibujo de rectángulo a visitar.</param>
            /// <returns>El resultado de procesar la sentencia de dibujo de rectángulo.</returns>
            T VisitDrawRectangle(DrawRectangle stmt);
            
            /// <summary>
            /// Visita una sentencia para rellenar una forma.
            /// </summary>
            /// <param name="stmt">La sentencia de relleno a visitar.</param>
            /// <returns>El resultado de procesar la sentencia de relleno.</returns>
            T VisitFill(Fill stmt);
            
            /// <summary>
            /// Visita una sentencia de salto a una etiqueta.
            /// </summary>
            /// <param name="stmt">La sentencia de salto a visitar.</param>
            /// <returns>El resultado de procesar la sentencia de salto.</returns>
            T VisitGoto(Goto stmt);
            
            /// <summary>
            /// Visita una sentencia de etiqueta.
            /// </summary>
            /// <param name="stmt">La sentencia de etiqueta a visitar.</param>
            /// <returns>El resultado de procesar la sentencia de etiqueta.</returns>
            T VisitLabel(Label stmt);
        }

        /// <summary>
        /// Representa una asignación de valor a una variable.
        /// </summary>
        public class Assignment : Stmt
        {
            /// <summary>
            /// Obtiene el nombre de la variable a la que se asignará el valor.
            /// </summary>
            public string Name { get; }
            
            /// <summary>
            /// Obtiene la expresión cuyo valor se asignará a la variable.
            /// </summary>
            public Expr Value { get; }

            /// <summary>
            /// Inicializa una nueva instancia de la clase Assignment.
            /// </summary>
            /// <param name="name">El nombre de la variable.</param>
            /// <param name="value">La expresión cuyo valor se asignará.</param>
            public Assignment(string name, Expr value)
            {
                Name = name;
                Value = value;
            }

            /// <summary>
            /// Acepta un visitante para procesar esta sentencia de asignación.
            /// </summary>
            /// <typeparam name="T">El tipo de retorno del visitante.</typeparam>
            /// <param name="visitor">El visitante que procesará esta sentencia de asignación.</param>
            /// <returns>El resultado de procesar esta sentencia de asignación.</returns>
            public override T Accept<T>(IStmtVisitor<T> visitor)
            {
                return visitor.VisitAssignment(this);
            }
        }

        /// <summary>
        /// Representa la sentencia para establecer la posición inicial del cursor de dibujo.
        /// </summary>
        public class Spawn : Stmt
        {
            /// <summary>
            /// Obtiene la expresión que representa la coordenada X inicial.
            /// </summary>
            public Expr X { get; }
            
            /// <summary>
            /// Obtiene la expresión que representa la coordenada Y inicial.
            /// </summary>
            public Expr Y { get; }

            /// <summary>
            /// Inicializa una nueva instancia de la clase Spawn.
            /// </summary>
            /// <param name="x">La expresión para la coordenada X inicial.</param>
            /// <param name="y">La expresión para la coordenada Y inicial.</param>
            public Spawn(Expr x, Expr y)
            {
                X = x;
                Y = y;
            }

            /// <summary>
            /// Acepta un visitante para procesar esta sentencia spawn.
            /// </summary>
            /// <typeparam name="T">El tipo de retorno del visitante.</typeparam>
            /// <param name="visitor">El visitante que procesará esta sentencia spawn.</param>
            /// <returns>El resultado de procesar esta sentencia spawn.</returns>
            public override T Accept<T>(IStmtVisitor<T> visitor)
            {
                return visitor.VisitSpawn(this);
            }
        }

        /// <summary>
        /// Representa la sentencia para cambiar el color de dibujo actual.
        /// </summary>
        public class Color : Stmt
        {
            /// <summary>
            /// Obtiene la expresión que representa el nombre del color.
            /// </summary>
            public Expr ColorName { get; }

            /// <summary>
            /// Inicializa una nueva instancia de la clase Color.
            /// </summary>
            /// <param name="colorName">La expresión para el nombre del color.</param>
            public Color(Expr colorName)
            {
                ColorName = colorName;
            }

            /// <summary>
            /// Acepta un visitante para procesar esta sentencia de cambio de color.
            /// </summary>
            /// <typeparam name="T">El tipo de retorno del visitante.</typeparam>
            /// <param name="visitor">El visitante que procesará esta sentencia de cambio de color.</param>
            /// <returns>El resultado de procesar esta sentencia de cambio de color.</returns>
            public override T Accept<T>(IStmtVisitor<T> visitor)
            {
                return visitor.VisitColor(this);
            }
        }

        /// <summary>
        /// Representa la sentencia para cambiar el tamaño del pincel.
        /// </summary>
        public class Size : Stmt
        {
            /// <summary>
            /// Obtiene la expresión que representa el valor del tamaño.
            /// </summary>
            public Expr SizeValue { get; }

            /// <summary>
            /// Inicializa una nueva instancia de la clase Size.
            /// </summary>
            /// <param name="sizeValue">La expresión para el valor del tamaño.</param>
            public Size(Expr sizeValue)
            {
                SizeValue = sizeValue;
            }

            /// <summary>
            /// Acepta un visitante para procesar esta sentencia de cambio de tamaño.
            /// </summary>
            /// <typeparam name="T">El tipo de retorno del visitante.</typeparam>
            /// <param name="visitor">El visitante que procesará esta sentencia de cambio de tamaño.</param>
            /// <returns>El resultado de procesar esta sentencia de cambio de tamaño.</returns>
            public override T Accept<T>(IStmtVisitor<T> visitor)
            {
                return visitor.VisitSize(this);
            }
        }

        /// <summary>
        /// Representa la sentencia para dibujar una línea.
        /// </summary>
        public class DrawLine : Stmt
        {
            /// <summary>
            /// Obtiene la expresión para la componente X de la dirección.
            /// </summary>
            public Expr DirX { get; }
            
            /// <summary>
            /// Obtiene la expresión para la componente Y de la dirección.
            /// </summary>
            public Expr DirY { get; }
            
            /// <summary>
            /// Obtiene la expresión para la distancia de la línea.
            /// </summary>
            public Expr Distance { get; }

            /// <summary>
            /// Inicializa una nueva instancia de la clase DrawLine.
            /// </summary>
            /// <param name="dirX">La expresión para la componente X de la dirección.</param>
            /// <param name="dirY">La expresión para la componente Y de la dirección.</param>
            /// <param name="distance">La expresión para la distancia de la línea.</param>
            public DrawLine(Expr dirX, Expr dirY, Expr distance)
            {
                DirX = dirX;
                DirY = dirY;
                Distance = distance;
            }

            /// <summary>
            /// Acepta un visitante para procesar esta sentencia de dibujo de línea.
            /// </summary>
            /// <typeparam name="T">El tipo de retorno del visitante.</typeparam>
            /// <param name="visitor">El visitante que procesará esta sentencia de dibujo de línea.</param>
            /// <returns>El resultado de procesar esta sentencia de dibujo de línea.</returns>
            public override T Accept<T>(IStmtVisitor<T> visitor)
            {
                return visitor.VisitDrawLine(this);
            }
        }

        /// <summary>
        /// Representa la sentencia para dibujar un círculo.
        /// </summary>
        public class DrawCircle : Stmt
        {
            /// <summary>
            /// Obtiene la expresión para la componente X del centro relativo.
            /// </summary>
            public Expr DirX { get; }
            
            /// <summary>
            /// Obtiene la expresión para la componente Y del centro relativo.
            /// </summary>
            public Expr DirY { get; }
            
            /// <summary>
            /// Obtiene la expresión para el radio del círculo.
            /// </summary>
            public Expr Radius { get; }

            /// <summary>
            /// Inicializa una nueva instancia de la clase DrawCircle.
            /// </summary>
            /// <param name="dirX">La expresión para la componente X del centro relativo.</param>
            /// <param name="dirY">La expresión para la componente Y del centro relativo.</param>
            /// <param name="radius">La expresión para el radio del círculo.</param>
            public DrawCircle(Expr dirX, Expr dirY, Expr radius)
            {
                DirX = dirX;
                DirY = dirY;
                Radius = radius;
            }

            /// <summary>
            /// Acepta un visitante para procesar esta sentencia de dibujo de círculo.
            /// </summary>
            /// <typeparam name="T">El tipo de retorno del visitante.</typeparam>
            /// <param name="visitor">El visitante que procesará esta sentencia de dibujo de círculo.</param>
            /// <returns>El resultado de procesar esta sentencia de dibujo de círculo.</returns>
            public override T Accept<T>(IStmtVisitor<T> visitor)
            {
                return visitor.VisitDrawCircle(this);
            }
        }

        /// <summary>
        /// Representa la sentencia para dibujar un rectángulo.
        /// </summary>
        public class DrawRectangle : Stmt
        {
            /// <summary>
            /// Obtiene la expresión para la componente X de la dirección.
            /// </summary>
            public Expr DirX { get; }
            
            /// <summary>
            /// Obtiene la expresión para la componente Y de la dirección.
            /// </summary>
            public Expr DirY { get; }
            
            /// <summary>
            /// Obtiene la expresión para la distancia desde la posición actual.
            /// </summary>
            public Expr Distance { get; }
            
            /// <summary>
            /// Obtiene la expresión para el ancho del rectángulo.
            /// </summary>
            public Expr Width { get; }
            
            /// <summary>
            /// Obtiene la expresión para la altura del rectángulo.
            /// </summary>
            public Expr Height { get; }

            /// <summary>
            /// Inicializa una nueva instancia de la clase DrawRectangle.
            /// </summary>
            /// <param name="dirX">La expresión para la componente X de la dirección.</param>
            /// <param name="dirY">La expresión para la componente Y de la dirección.</param>
            /// <param name="distance">La expresión para la distancia desde la posición actual.</param>
            /// <param name="width">La expresión para el ancho del rectángulo.</param>
            /// <param name="height">La expresión para la altura del rectángulo.</param>
            public DrawRectangle(Expr dirX, Expr dirY, Expr distance, Expr width, Expr height)
            {
                DirX = dirX;
                DirY = dirY;
                Distance = distance;
                Width = width;
                Height = height;
            }

            /// <summary>
            /// Acepta un visitante para procesar esta sentencia de dibujo de rectángulo.
            /// </summary>
            /// <typeparam name="T">El tipo de retorno del visitante.</typeparam>
            /// <param name="visitor">El visitante que procesará esta sentencia de dibujo de rectángulo.</param>
            /// <returns>El resultado de procesar esta sentencia de dibujo de rectángulo.</returns>
            public override T Accept<T>(IStmtVisitor<T> visitor)
            {
                return visitor.VisitDrawRectangle(this);
            }
        }

        /// <summary>
        /// Representa la sentencia para rellenar una forma cerrada.
        /// </summary>
        public class Fill : Stmt
        {
            /// <summary>
            /// Acepta un visitante para procesar esta sentencia de relleno.
            /// </summary>
            /// <typeparam name="T">El tipo de retorno del visitante.</typeparam>
            /// <param name="visitor">El visitante que procesará esta sentencia de relleno.</param>
            /// <returns>El resultado de procesar esta sentencia de relleno.</returns>
            public override T Accept<T>(IStmtVisitor<T> visitor)
            {
                return visitor.VisitFill(this);
            }
        }

        /// <summary>
        /// Representa la sentencia para saltar a una etiqueta específica.
        /// </summary>
        public class Goto : Stmt
        {
            /// <summary>
            /// Obtiene el nombre de la etiqueta a la que se saltará.
            /// </summary>
            public string LabelName { get; }
            
            /// <summary>
            /// Obtiene la expresión de condición para el salto.
            /// </summary>
            public Expr Condition { get; }

            /// <summary>
            /// Inicializa una nueva instancia de la clase Goto.
            /// </summary>
            /// <param name="label">El nombre de la etiqueta a la que se saltará.</param>
            /// <param name="condition">La expresión de condición para el salto.</param>
            public Goto(string label, Expr condition)
            {
                LabelName = label;
                Condition = condition;
            }

            /// <summary>
            /// Acepta un visitante para procesar esta sentencia de salto.
            /// </summary>
            /// <typeparam name="T">El tipo de retorno del visitante.</typeparam>
            /// <param name="visitor">El visitante que procesará esta sentencia de salto.</param>
            /// <returns>El resultado de procesar esta sentencia de salto.</returns>
            public override T Accept<T>(IStmtVisitor<T> visitor)
            {
                return visitor.VisitGoto(this);
            }
        }

        /// <summary>
        /// Representa una etiqueta a la que se puede saltar.
        /// </summary>
        public class Label : Stmt
        {
            /// <summary>
            /// Obtiene el nombre de la etiqueta.
            /// </summary>
            public string Name { get; }

            /// <summary>
            /// Inicializa una nueva instancia de la clase Label.
            /// </summary>
            /// <param name="name">El nombre de la etiqueta.</param>
            public Label(string name)
            {
                Name = name;
            }

            /// <summary>
            /// Acepta un visitante para procesar esta sentencia de etiqueta.
            /// </summary>
            /// <typeparam name="T">El tipo de retorno del visitante.</typeparam>
            /// <param name="visitor">El visitante que procesará esta sentencia de etiqueta.</param>
            /// <returns>El resultado de procesar esta sentencia de etiqueta.</returns>
            public override T Accept<T>(IStmtVisitor<T> visitor)
            {
                return visitor.VisitLabel(this);
            }
        }
    }
}
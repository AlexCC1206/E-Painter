namespace EPainter.Core
{
    /// <summary>
    /// Representa una declaración abstracta en el lenguaje.
    /// </summary>
    public abstract class Stmt
    {
        /// <summary>
        /// Acepta un visitante que procesa esta declaración.
        /// </summary>
        /// <typeparam name="T">El tipo de resultado producido por el visitante.</typeparam>
        /// <param name="visitor">El visitante que procesa esta declaración.</param>
        /// <returns>El resultado producido por el visitante.</returns>
        public abstract T Accept<T>(IVisitor<T> visitor);

        /// <summary>
        /// Interfaz para implementar visitantes de declaraciones.
        /// </summary>
        /// <typeparam name="T">El tipo de resultado producido por el visitante.</typeparam>
        public interface IVisitor<T>
        {
            T VisitSpawnStmt(Spawn stmt);
            T VisitColorStmt(Color stmt);
            T VisitSizeStmt(Size stmt);
            T VisitDrawLineStmt(DrawLine stmt);
            T VisitDrawCircleStmt(DrawCircle stmt);
            T VisitDrawRectangleStmt(DrawRectangle stmt);
            T VisitFillStmt(Fill stmt);
            T VisitAssignmentStmt(Assignment stmt);
            T VisitLabelStmt(Label stmt);
            T VisitGotoStmt(Goto stmt);
        }

        internal interface IStmtVisitor<T>
        {
        }


        /// <summary>
        /// Representa una declaración para generar un objeto en una posición.
        /// </summary>
        public class Spawn : Stmt
        {
            public Expr X { get; }
            public Expr Y { get; }

            /// <summary>
            /// Inicializa una nueva instancia de la clase <see cref="Spawn"/>.
            /// </summary>
            /// <param name="x">La coordenada X.</param>
            /// <param name="y">La coordenada Y.</param>
            public Spawn(Expr x, Expr y)
            {
                X = x;
                Y = y;
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.VisitSpawnStmt(this);
            }
        }

        /// <summary>
        /// Representa una declaración para cambiar el color.
        /// </summary>
        public class Color : Stmt
        {
            public string ColorName { get; }

            /// <summary>
            /// Inicializa una nueva instancia de la clase <see cref="Color"/>.
            /// </summary>
            /// <param name="colorName">El nombre del color.</param>
            public Color(string colorName)
            {
                ColorName = colorName;
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.VisitColorStmt(this);
            }
        }

        /// <summary>
        /// Representa una declaración para cambiar el tamaño.
        /// </summary>
        public class Size : Stmt
        {
            public Expr SizeValue { get; }

            /// <summary>
            /// Inicializa una nueva instancia de la clase <see cref="Size"/>.
            /// </summary>
            /// <param name="sizeValue">El valor del tamaño.</param>
            public Size(Expr sizeValue)
            {
                SizeValue = sizeValue;
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.VisitSizeStmt(this);
            }
        }

        /// <summary>
        /// Representa una declaración para dibujar una línea.
        /// </summary>
        public class DrawLine : Stmt
        {
            public Expr DirX { get; }
            public Expr DirY { get; }
            public Expr Distance { get; }

            /// <summary>
            /// Inicializa una nueva instancia de la clase <see cref="DrawLine"/>.
            /// </summary>
            /// <param name="dirX">La dirección X.</param>
            /// <param name="dirY">La dirección Y.</param>
            /// <param name="distance">La distancia.</param>
            public DrawLine(Expr dirX, Expr dirY, Expr distance)
            {
                DirX = dirX;
                DirY = dirY;
                Distance = distance;
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.VisitDrawLineStmt(this);
            }
        }

        /// <summary>
        /// Representa una declaración para dibujar un círculo.
        /// </summary>
        public class DrawCircle : Stmt
        {
            public Expr DirX { get; }
            public Expr DirY { get; }
            public Expr Radius { get; }

            /// <summary>
            /// Inicializa una nueva instancia de la clase <see cref="DrawCircle"/>.
            /// </summary>
            /// <param name="dirX">La dirección X.</param>
            /// <param name="dirY">La dirección Y.</param>
            /// <param name="radius">El radio.</param>
            public DrawCircle(Expr dirX, Expr dirY, Expr radius)
            {
                DirX = dirX;
                DirY = dirY;
                Radius = radius;
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.VisitDrawCircleStmt(this);
            }
        }

        /// <summary>
        /// Representa una declaración para dibujar un rectángulo.
        /// </summary>
        public class DrawRectangle : Stmt
        {
            public Expr DirX { get; }
            public Expr DirY { get; }
            public Expr Distance { get; }
            public Expr Width { get; }
            public Expr Height { get; }

            /// <summary>
            /// Inicializa una nueva instancia de la clase <see cref="DrawRectangle"/>.
            /// </summary>
            /// <param name="dirX">La dirección X.</param>
            /// <param name="dirY">La dirección Y.</param>
            /// <param name="distance">La distancia.</param>
            /// <param name="width">El ancho.</param>
            /// <param name="height">La altura.</param>
            public DrawRectangle(Expr dirX, Expr dirY, Expr distance, Expr width, Expr height)
            {
                DirX = dirX;
                DirY = dirY;
                Distance = distance;
                Width = width;
                Height = height;
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.VisitDrawRectangleStmt(this);
            }
        }

        /// <summary>
        /// Representa una declaración para rellenar una figura.
        /// </summary>
        public class Fill : Stmt
        {
            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.VisitFillStmt(this);
            }
        }

        /// <summary>
        /// Representa una declaración para realizar un salto condicional o incondicional a una etiqueta.
        /// </summary>
        public class Goto : Stmt
        {
            public Token label { get; }
            public Expr Condition { get; }

            /// <summary>
            /// Inicializa una nueva instancia de la clase <see cref="Goto"/>.
            /// </summary>
            /// <param name="label">La etiqueta a la que se realizará el salto.</param>
            /// <param name="condition">La condición para realizar el salto.</param>
            public Goto(Token label, Expr condition)
            {
                this.label = label;
                Condition = condition;
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.VisitGotoStmt(this);
            }
        }

        /// <summary>
        /// Representa una declaración para asignar un valor a una variable.
        /// </summary>
        public class Assignment : Stmt
        {
            public Token Name { get; }
            public Expr Value { get; }

            /// <summary>
            /// Inicializa una nueva instancia de la clase <see cref="Assignment"/>.
            /// </summary>
            /// <param name="name">El nombre de la variable.</param>
            /// <param name="value">El valor a asignar.</param>
            public Assignment(Token name, Expr value)
            {
                Name = name;
                Value = value;
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.VisitAssignmentStmt(this);
            }
        }


        /// <summary>
        /// Representa una declaración para definir una etiqueta en el código.
        /// </summary>
        public class Label : Stmt
        {
            public Token Name { get; }

            /// <summary>
            /// Inicializa una nueva instancia de la clase <see cref="Label"/>.
            /// </summary>
            /// <param name="name">El nombre de la etiqueta.</param>
            public Label(Token name)
            {
                Name = name;
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.VisitLabelStmt(this);
            }
        }
    }
}
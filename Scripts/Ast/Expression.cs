using System.Collections.Generic;

namespace EPainter
{
    /// <summary>
    /// Clase base abstracta para todas las expresiones en el AST (Árbol de Sintaxis Abstracta).
    /// </summary>
    public abstract class Expr
    {
        /// <summary>
        /// Método abstracto que acepta un visitante para procesar la expresión.
        /// </summary>
        /// <typeparam name="T">El tipo de retorno del visitante.</typeparam>
        /// <param name="visitor">El visitante que procesará la expresión.</param>
        /// <returns>El resultado del procesamiento del visitante.</returns>
        public abstract T Accept<T>(IVisitor<T> visitor);

        /// <summary>
        /// Interfaz para implementar el patrón visitante en las expresiones.
        /// </summary>
        /// <typeparam name="T">El tipo de retorno del visitante.</typeparam>
        public interface IVisitor<T>
        {
            T VisitBinaryExpr(Binary expr);
            T VisitGroupingExpr(Grouping expr);
            T VisitLiteralExpr(Literal expr);
            T VisitVariableExpr(Variable expr);
            T VisitFunctionCallExpr(FunctionCall expr);
            T VisitUnaryExpr(Unary expr);
        }

        /// <summary>
        /// Representa una expresión binaria.
        /// </summary>
        public class Binary : Expr
        {
            public Expr Left { get; }
            public Token Op { get; }
            public Expr Right { get; }

            /// <summary>
            /// Constructor para inicializar una expresión binaria.
            /// </summary>
            /// <param name="left">La expresión del lado izquierdo.</param>
            /// <param name="op">El operador.</param>
            /// <param name="rigth">La expresión del lado derecho.</param>
            public Binary(Expr left, Token op, Expr right)
            {
                Left = left;
                Op = op;
                Right = right;
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.VisitBinaryExpr(this);
            }
        }

        /// <summary>
        /// Representa una expresión unaria.
        /// </summary>
        public class Unary : Expr
        {
            public Token Op { get; }
            public Expr Right { get; }

            /// <summary>
            /// Constructor para inicializar una expresión unaria.
            /// </summary>
            /// <param name="op">El operador.</param>
            /// <param name="right">La expresión del lado derecho.</param>
            public Unary(Token op, Expr right)
            {
                Op = op;
                Right = right;
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.VisitUnaryExpr(this);
            }
        }

        /// <summary>
        /// Representa una expresión de agrupación (por ejemplo, paréntesis).
        /// </summary>
        public class Grouping : Expr
        {
            public Expr Expr { get; }

            /// <summary>
            /// Constructor para inicializar una expresión de agrupación.
            /// </summary>
            /// <param name="expr">La expresión agrupada.</param>
            public Grouping(Expr expr)
            {
                Expr = expr;
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.VisitGroupingExpr(this);
            }
        }

        /// <summary>
        /// Representa una expresión literal (por ejemplo, un número o cadena).
        /// </summary>
        public class Literal : Expr
        {
            public object Value { get; }

            /// <summary>
            /// Constructor para inicializar una expresión literal.
            /// </summary>
            /// <param name="value">El valor literal.</param>
            public Literal(object value)
            {
                Value = value;
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.VisitLiteralExpr(this);
            }
        }

        /// <summary>
        /// Representa una expresión de variable.
        /// </summary>
        public class Variable : Expr
        {
            public Token Name { get; }

            /// <summary>
            /// Constructor para inicializar una expresión de variable.
            /// </summary>
            /// <param name="name">El token del nombre de la variable.</param>
            public Variable(Token name)
            {
                Name = name;
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.VisitVariableExpr(this);
            }
        }

        /// <summary>
        /// Representa una expresión de llamada a función.
        /// </summary>
        public class FunctionCall : Expr
        {
            public Token Name { get; }
            public List<Expr> Arguments { get; }

            /// <summary>
            /// Constructor para inicializar una expresión de llamada a función.
            /// </summary>
            /// <param name="name">El token del nombre de la función.</param>
            /// <param name="args">La lista de argumentos.</param>
            public FunctionCall(Token name, List<Expr> args)
            {
                Name = name;
                Arguments = args;
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.VisitFunctionCallExpr(this);
            }
        }
    }
}



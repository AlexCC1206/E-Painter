using System.Collections.Generic;

namespace EPainter.Core
{
    /// <summary>
    /// Clase base abstracta para todas las expresiones en el lenguaje E-Painter.
    /// </summary>
    public abstract class Expr
    {
        /// <summary>
        /// Método abstracto que implementa el patrón Visitor para procesar expresiones.
        /// </summary>
        /// <typeparam name="T">El tipo de retorno del visitante.</typeparam>
        /// <param name="visitor">El visitante que procesará esta expresión.</param>
        /// <returns>Un resultado de tipo T producido por el visitante.</returns>
        public abstract T Accept<T>(IExprVisitor<T> visitor);

        /// <summary>
        /// Interfaz para implementar el patrón Visitor para las expresiones.
        /// </summary>
        /// <typeparam name="T">El tipo de retorno del visitante.</typeparam>
        public interface IExprVisitor<T>
        {
            /// <summary>
            /// Visita una expresión literal.
            /// </summary>
            /// <param name="expr">La expresión literal a visitar.</param>
            /// <returns>El resultado de procesar la expresión literal.</returns>
            T VisitLiteral(Literal expr);
            
            /// <summary>
            /// Visita una expresión de variable.
            /// </summary>
            /// <param name="expr">La expresión de variable a visitar.</param>
            /// <returns>El resultado de procesar la expresión de variable.</returns>
            T VisitVariable(Variable expr);
            
            /// <summary>
            /// Visita una expresión unaria.
            /// </summary>
            /// <param name="expr">La expresión unaria a visitar.</param>
            /// <returns>El resultado de procesar la expresión unaria.</returns>
            T VisitUnary(Unary expr);
            
            /// <summary>
            /// Visita una expresión binaria.
            /// </summary>
            /// <param name="expr">La expresión binaria a visitar.</param>
            /// <returns>El resultado de procesar la expresión binaria.</returns>
            T VisitBinary(Binary expr);
            
            /// <summary>
            /// Visita una expresión de agrupación.
            /// </summary>
            /// <param name="expr">La expresión de agrupación a visitar.</param>
            /// <returns>El resultado de procesar la expresión de agrupación.</returns>
            T VisitGrouping(Grouping expr);
            
            /// <summary>
            /// Visita una expresión de llamada a función.
            /// </summary>
            /// <param name="expr">La expresión de llamada a función a visitar.</param>
            /// <returns>El resultado de procesar la expresión de llamada a función.</returns>
            T VisitCall(Call expr);
        }

        /// <summary>
        /// Representa un valor literal en el lenguaje E-Painter.
        /// </summary>
        public class Literal : Expr
        {
            /// <summary>
            /// Obtiene el valor literal.
            /// </summary>
            public object Value { get; }

            /// <summary>
            /// Inicializa una nueva instancia de la clase Literal.
            /// </summary>
            /// <param name="value">El valor literal a almacenar.</param>
            public Literal(object value)
            {
                Value = value;
            }

            /// <summary>
            /// Acepta un visitante para procesar este literal.
            /// </summary>
            /// <typeparam name="T">El tipo de retorno del visitante.</typeparam>
            /// <param name="visitor">El visitante que procesará este literal.</param>
            /// <returns>El resultado de procesar este literal.</returns>
            public override T Accept<T>(IExprVisitor<T> visitor)
            {
                return visitor.VisitLiteral(this);
            }
        }

        /// <summary>
        /// Representa una referencia a una variable en el lenguaje E-Painter.
        /// </summary>
        public class Variable : Expr
        {
            /// <summary>
            /// Obtiene el nombre de la variable.
            /// </summary>
            public string Name { get; }

            /// <summary>
            /// Inicializa una nueva instancia de la clase Variable.
            /// </summary>
            /// <param name="name">El nombre de la variable.</param>
            public Variable(string name)
            {
                Name = name;
            }

            /// <summary>
            /// Acepta un visitante para procesar esta variable.
            /// </summary>
            /// <typeparam name="T">El tipo de retorno del visitante.</typeparam>
            /// <param name="visitor">El visitante que procesará esta variable.</param>
            /// <returns>El resultado de procesar esta variable.</returns>
            public override T Accept<T>(IExprVisitor<T> visitor)
            {
                return visitor.VisitVariable(this);
            }
        }

        /// <summary>
        /// Representa una expresión binaria en el lenguaje E-Painter.
        /// </summary>
        public class Binary : Expr
        {
            /// <summary>
            /// Obtiene la expresión del lado izquierdo.
            /// </summary>
            public Expr Left { get; }
            
            /// <summary>
            /// Obtiene el token del operador.
            /// </summary>
            public Token Op { get; }
            
            /// <summary>
            /// Obtiene la expresión del lado derecho.
            /// </summary>
            public Expr Right { get; }

            /// <summary>
            /// Inicializa una nueva instancia de la clase Binary.
            /// </summary>
            /// <param name="left">La expresión del lado izquierdo.</param>
            /// <param name="op">El token del operador.</param>
            /// <param name="right">La expresión del lado derecho.</param>
            public Binary(Expr left, Token op, Expr right)
            {
                Left = left;
                Op = op;
                Right = right;
            }

            /// <summary>
            /// Acepta un visitante para procesar esta expresión binaria.
            /// </summary>
            /// <typeparam name="T">El tipo de retorno del visitante.</typeparam>
            /// <param name="visitor">El visitante que procesará esta expresión binaria.</param>
            /// <returns>El resultado de procesar esta expresión binaria.</returns>
            public override T Accept<T>(IExprVisitor<T> visitor)
            {
                return visitor.VisitBinary(this);
            }
        }

        /// <summary>
        /// Representa una expresión unaria en el lenguaje E-Painter.
        /// </summary>
        public class Unary : Expr
        {
            /// <summary>
            /// Obtiene el token del operador.
            /// </summary>
            public Token Op { get; }
            
            /// <summary>
            /// Obtiene la expresión operando.
            /// </summary>
            public Expr Right { get; }

            /// <summary>
            /// Inicializa una nueva instancia de la clase Unary.
            /// </summary>
            /// <param name="op">El token del operador.</param>
            /// <param name="right">La expresión operando.</param>
            public Unary(Token op, Expr right)
            {
                Op = op;
                Right = right;
            }

            /// <summary>
            /// Acepta un visitante para procesar esta expresión unaria.
            /// </summary>
            /// <typeparam name="T">El tipo de retorno del visitante.</typeparam>
            /// <param name="visitor">El visitante que procesará esta expresión unaria.</param>
            /// <returns>El resultado de procesar esta expresión unaria.</returns>
            public override T Accept<T>(IExprVisitor<T> visitor)
            {
                return visitor.VisitUnary(this);
            }
        }

        /// <summary>
        /// Representa una expresión agrupada entre paréntesis en el lenguaje E-Painter.
        /// </summary>
        public class Grouping : Expr
        {
            /// <summary>
            /// Obtiene la expresión agrupada.
            /// </summary>
            public Expr Expression { get; }

            /// <summary>
            /// Inicializa una nueva instancia de la clase Grouping.
            /// </summary>
            /// <param name="expression">La expresión agrupada.</param>
            public Grouping(Expr expression)
            {
                Expression = expression;
            }

            /// <summary>
            /// Acepta un visitante para procesar esta expresión agrupada.
            /// </summary>
            /// <typeparam name="T">El tipo de retorno del visitante.</typeparam>
            /// <param name="visitor">El visitante que procesará esta expresión agrupada.</param>
            /// <returns>El resultado de procesar esta expresión agrupada.</returns>
            public override T Accept<T>(IExprVisitor<T> visitor)
            {
                return visitor.VisitGrouping(this);
            }
        }
        
        /// <summary>
        /// Representa una llamada a función en el lenguaje E-Painter.
        /// </summary>
        public class Call : Expr
        {
            /// <summary>
            /// Obtiene el nombre de la función a llamar.
            /// </summary>
            public string FunctionName { get; }
            
            /// <summary>
            /// Obtiene la lista de argumentos para la llamada a función.
            /// </summary>
            public List<Expr> Arguments { get; }

            /// <summary>
            /// Inicializa una nueva instancia de la clase Call.
            /// </summary>
            /// <param name="functionName">El nombre de la función a llamar.</param>
            /// <param name="args">La lista de argumentos para la llamada a función.</param>
            public Call(string functionName, List<Expr> args)
            {
                FunctionName = functionName;
                Arguments = args;
            }

            /// <summary>
            /// Acepta un visitante para procesar esta llamada a función.
            /// </summary>
            /// <typeparam name="T">El tipo de retorno del visitante.</typeparam>
            /// <param name="visitor">El visitante que procesará esta llamada a función.</param>
            /// <returns>El resultado de procesar esta llamada a función.</returns>
            public override T Accept<T>(IExprVisitor<T> visitor)
            {
                return visitor.VisitCall(this);
            }
        }
    }
}



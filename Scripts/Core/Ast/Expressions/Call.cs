using System.Collections.Generic;

namespace EPainter.Core
{
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
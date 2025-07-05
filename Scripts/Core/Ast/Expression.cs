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
    }

    
}




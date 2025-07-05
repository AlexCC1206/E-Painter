namespace EPainter.Core
{
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
}
namespace EPainter.Core
{
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
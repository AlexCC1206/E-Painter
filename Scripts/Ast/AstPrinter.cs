using System.Collections.Generic;
using System.Text;

namespace EPainter
{
    /// <summary>
    /// Clase que implementa los visitantes para imprimir representaciones en texto de expresiones y declaraciones.
    /// </summary>
    public class AstPrinter : Expr.IVisitor<string>, Stmt.IVisitor<string>
    {
        /// <summary>
        /// Genera una representación en texto de una expresión.
        /// </summary>
        /// <param name="expr">La expresión a imprimir.</param>
        /// <returns>Una cadena que representa la expresión.</returns>
        public string Print(Expr expr)
        {
            return expr.Accept(this);
        }

        /// <summary>
        /// Genera una representación en texto de una declaración.
        /// </summary>
        /// <param name="stmt">La declaración a imprimir.</param>
        /// <returns>Una cadena que representa la declaración.</returns>
        public string Print(Stmt stmt)
        {
            return stmt.Accept(this);
        }

        /// <summary>
        /// Genera una representación en texto de un programa completo compuesto por múltiples declaraciones.
        /// </summary>
        /// <param name="stmts">Lista de declaraciones del programa.</param>
        /// <returns>Una cadena que representa el programa.</returns>
        public string PrintProgram(List<Stmt> stmts)
        {
            var builder = new StringBuilder();
            foreach (var stmt in stmts)
            {
                builder.AppendLine(stmt.Accept(this));
            }

            return builder.ToString();
        }

        /// <summary>
        /// Visita una expresión binaria y genera su representación en texto.
        /// </summary>
        /// <param name="expr">La expresión binaria.</param>
        /// <returns>Una cadena que representa la expresión binaria.</returns>
        public string VisitBinaryExpr(Expr.Binary expr)
        {
            return Parenthesize(expr.Op.Lexeme, expr.Left, expr.Right);
        }

        /// <summary>
        /// Visita una expresión unaria y genera su representación en texto.
        /// </summary>
        /// <param name="expr">La expresión unaria.</param>
        /// <returns>Una cadena que representa la expresión unaria.</returns>
        public string VisitUnaryExpr(Expr.Unary expr)
        {
            return Parenthesize(expr.Op.Lexeme, expr.Right);
        }

        /// <summary>
        /// Visita una expresión de agrupación y genera su representación en texto.
        /// </summary>
        /// <param name="expr">La expresión de agrupación.</param>
        /// <returns>Una cadena que representa la expresión de agrupación.</returns>
        public string VisitGroupingExpr(Expr.Grouping expr)
        {
            return Parenthesize("group", expr.Expression);
        }

        /// <summary>
        /// Visita una expresión literal y genera su representación en texto.
        /// </summary>
        /// <param name="expr">La expresión literal.</param>
        /// <returns>Una cadena que representa el valor literal.</returns>
        public string VisitLiteralExpr(Expr.Literal expr)
        {
            if (expr.Value is string str)
            {
                return $"\"{str}\"";
            }

            if (expr.Value == null)
            {
                return "null";
            }

            return expr.Value.ToString();
        }

        /// <summary>
        /// Visita una expresión de variable y genera su representación en texto.
        /// </summary>
        /// <param name="expr">La expresión de variable.</param>
        /// <returns>Una cadena que representa la variable.</returns>
        public string VisitVariableExpr(Expr.Variable expr)
        {
            return expr.Name.Lexeme;
        }

        public string VisitLogicalExpr(Expr.Logical expr)
        {
            return Parenthesize(expr.Op.Lexeme, expr.Left, expr.Right);
        }

        /// <summary>
        /// Visita una llamada a función y genera su representación en texto.
        /// </summary>
        /// <param name="expr">La expresión de llamada a función.</param>
        /// <returns>Una cadena que representa la llamada a función.</returns>
        public string VisitCallExpr(Expr.Call expr)
        {
            var builder = new StringBuilder();
            builder.Append($"{expr.Callee}(");

            for (var i = 0; i < expr.Arguments.Count; i++)
            {
                if (i > 0) builder.Append(", ");
                builder.Append(expr.Arguments[i].Accept(this));
            }

            builder.Append(")");
            return builder.ToString();
        }

        /// <summary>
        /// Visita una declaración de tipo Spawn y genera su representación en texto.
        /// </summary>
        /// <param name="stmt">La declaración Spawn.</param>
        /// <returns>Una cadena que representa la declaración Spawn.</returns>
        public string VisitSpawnStmt(Stmt.Spawn stmt)
        {
            return $"Spawn(dirX: {stmt.X.Accept(this)}, dirY: {stmt.Y.Accept(this)})";
        }

        /// <summary>
        /// Visita una declaración de tipo Color y genera su representación en texto.
        /// </summary>
        /// <param name="stmt">La declaración Color.</param>
        /// <returns>Una cadena que representa la declaración Color.</returns>
        public string VisitColorStmt(Stmt.Color stmt)
        {
            return $"Color(color: {stmt.ColorName})";
        }

        /// <summary>
        /// Visita una declaración de tipo Size y genera su representación en texto.
        /// </summary>
        /// <param name="stmt">La declaración Size.</param>
        /// <returns>Una cadena que representa la declaración Size.</returns>
        public string VisitSizeStmt(Stmt.Size stmt)
        {
            return $"Size(size: {stmt.SizeValue.Accept(this)})";
        }

        /// <summary>
        /// Visita una declaración de tipo DrawLine y genera su representación en texto.
        /// </summary>
        /// <param name="stmt">La declaración DrawLine.</param>
        /// <returns>Una cadena que representa la declaración DrawLine.</returns>
        public string VisitDrawLineStmt(Stmt.DrawLine stmt)
        {
            return $"DrawLine(dirX: {stmt.DirX.Accept(this)}, dirY:{stmt.DirY.Accept(this)}, distance: {stmt.Distance.Accept(this)})";
        }

        /// <summary>
        /// Visita una declaración de tipo DrawCircle y genera su representación en texto.
        /// </summary>
        /// <param name="stmt">La declaración DrawCircle.</param>
        /// <returns>Una cadena que representa la declaración DrawCircle.</returns>
        public string VisitDrawCircleStmt(Stmt.DrawCircle stmt)
        {
            return $"DrawCircle(dirX: {stmt.DirX.Accept(this)}, dirY: {stmt.DirY.Accept(this)}, radius: {stmt.Radius.Accept(this)})";
        }

        /// /// <summary>
        /// Visita una declaración de tipo DrawRectangle y genera su representación en texto.
        /// </summary>
        /// <param name="stmt">La declaración DrawRectangle.</param>
        /// <returns>Una cadena que representa la declaración DrawRectangle.</returns>
        public string VisitDrawRectangleStmt(Stmt.DrawRectangle stmt)
        {
            return $"DrawRectangle(dirX: {stmt.DirX.Accept(this)}, dirY: {stmt.DirY.Accept(this)}, distance: {stmt.Distance.Accept(this)}, width: {stmt.Width.Accept(this)}, height: {stmt.Height.Accept(this)})";
        }

        /// <summary>
        /// Visita una declaración de tipo Fill y genera su representación en texto.
        /// </summary>
        /// <param name="stmt">La declaración Fill.</param>
        /// <returns>Una cadena que representa la declaración Fill.</returns>
        public string VisitFillStmt(Stmt.Fill stmt)
        {
            return $"Fill()";
        }

        /// <summary>
        /// Visita una declaración de asignación y genera su representación en texto.
        /// </summary>
        /// <param name="stmt">La declaración de asignación.</param>
        /// <returns>Una cadena que representa la asignación.</returns>
        public string VisitAssignmentStmt(Stmt.Assignment stmt)
        {
            return $"Assign({stmt.Name.Lexeme} <- {stmt.Value.Accept(this)})";
        }

        /// <summary>
        /// Visita una declaración de etiqueta y genera su representación en texto.
        /// </summary>
        /// <param name="stmt">La declaración de etiqueta.</param>
        /// <returns>Una cadena que representa la etiqueta.</returns>
        public string VisitLabelStmt(Stmt.Label stmt)
        {
            return $"{stmt.Name.Lexeme}";
        }

        /// <summary>
        /// Visita una declaración de tipo Goto y genera su representación en texto.
        /// </summary>
        /// <param name="stmt">La declaración Goto.</param>
        /// <returns>Una cadena que representa la declaración Goto.</returns>
        public string VisitGotoStmt(Stmt.Goto stmt)
        {
            return $"Goto [{stmt.label.Lexeme}] ({stmt.Condition.Accept(this)})";
        }

        /// <summary>
        /// Genera una representación en texto de una expresión con formato de paréntesis.
        /// </summary>
        /// <param name="name">El nombre de la operación o expresión.</param>
        /// <param name="exprs">Las expresiones contenidas.</param>
        /// <returns>Una cadena que representa la expresión con formato de paréntesis.</returns>
        public string Parenthesize(string name, params Expr[] exprs)
        {
            var builder = new StringBuilder();
            builder.Append('(').Append(name);

            foreach (var expr in exprs)
            {
                builder.Append(' ').Append(expr.Accept(this));
            }

            builder.Append(')');
            return builder.ToString();
        }
    }
}
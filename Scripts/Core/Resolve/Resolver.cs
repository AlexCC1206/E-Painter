using System;
using System.Collections.Generic;
using System.Linq;
using static EPainter.Core.Expr;
using static EPainter.Core.Stmt;

namespace EPainter.Core
{
    public class Resolver : IExprVisitor<object>, IStmtVisitor<object>
    {
        private Interpreter interpreter;
        private Stack<Dictionary<string, bool>> Scopes = new Stack<Dictionary<string, bool>>();
        private int CurrentLine = 0;
        private bool hasSpawn = false;
        private bool isFirstStatement = true;
        private int spawnCount = 0;
        private int canvasSize; // Tamaño del canvas para verificar límites

        public Resolver(Interpreter interpreter, int canvasSize)
        {
            this.interpreter = interpreter;
            this.canvasSize = canvasSize;
        }

        public void Resolve(List<Stmt> statements)
        {
            // Reiniciar variables de control para una nueva ejecución
            hasSpawn = false;
            isFirstStatement = true;
            spawnCount = 0;
            
            foreach (var stmt in statements)
            {
                try
                {
                    CurrentLine++;
                    Resolve(stmt);
                    if (isFirstStatement)
                    {
                        isFirstStatement = false;
                    }
                }
                catch (Exception ex)
                {
                    // Convertir excepciones en errores reportados
                    ErrorReporter.SemanticError(CurrentLine, ex.Message);
                }
            }
            
            // Verificar que haya un comando Spawn
            if (!hasSpawn)
            {
                ErrorReporter.SemanticError(1, "El programa debe comenzar con un comando Spawn(x, y)");
            }
        }

        private void Resolve(Stmt stmt)
        {
            stmt.Accept(this);
        }

        private void Resolve(Expr expr)
        {
            expr.Accept(this);
        }

        private void BeginScope()
        {
            Scopes.Push(new Dictionary<string, bool>());
        }

        private void EndScope()
        {
            Scopes.Pop();
        }

        private void Declare(string name)
        {
            if (Scopes.Count == 0)
            {
                return;
            }

            var scope = Scopes.Peek();
            if (scope.ContainsKey(name))
            {
                ReportSemanticError($"Redefinición de variable '{name}' en el mismo ámbito.");
            }

            scope[name] = false;
        }

        private void Define(string name)
        {
            if (Scopes.Count == 0)
            {
                return;
            }

            var scope = Scopes.Peek();
            scope[name] = true;
        }
        private void ResolveLocal(string name, Expr expr)
        {
            for (int i = 0; i < Scopes.Count; i++)
            {
                if (Scopes.ElementAt(i).ContainsKey(name))
                {
                    return;
                }
            }

            ReportSemanticError($"Variable '{name}' no declarada.");
        }

        public object VisitLiteral(Literal expr)
        {
            return null;
        }

        public object VisitVariable(Variable expr)
        {
            if (Scopes.Count > 0 && Scopes.Peek().ContainsKey(expr.Name))
            {
                var isInitialize = Scopes.Peek()[expr.Name];
                if (!isInitialize)
                {
                    ReportSemanticError($"Variable '{expr.Name}' usada antes de asignarle valor.");
                }
            }
            else if (Scopes.Count == 0)
            {
                if (interpreter.Labels.ContainsKey(expr.Name))
                {
                    return null;
                }
            }

            ResolveLocal(expr.Name, expr);
            return null;
        }

        public object VisitUnary(Unary expr)
        {
            Resolve(expr.Right);
            
            // Verificación de tipos para operadores unarios
            switch (expr.Op.Type)
            {
                case TokenType.MIN: // Operador unario - (negación numérica)
                    CheckNumericOperand(expr.Op, expr.Right);
                    break;
            }
            
            return null;
        }

        public object VisitBinary(Binary expr)
        {
            Resolve(expr.Left);
            Resolve(expr.Right);
            
            // Verificación de tipos en operaciones
            switch (expr.Op.Type)
            {
                // Operadores aritméticos (requieren números)
                case TokenType.SUM:
                case TokenType.MIN:
                case TokenType.MULT:
                case TokenType.DIV:
                case TokenType.MOD:
                case TokenType.POW:
                    CheckNumericOperands(expr.Op, expr.Left, expr.Right);
                    break;
                
                // Operadores de comparación (requieren números)
                case TokenType.GREATER:
                case TokenType.GREATER_EQUAL:
                case TokenType.LESS:
                case TokenType.LESS_EQUAL:
                    CheckNumericOperands(expr.Op, expr.Left, expr.Right);
                    break;
                
                // Operadores de igualdad (permiten cualquier tipo pero deben ser iguales)
                case TokenType.EQUAL_EQUAL:
                case TokenType.BANG_EQUAL:
                    CheckMatchingTypes(expr.Op, expr.Left, expr.Right);
                    break;
                
                // Operadores lógicos (requieren booleanos)
                case TokenType.AND:
                case TokenType.OR:
                    CheckBooleanOperands(expr.Op, expr.Left, expr.Right);
                    break;
            }
            
            return null;
        }

        public object VisitGrouping(Grouping expr)
        {
            Resolve(expr.Expression);
            return null;
        }

        public object VisitCall(Call expr)
        {
            switch (expr.FunctionName)
            {
                case "GetActualX":
                case "GetActualY":
                case "GetCanvasSize":
                    if (expr.Arguments.Count != 0)
                        throw new Exception($"Línea {CurrentLine}: Función '{expr.FunctionName}' no acepta argumentos.");
                    break;

                case "IsBrushColor":
                    if (expr.Arguments.Count != 1)
                        throw new Exception($"Línea {CurrentLine}: Función '{expr.FunctionName}' requiere 1 argumento: color");
                    
                    // Validar tipo color (string)
                    CheckColorArgument(expr.FunctionName, expr.Arguments[0], 1);
                    break;

                case "IsBrushSize":
                    if (expr.Arguments.Count != 1)
                        throw new Exception($"Línea {CurrentLine}: Función '{expr.FunctionName}' requiere 1 argumento: tamaño");
                    
                    // Validar tipo numérico
                    CheckNumericArgument(expr.FunctionName, expr.Arguments[0], 1);
                    break;

                case "IsCanvasColor":
                    if (expr.Arguments.Count != 3)
                        throw new Exception($"Línea {CurrentLine}: Función '{expr.FunctionName}' requiere 3 argumentos: color, dx, dy.");

                    // Validar tipos: color, número, número
                    CheckColorArgument(expr.FunctionName, expr.Arguments[0], 1);
                    CheckNumericArgument(expr.FunctionName, expr.Arguments[1], 2);
                    CheckNumericArgument(expr.FunctionName, expr.Arguments[2], 3);
                    break;

                case "GetColorCount":
                    if (expr.Arguments.Count != 5)
                        throw new Exception($"Línea {CurrentLine}: Función '{expr.FunctionName}' requiere 5 argumentos: color, x1, y1, x2, y2.");

                    // Validar tipos: color, número, número, número, número
                    CheckColorArgument(expr.FunctionName, expr.Arguments[0], 1);
                    CheckNumericArgument(expr.FunctionName, expr.Arguments[1], 2);
                    CheckNumericArgument(expr.FunctionName, expr.Arguments[2], 3);
                    CheckNumericArgument(expr.FunctionName, expr.Arguments[3], 4);
                    CheckNumericArgument(expr.FunctionName, expr.Arguments[4], 5);
                    break;

                default:
                    throw new Exception($"Línea {CurrentLine}: Función '{expr.FunctionName}' no está definida.");
            }

            // Resolver cada argumento (asegurarnos que todos los argumentos son analizados)
            foreach (var arg in expr.Arguments)
            {
                Resolve(arg);
            }

            return null;
        }

        public object VisitAssignment(Assignment stmt)
        {
            Resolve(stmt.Value);

            Declare(stmt.Name);
            Define(stmt.Name);

            return null;
        }

        public object VisitSpawn(Spawn stmt)
        {
            // Verificar si es el primer comando Spawn
            spawnCount++;
            
            // Verificar si hay más de un comando Spawn
            if (spawnCount > 1)
            {
                ReportSemanticError("Solo puede haber un único comando 'Spawn' en todo el código.");
            }
            
            // Verificar si es la primera instrucción del programa
            if (!isFirstStatement)
            {
                ReportSemanticError("El programa debe comenzar con un comando Spawn(x, y)");
            }
            
            // Marcar que se ha encontrado un Spawn
            hasSpawn = true;
            
            // Continuar con la resolución normal
            Resolve(stmt.X);
            Resolve(stmt.Y);
            
            // Verificar que las coordenadas estén dentro de los límites del lienzo
            // usando un enfoque similar al del intérprete
            CheckCanvasLimits(stmt.X, stmt.Y);
            
            return null;
        }

        public object VisitColor(Color stmt)
        {
            Resolve(stmt.ColorName);
            return null;
        }

        public object VisitSize(Size stmt)
        {
            Resolve(stmt.SizeValue);
            
            // Verificar que el tamaño del pincel sea positivo y no excesivamente grande
            if (stmt.SizeValue is Literal sizeLit && sizeLit.Value != null)
            {
                int size = Convert.ToInt32(sizeLit.Value);
                if (size <= 0)
                {
                    ReportSemanticError($"El tamaño del pincel debe ser mayor que cero ({size}).");
                }
                else if (size > 100) // Valor arbitrario pero razonable para detectar errores
                {
                    throw new Exception($"Línea {CurrentLine}: El tamaño del pincel es extremadamente grande ({size}). Posible error de programación.");
                }
            }
            
            return null;
        }

        public object VisitDrawLine(DrawLine stmt)
        {
            Resolve(stmt.DirX);
            Resolve(stmt.DirY);
            Resolve(stmt.Distance);
            
            // Verificar que la dirección sea válida (debe ser -1, 0 o 1)
            if (stmt.DirX is Literal dirX && stmt.DirY is Literal dirY)
            {
                int dx = Convert.ToInt32(dirX.Value);
                int dy = Convert.ToInt32(dirY.Value);
                
                if ((dx == 0 && dy == 0) || Math.Abs(dx) > 1 || Math.Abs(dy) > 1)
                {
                    ReportSemanticError($"Dirección inválida para DrawLine. Los valores deben ser -1, 0 o 1, pero no ambos 0.");
                }
            }
            
            // Verificar que la distancia no sea excesiva
            if (stmt.Distance is Literal distLit && distLit.Value != null)
            {
                int distance = Convert.ToInt32(distLit.Value);
                if (distance < 0)
                {
                    throw new Exception($"Línea {CurrentLine}: La distancia no puede ser negativa ({distance}).");
                }
                else if (distance > 1000) // Valor arbitrario pero razonable para detectar errores
                {
                    throw new Exception($"Línea {CurrentLine}: La distancia es extremadamente grande ({distance}). Posible error de programación.");
                }
            }
            
            return null;
        }

        public object VisitDrawCircle(DrawCircle stmt)
        {
            Resolve(stmt.DirX);
            Resolve(stmt.DirY);
            Resolve(stmt.Radius);
            
            // Verificar que los valores de dirección sean válidos
            if (stmt.DirX is Literal dirX && stmt.DirY is Literal dirY)
            {
                int dx = Convert.ToInt32(dirX.Value);
                int dy = Convert.ToInt32(dirY.Value);
                
                if (dx == 0 && dy == 0)
                {
                    throw new Exception($"Línea {CurrentLine}: Al menos una de las direcciones (dirX, dirY) debe ser distinta de cero para DrawCircle.");
                }
            }
            
            // Verificar que el radio no sea negativo o excesivamente grande
            if (stmt.Radius is Literal radiusLit && radiusLit.Value != null)
            {
                int radius = Convert.ToInt32(radiusLit.Value);
                if (radius < 0)
                {
                    throw new Exception($"Línea {CurrentLine}: El radio no puede ser negativo ({radius}).");
                }
                else if (radius > 500) // Valor arbitrario pero razonable para detectar errores
                {
                    throw new Exception($"Línea {CurrentLine}: El radio es extremadamente grande ({radius}). Posible error de programación.");
                }
            }
            
            return null;
        }

        public object VisitDrawRectangle(DrawRectangle stmt)
        {
            Resolve(stmt.DirX);
            Resolve(stmt.DirY);
            Resolve(stmt.Distance);
            Resolve(stmt.Width);
            Resolve(stmt.Height);
            
            // Verificar que los valores de dirección sean válidos
            if (stmt.DirX is Literal dirX && stmt.DirY is Literal dirY)
            {
                int dx = Convert.ToInt32(dirX.Value);
                int dy = Convert.ToInt32(dirY.Value);
                
                if (Math.Abs(dx) > 1 || Math.Abs(dy) > 1)
                {
                    throw new Exception($"Línea {CurrentLine}: Las direcciones para DrawRectangle deben estar entre -1 y 1.");
                }
            }
            
            // Verificar que la distancia no sea negativa o excesivamente grande
            if (stmt.Distance is Literal distLit && distLit.Value != null)
            {
                int distance = Convert.ToInt32(distLit.Value);
                if (distance < 0)
                {
                    throw new Exception($"Línea {CurrentLine}: La distancia no puede ser negativa ({distance}).");
                }
                else if (distance > 1000) // Valor arbitrario pero razonable para detectar errores
                {
                    throw new Exception($"Línea {CurrentLine}: La distancia es extremadamente grande ({distance}). Posible error de programación.");
                }
            }
            
            // Verificar que el ancho y alto sean positivos y no excesivamente grandes
            if (stmt.Width is Literal widthLit && widthLit.Value != null)
            {
                int width = Convert.ToInt32(widthLit.Value);
                if (width <= 0)
                {
                    throw new Exception($"Línea {CurrentLine}: El ancho debe ser mayor que cero ({width}).");
                }
                else if (width > 1000) // Valor arbitrario pero razonable para detectar errores
                {
                    throw new Exception($"Línea {CurrentLine}: El ancho es extremadamente grande ({width}). Posible error de programación.");
                }
            }
            
            if (stmt.Height is Literal heightLit && heightLit.Value != null)
            {
                int height = Convert.ToInt32(heightLit.Value);
                if (height <= 0)
                {
                    throw new Exception($"Línea {CurrentLine}: La altura debe ser mayor que cero ({height}).");
                }
                else if (height > 1000) // Valor arbitrario pero razonable para detectar errores
                {
                    throw new Exception($"Línea {CurrentLine}: La altura es extremadamente grande ({height}). Posible error de programación.");
                }
            }
            
            return null;
        }

        public object VisitFill(Fill stmt)
        {
            return null;
        }
        public object VisitGoto(Goto stmt)
        {
            if (!interpreter.Labels.ContainsKey(stmt.LabelName))
            {
                throw new Exception($"Línea {CurrentLine}: Salto condicional a etiqueta inexistente '{stmt.LabelName}'.");
            }

            // No verificamos directamente el resultado de Resolve, simplemente revisamos la condición
            Resolve(stmt.Condition);

            // En lugar de evaluar la condición aquí (que sería en tiempo de compilación),
            // solo verificamos que sea una expresión que pueda evaluarse a booleano en tiempo de ejecución
            return null;
        }

        public object VisitLabel(Label stmt)
        {
            if (!char.IsLetter(stmt.Name[0]))
            {
                throw new Exception($"Línea {CurrentLine}: Las etiquetas deben comenzar con una letra.");
            }

            return null;
        }

        private object Visit(Expr expr)
        {
            return expr.Accept(this);
        }

        private object Visit(Stmt stmt)
        {
            return stmt.Accept(this);
        }
        
        // Métodos para verificación de tipos
        
        /// <summary>
        /// Verifica que ambos operandos sean números.
        /// </summary>
        private void CheckNumericOperands(Token op, Expr left, Expr right)
        {
            // En un análisis estático no podemos saber el valor exacto en tiempo de ejecución,
            // pero podemos verificar expresiones literales para validación temprana
            if (left is Literal leftLit && right is Literal rightLit)
            {
                object leftVal = leftLit.Value;
                object rightVal = rightLit.Value;
                
                bool leftIsNum = leftVal is double || leftVal is int || leftVal is float;
                bool rightIsNum = rightVal is double || rightVal is int || rightVal is float;
                
                if (!leftIsNum || !rightIsNum)
                {
                    throw new Exception($"Línea {CurrentLine}: Operador '{op.Lexeme}' requiere operandos numéricos.");
                }
            }
            // Para expresiones no literales, solo podemos verificar en ejecución
        }
        
        /// <summary>
        /// Verifica que el operando sea un número (para operaciones unarias).
        /// </summary>
        private void CheckNumericOperand(Token op, Expr operand)
        {
            // Verificación para literales
            if (operand is Literal lit)
            {
                object val = lit.Value;
                bool isNum = val is double || val is int || val is float;
                
                if (!isNum)
                {
                    throw new Exception($"Línea {CurrentLine}: Operador '{op.Lexeme}' requiere un operando numérico.");
                }
            }
            // Para expresiones no literales, solo podemos verificar en ejecución
        }
        
        /// <summary>
        /// Verifica que ambos operandos sean booleanos.
        /// </summary>
        private void CheckBooleanOperands(Token op, Expr left, Expr right)
        {
            // Verificación para literales
            if (left is Literal leftLit && right is Literal rightLit)
            {
                object leftVal = leftLit.Value;
                object rightVal = rightLit.Value;
                
                if (!(leftVal is bool) || !(rightVal is bool))
                {
                    throw new Exception($"Línea {CurrentLine}: Operador '{op.Lexeme}' requiere operandos booleanos.");
                }
            }
        }
        
        /// <summary>
        /// Verifica que ambos operandos sean del mismo tipo.
        /// </summary>
        private void CheckMatchingTypes(Token op, Expr left, Expr right)
        {
            // Verificación para literales
            if (left is Literal leftLit && right is Literal rightLit)
            {
                object leftVal = leftLit.Value;
                object rightVal = rightLit.Value;
                
                if ((leftVal is bool && !(rightVal is bool)) ||
                    ((leftVal is double || leftVal is int || leftVal is float) && 
                     !(rightVal is double || rightVal is int || rightVal is float)) ||
                    (leftVal is string && !(rightVal is string)))
                {
                    throw new Exception($"Línea {CurrentLine}: Operador '{op.Lexeme}' requiere operandos del mismo tipo.");
                }
            }
        }
        
        /// <summary>
        /// Verifica si una expresión es de tipo color (string).
        /// </summary>
        private void CheckColorArgument(string functionName, Expr expr, int argPosition)
        {
            if (expr is Literal literal && literal.Value != null)
            {
                if (!(literal.Value is string))
                {
                    throw new Exception($"Línea {CurrentLine}: El argumento {argPosition} de '{functionName}' debe ser un color (cadena de texto).");
                }
            }
            // Para expresiones no literales, se validará en tiempo de ejecución
        }

        /// <summary>
        /// Verifica si una expresión es de tipo numérico.
        /// </summary>
        private void CheckNumericArgument(string functionName, Expr expr, int argPosition)
        {
            if (expr is Literal literal && literal.Value != null)
            {
                object val = literal.Value;
                if (!(val is double || val is int || val is float))
                {
                    throw new Exception($"Línea {CurrentLine}: El argumento {argPosition} de '{functionName}' debe ser un número.");
                }
            }
            // Para expresiones no literales, se validará en tiempo de ejecución
        }
        
        /// <summary>
        /// Extrae el valor entero de una expresión literal si es posible.
        /// Devuelve null si la expresión no es una literal o no puede convertirse a entero.
        /// </summary>
        private int? GetLiteralIntValue(Expr expr)
        {
            if (expr is Literal literal && literal.Value != null)
            {
                if (literal.Value is int intValue)
                {
                    return intValue;
                }
                else if (literal.Value is double doubleValue)
                {
                    int intVal = (int)doubleValue;
                    // Asegurarse de que la conversión no pierde información
                    if (intVal == doubleValue)
                    {
                        return intVal;
                    }
                }
                else if (literal.Value is float floatValue)
                {
                    int intVal = (int)floatValue;
                    // Asegurarse de que la conversión no pierde información
                    if (intVal == floatValue)
                    {
                        return intVal;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Verifica si una coordenada literal está dentro de los límites del lienzo.
        /// Comprueba tanto el límite inferior (0) como el superior (canvasSize-1).
        /// </summary>
        private bool IsValidPosition(int x, int y)
        {
            return x >= 0 && x < canvasSize && y >= 0 && y < canvasSize;
        }
        
        /// <summary>
        /// Extrae valores de coordenadas de expresiones literales y verifica si están dentro de los límites.
        /// Similar al enfoque utilizado en el Interpreter.
        /// </summary>
        private void CheckCanvasLimits(Expr xExpr, Expr yExpr)
        {
            // Solo verificamos coordenadas que sean literales constantes
            int? x = GetLiteralIntValue(xExpr);
            int? y = GetLiteralIntValue(yExpr);
            
            if (x.HasValue && y.HasValue)
            {
                if (!IsValidPosition(x.Value, y.Value))
                {
                    throw new Exception($"Línea {CurrentLine}: Spawn position ({x.Value}, {y.Value}) out of bounds. Canvas es {canvasSize}x{canvasSize}.");
                }
            }
            else
            {
                // Si alguna coordenada no es literal, verificamos individualmente lo que podamos
                if (x.HasValue && (x.Value < 0 || x.Value >= canvasSize))
                {
                    throw new Exception($"Línea {CurrentLine}: Coordenada X ({x.Value}) fuera de límites. Debe estar entre 0 y {canvasSize-1}.");
                }
                
                if (y.HasValue && (y.Value < 0 || y.Value >= canvasSize))
                {
                    throw new Exception($"Línea {CurrentLine}: Coordenada Y ({y.Value}) fuera de límites. Debe estar entre 0 y {canvasSize-1}.");
                }
            }
        }
        
        // Método auxiliar para reportar errores semánticos
        private void ReportSemanticError(string message)
        {
            ErrorReporter.SemanticError(CurrentLine, message);
        }
    }

}
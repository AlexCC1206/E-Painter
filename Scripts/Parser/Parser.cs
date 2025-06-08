using System;
using System.Collections.Generic;

namespace EPainter
{
    /// <summary>
    /// Clase encargada de analizar una lista de tokens y convertirlos en una lista de declaraciones (statements).
    /// </summary>
    public class Parser
    {
        private List<Token> Tokens;
        private int current = 0;

        /// <summary>
        /// Constructor de la clase Parser.
        /// </summary>
        /// <param name="tokens">Lista de tokens a analizar.</param>
        public Parser(List<Token> tokens)
        {
            Tokens = tokens;
        }

        /// <summary>
        /// Inicia el proceso de análisis y devuelve una lista de declaraciones.
        /// </summary>
        /// <returns>Lista de declaraciones (statements).</returns>
        public List<Stmt> Parse()
        {
            var statements = new List<Stmt>();

            while (!IsAtEnd())
            {
                statements.Add(Declaration());
            }

            return statements;
        }

        /// <summary>
        /// Analiza una declaración y devuelve el statement correspondiente.
        /// </summary>
        /// <returns>Un statement correspondiente a la declaración.</returns>
        private Stmt Declaration()
        {
            try
            {
                if (Match(TokenType.SPAWN)) return SpawnStatement();
                if (Match(TokenType.COLOR)) return ColorStatement();
                if (Match(TokenType.SIZE)) return SizeStatement();
                if (Match(TokenType.DRAW_LINE)) return DrawLineStatement();
                if (Match(TokenType.DRAW_CIRCLE)) return DrawCircleStatement();
                if (Match(TokenType.DRAW_RECTANGLE)) return DrawRectangleStatement();
                if (Match(TokenType.FILL)) return FillStatement();
                if (Match(TokenType.GOTO)) return GotoStatement();

                return AssignmenStatement();
            }
            catch (ParseError)
            {
                Syncronize();
                return null;
            }
        }

        /// <summary>
        /// Analiza una declaración de tipo 'Spawn'.
        /// </summary>
        /// <returns>Un statement de tipo Spawn.</returns>
        private Stmt SpawnStatement()
        {
            Consume(TokenType.LEFT_PAREN, "Expect '(' after 'Spawn'.");
            Expr x = Expression();
            Consume(TokenType.COMMA, "Expect ',' between x and y coordinates.");
            Expr y = Expression();
            Consume(TokenType.RIGHT_PAREN, "Expect ')' after Spawn arguments.");
            return new Stmt.Spawn(x, y);
        }

        /// <summary>
        /// Analiza una declaración de tipo 'Color'.
        /// </summary>
        /// <returns>Un statement de tipo Color.</returns>
        private Stmt ColorStatement()
        {
            Consume(TokenType.LEFT_PAREN, "Expect '(' after 'Color'.");
            Token color = Consume(TokenType.COLOR_LITERAL, "Expect color literal.");
            Consume(TokenType.RIGHT_PAREN, "Expect ')' after color.");
            return new Stmt.Color(color.Lexeme);
        }

        /// <summary>
        /// Analiza una declaración de tipo 'Size'.
        /// </summary>
        /// <returns>Un statement de tipo Size.</returns>
        private Stmt SizeStatement()
        {
            Consume(TokenType.LEFT_PAREN, "Expect '(' after 'Size'");
            Expr size = Expression();
            Consume(TokenType.RIGHT_PAREN, "Expect ')' after size.");
            return new Stmt.Size(size);
        }

        /// <summary>
        /// Analiza una declaración de tipo 'DrawLine'.
        /// </summary>
        /// <returns>Un statement de tipo DrawLine.</returns>
        private Stmt DrawLineStatement()
        {
            Consume(TokenType.LEFT_PAREN, "Expect '(' after 'DrawLine'.");
            Expr dirX = Expression();
            Consume(TokenType.COMMA, "Expect ',' after direction X.");
            Expr dirY = Expression();
            Consume(TokenType.COMMA, "Expect ',' after direction Y.");
            Expr distance = Expression();
            Consume(TokenType.RIGHT_PAREN, "Expect ')' after DrawLine arguments.");
            return new Stmt.DrawLine(dirX, dirY, distance);
        }

        /// <summary>
        /// Analiza una declaración de tipo 'DrawCircle'.
        /// </summary>
        /// <returns>Un statement de tipo DrawCircle.</returns>
        private Stmt DrawCircleStatement()
        {
            Consume(TokenType.LEFT_PAREN, "Expect '(' after 'DrawCircle'.");
            Expr dirX = Expression();
            Consume(TokenType.COMMA, "Expect ',' after direction X.");
            Expr dirY = Expression();
            Consume(TokenType.COMMA, "Expect ',' after direction Y.");
            Expr radius = Expression();
            Consume(TokenType.RIGHT_PAREN, "Expect ')' after DrawCircle arguments.");
            return new Stmt.DrawCircle(dirX, dirY, radius);
        }

        /// <summary>
        /// Analiza una declaración de tipo 'DrawRectangle'.
        /// </summary>
        /// <returns>Un statement de tipo DrawRectangle.</returns>
        private Stmt DrawRectangleStatement()
        {
            Consume(TokenType.LEFT_PAREN, "Expect '(' after 'DrawRectangle'.");
            Expr dirX = Expression();
            Consume(TokenType.COMMA, "Expect ',' after direction X.");
            Expr dirY = Expression();
            Consume(TokenType.COMMA, "Expect ',' after direction Y.");
            Expr distance = Expression();
            Consume(TokenType.COMMA, "Expect ',' after distance.");
            Expr width = Expression();
            Consume(TokenType.COMMA, "Expect ',' after width.");
            Expr height = Expression();
            Consume(TokenType.RIGHT_PAREN, "Expect ')' after DrawRectangle arguments.");
            return new Stmt.DrawRectangle(dirX, dirY, distance, width, height);
        }

        /// <summary>
        /// Analiza una declaración de tipo 'Fill'.
        /// </summary>
        /// <returns>Un statement de tipo Fill.</returns>
        private Stmt FillStatement()
        {
            Consume(TokenType.LEFT_PAREN, "Expect '(' after 'Fill'.");
            Consume(TokenType.RIGHT_PAREN, "Expect ')' after Fill.");
            return new Stmt.Fill();
        }

        /// <summary>
        /// Analiza una declaración de tipo 'Goto'.
        /// </summary>
        /// <returns>Un statement de tipo Goto.</returns>
        private Stmt GotoStatement()
        {
            Consume(TokenType.LEFT_BRACKET, "Expect '[' after  'GoTo'.");
            Token label = Consume(TokenType.IDENTIFIER, "Expect label name.");
            Consume(TokenType.RIGHT_BRACKET, "Expect ']' after label name.");
            Consume(TokenType.LEFT_PAREN, "Expect '(' before condition.");
            Expr condition = Expression();
            Consume(TokenType.RIGHT_PAREN, "Expect ')' after condition.");
            return new Stmt.Goto(label, condition);
        }

        /// <summary>
        /// Analiza una declaración de asignación.
        /// </summary>
        /// <returns>Un statement de tipo Assignment.</returns>
        private Stmt AssignmenStatement()
        {
            Token name = Consume(TokenType.IDENTIFIER, "Expect variable name.");
            Consume(TokenType.LEFT_ARROW, "Expect '<-' after variable name");
            Expr value = Expression();
            return new Stmt.Assignment(name, value);
        }

        /// <summary>
        /// Analiza una expresión.
        /// </summary>
        /// <returns>Una expresión.</returns>
        private Expr Expression()
        {
            return LogicalAnd();
        }

        /// <summary>
        /// Analiza una expresión lógica con el operador 'AND'.
        /// </summary>
        /// <returns>Una expresión lógica.</returns>
        private Expr LogicalAnd()
        {
            Expr expr = LogicalOr();

            while (Match(TokenType.AND))
            {
                Token op = Previous();
                Expr rigth = LogicalOr();
                expr = new Expr.Logical(expr, op, rigth);
            }

            return expr;
        }

        /// <summary>
        /// Analiza una expresión lógica con el operador 'OR'.
        /// </summary>
        /// <returns>Una expresión lógica.</returns>
        private Expr LogicalOr()
        {
            Expr expr = Equality();

            while (Match(TokenType.OR))
            {
                Token op = Previous();
                Expr rigth = Equality();
                expr = new Expr.Logical(expr, op, rigth);
            }

            return expr;
        }

        /// <summary>
        /// Analiza una expresión de igualdad.
        /// </summary>
        /// <returns>Una expresión de igualdad.</returns>
        private Expr Equality()
        {
            Expr expr = Comparison();

            while (Match(TokenType.EQUAL_EQUAL))
            {
                Token op = Previous();
                Expr right = Comparison();
                expr = new Expr.Binary(expr, op, right);
            }

            return expr;
        }

        /// <summary>
        /// Analiza una expresión de comparación.
        /// </summary>
        /// <returns>Una expresión de comparación.</returns>
        private Expr Comparison()
        {
            Expr expr = Term();

            while (Match(TokenType.GREATER, TokenType.GREATER_EQUAL, TokenType.LESS, TokenType.LESS_EQUAL))
            {
                Token op = Previous();
                Expr right = Term();
                expr = new Expr.Binary(expr, op, right);
            }

            return expr;
        }

        /// <summary>
        /// Analiza una expresión de términos (suma o resta).
        /// </summary>
        /// <returns>Una expresión de términos.</returns>
        private Expr Term()
        {
            Expr expr = Factor();

            while (Match(TokenType.MIN, TokenType.SUM))
            {
                Token op = Previous();
                Expr right = Factor();
                expr = new Expr.Binary(expr, op, right);
            }

            return expr;
        }

        /// <summary>
        /// Analiza una expresión de factores (multiplicación, división o módulo).
        /// </summary>
        /// <returns>Una expresión de factores.</returns>
        private Expr Factor()
        {
            Expr expr = Pow();

            while (Match(TokenType.MULT, TokenType.DIV, TokenType.MOD))
            {
                Token op = Previous();
                Expr right = Pow();
                expr = new Expr.Binary(expr, op, right);
            }

            return expr;
        }

        /// <summary>
        /// Analiza una expresión de potencia.
        /// </summary>
        /// <returns>Una expresión de potencia.</returns>
        private Expr Pow()
        {
            Expr Expr = Unary();

            while (Match(TokenType.POW))
            {
                Token op = Previous();
                Expr right = Primary();
                Expr = new Expr.Binary(Expr, op, right);
            }

            return Expr;
        }

        /// <summary>
        /// Analiza una expresión unaria (como un operador negativo).
        /// </summary>
        /// <returns>Una expresión unaria.</returns>
        private Expr Unary()
        {
            if (Match(TokenType.MIN))
            {
                Token op = Previous();
                Expr right = Unary();
                return new Expr.Unary(op, right);
            }

            return Primary();
        }

        /// <summary>
        /// Analiza una expresión primaria (números, literales, variables, etc.).
        /// </summary>
        /// <returns>Una expresión primaria.</returns>
        private Expr Primary()
        {
            if (Match(TokenType.NUMBER))
            {
                return new Expr.Literal(Previous().Literal);
            }

            if (Match(TokenType.COLOR_LITERAL))
            {
                return new Expr.Literal(Previous().Lexeme);
            }

            if (Match(TokenType.STRING))
            {
                return new Expr.Literal(Previous().Literal);
            }

            if (Match(TokenType.IDENTIFIER))
            {
                if (Match(TokenType.LEFT_PAREN))
                {
                    return FunctionCall(Previous());
                }
                return new Expr.Variable(Previous());
            }

            if (Match(TokenType.LEFT_PAREN))
            {
                Expr expr = Expression();
                Consume(TokenType.RIGHT_PAREN, "Expect ')' after expression");
                return new Expr.Grouping(expr);
            }

            throw Error(Peek(), "Expected expression");
        }

        /// <summary>
        /// Analiza una llamada a función.
        /// </summary>
        /// <param name="name">El token que representa el nombre de la función.</param>
        /// <returns>Una expresión de llamada a función.</returns>
        private Expr FunctionCall(Token name)
        {
            var arguments = new List<Expr>();

            if (!Check(TokenType.RIGHT_PAREN))
            {
                do
                {
                    arguments.Add(Expression());
                } while (Match(TokenType.COMMA));
            }

            Consume(TokenType.RIGHT_PAREN, "Expect ')' after function arguments");

            return new Expr.Call(name, arguments);
        }

        /// <summary>
        /// Verifica si el token actual coincide con alguno de los tipos especificados y avanza si es así.
        /// </summary>
        /// <param name="types">Tipos de tokens a verificar.</param>
        /// <returns>True si coincide, de lo contrario false.</returns>
        private bool Match(params TokenType[] types)
        {
            foreach (var type in types)
            {
                if (Check(type))
                {
                    Advance();
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Consume el token actual si coincide con el tipo esperado, de lo contrario lanza un error.
        /// </summary>
        /// <param name="type">El tipo de token esperado.</param>
        /// <param name="message">El mensaje de error si no coincide.</param>
        /// <returns>El token consumido.</returns>
        private Token Consume(TokenType type, string message)
        {
            if (Check(type)) return Advance();
            throw Error(Peek(), message);
        }

        /// <summary>
        /// Verifica si el token actual coincide con el tipo especificado.
        /// </summary>
        /// <param name="type">El tipo de token a verificar.</param>
        /// <returns>True si coincide, de lo contrario false.</returns>
        private bool Check(TokenType type)
        {
            if (IsAtEnd()) return false;
            return Peek().Type == type;
        }

        /// <summary>
        /// Avanza al siguiente token y devuelve el token anterior.
        /// </summary>
        /// <returns>El token anterior.</returns>
        private Token Advance()
        {
            if (!IsAtEnd()) current++;
            return Previous();
        }

        /// <summary>
        /// Verifica si se ha llegado al final de la lista de tokens.
        /// </summary>
        /// <returns>True si se ha llegado al final, de lo contrario false.</returns>
        private bool IsAtEnd()
        {
            return Peek().Type == TokenType.EOF;
        }

        /// <summary>
        /// Obtiene el token actual sin avanzar.
        /// </summary>
        /// <returns>El token actual.</returns>
        private Token Peek()
        {
            return Tokens[current];
        }

        /// <summary>
        /// Obtiene el token anterior.
        /// </summary>
        /// <returns>El token anterior.</returns>
        private Token Previous()
        {
            return Tokens[current - 1];
        }

        /// <summary>
        /// Genera un error de análisis con un mensaje específico.
        /// </summary>
        /// <param name="token">El token donde ocurrió el error.</param>
        /// <param name="message">El mensaje de error.</param>
        /// <returns>Un objeto ParseError.</returns>
        private ParseError Error(Token token, string message)
        {
            ErrorReporter.Error(token, message);
            return new ParseError();
        }

        /// <summary>
        /// Sincroniza el parser después de un error para continuar el análisis.
        /// </summary>
        private void Syncronize()
        {
            Advance();

            while (!IsAtEnd())
            {
                if (Previous().Type == TokenType.NEWLINE)
                {
                    return;
                }

                switch (Peek().Type)
                {
                    case TokenType.SPAWN:
                    case TokenType.COLOR:
                    case TokenType.SIZE:
                    case TokenType.DRAW_LINE:
                    case TokenType.DRAW_CIRCLE:
                    case TokenType.DRAW_RECTANGLE:
                    case TokenType.FILL:
                    case TokenType.GOTO:
                        return;
                }

                Advance();
            }
        }
    }
}
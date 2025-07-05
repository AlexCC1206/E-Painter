using System;
using System.Collections.Generic;

namespace EPainter.Core
{
    /// <summary>
    /// Analizador sintáctico que convierte tokens en un árbol de sintaxis abstracta (AST).
    /// </summary>
    public class Parser
    {
        #region Fields and Properties
        /// <summary>
        /// Lista de tokens a analizar.
        /// </summary>
        private List<Token> Tokens;

        /// <summary>
        /// Índice del token actual.
        /// </summary>
        private int Current = 0;

        private readonly List<string> Errors = new();
        public IEnumerable<string> GetErrors() => Errors;
        #endregion

        #region Constructor
        /// <summary>
        /// Inicializa una nueva instancia de la clase Parser.
        /// </summary>
        /// <param name="tokens">La lista de tokens a analizar.</param>
        public Parser(List<Token> tokens)
        {
            Tokens = tokens;
        }
        #endregion

        #region Parsing Methods
        /// <summary>
        /// Analiza los tokens y produce una lista de sentencias.
        /// </summary>
        /// <returns>Lista de sentencias que conforman el programa.</returns>
        public List<Stmt> Parse()
        {
            var statements = new List<Stmt>();

            while (!IsAtEnd())
            {
                var stmt = Declaration();
                if (stmt != null)
                {
                    statements.Add(stmt);
                }
            }

            return statements;
        }

        /// <summary>
        /// Analiza y devuelve la siguiente sentencia de declaración.
        /// </summary>
        /// <returns>Una nueva sentencia o null si ocurre un error.</returns>
        private Stmt Declaration()
        {
            try
            {
                while (Match(TokenType.NEWLINE)) ;

                if (IsAtEnd()) return null;

                if (Match(TokenType.SPAWN)) return SpawnStatement();
                if (Match(TokenType.COLOR)) return ColorStatement();
                if (Match(TokenType.SIZE)) return SizeStatement();
                if (Match(TokenType.DRAW_LINE)) return DrawLineStatement();
                if (Match(TokenType.DRAW_CIRCLE)) return DrawCircleStatement();
                if (Match(TokenType.DRAW_RECTANGLE)) return DrawRectangleStatement();
                if (Match(TokenType.FILL)) return FillStatement();
                if (Match(TokenType.GOTO)) return GoToStatement();

                if (Check(TokenType.IDENTIFIER))
                {
                    Advance();
                    return MaybeLabelOrAssignment();
                }

                throw Error(Peek(), "Unexpected token.");
            }
            catch
            {
                Synchronize();
                return null;
            }
        }
        #endregion

        #region Graphic Commands
        /// <summary>
        /// Analiza una sentencia Spawn.
        /// </summary>
        /// <returns>Una nueva sentencia Spawn.</returns>
        /// <exception cref="ParseError">Se lanza si la sintaxis es incorrecta.</exception>
        private Stmt SpawnStatement()
        {
            Consume(TokenType.LEFT_PAREN, "Expect '(' after 'Spawn'.");
            Expr x = Expression();
            Consume(TokenType.COMMA, "Expect ',' after int x.");
            Expr y = Expression();
            Consume(TokenType.RIGHT_PAREN, "Expect ')' after int y.");
            Consume(TokenType.NEWLINE, "Expected newline after 'Spawn(int x, int y)'.");
            return new Spawn(x, y);
        }

        /// <summary>
        /// Analiza una sentencia Color.
        /// </summary>
        /// <returns>Una nueva sentencia Color.</returns>
        /// <exception cref="ParseError">Se lanza si la sintaxis es incorrecta.</exception>
        private Stmt ColorStatement()
        {
            Consume(TokenType.LEFT_PAREN, "Expect '(' after 'Color'.");
            var color = Expression();
            Consume(TokenType.RIGHT_PAREN, "Expect ')' after string color.");
            Consume(TokenType.NEWLINE, "Expected newline after 'Color(string color)'.");
            return new Color(color);
        }

        /// <summary>
        /// Analiza una sentencia Size.
        /// </summary>
        /// <returns>Una nueva sentencia Size.</returns>
        /// <exception cref="ParseError">Se lanza si la sintaxis es incorrecta.</exception>
        private Stmt SizeStatement()
        {
            Consume(TokenType.LEFT_PAREN, "Expect '(' after 'Size'.");
            var size = Expression();
            Consume(TokenType.RIGHT_PAREN, "Expect ')' after int k.");
            Consume(TokenType.NEWLINE, "Expected newline after 'Size(int k)'.");
            return new Size(size);
        }

        /// <summary>
        /// Analiza una sentencia DrawLine.
        /// </summary>
        /// <returns>Una nueva sentencia DrawLine.</returns>
        /// <exception cref="ParseError">Se lanza si la sintaxis es incorrecta.</exception>
        private Stmt DrawLineStatement()
        {
            Consume(TokenType.LEFT_PAREN, "Expect '(' after 'DrawLine'.");
            Expr dirX = Expression();
            Consume(TokenType.COMMA, "Expect ',' after int dirX.");
            Expr dirY = Expression();
            Consume(TokenType.COMMA, "Expect ',' after int dirY.");
            Expr distance = Expression();
            Consume(TokenType.RIGHT_PAREN, "Expect ')' after int distance.");
            Consume(TokenType.NEWLINE, "Expected newline after 'DrawLine(int dirX, int dirY, int distance)'.");
            return new DrawLine(dirX, dirY, distance);
        }

        /// <summary>
        /// Analiza una sentencia DrawCircle.
        /// </summary>
        /// <returns>Una nueva sentencia DrawCircle.</returns>
        /// <exception cref="ParseError">Se lanza si la sintaxis es incorrecta.</exception>
        private Stmt DrawCircleStatement()
        {
            Consume(TokenType.LEFT_PAREN, "Expect '(' after 'DrawCircle'.");
            Expr dirX = Expression();
            Consume(TokenType.COMMA, "Expect ',' after int dirX.");
            Expr dirY = Expression();
            Consume(TokenType.COMMA, "Expect ',' after int dirY.");
            Expr radius = Expression();
            Consume(TokenType.RIGHT_PAREN, "Expect ')' after int radius.");
            Consume(TokenType.NEWLINE, "Expected newline after 'DrawCircle(int dirX, int dirY, int radius)'.");
            return new DrawCircle(dirX, dirY, radius);
        }


        /// <summary>
        /// Analiza una sentencia DrawRectangle.
        /// </summary>
        /// <returns>Una nueva sentencia DrawRectangle.</returns>
        /// <exception cref="ParseError">Se lanza si la sintaxis es incorrecta.</exception>
        private Stmt DrawRectangleStatement()
        {
            Consume(TokenType.LEFT_PAREN, "Expect '(' after 'DrawRectangle'.");
            Expr dirX = Expression();
            Consume(TokenType.COMMA, "Expect ',' after int dirX.");
            Expr dirY = Expression();
            Consume(TokenType.COMMA, "Expect ',' after int dirY.");
            Expr distance = Expression();
            Consume(TokenType.COMMA, "Expect ',' after int distance.");
            Expr width = Expression();
            Consume(TokenType.COMMA, "Expect ',' after int width.");
            Expr height = Expression();
            Consume(TokenType.RIGHT_PAREN, "Expect ')' after int height.");
            Consume(TokenType.NEWLINE, "Expected newline after 'DrawRectangle(int dirX, int dirY, int distance, int width, int height)'.");
            return new DrawRectangle(dirX, dirY, distance, width, height);
        }

        /// <summary>
        /// Analiza una sentencia Fill.
        /// </summary>
        /// <returns>Una nueva sentencia Fill.</returns>
        /// <exception cref="ParseError">Se lanza si la sintaxis es incorrecta.</exception>
        private Stmt FillStatement()
        {
            Consume(TokenType.LEFT_PAREN, "Expect '(' after 'Fill'.");
            Consume(TokenType.RIGHT_PAREN, "Expect ')'.");
            Consume(TokenType.NEWLINE, "Expected newline after 'Fill()'.");
            return new Fill();
        }
        #endregion

        #region Conditional Jumps and Labels
        /// <summary>
        /// Analiza una sentencia GoTo para el control de flujo condicional.
        /// </summary>
        /// <returns>Una nueva sentencia GoTo.</returns>
        /// <exception cref="ParseError">Se lanza si la sintaxis es incorrecta.</exception>
        private Stmt GoToStatement()
        {
            //try
            //{
            Consume(TokenType.LEFT_BRACKET, "Expect '[' after 'GoTo'.");
            var label = Consume(TokenType.IDENTIFIER, "Expect label name inside brackets.");
            Consume(TokenType.RIGHT_BRACKET, "Expect ']' after label name.");
            Consume(TokenType.LEFT_PAREN, "Expect '(' before condition.");
            Expr condition = Expression();
            Consume(TokenType.RIGHT_PAREN, "Expect ')' after condition.");
            Consume(TokenType.NEWLINE, "Expected newline after 'GoTo[label](condition)'.");
            return new GoTo(label.Lexeme, condition);
            //}
            /*
            catch (ParseError error)
            {
                var token = Previous();
                ErrorReporter.ReportTokenError(token, "Error in GoTo statement: " + error.Message);
                throw;
            }*/
        }

        /// <summary>
        /// Determina si un identificador es una etiqueta o una asignación.
        /// </summary>
        /// <returns>Una sentencia Label o Assignment dependiendo del contexto.</returns>
        /// <exception cref="ParseError">Se lanza si la sintaxis es incorrecta.</exception>
        private Stmt MaybeLabelOrAssignment()
        {
            var identifierToken = Previous();

            if (Peek().Type == TokenType.NEWLINE || Peek().Type == TokenType.EOF)
            {
                Advance();
                return new Label(identifierToken.Lexeme);
            }

            if (Match(TokenType.ARROW))
            {
                var value = Expression();

                if (Peek().Type == TokenType.NEWLINE)
                {
                    Advance();
                }
                else if (Peek().Type != TokenType.EOF)
                {
                    Consume(TokenType.NEWLINE, "Expected newline after assignment.");
                }
                return new Assignment(identifierToken.Lexeme, value);
            }

            throw Error(Peek(), $"Unexpected token after '{identifierToken.Lexeme}'.");
        }
        #endregion

        #region Expression Parsing
        /// <summary>
        /// Punto de entrada para analizar expresiones.
        /// </summary>
        /// <returns>Una expresión analizada.</returns>
        private Expr Expression()
        {
            return LogicalAnd();
        }

        /// <summary>
        /// Analiza expresiones lógicas AND.
        /// </summary>
        /// <returns>Una expresión lógica AND o una expresión de nivel inferior.</returns>
        private Expr LogicalAnd()
        {
            Expr expr = LogicalOr();

            while (Match(TokenType.AND))
            {
                Token op = Previous();
                Expr right = LogicalOr();
                expr = new Binary(expr, op, right);
            }

            return expr;
        }

        /// <summary>
        /// Analiza expresiones lógicas OR.
        /// </summary>
        /// <returns>Una expresión lógica OR o una expresión de nivel inferior.</returns>
        private Expr LogicalOr()
        {
            Expr expr = Equality();

            while (Match(TokenType.OR))
            {
                Token op = Previous();
                Expr right = Equality();
                expr = new Binary(expr, op, right);
            }

            return expr;
        }

        /// <summary>
        /// Analiza expresiones de igualdad (== y !=).
        /// </summary>
        /// <returns>Una expresión de igualdad o una expresión de nivel inferior.</returns>
        private Expr Equality()
        {
            Expr expr = Comparison();

            while (Match(TokenType.EQUAL_EQUAL, TokenType.BANG_EQUAL))
            {
                Token op = Previous();
                Expr right = Comparison();
                expr = new Binary(expr, op, right);
            }

            return expr;
        }

        /// <summary>
        /// Analiza expresiones de comparación (>, >=, <, <=).
        /// </summary>
        /// <returns>Una expresión de comparación o una expresión de nivel inferior.</returns>
        private Expr Comparison()
        {
            Expr expr = Term();

            while (Match(TokenType.GREATER, TokenType.GREATER_EQUAL, TokenType.LESS, TokenType.LESS_EQUAL))
            {
                Token op = Previous();
                Expr right = Term();
                expr = new Binary(expr, op, right);
            }

            return expr;
        }


        /// <summary>
        /// Analiza expresiones de términos (suma y resta).
        /// </summary>
        /// <returns>Una expresión de término o una expresión de nivel inferior.</returns>
        private Expr Term()
        {
            Expr expr = Factor();

            while (Match(TokenType.MIN, TokenType.SUM))
            {
                Token op = Previous();
                Expr right = Factor();
                expr = new Binary(expr, op, right);
            }

            return expr;
        }


        /// <summary>
        /// Analiza expresiones de factores (multiplicación, división y módulo).
        /// </summary>
        /// <returns>Una expresión de factor o una expresión de nivel inferior.</returns>
        private Expr Factor()
        {
            Expr expr = Unary();

            while (Match(TokenType.MULT, TokenType.DIV, TokenType.MOD))
            {
                Token op = Previous();
                Expr right = Unary();
                expr = new Binary(expr, op, right);
            }

            return expr;
        }

        /// <summary>
        /// Analiza expresiones unarias (negativos).
        /// </summary>
        /// <returns>Una expresión unaria o una expresión de nivel inferior.</returns>
        private Expr Unary()
        {
            if (Match(TokenType.MIN))
            {
                Token op = Previous();
                Expr right = Pow();
                return new Unary(op, right);
            }

            return Pow();
        }

        /// <summary>
        /// Analiza expresiones de potencia.
        /// </summary>
        /// <returns>Una expresión de potencia o una expresión primaria.</returns>
        private Expr Pow()
        {
            Expr expr = Primary();

            while (Match(TokenType.POW))
            {
                Token op = Previous();
                Expr right = Primary();
                expr = new Binary(expr, op, right);
            }

            return expr;
        }

        /// <summary>
        /// Analiza expresiones primarias (literales, agrupaciones, variables).
        /// </summary>
        /// <returns>Una expresión primaria.</returns>
        private Expr Primary()
        {
            if (Match(TokenType.NUMBER)) return new Literal(Previous().Literal);

            if (Match(TokenType.COLOR_LITERAL)) return new Literal(Previous().Literal);

            if (Match(TokenType.TRUE)) return new Literal(true);
            if (Match(TokenType.FALSE)) return new Literal(false);

            if (Match(TokenType.GET_ACTUAL_X, TokenType.GET_ACTUAL_Y, TokenType.GET_CANVAS_SIZE))
            {
                string functionName = Previous().Lexeme;
                Consume(TokenType.LEFT_PAREN, $"Expect '(' after '{functionName}'.");
                Consume(TokenType.RIGHT_PAREN, $"Expect ')'.");
                Consume(TokenType.NEWLINE, $"Expected newline after '{functionName}()'.");
                return new Call(functionName, new List<Expr>());
            }

            if (Match(TokenType.IS_BRUSH_COLOR))
            {
                Consume(TokenType.LEFT_PAREN, "Expect '(' after 'IsBrushColor'.");
                Expr colorArg = Expression();
                Consume(TokenType.RIGHT_PAREN, "Expect ')' after string color.");
                Consume(TokenType.NEWLINE, $"Expected newline after 'IsBrushColor(string color)'.");
                return new Call("IsBrushColor", new List<Expr> { colorArg });
            }

            if (Match(TokenType.IS_BRUSH_SIZE))
            {
                Consume(TokenType.LEFT_PAREN, "Expect '(' after 'IsBrushSize'.");
                Expr sizeArg = Expression();
                Consume(TokenType.RIGHT_PAREN, "Expect ')' after int size.");
                Consume(TokenType.NEWLINE, $"Expected newline after 'IsBrushSize(int size)'.");
                return new Call("IsBrushSize", new List<Expr> { sizeArg });
            }

            if (Match(TokenType.IS_CANVAS_COLOR))
            {
                Consume(TokenType.LEFT_PAREN, "Expect '(' after 'IsCanvasColor'.");
                Expr colorArg = Expression();
                Consume(TokenType.COMMA, "Expect ',' after string color.");
                Expr yArg = Expression();
                Consume(TokenType.RIGHT_PAREN, "Expect ')' after int vertical.");
                Expr xArg = Expression();
                Consume(TokenType.COMMA, "Expect ',' after int horizontal.");
                Consume(TokenType.NEWLINE, $"Expected newline after 'IsCanvasColor(string color, int vertical, int horizontal)'.");
                return new Call("IsCanvasColor", new List<Expr> { colorArg, yArg, xArg });
            }

            if (Match(TokenType.GET_COLOR_COUNT))
            {
                Consume(TokenType.LEFT_PAREN, "Expect '(' after 'GetColorCount'.");
                Expr colorArg = Expression();
                Consume(TokenType.COMMA, "Expect ',' after string color.");
                Expr x1Arg = Expression();
                Consume(TokenType.COMMA, "Expect ',' after int x1.");
                Expr y1Arg = Expression();
                Consume(TokenType.COMMA, "Expect ',' after int y1.");
                Expr x2Arg = Expression();
                Consume(TokenType.COMMA, "Expect ',' after int x2.");
                Expr y2Arg = Expression();
                Consume(TokenType.RIGHT_PAREN, "Expect ')' after int y2.");
                Consume(TokenType.NEWLINE, $"Expected newline after 'GetColorCount(string color, int x1, int y1, int x2, int y2)'.");
                return new Call("GetColorCount", new List<Expr> { colorArg, x1Arg, y1Arg, x2Arg, y2Arg });
            }

            if (Match(TokenType.IDENTIFIER))
            {
                return new Variable(Previous().Lexeme);
            }

            if (Match(TokenType.LEFT_PAREN))
            {
                Expr expr = Expression();
                Consume(TokenType.RIGHT_PAREN, "Expect ')' after expression");
                return new Grouping(expr);
            }

            throw Error(Peek(), "Expected expression");
        }
        #endregion

        #region Token Management
        /// <summary>
        /// Verifica si el token actual coincide con alguno de los tipos especificados y avanza al siguiente token si hay coincidencia.
        /// </summary>
        /// <param name="types">Los tipos de token a comprobar.</param>
        /// <returns>True si hay coincidencia, false en caso contrario.</returns>
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
        /// Verifica si el token actual es del tipo especificado sin avanzar al siguiente token.
        /// </summary>
        /// <param name="type">El tipo de token a comprobar.</param>
        /// <returns>True si el token actual es del tipo especificado, false en caso contrario.</returns>
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
            if (!IsAtEnd()) Current++;
            return Previous();
        }

        /// <summary>
        /// Verifica si se ha alcanzado el final de la lista de tokens.
        /// </summary>
        /// <returns>True si se ha alcanzado el fin de la lista, false en caso contrario.</returns>
        private bool IsAtEnd()
        {
            return Peek().Type == TokenType.EOF;
        }

        /// <summary>
        /// Devuelve el token actual sin avanzar.
        /// </summary>
        /// <returns>El token actual.</returns>
        private Token Peek()
        {
            return Tokens[Current];
        }

        /// <summary>
        /// Devuelve el token anterior al actual.
        /// </summary>
        /// <returns>El token anterior al actual.</returns>
        private Token Previous()
        {
            return Tokens[Current - 1];
        }

        /// <summary>
        /// Consume el token actual si es del tipo especificado, avanzando al siguiente token.
        /// Si no coincide, lanza un error con el mensaje especificado.
        /// </summary>
        /// <param name="type">El tipo de token esperado.</param>
        /// <param name="message">El mensaje de error en caso de no coincidencia.</param>
        /// <returns>El token consumido.</returns>
        /// <exception cref="ParseError">Se lanza si el token actual no es del tipo esperado.</exception>
        private Token Consume(TokenType type, string message)
        {
            if (Check(type)) return Advance();
            throw Error(Peek(), message);
        }
        #endregion

        #region Error Handling
        private Exception Error(Token token, string message)
        {
            var error = new ParseException(token, message);
            Errors.Add($"[Line {token.Line}] Error: {message}");
            return error;
        }

        /// <summary>
        /// Sincroniza el parser después de un error para continuar el análisis.
        /// Avanza hasta encontrar el inicio de una nueva sentencia.
        /// </summary>
        private void Synchronize()
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
                    case TokenType.IDENTIFIER:
                        return;
                }

                Advance();
            }
        }
        #endregion

        #region Helper Classes
        public class ParseException : Exception
        {
            public Token Token { get; }
            public ParseException(Token token, string message) : base(message)
            {
                Token = token;
            }
        }
        #endregion
        /*
        /// <summary>
        /// Crea un error de análisis sintáctico con el token y mensaje proporcionados.
        /// </summary>
        /// <param name="token">El token donde ocurrió el error.</param>
        /// <param name="message">El mensaje de error.</param>
        /// <returns>Un objeto ParseError.</returns>
        private ParseError Error(Token token, string message)
        {
            ErrorReporter.ReportTokenError(token, message);
            return new ParseError(message, token.Line);
        }*/
    }
}
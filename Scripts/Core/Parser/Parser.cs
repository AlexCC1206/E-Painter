using System;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace EPainter.Core
{
    public class Parser
    {
        private List<Token> Tokens;
        private int current = 0;

        public Parser(List<Token> tokens)
        {
            Tokens = tokens;
        }

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

        #region Stmt 
        private Stmt Declaration()
        {
            try
            {
                // Ignorar saltos de línea
                while (Match(TokenType.NEWLINE));
                
                if (IsAtEnd()) return null;
                
                if (Match(TokenType.SPAWN)) return SpawnStatement();
                if (Match(TokenType.COLOR)) return ColorStatement();
                if (Match(TokenType.SIZE)) return SizeStatement();
                if (Match(TokenType.DRAW_LINE)) return DrawLineStatement();
                if (Match(TokenType.DRAW_CIRCLE)) return DrawCircleStatement();
                if (Match(TokenType.DRAW_RECTANGLE)) return DrawRectangleStatement();
                if (Match(TokenType.FILL)) return FillStatement();
                if (Match(TokenType.GOTO)) return GotoStatement();

                if (Check(TokenType.IDENTIFIER))
                {
                    Advance(); // Consume el identificador
                    return MaybeLabelOrAssignment();
                }

                throw Error(Peek(), "Unexpected token.");
            }
            catch (ParseError)
            {
                Syncronize();
                return null;
            }
        }

        private Stmt SpawnStatement()
        {
            Consume(TokenType.LEFT_PAREN, "Expect '(' after 'Spawn'.");
            Expr x = Expression();
            Consume(TokenType.COMMA, "Expect ',' after X coordinate.");
            Expr y = Expression();
            Consume(TokenType.RIGHT_PAREN, "Expect ')' after Y coordinate.");
            Consume(TokenType.NEWLINE, "Expected newline after 'Spawn(...)'.");
            return new Stmt.Spawn(x, y);
        }

        private Stmt ColorStatement()
        {
            Consume(TokenType.LEFT_PAREN, "Expect '(' after 'Color'.");
            var color = Expression();
            Consume(TokenType.RIGHT_PAREN, "Expect ')' after color.");
            Consume(TokenType.NEWLINE, "Expected newline after 'Color(...)'.");
            return new Stmt.Color(color);
        }


        private Stmt SizeStatement()
        {
            Consume(TokenType.LEFT_PAREN, "Expect '(' after 'Size'");
            var size = Expression();
            Consume(TokenType.RIGHT_PAREN, "Expect ')' after size.");
            Consume(TokenType.NEWLINE, "Expected newline after 'Size(...)'.");
            return new Stmt.Size(size);
        }


        private Stmt DrawLineStatement()
        {
            Consume(TokenType.LEFT_PAREN, "Expect '(' after 'DrawLine'.");
            Expr dirX = Expression();
            Consume(TokenType.COMMA, "Expect ',' after direction X.");
            Expr dirY = Expression();
            Consume(TokenType.COMMA, "Expect ',' after direction Y.");
            Expr distance = Expression();
            Consume(TokenType.RIGHT_PAREN, "Expect ')' after distance.");
            Consume(TokenType.NEWLINE, "Expected newline after 'DrawLine(...)'.");
            return new Stmt.DrawLine(dirX, dirY, distance);
        }

        private Stmt DrawCircleStatement()
        {
            Consume(TokenType.LEFT_PAREN, "Expect '(' after 'DrawCircle'.");
            Expr dirX = Expression();
            Consume(TokenType.COMMA, "Expect ',' after direction X.");
            Expr dirY = Expression();
            Consume(TokenType.COMMA, "Expect ',' after direction Y.");
            Expr radius = Expression();
            Consume(TokenType.RIGHT_PAREN, "Expect ')' after radius.");
            Consume(TokenType.NEWLINE, "Expected newline after 'DrawCircle(...)'.");
            return new Stmt.DrawCircle(dirX, dirY, radius);
        }


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
            Consume(TokenType.RIGHT_PAREN, "Expect ')' after height.");
            Consume(TokenType.NEWLINE, "Expected newline after 'DrawRectangle(...)'.");
            return new Stmt.DrawRectangle(dirX, dirY, distance, width, height);
        }

        private Stmt FillStatement()
        {
            Consume(TokenType.LEFT_PAREN, "Expect '(' after 'Fill'.");
            Consume(TokenType.RIGHT_PAREN, "Expect ')' after Fill.");
            Consume(TokenType.NEWLINE, "Expected newline after 'Fill()'.");
            return new Stmt.Fill();
        }


        private Stmt GotoStatement()
        {
            Consume(TokenType.LEFT_BRACKET, "Expect '[' after  'GoTo'.");
            var label = Consume(TokenType.IDENTIFIER, "Expect label name inside brackets.");
            Consume(TokenType.RIGHT_BRACKET, "Expect ']' after label name.");
            Consume(TokenType.LEFT_PAREN, "Expect '(' before condition.");
            Expr condition = Expression();
            Consume(TokenType.RIGHT_PAREN, "Expect ')' after condition.");
            Consume(TokenType.NEWLINE, "Expected newline after 'GoTo[...](...)'.");
            return new Stmt.Goto(label.Lexeme, condition);
        }
        
        private Stmt MaybeLabelOrAssignment()
        {
            var identifierToken = Previous();

            if (Peek().Type == TokenType.NEWLINE || Peek().Type == TokenType.EOF)
            {
                Advance();
                return new Stmt.Label(identifierToken.Lexeme);
            }

            if (Match(TokenType.ARROW))
            {
                var value = Expression();
                
                // Si ya estamos al final del archivo o hay un salto de línea, avanzar
                if (Peek().Type == TokenType.NEWLINE) 
                {
                    Advance();
                }
                else if (Peek().Type != TokenType.EOF)
                {
                    Consume(TokenType.NEWLINE, "Expected newline after assignment.");
                }
                return new Stmt.Assignment(identifierToken.Lexeme, value);
            }

            throw Error(Peek(), $"Unexpected token after '{identifierToken.Lexeme}'.");
        }
        #endregion

        #region Expr
        private Expr Expression()
        {
            return LogicalAnd();
        }

        private Expr LogicalAnd()
        {
            Expr expr = LogicalOr();

            while (Match(TokenType.AND))
            {
                Token op = Previous();
                Expr rigth = LogicalOr();
                expr = new Expr.Binary(expr, op, rigth);
            }

            return expr;
        }

        private Expr LogicalOr()
        {
            Expr expr = Equality();

            while (Match(TokenType.OR))
            {
                Token op = Previous();
                Expr rigth = Equality();
                expr = new Expr.Binary(expr, op, rigth);
            }

            return expr;
        }

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


        private Expr Factor()
        {
            Expr expr = Unary();

            while (Match(TokenType.MULT, TokenType.DIV, TokenType.MOD))
            {
                Token op = Previous();
                Expr right = Unary();
                expr = new Expr.Binary(expr, op, right);
            }

            return expr;
        }

        private Expr Unary()
        {
            if (Match(TokenType.MIN, TokenType.BANG_EQUAL))
            {
                Token op = Previous();
                Expr right = Pow();
                return new Expr.Unary(op, right);
            }

            return Primary();
        }

        private Expr Pow()
        {
            Expr Expr = Primary();

            while (Match(TokenType.POW))
            {
                Token op = Previous();
                Expr right = Primary();
                Expr = new Expr.Binary(Expr, op, right);
            }

            return Expr;
        }

        private Expr Primary()
        {
            if (Match(TokenType.NUMBER)) return new Expr.Literal(Previous().Literal);

            if (Match(TokenType.COLOR_LITERAL)) return new Expr.Literal(Previous().Literal);

            if (Match(TokenType.TRUE)) return new Expr.Literal(true);
            if (Match(TokenType.FALSE)) return new Expr.Literal(false);

            if (Match(TokenType.IDENTIFIER))
            {
                if (Peek().Type == TokenType.LEFT_PAREN)
                {
                    return FunctionCall(Previous().Lexeme);
                }
                return new Expr.Variable(Previous().Lexeme);
            }

            if (Match(TokenType.LEFT_PAREN))
            {
                Expr expr = Expression();
                Consume(TokenType.RIGHT_PAREN, "Expect ')' after expression");
                return new Expr.Grouping(expr);
            }

            throw Error(Peek(), "Expected expression");
        }


        private Expr FunctionCall(string name)
        {
            Consume(TokenType.LEFT_PAREN, $"Expected '(' after {name}.");

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
        #endregion

        #region Utils
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

        private bool Check(TokenType type)
        {
            if (IsAtEnd()) return false;
            return Peek().Type == type;
        }

        private Token Advance()
        {
            if (!IsAtEnd()) current++;
            return Previous();
        }

        private bool IsAtEnd()
        {
            return Peek().Type == TokenType.EOF;
        }

        private Token Peek()
        {
            return Tokens[current];
        }

        private Token Previous()
        {
            return Tokens[current - 1];
        }

        private Token Consume(TokenType type, string message)
        {
            if (Check(type)) return Advance();
            throw Error(Peek(), message);
        }

        private ParseError Error(Token token, string message)
        {
            ErrorReporter.Error(token, message);
            return new ParseError();
        }

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
        #endregion
    }
}
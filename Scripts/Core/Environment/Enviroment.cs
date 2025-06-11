using System.Collections.Generic;

namespace EPainter.Core
{
    public class Environment
    {
        private Dictionary<string, object> values = new Dictionary<string, object>();
        public Environment Enclosing;

        public Environment()
        {
            Enclosing = null; 
        }

        public Environment(Environment enclosing)
        {
            Enclosing = enclosing;
        }

        public void Define(string name, object value)
        {
            values[name] = value;
        }

        public object Get(Token name)
        {
            if (values.TryGetValue(name.Lexeme, out var value))
            {
                return value;
            }

            if (Enclosing != null)
            {
                return Enclosing.Get(name);
            }

            throw new RuntimeError(name, $"Undefined variable '{name.Lexeme}'");
        }

        public void Assign(Token name, object value)
        {
            if (values.ContainsKey(name.Lexeme))
            {
                values[name.Lexeme] = value;
                return;
            }

            if (Enclosing != null)
            {
                Enclosing.Assign(name, value);
                return;
            }

            throw new RuntimeError(name, $"Undefined variable '{name.Lexeme}'");
        }

        public object GetAt(int distance, string name)
        {
            return Ancestor(distance).values[name];
        }

        public void AssignAt(int distance, Token name, object value)
        {
            Ancestor(distance).values[name.Lexeme] = value;
        }

        private Environment Ancestor(int distance)
        {
            Environment environment = this;
            for (var i = 0; i < distance; i++)
            {
                environment = environment.Enclosing;
            }

            return environment;
        }

        public override string ToString()
        {
            string result = values.ToString();
            if (Enclosing != null)
            {
                result += " <- " + Enclosing.ToString();
            }
            
            return result;
        }


    }
}
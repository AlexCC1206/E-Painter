using System.Collections.Generic;

namespace EPainter
{
    public class Environment
    {
        private Dictionary<string, object> variables = new Dictionary<string, object>();
        //private Dictionary<string, int> labels = new Dictionary<string, int>();

        public void Define(string name, object value)
        {
            variables[name] = value;
        }

        public object Get(Token name)
        {
            if (variables.TryGetValue(name.Lexeme, out var value))
            {
                return value;
            }
            throw new RuntimeError(name, $"Undefined variable '{name.Lexeme}'");
        }

        public void Assign(Token name, object value)
        {
            if (variables.ContainsKey(name.Lexeme))
            {
                Define(name.Lexeme, value);
                return;   
            }
            throw new RuntimeError(name, $"Undefined variable '{name.Lexeme}'");     
        }

        
/*
        public void DefineLabel(string name, int position)
        {
            labels[name] = position;
        }

        public int GetLabelPosition(Token name)
        {
            if (labels.TryGetValue(name.Lexeme, out var position))
            {
                return position;
            }
            throw new RuntimeError(name, $"Undefined label '{name.Lexeme}'");
        }*/
    }
}
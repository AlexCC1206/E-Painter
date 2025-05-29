using System.Collections.Generic;

namespace EPainter
{
    public class Environment
    {
        private Dictionary<string, object> variables = new Dictionary<string, object>();

        public object DefineVariable(Token name)
        {
            if (variables.ContainsKey(name.lexeme))
            {
                return variables[name.lexeme];
            }

            throw new RuntimeError(name, $"Undefined variable '{name.lexeme}'.");
        }

        void AssignVariable(string name, object value)
        {
            variables.Add(name, value);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gravlox
{
    class LoxEnvironment
    {
        readonly LoxEnvironment enclosing;
        private readonly Dictionary<string, object> Values = new Dictionary<string, object>();

        internal LoxEnvironment()
        {
            enclosing = null;
        }

        internal LoxEnvironment(LoxEnvironment enclosing)
        {
            this.enclosing = enclosing;
        }

        internal void Define(string name, object value)
        {
            Values[name] = value;
        }

        internal object Get(Token name)
        {
            if (Values.ContainsKey(name.Lexeme))
            {
                return Values[name.Lexeme];
            }
            if (enclosing != null)
            {
                return enclosing.Get(name);
            }

            throw new RuntimeError(name, "Undefined variable '" + name.Lexeme + "'.");
        }

        internal void Assign(Token name, Object value)
        {
            if (Values.ContainsKey(name.Lexeme))
            {
                Values[name.Lexeme] = value;
                return;
            }

            if (enclosing != null)
            {
                enclosing.Assign(name, value);
                return;
            }

            throw new RuntimeError(name, "Undefined variable '" + name.Lexeme + "'");
        }
    }
}

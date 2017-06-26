using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gravlox
{
    class RuntimeError : Exception
    {
        public readonly Token Token;

        public RuntimeError(Token token, String message) : base(message) {
            this.Token = token;
        }
    }
}

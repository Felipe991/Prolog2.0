using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prolonga
{
    internal class Variable : Termino
    {
        public List<string> valores; 
        public Variable(string texto) : base(texto)
        {
            this.valores = new List<string>();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prolonga
{
    internal class Condicion
    {
        public List<Premisa> premisas;
        public bool cumplida = false;
        public Condicion(List<Premisa> premisas)
        {
            this.premisas = premisas;
        }
    }
}

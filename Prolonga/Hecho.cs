using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prolonga
{
    internal class Hecho : Clausula
    {
        public Hecho(string predicadoPrincipal,List<Compound> compounds) : base(predicadoPrincipal,compounds)
        {
        }

        public override string ToString()
        {
            return "\nTipo clausula: Hecho" +
                "\nPredicado = " + predicadoPrincipal;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prolonga
{
    internal class Hecho : Clausula
    {
        public Compound compound;
        public Hecho(string predicadoPrincipal, Compound compound) : base(predicadoPrincipal)
        {
            this.compound = compound;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prolonga
{
    internal class Propiedad : Hecho
    {
        string atomo = "";
        public Propiedad(string textoClausula, Compound compound) : base(textoClausula,compound)
        {
            this.atomo = compound.terminos[0];
        }
        public override string ToString()
        {
            return "\nTipo clausula: Propiedad" +
                   "\nPredicado principal: " + predicadoPrincipal +
                   "\nCompound: " + compound.ToString();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prolonga
{
    internal class Propiedad : Hecho
    {
        Termino termino;
        Compound compound;
        public Propiedad(string textoClausula, Compound compound) : base(textoClausula)
        {
            this.termino = compound.terminos[0];
            this.compound = compound;
        }
        public override string ToString()
        {
            return "\nTipo clausula: Propiedad" +
                   "\nPredicado principal: " + predicadoPrincipal +
                   "\nCompound: " + compound.ToString();
        }
    }
}

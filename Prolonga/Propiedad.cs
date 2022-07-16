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
        public Propiedad(string textoClausula, List<Compound> compounds) : base(textoClausula,compounds)
        {
            this.termino = compounds[0].terminos[0];
            this.compound = compounds[0];
        }
        public override string ToString()
        {
            return "\nTipo clausula: Propiedad" +
                   "\nPredicado principal: " + predicadoPrincipal +
                   "\nCompound: " + compound.ToString();
        }
    }
}

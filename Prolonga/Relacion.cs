using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prolonga
{
    internal class Relacion : Hecho
    {
        List<string> atomos;
        List<string> operadores;
        public Relacion(string predicadoPrincipal, Compound compound) : base(predicadoPrincipal, compound)
        {
            this.atomos = compound.terminos;
            this.operadores = compound.operadores;
        }

        public override string ToString()
        {
            return "\nTipo clausula: Relación" +
                   "\nPredicado principal: " + predicadoPrincipal +
                   "\nCompound: " + compound.ToString();
        }
    }
}

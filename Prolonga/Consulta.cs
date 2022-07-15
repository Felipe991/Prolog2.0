using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Prolonga
{
    internal class Consulta
    {
        public string predicado = "";
        public List<Termino> terminos = new List<Termino>();
        public string respuesta = "";
        Compound compound;
        public Consulta(Compound compound)
        {
            this.compound = compound;
            this.predicado = compound.predicado;
            this.terminos = compound.terminos;
        }
        public override string ToString()
        {
            return "\nPredicado: " + predicado +
                   "\nCompound: " + compound.ToString() +
                   "\nRespuesta: " + respuesta;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prolonga
{
    internal class Relacion : Hecho
    {
        List<Termino> terminosSecundarios;
        Termino terminoPrincipal;
        public Relacion(string predicadoPrincipal, List<Compound> compounds) : base(predicadoPrincipal,compounds)
        {
            terminoPrincipal = compounds[0].terminos[0];
            terminosSecundarios = compounds[0].terminos.Skip(1).ToList();
        }

        public override string ToString()
        {
            return "\nTipo clausula: Relación" +
                   "\nPredicado principal: " + predicadoPrincipal +
                   "\nTermino principal: " + terminoPrincipal.nombreTermino +
                   "\nTerminos secundarios: " + getTerminosSecundarios();
        }

        private string getTerminosSecundarios()
        {
            string terminosSecundarios = " ";
            foreach (Termino termino in this.terminosSecundarios)
            {
                terminosSecundarios += termino.nombreTermino + " ";
            }
            return terminosSecundarios;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prolonga
{
    internal class Regla : Clausula
    {
        List<Compound> compoundsAntecedentes;
        List<string> operadoresRegla;
        Compound compoundPrincipal;
        public Regla(string predicadoPrincipal,List<Compound> compounds, List<string> operadoresRegla) : base(predicadoPrincipal)
        {
            compoundPrincipal = compounds[0];
            compoundsAntecedentes = compounds.Skip(1).ToList();
            this.operadoresRegla = operadoresRegla;
        }

        public override string ToString()
        {
            return "\nTipo clausula: Regla" +
                   "\nPredicado principal: " + predicadoPrincipal +
                   "\nCompound principal: " + this.compoundPrincipal.ToString()+
                   "\nCompounds Antecedentes: " + getDatosCompounds()+
                   "\nSeparadores logicos: " + getSeparadoresLogicos();
        }

        private string getSeparadoresLogicos()
        {
            string separadoresLogicos = "";
            foreach(string separadorLogico in this.operadoresRegla)
            {
                separadoresLogicos += separadorLogico + " ";
            }
            return separadoresLogicos;
        }

        private string getDatosCompounds()
        {
            string datosCompounds = "";
            foreach (Compound compound in compoundsAntecedentes)
            {
                datosCompounds += compound.ToString() + "\n";
            }
            return datosCompounds;
        }
    }
}

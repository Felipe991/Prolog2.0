﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prolonga
{
    internal class Regla : Clausula
    {
        List<Compound> compoundsAntecedentes;
        List<List<Compound>> condiciones;
        Compound compoundPrincipal;
        public Regla(string predicadoPrincipal,List<Compound> compounds, List<string> operadoresRegla) : base(predicadoPrincipal,compounds)
        {
            compoundPrincipal = compounds[0];
            compoundsAntecedentes = compounds.Skip(1).ToList();
            condiciones = new List<List<Compound>>();
            enlistCondiciones(compoundsAntecedentes,operadoresRegla);
        }

        private void enlistCondiciones(List<Compound> compoundAntecedentes, List<string> operadoresRegla)
        {
            List<Compound> condicion = new List<Compound>();
            condicion.Add(compoundAntecedentes[0]);
            for(int contador = 0;contador<operadoresRegla.Count;contador++)
            {
                if (operadoresRegla[contador].Equals(";"))
                {
                    condiciones.Add(condicion.ToList());
                    condicion.Clear();
                }
                condicion.Add(compoundAntecedentes[contador+1]);
            }
            condiciones.Add(condicion.ToList());
        }

        public override string ToString()
        {
            return "\nTipo clausula: Regla" +
                   "\nPredicado principal: " + predicadoPrincipal +
                   "\nCompound principal: " + this.compoundPrincipal.ToString()+
                   "\nCompounds Antecedentes: " + getDatosCompounds();
        }

        private string getDatosCompounds()
        {
            string datosCompounds = "";
            int contador = 1;
            foreach (List<Compound> condicion in condiciones)
            {
                datosCompounds += "\nCondicion N(" + contador + ")\n";
                foreach (Compound compound in condicion)
                {
                    datosCompounds += compound.ToString() + " ";
                }
                contador++;
            }
            return datosCompounds;
        }
    }
}

﻿using System;
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
        public List<string> respuestas;
        public bool hasVariable;
        Compound compound;
        public Consulta(Compound compound)
        {
            this.compound = compound;
            this.predicado = compound.predicado;
            this.terminos = compound.terminos;
            this.respuestas = new List<string>();
            this.hasVariable = containsVariable();
        }

        private bool containsVariable()
        {
            foreach (Termino termino in terminos)
            {
                if (termino is Variable)
                {
                    return true;
                }
            }
            return false;
        }

        public override string ToString()
        {
            return "\nPredicado: " + predicado +
                   "\nCompound: " + compound.ToString() +
                   "\nRespuestas: " + getRespuestas();
        }
        private string getRespuestas()
        {
            string respuestas = "";
            foreach (string respuesta in this.respuestas)
            {
                respuestas += respuesta + "\n";
            }
            return respuestas;
        }
        public void addRespuesta(string respuesta, List<string> valoresAgregados)
        {
            int contador = 0;
            string respuestaVariable = "";
            if (valoresAgregados.Count!=0 && this.hasVariable)
            {
                foreach (Termino termino in this.terminos)
                {
                    if (termino is Variable)
                    {
                        if (!valoresAgregados[contador].Contains("True"))
                        {
                            Variable variable = (Variable)termino;
                            variable.valores.Add(valoresAgregados[contador]);
                            respuestaVariable += variable.nombreTermino + " = " + valoresAgregados[contador] + " ";
                        }
                        else
                        {
                            respuestaVariable += valoresAgregados[contador];
                        }
                        contador++;
                    }
                }
                this.respuestas.Add(respuestaVariable);
            }
            else
            {
                this.respuestas.Add(respuesta);
            }
        }
        public void addRespuestaRecursiva(string respuesta, List<string> valoresAgregados)
        {
            int contador = 0;
            string respuestaVariable = "";
            if (valoresAgregados.Count != 0 && this.hasVariable)
            {
                foreach (string valor in valoresAgregados)
                {
                    foreach (Termino termino in this.terminos)
                    {
                        if (termino is Variable)
                        {
                            if (!valoresAgregados[contador].Contains("True"))
                            {
                                Variable variable = (Variable)termino;
                                variable.valores.Add(valoresAgregados[contador]);
                                respuestaVariable += variable.nombreTermino + " = " + valoresAgregados[contador] + "";
                            }
                            else
                            {
                                respuestaVariable += valoresAgregados[contador];
                            }
                            contador++;
                        }
                    }
                }
                this.respuestas.Add(respuestaVariable);
            }
            else
            {
                this.respuestas.Add(respuesta);
            }
        }
    }
}

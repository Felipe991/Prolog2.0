using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prolonga
{
    internal class MotorDeInferencia
    {
        BaseDeConocimiento baseDeConocimiento;
        Consulta consulta;
        bool matchPredicado = false;
        bool matchTerminos = false;
        public MotorDeInferencia(BaseDeConocimiento baseDeConocimiento)
        {
            this.baseDeConocimiento = baseDeConocimiento;
        }

        internal void encadenarHaciaAtras(Consulta consulta)
        {
            foreach(Clausula clausula in baseDeConocimiento.clausulas)
            {
                if(clausula is Hecho)
                {
                    if (satisfaceConsulta(clausula, consulta))
                    {
                        addRespuesta(consulta);
                    }
                }
            }
            if (!matchPredicado)
            {
                consulta.respuestas.Add("No existe procedimiento para: "+consulta.predicado);
            }else if(!matchTerminos)
            {
                consulta.respuestas.Add("No existe procedimiento para: " + consulta.predicado+" con "+consulta.terminos.Count+" terminos");
            }
            else if(consulta.respuestas.Count == 0)
            {
                consulta.respuestas.Add("False");
            }
            matchTerminos = false;
            matchPredicado = false;
        }

        private void addRespuesta(Consulta consulta)
        {
            if(consulta.respuestas.Count > 0)
            {
                if (!consulta.respuestas[consulta.respuestas.Count - 1].Contains("="))
                {
                    consulta.respuestas.Add("True");
                }
            }
            else
            {
                consulta.respuestas.Add("True");
            }
        }

        private bool satisfaceConsulta(Clausula clausula, Consulta consulta)
        {
            bool satisfecha = false;
            if (clausula.predicadoPrincipal.Equals(consulta.predicado))
            {
                matchPredicado = true;
                if (consulta.terminos.Count == 0)
                {
                    matchTerminos = true;
                    satisfecha = true;
                }
                else if (clausula.compounds[0].terminos.Count == consulta.terminos.Count)
                {
                    matchTerminos = true;
                    satisfecha = terminosEquivalentes(clausula, consulta);
                    Console.WriteLine(satisfecha ? "Se determino que sus terminos eran equivalentes":"Se determino que sus terminos no eran equivalentes");
                }
                else
                {
                    satisfecha = false;
                }
            }
            Console.WriteLine("****DEBUG****");
            Console.WriteLine("\nClausula recibida:"+clausula.ToString());
            Console.WriteLine("\nConsulta recibida:" + consulta.ToString());
            Console.WriteLine("\nSatisface = "+satisfecha);
            Console.WriteLine("\n****DEBUG****");
            return satisfecha;
        }

        private bool terminosEquivalentes(Clausula clausula, Consulta consulta)
        {
            bool terminosEquivalentes = true;
            bool add = false;
            List<string> valoresAgregados = new List<string>();
            List<string> nombresVariables = new List<string>();
            for (int contador = 0; contador < consulta.terminos.Count; contador++)
            {
                if (consulta.terminos[contador] is Atomo)
                {
                    if (!consulta.terminos[contador].nombreTermino.Equals(clausula.compounds[0].terminos[contador].nombreTermino))
                    {
                        Console.WriteLine("No tienen el mismo nombre atomo así que no son equivalentes");
                        terminosEquivalentes = false;
                        break;
                    }
                }
                if (consulta.terminos[contador] is Variable)
                {
                    if (clausula.compounds[0].terminos[contador] is Atomo)
                    {
                        Variable variable = (Variable)consulta.terminos[contador];
                        if (!nombresVariables.Contains(variable.nombreTermino))
                        {
                            variable.valores.Add(clausula.compounds[0].terminos[contador].nombreTermino);
                            valoresAgregados.Add(clausula.compounds[0].terminos[contador].nombreTermino);
                            nombresVariables.Add(variable.nombreTermino);
                            add = true;
                        }
                        else
                        {
                            if (!variable.valores[variable.valores.Count - 1].Equals(clausula.compounds[0].terminos[contador].nombreTermino))
                            {
                                Console.WriteLine("No tienen el mismo nombre atomo así que no son equivalentes");
                                terminosEquivalentes = false;
                                add = false;
                            }
                        }
                    }
                }
            }
            if (add)
            {
                addValoresVariable(nombresVariables,valoresAgregados,consulta);
            }
            return terminosEquivalentes;
        }

        private void addValoresVariable(List<string> nombresVariables, List<string> valoresAgregados, Consulta consulta)
        {
            int contador = 0;
            string respuesta = "";
            foreach(Termino termino in consulta.terminos)
            {
                if(termino is Variable)
                {
                    Variable variable = (Variable)termino;
                    variable.valores.Add(valoresAgregados[contador]);
                    respuesta += variable.nombreTermino + " = " + valoresAgregados[contador]+" ";
                    contador++;
                }
            }
            consulta.respuestas.Add(respuesta);
        }
    }
}

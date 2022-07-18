﻿namespace Prolonga
{
    internal class MotorDeInferencia
    {
        BaseDeConocimiento baseDeConocimiento;
        public MotorDeInferencia(BaseDeConocimiento baseDeConocimiento)
        {
            this.baseDeConocimiento = baseDeConocimiento;
        }

        private void addRespuesta(Consulta consulta)
        {
            if (consulta.respuestas.Count > 0)
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
            int cantidadTerminosConsulta = consulta.terminos.Count;
            int cantidadTerminosClausula = clausula.compounds[0].terminos.Count;
            if (clausula.predicadoPrincipal.Equals(consulta.predicado))
            {
                if (cantidadTerminosConsulta == cantidadTerminosClausula)
                {
                    if (cantidadTerminosConsulta == 0 && cantidadTerminosClausula == 0)
                    {
                        satisfecha = true;
                    }
                    else
                    {
                        List<string> valoresAgregados;
                        if (consulta.hasVariable)
                        {
                            valoresAgregados = variablesEquivalentes(clausula, consulta);
                            satisfecha = atomosEquivalentes(clausula, consulta) && valoresAgregados.Count > 0;
                            if (valoresAgregados.Count > 0)
                            {
                                addValoresVariable(valoresAgregados, consulta);
                            }
                        }
                        else
                        {
                            satisfecha = atomosEquivalentes(clausula, consulta);
                            
                        }
                        //Console.WriteLine(satisfecha ? "Se determino que sus terminos eran equivalentes" : "Se determino que sus terminos no eran equivalentes");
                    }
                }
                else
                {
                    satisfecha = false;
                }
            }
            /*Console.WriteLine("****DEBUG****");
            Console.WriteLine("\nClausula recibida:" + clausula.ToString());
            Console.WriteLine("\nConsulta recibida:" + consulta.ToString());
            Console.WriteLine("\nSatisface = " + satisfecha);
            Console.WriteLine("\n****DEBUG****");*/
            return satisfecha;
        }

        internal bool encadenarHaciaAtras(Consulta consulta)
        {
            List<Clausula> clausulasAux = new List<Clausula>();
            clausulasAux.AddRange(baseDeConocimiento.clausulas);
            if (existeClausula(clausulasAux, consulta))
            {
                List<Clausula> metas = findClausulasMeta(clausulasAux, consulta);
                foreach(Clausula meta in metas)
                {
                    Consulta consultaAux = new Consulta(meta.compounds[0]);
                    resolver(meta,consultaAux,clausulasAux);
                    foreach(string respuesta in consultaAux.respuestas)
                    {
                        consulta.respuestas.Add(respuesta);
                    }
                }
            }
            return false;
        }
        private bool resolver(Clausula meta, Consulta consulta, List<Clausula> clausulas)
        {
            if (meta is Hecho)
            {
                addRespuesta(consulta);
                return true;
            }
            else
            {
                Regla regla = (Regla)meta;
                foreach (Condicion condicion in regla.condiciones)
                {
                    bool satisfecha = false;
                    foreach (Premisa premisa in condicion.premisas)
                    {
                        Consulta subConsulta = new Consulta(premisa.compound);
                        Clausula subMeta= findClausula(clausulas, subConsulta);
                        if(subMeta is null)
                        {
                            satisfecha = false;
                            break;
                        }
                        if (!resolver(subMeta,subConsulta,clausulas))
                        {
                            satisfecha = false;
                            break;
                        }
                        else
                        {
                            satisfecha = true;
                        }
                    }
                    if (satisfecha)
                    {
                        addRespuesta(consulta);
                        return true;
                    }
                }
            }
            return false;
        }

        private List<Clausula> findClausulasMeta(List<Clausula> clausulas, Consulta consulta)
        {
            List<Clausula> metas = new List<Clausula>();
            foreach (Clausula clausula in clausulas)
            {
                if (satisfaceConsulta(clausula, consulta))
                {
                    metas.Add(clausula);
                }
            }
            return metas;
        }

        private Clausula findClausula(List<Clausula> clausulas, Consulta consulta)
        {
            foreach (Clausula clausula in clausulas)
            {
                if (satisfaceConsulta(clausula, consulta))
                {
                    return clausula;
                }
            }
            return null;
        }

        private bool atomosEquivalentes (Clausula clausula, Consulta consulta)
        {
            bool atomosEquivalentes = true;
            for (int i = 0; i < consulta.terminos.Count; i++)
            {
                Termino terminoConsulta = consulta.terminos[i];
                Termino terminoClausula = clausula.compounds[0].terminos[i];
                if (terminoConsulta is Atomo)
                {
                    if (terminoClausula is Atomo)
                    {
                        if (!terminoConsulta.nombreTermino.Equals(terminoClausula.nombreTermino))
                        {
                            //Console.WriteLine("No tienen el mismo nombre atomo así que no son equivalentes");
                            atomosEquivalentes = false;
                            break;
                        }
                    } 
                }
            }
            return atomosEquivalentes;
        }
        
        private List<string> variablesEquivalentes(Clausula clausula, Consulta consulta)
        {
            List<string> valoresAgregados = new List<string>();
            List<string> nombresVariables = new List<string>();
            for (int i = 0; i < consulta.terminos.Count; i++)
            {
                Termino terminoConsulta = consulta.terminos[i];
                Termino terminoClausula = clausula.compounds[0].terminos[i];
                if (terminoConsulta is Variable)
                {
                    if (terminoClausula is Atomo)
                    {
                        Variable variable = (Variable)terminoConsulta;
                        if (!nombresVariables.Contains(variable.nombreTermino))
                        {
                            variable.valores.Add(terminoClausula.nombreTermino);
                            valoresAgregados.Add(terminoClausula.nombreTermino);
                            nombresVariables.Add(variable.nombreTermino);
                        }
                        else
                        {
                            if (!variable.valores[variable.valores.Count - 1].Equals(terminoClausula.nombreTermino))
                            {
                                //Console.WriteLine("No tienen el mismo nombre atomo así que no son equivalentes");
                                return new List<string>();
                            }
                        }
                    }
                    else
                    {
                        valoresAgregados.Add(null);
                    }
                    
                }
            }
            return valoresAgregados;
        }

        private void addValoresVariable(List<string> valoresAgregados, Consulta consulta)
        {
            int contador = 0;
            string respuesta = "";
            foreach (Termino termino in consulta.terminos)
            {
                if (termino is Variable)
                {
                    if (!String.IsNullOrEmpty(valoresAgregados[contador]))
                    {
                        Variable variable = (Variable)termino;
                        variable.valores.Add(valoresAgregados[contador]);
                        respuesta += variable.nombreTermino + " = " + valoresAgregados[contador] + " ";
                    }
                    
                    contador++;
                }
            }
            consulta.respuestas.Add(respuesta);
            
        }


        public void encadenarHaciaAdelante(Consulta consulta)
        {
            List<Clausula> clausulasAux = new List<Clausula>(); 
            clausulasAux.AddRange(baseDeConocimiento.clausulas);
            while (existeClausula(clausulasAux, consulta))
            {
                List<Clausula> clausulasAplicables = findAplicables(clausulasAux);
                if (clausulasAplicables.Count > 0)
                {
                    Clausula mejorClausula = findMejorClausula(clausulasAplicables);
                    if (satisfaceConsulta(mejorClausula, consulta))
                    {
                        addRespuesta(consulta);
                        
                    }
                    aplicarClausula(mejorClausula, clausulasAux);
                    clausulasAux.Remove(mejorClausula);
                }
                else
                {
                    break;
                }
            }
        }
        private bool existeClausula(List<Clausula> clausulas, Consulta consulta)
        {
            foreach (Clausula clausula in clausulas)
            {
                if (consulta.hasVariable)
                {
                    if (atomosEquivalentes(clausula, consulta) && variablesEquivalentes(clausula, consulta).Count > 0)
                    {
                        return true;
                    }
                }
                else
                {
                    if (atomosEquivalentes(clausula, consulta))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        private List<Clausula> findAplicables(List<Clausula> clausulas)
        {
            List<Clausula> clausulasAplicables = new List<Clausula>();
            foreach (Clausula clausula in clausulas)
            {
                if (clausula is Hecho)
                {
                    clausulasAplicables.Add(clausula);
                }
                else if (clausula is Regla)
                {
                    Regla regla = (Regla)clausula;
                    if (hasCondicionesCumplidas(regla))
                    {
                        clausulasAplicables.Add(regla);
                    }
                }
            }
            return clausulasAplicables;
        }

        private bool hasCondicionesCumplidas(Regla regla)
        {
            foreach (Condicion condicion in regla.condiciones)
            {
                foreach (Premisa premisa in condicion.premisas)
                {
                    if (!premisa.cumplida)
                    {
                        break;
                    }
                    else if (premisa.cumplida && condicion.premisas.IndexOf(premisa) == condicion.premisas.Count - 1)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private Clausula findMejorClausula (List<Clausula> clausulas)
        {
            Clausula mejorClausula = clausulas[0];

            foreach (Clausula clausula in clausulas)
            {
                if (clausula is Regla)
                {
                    mejorClausula = clausula;
                    return mejorClausula;
                }
            }
            return mejorClausula;
        }
        
        private void aplicarClausula(Clausula clausulaAplicable, List<Clausula> clausulas)
        {
            foreach(Clausula clausula in clausulas)
            {
                if (clausula is Regla)
                {
                    Regla regla = (Regla)clausula;
                    foreach (Condicion condicion in regla.condiciones)
                    {
                        foreach (Premisa premisa in condicion.premisas)
                        {
                            Consulta consulta = new Consulta(premisa.compound);
                            if (satisfaceConsulta(clausulaAplicable,consulta))
                            {
                                premisa.cumplida = true;
                            }
                        }
                    }
                }
            }
        }

      


    }
}

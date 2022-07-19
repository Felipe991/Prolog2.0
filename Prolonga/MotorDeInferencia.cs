using System.Text.Json;
using System.Text.RegularExpressions;

namespace Prolonga
{
    internal class MotorDeInferencia
    {
        BaseDeConocimiento baseDeConocimiento;
        List<string> valoresAgregados = new List<string>();
        public MotorDeInferencia(BaseDeConocimiento baseDeConocimiento)
        {
            this.baseDeConocimiento = baseDeConocimiento;
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
                        if (consulta.hasVariable)
                        {
                            satisfecha = atomosEquivalentes(clausula, consulta) && variablesEquivalentes(clausula, consulta);
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
                this.valoresAgregados.Clear();
                foreach (Clausula meta in metas)
                {
                    Consulta consultaAux = new Consulta(meta.compounds[0]);
                    if (resolver(meta, consulta))
                    {
                        foreach (string respuesta in consultaAux.respuestas)
                        {
                            consulta.respuestas.Add(respuesta);
                        }
                    }
                }
                if (consulta.respuestas.Count > 0)
                {
                    Console.WriteLine("true");
                    return true;
                }
            }
            Console.WriteLine("false");
            return false;
        }
        private bool resolver(Clausula meta, Consulta consulta)
        {
            if (meta is Hecho)
            {
                string respuesta = "";
                if (consulta.hasVariable)
                {
                    foreach(Termino termino in meta.compounds[0].terminos)
                    {
                        if(termino is Atomo)
                        {
                            respuesta += consulta.terminos[meta.compounds[0].terminos.IndexOf(termino)].nombreTermino + " = ";
                            respuesta += termino.nombreTermino + " ";
                        } 
                    }
                    consulta.respuestas.Add(respuesta);
                }
                else
                {
                    consulta.respuestas.Add("True");
                }
                return true;
            }
            else
            {
                Regla? regla = copyRegla(meta);
                bool respuesta = false;
                condicionarRegla(regla, consulta);
                foreach (Condicion condicion in regla.condiciones)
                {
                    List<List<string>> respuestasPreliminares = new List<List<string>>();
                    bool hasVariable = false, clean = true;
                    
                    foreach (Premisa premisa in condicion.premisas)
                    {
                        Consulta subConsulta = new Consulta(premisa.compound);
                        hasVariable = premisa.compound.hasVariable ? true : hasVariable;
                        
                        if (encadenarHaciaAtras(subConsulta))
                        {
                            respuestasPreliminares.Add(subConsulta.respuestas);
                        }
                        else
                        {
                            clean = false;
                            break;
                        }
                    }
                    if (clean)
                    {
                        respuesta = true;
                        string respuestaVariable = "";
                        if (hasVariable)
                        {
                            respuestaVariable = getRespuestaVariable(respuestasPreliminares);
                        }
                        consulta.respuestas.Add("True" + respuestaVariable);
                    }
                }
                return respuesta;
            }
        }

        private Regla? copyRegla(Clausula meta)
        {
            Regla reglaAux = (Regla)meta;
            List<Compound> compoundsAux = getCompounds(meta);
            return new Regla(reglaAux.predicadoPrincipal,compoundsAux, reglaAux.operadoresRegla);
        }

        private List<Compound> getCompounds(Clausula meta)
        {
            List<Compound> compoundsAux = new List<Compound>();
            foreach (Compound compound in meta.compounds)
            {
                compoundsAux.Add(new Compound(compound.predicado, compound.nombresTerminoString, compound.operadores));
            }
            return compoundsAux;
        }

        private void condicionarRegla(Regla regla, Consulta consulta)
        {
            foreach(Termino terminoRegla in regla.compounds[0].terminos)
            {
                int posicion = regla.compounds[0].terminos.IndexOf(terminoRegla);
                Termino terminoConsulta = consulta.terminos[posicion];
                if ((consulta.terminos[posicion] is Atomo && (terminoRegla is Variable)) || (consulta.terminos[posicion] is Variable && (terminoRegla is Variable)) )
                {
                    reemplazarTermino(terminoRegla,terminoConsulta,regla);
                }
            }
        }

        private void reemplazarTermino(Termino terminoRegla, Termino terminoConsulta, Regla regla)
        {
            foreach(Condicion condicionRegla in regla.condiciones)
            {
                foreach(Premisa premisaCondicion in condicionRegla.premisas)
                {
                    for (int i = 0; i < premisaCondicion.compound.terminos.Count; i++)
                    {
                        if (premisaCondicion.compound.terminos[i].nombreTermino.Equals(terminoRegla.nombreTermino))
                        {
                            premisaCondicion.compound.terminos[i] = terminoConsulta;
                        }
                    }
                }
            }
        }

        private bool hasAtomo(Consulta consulta)
        {
            foreach (Termino termino in consulta.terminos)
            {
                if (termino is Atomo)
                {
                    return true;
                }
            }
            return false;
        }

        private string getRespuestaVariable(List<List<string>> respuestasPreliminares)
        {
            string respuestaVariable = "( ";
            foreach (List<string> listaRespuestaPremisa in respuestasPreliminares)
            {
                foreach (string respuesta in listaRespuestaPremisa)
                {
                    if (respuesta.Contains("="))
                    {
                        respuestaVariable += respuesta + " | ";
                    }
                }
            }
            respuestaVariable += ") ";
            return respuestaVariable;
        }

        private bool verifyCondicion(Condicion condicion, List<List<string>> respuestasPreliminares)
        {
            //Hacer una lista de valores x cada variable [A-Z]
            return true;
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
        
        private bool variablesEquivalentes(Clausula clausula, Consulta consulta)
        {
            List<string> valoresNuevos = new List<string>();
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
                            valoresNuevos.Add(terminoClausula.nombreTermino);
                            nombresVariables.Add(variable.nombreTermino);
                        }
                        else
                        {
                            if (!variable.valores[variable.valores.Count - 1].Equals(terminoClausula.nombreTermino))
                            {
                                //Console.WriteLine("No tienen el mismo nombre atomo así que no son equivalentes");
                                return false;
                            }
                        }
                    }
                    else
                    {
                        valoresNuevos.Add("True match variables");
                    }
                }
            }
            foreach (string valor in valoresNuevos)
            {
                this.valoresAgregados.Add(valor);
            }
            return true;
        }

        /*public void encadenarHaciaAdelante(Consulta consulta)
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
                        consulta.addRespuesta("True", this.valoresAgregados);
                        this.valoresAgregados.Clear();
                    }
                    aplicarClausula(mejorClausula, clausulasAux);
                    clausulasAux.Remove(mejorClausula);
                }
                else
                {
                    break;
                }
            }
        }*/
        public bool encadenarHaciaAdelante(Consulta consulta)
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
                        consulta.addRespuesta("True", this.valoresAgregados);
                        this.valoresAgregados.Clear();
                    }
                    aplicarClausula(mejorClausula, clausulasAux);
                    clausulasAux.Remove(mejorClausula);
                }
                else
                {
                    break;
                }
            }
            if (consulta.respuestas.Count > 0)
            {
                Console.WriteLine("true");
                return true;
            }
            else
            {
                Console.WriteLine("false");
                return false;
            }
        }

        public void encadenarMixto(Consulta consulta)
        {
            // create a cancellation token for tasks
            CancellationTokenSource cts = new CancellationTokenSource();
            var task1 = Task.Factory.StartNew(() => encadenarHaciaAdelante(consulta, cts), cts.Token);
            var task2 = Task.Factory.StartNew(() => encadenarHaciaAtras(consulta, cts), cts.Token);
            Task t = Task.WhenAny(task1, task2);
            if (t.IsCompletedSuccessfully || t.IsCompleted || t.IsCanceled || t.IsFaulted)
                t.Dispose();

        }
        public bool encadenarHaciaAdelante(Consulta consulta, CancellationTokenSource source)
        {
            var token = source.Token;
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
                        consulta.addRespuesta("True", this.valoresAgregados);
                        this.valoresAgregados.Clear();
                    }
                    aplicarClausula(mejorClausula, clausulasAux);
                    clausulasAux.Remove(mejorClausula);
                }
                else
                {
                    break;
                }
            }
            if (consulta.respuestas.Count > 0)
            {
                if (token.IsCancellationRequested)
                {
                    return false;
                }
                Console.WriteLine("true");
                source.Cancel();
                return true;
            }
            else
            {
                if (token.IsCancellationRequested)
                {
                    return false;
                }
                Console.WriteLine("false");
                source.Cancel();
                return false;
            }
        }
        internal bool encadenarHaciaAtras(Consulta consulta, CancellationTokenSource source)
        {
            var token = source.Token;
            List<Clausula> clausulasAux = new List<Clausula>();
            clausulasAux.AddRange(baseDeConocimiento.clausulas);
            if (existeClausula(clausulasAux, consulta))
            {
                List<Clausula> metas = findClausulasMeta(clausulasAux, consulta);
                this.valoresAgregados.Clear();
                foreach (Clausula meta in metas)
                {
                    Consulta consultaAux = new Consulta(meta.compounds[0]);
                    if (resolver(meta, consulta))
                    {
                        foreach (string respuesta in consultaAux.respuestas)
                        {
                            consulta.respuestas.Add(respuesta);
                        }
                    }
                }
                if (consulta.respuestas.Count > 0)
                {
                    if (token.IsCancellationRequested)
                    {
                        return false;
                    }
                    Console.WriteLine("true");
                    source.Cancel();
                    return true;
                }
            }
            if (token.IsCancellationRequested)
            {
                return false;
            }
            Console.WriteLine("false");
            source.Cancel();
            return false;
        }

        private bool existeClausula(List<Clausula> clausulas, Consulta consulta)
        {
            foreach (Clausula clausula in clausulas)
            {
                if (satisfaceConsulta(clausula, consulta))
                {
                    return true;
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

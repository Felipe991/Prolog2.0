namespace Prolonga
{
    internal class MotorDeInferencia
    {
        BaseDeConocimiento baseDeConocimiento;
        Consulta consulta;
        List<string> memoriaRespuestas = new List<string>();
        bool matchPredicado = false;
        bool matchTerminos = false;
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
            if (clausula.predicadoPrincipal.Equals(consulta.predicado))
            {
                matchPredicado = true;
                if (consulta.terminos.Count == clausula.compounds[0].terminos.Count)
                {
                    if (consulta.terminos.Count == 0 && clausula.compounds[0].terminos.Count == 0)
                    {
                        matchTerminos = true;
                        satisfecha = true;
                    }
                    else
                    {
                        matchTerminos = true;
                        satisfecha = terminosEquivalentes(clausula, consulta);
                        Console.WriteLine(satisfecha ? "Se determino que sus terminos eran equivalentes" : "Se determino que sus terminos no eran equivalentes");
                    }
                }
                else
                {
                    satisfecha = false;
                }
            }
            Console.WriteLine("****DEBUG****");
            Console.WriteLine("\nClausula recibida:" + clausula.ToString());
            Console.WriteLine("\nConsulta recibida:" + consulta.ToString());
            Console.WriteLine("\nSatisface = " + satisfecha);
            Console.WriteLine("\n****DEBUG****");
            return satisfecha;
        }

        internal void encadenarHaciaAtras(Consulta consulta)
        {
            foreach (Clausula clausulaBaseConocimiento in baseDeConocimiento.clausulas)
            {
                if (satisfaceConsulta(clausulaBaseConocimiento, consulta))
                {
                    if (clausulaBaseConocimiento is Hecho)
                    {
                        addRespuesta(consulta);
                    }
                    {
                        //Verificar condiciones
                        //mandar a getRespuestas con consulta igual a un compound de la conclusiones (así hasta que se agoten) y si todos retornan true
                        //entonces se cumple la condicion
                        //Si hay alguna que se cumpla asignar los valores correspondientes (true si la regla no contiene ninguna variable, los atomos si es que contiene).
                        Regla regla = (Regla)clausulaBaseConocimiento;
                        //Para cada condicion de la regla
                        //para cada premisa de la condicion buscar los valores
                        //Si existe un match (un atomo que se encuentre en todas las listas de valores con un mismo nombre de termino) se añade a la lista de terminos a agregar
                        //Si la lista de matches contiene por lo menos una, entonces, se encontró solucion
                        checkCondiciones(regla.condiciones,consulta);
                        if (hasCondicionesCumplidas(regla))
                        {
                            addRespuesta(consulta);
                        }
                    }
                }
            }
            if (!matchPredicado)
            {
                consulta.respuestas.Add("No existe procedimiento para: " + consulta.predicado);
            }
            else if (!matchTerminos)
            {
                consulta.respuestas.Add("No existe procedimiento para: " + consulta.predicado + " con " + consulta.terminos.Count + " terminos");
            }
            else if (consulta.respuestas.Count == 0)
            {
                consulta.respuestas.Add("False");
            }
        }


        //Se tiene una regla que calza con los requisitos de la consulta, es decir, mismo predicado y mismos terminos (o terminos equivalentes).
        //Si el compound principal de la regla contiene atomos o variables anonimas entonces de este solo se retorna true o false
        //Si el compound principal tiene variables entonces se retornan los valores de las variables.
        //Una vez se manda a buscar se almacenan los datos recolestados en la consulta auxiliar.
        //Si la consulta principal contiene variables entonces
        private void checkCondiciones(List<Condicion> condiciones,Consulta consultaPadre)
        {
            foreach(Condicion condicion in condiciones)
            {
                List<string> valores = new List<string>();
                foreach (Premisa premisa in condicion.premisas)
                {
                    Consulta consultaAux = new Consulta(premisa.compound);
                    encadenarHaciaAtras(consultaAux);
                    if (!consultaAux.respuestas[0].Equals("False"))
                    {
                        premisa.cumplida = true;
                    }
                }
                condicion.cumplida = premisasCumplidas(condicion.premisas);
            }
        }

        private bool premisasCumplidas(List<Premisa> premisas)
        {
            bool cumplida = true;
            foreach (Premisa premisa in premisas)
            {
                if (!premisa.cumplida)
                {
                    return false;
                }
            }
            return true;
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
                addValoresVariable(nombresVariables, valoresAgregados, consulta);
            }
            return terminosEquivalentes;
        }

        private void addValoresVariable(List<string> nombresVariables, List<string> valoresAgregados, Consulta consulta)
        {
            int contador = 0;
            string respuesta = "";
            foreach (Termino termino in consulta.terminos)
            {
                if (termino is Variable)
                {
                    Variable variable = (Variable)termino;
                    variable.valores.Add(valoresAgregados[contador]);
                    respuesta += variable.nombreTermino + " = " + valoresAgregados[contador] + " ";
                    contador++;
                }
            }
            consulta.respuestas.Add(respuesta);
        }
    }
}

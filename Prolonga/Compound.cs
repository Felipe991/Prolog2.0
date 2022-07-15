using System.Text.RegularExpressions;

namespace Prolonga
{
    internal class Compound
    {
        public string predicado;
        public List<Termino> terminos;
        public List<string> operadores;

        public Compound(string predicado, List<string> nombreTerminos, List<string> operadores)
        {
            this.predicado = predicado;
            this.terminos = new List<Termino>();
            enlistTerminos(nombreTerminos);
            this.operadores = operadores;
        }

        private void enlistTerminos(List<string> terminos)
        {
            foreach (string nombreTermino in terminos)
            {
                if (isAtomo(nombreTermino))
                {
                    this.terminos.Add(new Atomo(nombreTermino));
                }
                else if (isVariableAnonima(nombreTermino))
                {
                    this.terminos.Add(new VariableAnonima(nombreTermino));
                }
                else if (isVariable(nombreTermino))
                {
                    this.terminos.Add(new Variable(nombreTermino));
                }
            }
        }

        private bool isVariable(string nombreTermino)
        {
            bool isVariable = false;
            Regex regexVariable = new Regex("^[A-Z]$");
            if (regexVariable.IsMatch(nombreTermino))
            {
                isVariable = true;
            }
            return isVariable;
        }

        private bool isVariableAnonima(string nombreTermino)
        {
            bool isVariableAnonima = false;
            Regex regexVariable = new Regex("^_$");
            if (regexVariable.IsMatch(nombreTermino))
            {
                isVariableAnonima = true;
            }
            return isVariableAnonima;
        }

        private bool isAtomo(string nombreTermino)
        {
            bool isAtomo = false;
            Regex regexVariable = new Regex("([a-z]).*");
            if (regexVariable.IsMatch(nombreTermino))
            {
                isAtomo = true;
            }
            return isAtomo;
        }

        public override string ToString()
        {
            return "\nPredicado: "+ predicado+
                   "\nTerminos: ("+ getListedTerminos()+")";
        }

        private string getListedTerminos()
        {
            string listedTerminos = terminos[0].nombreTermino+"["+ terminos[0].GetType() + "]";
            for(int index = 0;index<operadores.Count;index++)
            {
                listedTerminos = listedTerminos + operadores[index] + terminos[index + 1].nombreTermino+"["+ terminos[index + 1].GetType()+ "]";
            }
            return listedTerminos;
        }
    }
}
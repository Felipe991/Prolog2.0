namespace Prolonga
{
    internal class Compound
    {
        public string predicado;
        public List<string> terminos;
        public List<string> operadores;

        public Compound(string predicado, List<string> terminos, List<string> operadores)
        {
            this.predicado = predicado;
            this.terminos = terminos;
            this.operadores = operadores;
        }

        public override string ToString()
        {
            return "\nPredicado: "+ predicado+
                   "\nTerminos: ("+ getListedTerminos()+")";
        }

        private string getListedTerminos()
        {
            string listedTerminos = terminos[0];
            for(int index = 0;index<operadores.Count;index++)
            {
                listedTerminos = listedTerminos + operadores[index] + terminos[index + 1];
            }
            return listedTerminos;
        }
    }
}
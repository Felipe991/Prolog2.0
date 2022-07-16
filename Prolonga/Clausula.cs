namespace Prolonga
{
    internal class Clausula
    {
        public string predicadoPrincipal;
        public List<Compound> compounds;

        public Clausula(string predicadoPrincipal, List<Compound> compounds)
        {
            this.predicadoPrincipal = predicadoPrincipal;
            this.compounds = compounds;
        }
        
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prolonga
{
    internal class BaseDeReglas
    {
        List<Regla> reglas;
        public BaseDeReglas(List<Clausula> clausulas)
        {
            reglas = new List<Regla>();
            foreach (Clausula clausula in clausulas)
            {
                if (clausula is Regla)
                {
                    reglas.Add((Regla)clausula);
                }
            }
        }

        public void printReglas()
        {
            Console.WriteLine("\n****IMPRIMIENDO LISTA DE REGLAS DE LA BASE DE REGLAS****\n");
            foreach (Regla regla in reglas)
            {
                Console.WriteLine(regla.ToString());
            }
        }
    }
}

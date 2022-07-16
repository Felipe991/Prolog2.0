using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prolonga
{
    internal class BaseDeConocimiento
    {
        public BaseDeHechos baseDeHechos;
        public BaseDeReglas baseDeReglas;
        public List<Clausula> clausulas;
        public BaseDeConocimiento(BaseDeHechos baseDeHechos, BaseDeReglas baseDeReglas, List<Clausula> clausulas)
        {
            this.baseDeHechos = baseDeHechos;
            this.baseDeReglas = baseDeReglas;
            this.clausulas = clausulas;
        }

    }
}

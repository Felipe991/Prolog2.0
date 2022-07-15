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
        public BaseDeConocimiento(BaseDeHechos baseDeHechos, BaseDeReglas baseDeReglas)
        {
            this.baseDeHechos = baseDeHechos;
            this.baseDeReglas = baseDeReglas;
        }
    }
}

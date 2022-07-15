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

        internal Clausula? consultar(Consulta consulta)
        {
            Clausula respuesta = null;
            respuesta = baseDeHechos.findHechos(consulta);
            if(respuesta is null)
            {
                respuesta = baseDeReglas.findReglas(consulta);
            }
            return respuesta;
        }
    }
}

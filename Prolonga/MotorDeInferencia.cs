using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prolonga
{
    internal class MotorDeInferencia
    {
        BaseDeConocimiento baseDeConocimiento;
        Consulta consulta;
        public MotorDeInferencia(BaseDeConocimiento baseDeConocimiento)
        {
            this.baseDeConocimiento = baseDeConocimiento;
        }

        public void consultar(Consulta consulta)
        {
            this.consulta = consulta;
        }

    }
}

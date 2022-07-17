using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prolonga
{
    internal class Premisa
    {
        public Compound compound;
        public bool cumplida = false;
        public Premisa(Compound compound)
        {
            this.compound = compound;
        }
    }
}

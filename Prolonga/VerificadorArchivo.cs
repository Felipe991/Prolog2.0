using Antlr4.Runtime.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prolonga
{
    internal class VerificadorArchivo : prologBaseVisitor<string>
    {
        public List<Clausula> clausulas;
        List<Compound> compounds;
        string predicado;
        List<string> terminos;
        List<string> operadores;
        List<string> operadoresRegla;
        bool isRegla,insideCompound;

        //Si isRegla es falso es un Hecho.
        //Una regla puede tener varios compunds.
        //Si la cantidad de terminos es > 1 && isRegla es falso entonces es una relacion.
        //Si la cantidad de terminos es == 1 && isRegla es falso entonces es una propiedad.
        //Si la clausula Regla contiene un operador ';' (o) entonces hacer varias reglas con la misma cabeza.
        

        public VerificadorArchivo()
        {
            this.clausulas = new List<Clausula>();
            this.compounds = new List<Compound>();
            this.terminos = new List<string>();
            this.operadores = new List<string>();
            this.operadoresRegla = new List<string>();
            this.predicado = "";
            isRegla = false;
            insideCompound = false;
        }
        
        private void resetCompoundsData()
        {
            //Console.WriteLine("\nNuevo compound\n");
            this.operadores = new List<string>();
            this.terminos = new List<string>();
            insideCompound = false;
            predicado = "";
        }

        private void resetCompoundsList()
        {
            //Console.WriteLine("\nNueva clausula\n");
            this.compounds = new List<Compound>();
            this.operadoresRegla = new List<string>();
            isRegla = false;
        }
        
        private void addNuevaClausula()
        {
            if (isRegla)
            {
                this.clausulas.Add(new Regla(compounds[0].predicado, compounds,operadoresRegla));
            }
            else if (compounds.Count == 0)
            {
                this.clausulas.Add(new Hecho(terminos[0]));
            }
            else if(compounds[0].terminos.Count == 1)
            {
                this.clausulas.Add(new Propiedad(compounds[0].predicado, compounds[0]));
            }else if(compounds[0].terminos.Count > 1)
            {
                this.clausulas.Add(new Relacion(compounds[0].predicado, compounds[0]));
            }
            resetCompoundsData();
            resetCompoundsList();
        }

        private void addNuevoCompound()
        {
            //Console.WriteLine("\nSe ha añadido un compound\n");
            compounds.Add(new Compound(predicado, terminos, operadores));
            resetCompoundsData();
        }

        public override string VisitProlog([NotNull] prologParser.PrologContext context)
        {
            var textoContenido = context.GetText();
            Console.WriteLine("Visit Prolog: " + textoContenido);
            return base.VisitProlog(context);
        }
        
        public override string VisitClause([NotNull] prologParser.ClauseContext context)
        {
            var textoContenido = context.GetText();
            Console.WriteLine("\nVisit Clause: " + textoContenido);
            string retorno = base.VisitClause(context);
            addNuevaClausula();
            return retorno;
        }
        
        public override string VisitCompound_term([NotNull] prologParser.Compound_termContext context)
        {
            var textoContenido = context.GetText();
            Console.WriteLine("Visit Compound_term: " + textoContenido);
            this.insideCompound = true;
            string retorno = base.VisitCompound_term(context);
            this.insideCompound = false;
            addNuevoCompound();
            return retorno;
        }

        public override string VisitTermlist([NotNull] prologParser.TermlistContext context)
        {
            var textoContenido = context.GetText();
            Console.WriteLine("Visit term list: " + textoContenido);
            return base.VisitTermlist(context); ;
        }

        public override string VisitPredicado([NotNull] prologParser.PredicadoContext context)
        {
            var textoContenido = context.GetText();
            predicado = textoContenido;
            Console.WriteLine("Visit predicado: " + textoContenido);
            return base.VisitPredicado(context);
        }

        public override string VisitVariable([NotNull] prologParser.VariableContext context)
        {
            var textoContenido = context.GetText();
            Console.WriteLine("Visit variable: " + textoContenido);
            terminos.Add(textoContenido);
            return base.VisitVariable(context);
        }

        public override string VisitTerm([NotNull] prologParser.TermContext context)
        {
            var textoContenido = context.GetText();
            Console.WriteLine("Visit Term: "+textoContenido);
            return base.VisitTerm(context);
        }

        public override string VisitAtom([NotNull] prologParser.AtomContext context)
        {
            var textoContenido = context.GetText();
            Console.WriteLine("Visit atom: "+textoContenido);
            return base.VisitAtom(context);
        }

        public override string VisitAtom_term([NotNull] prologParser.Atom_termContext context)
        {
            var textoContenido = context.GetText();
            Console.WriteLine("Visit atom_term: "+textoContenido);
            this.terminos.Add(textoContenido);
            return base.VisitAtom_term(context);
        }

        public override string VisitOperator_([NotNull] prologParser.Operator_Context context)
        {
            var textoContenido = context.GetText();
            Console.WriteLine("Visit Operator_: "+textoContenido);
            if (textoContenido.Equals(":-"))
            {
                isRegla = true;
            }
            else if(this.insideCompound)
            {
                this.operadores.Add(textoContenido);
            }
            else
            {
                this.operadoresRegla.Add(textoContenido);
            }
            return base.VisitOperator_(context);
        }

        public void printClausulas()
        {
            foreach (var clausula in clausulas)
            {
                Console.WriteLine(clausula.ToString());
            }
        }

    }
}

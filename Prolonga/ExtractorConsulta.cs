using Antlr4.Runtime.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prolonga
{
    internal class ExtractorConsulta : prologBaseVisitor<string>
    {
        string predicado;
        List<string> terminos;
        List<string> operadores;
        Compound compound;
        public Consulta consulta;
        public ExtractorConsulta()
        {
            predicado = "";
            terminos = new List<string>();
            operadores = new List<string>();
        }
        
        private void addNuevaConsulta()
        {
            consulta = new Consulta(compound);
        }

        private void addNuevoCompound()
        {
            compound = new Compound(predicado, terminos, operadores);
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
            addNuevaConsulta();
            return retorno;
        }

        public override string VisitCompound_term([NotNull] prologParser.Compound_termContext context)
        {
            var textoContenido = context.GetText();
            Console.WriteLine("Visit Compound_term: " + textoContenido);
            string retorno = base.VisitCompound_term(context);
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
            Console.WriteLine("Visit Term: " + textoContenido);
            return base.VisitTerm(context);
        }

        public override string VisitAtom([NotNull] prologParser.AtomContext context)
        {
            var textoContenido = context.GetText();
            Console.WriteLine("Visit atom: " + textoContenido);
            return base.VisitAtom(context);
        }

        public override string VisitAtom_term([NotNull] prologParser.Atom_termContext context)
        {
            var textoContenido = context.GetText();
            Console.WriteLine("Visit atom_term: " + textoContenido);
            this.terminos.Add(textoContenido);
            return base.VisitAtom_term(context);
        }

        public override string VisitOperator_([NotNull] prologParser.Operator_Context context)
        {
            var textoContenido = context.GetText();
            Console.WriteLine("Visit Operator_: " + textoContenido);
            if (textoContenido == ",")
            {
                operadores.Add(textoContenido);
            }
            return base.VisitOperator_(context);
        }

    }
}

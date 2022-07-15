﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prolonga
{
    internal class BaseDeHechos
    {
        List<Hecho> hechos;
        List<Relacion> relaciones;
        List<Propiedad> propiedades;
        public BaseDeHechos(List<Clausula> clausulas)
        {
            hechos = new List<Hecho>();
            relaciones = new List<Relacion>();
            propiedades = new List<Propiedad>();
            foreach (Clausula clausula in clausulas)
            {
                if (clausula is Relacion)
                {
                    relaciones.Add((Relacion)clausula);
                }
                else if (clausula is Propiedad)
                {
                    propiedades.Add((Propiedad)clausula);
                } else if( clausula is Hecho)
                {
                    hechos.Add((Hecho)clausula);
                }
            }
        }

        internal Clausula? findHechos(Consulta consulta)
        {
            Clausula clausula = null;
            if (consulta.terminos.Count == 0)
            {
                clausula = findHecho(consulta);    
            }else if (consulta.terminos.Count == 1)
            {
                clausula = findPropiedad(consulta);
            }else if (consulta.terminos.Count > 1)
            {
                clausula = findRelacion(consulta);
            }
            return clausula;
        }

        private Clausula? findPropiedad(Consulta consulta)
        {
            Clausula clausula = null;
            foreach (Propiedad propiedad in propiedades)
            {
                if (propiedad.predicadoPrincipal.Equals(consulta.predicado) && )
                {
                    
                }
            }
            return clausula;
        }

        private Clausula? findHecho(Consulta consulta)
        {
            Clausula clausula = null;
            foreach (Hecho hecho in hechos)
            {
                if (hecho.predicadoPrincipal.Equals(consulta.predicado)){
                    clausula = hecho;
                }
            }
            return clausula;
        }

        public void printHechos()
        {
            Console.WriteLine("\n****IMPRIMIENDO LISTA DE HECHOS DE LA BASE DE HECHOS****\n");
            foreach (Hecho hecho in hechos)
            {
                Console.WriteLine(hecho.ToString());
            }
            foreach (Propiedad propiedad in propiedades)
            {
                Console.WriteLine(propiedad.ToString());
            }
            foreach (Relacion relacion in relaciones)
            {
                Console.WriteLine(relacion.ToString());
            }
        }
    }
}

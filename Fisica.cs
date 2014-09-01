using Microsoft.DirectX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlumnoEjemplos.TheGRID
{
    class Fisica
    {
        //-----Atributos-----
        private Dibujable duenio;
        private Vector3Doble inercia;
        private Vector3Doble direccion;
        private Doble aceleracion;
        private Doble masa;
        //-------------------
        public Fisica(Dibujable owner)
        {
            duenio = owner;
            inercia.setActual(0, 0, 0);
        }
        public void rotar(float time)
        {

        }
        public void rotar(float time, List<Dibujable> dibujables)
        {

        }
        public Vector3 indicarGravedad(Vector3 posicionSolicitante)
        {
            Vector3 temp = new Vector3();
            return temp;
        }
        public void trasladar(float time)
        {

        }
        public void trasladar(float time, List<Dibujable> dibujables)
        {
            throw new NotImplementedException();
        }
    }
}

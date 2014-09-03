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
        private Doble aceleracion;
        private Doble masa;
        //-------------------
        public Fisica(Dibujable owner)
        {
            duenio = owner;
        }
        public void rotar(float time, List<Dibujable> dibujables)
        {

        }
        public void trasladar(float time, List<Dibujable> dibujables)
        {


            /*
            Vector3 director = vectorDireccion.direccion();
            director.Normalize();
            director.X *= traslacion * velocidad * time;
            director.Y *= traslacion * velocidad * time;
            director.Z *= traslacion * velocidad * time;
            Matrix translate = Matrix.Translation(director);

            Vector4 vector4 = Vector3.Transform(posicion.getActual(), translate);
            posicion.setActual(vector4.X, vector4.Y, vector4.Z);

            Transform *= translate;
            //traslacion = 0;
            */




        }
        public Vector3 indicarGravedad(Vector3 posicionSolicitante)
        {
            Vector3 temp = new Vector3();
            return temp;
        }
        public Vector3 calcularGravedad()
        {
            Vector3 temp = new Vector3();
            return temp;
        }
    }
}

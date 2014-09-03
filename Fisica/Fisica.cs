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
        private float aceleracion;
        private float masa;
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
            Vector3 direccion = duenio.getDireccion();
            Vector3 inercia 

            
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
            




        }
        public void trasladar2(float time, List<Dibujable> dibujables)
        {
            Vector3 direccion = duenio.getDireccion();
            direccion += duenio.getTrayectoria();
            direccion.Normalize();
            direccion.Multiply(aceleracion);
            Vector3 gravedad = calcularGravedad(dibujables, masa);
            direccion += gravedad;



            
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
            




        }
        public Vector3 indicarGravedad(Vector3 posicionSolicitante)
        {
            Vector3 temp = new Vector3();
            return temp;
        }
        public Vector3 calcularGravedad(List<Dibujable> dibujables, float masa)
        {
            Vector3 temp = new Vector3();
            return temp;
        }
    }
}

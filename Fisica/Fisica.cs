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
        public float velocidadMaxima;
        private float masa;
        //-------------------
        public Fisica(Dibujable owner, float vm, float ac, float mas)
        {
            duenio = owner;
            velocidadMaxima = vm;
            aceleracion = ac;
            masa = mas;
        }
        public void rotar(float time, List<Dibujable> dibujables)
        {
            duenio.rotar(time, dibujables);
        }
        public void trasladar(float time, List<Dibujable> dibujables)
        {
            if (duenio.traslacion == 0) 
            { 
                duenio.velocidad = 0;
                return;
            }
            duenio.velocidad += (aceleracion * time);
            
            Vector3 direccion = duenio.getDireccion();
            direccion += duenio.getTrayectoria();
            direccion.Normalize();
            Vector3 gravedad = calcularGravedad(dibujables);
            float acelTemp = Vector3.Length(gravedad);
            gravedad.Normalize();
            if (duenio.velocidad >= velocidadMaxima) duenio.velocidad = velocidadMaxima;
            else acelTemp += aceleracion;
            direccion += gravedad;
            direccion.Multiply(acelTemp);  //Falta si es 0 hacer que no se cancele todo.
            float vT = duenio.velocidad * time;
            float timeTemp = (float)Math.Pow(time, 2);
            timeTemp = timeTemp / (float) 2;
            direccion.Multiply(timeTemp);
            Vector3 temp = direccion;
            temp.Normalize();
            temp.Multiply(vT);
            direccion += temp;
            direccion.Multiply(duenio.traslacion);

            Matrix translate = Matrix.Translation(direccion);

            Vector4 vector4 = Vector3.Transform(duenio.getPosicion(), translate);
            duenio.setPosicion(new Vector3(vector4.X, vector4.Y, vector4.Z));

            duenio.Transform *= translate;
            //duenio.traslacion = 0;
        }
        public Vector3 indicarGravedad(Vector3 posicionSolicitante, float masa)
        {
            Vector3 temp = new Vector3(0,0,0);
            return temp;
        }
        public Vector3 calcularGravedad(List<Dibujable> dibujables)
        {
            List<Vector3> vectores = dibujables.ConvertAll(dibujable => dibujable.indicarGravedad(duenio.getPosicion(),masa));
            Vector3 resultante = new Vector3(0,0,0);
            foreach (Vector3 vector in vectores) resultante += vector;
            return resultante;
        }
    }
}

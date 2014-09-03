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
        public float velocidadInstantanea;
        private float masa;
        //-------------------
        public Fisica(Dibujable owner, float ac, float mas)
        {
            duenio = owner;
            aceleracion = ac;
            masa = mas;
            velocidadInstantanea = 0;
        }
        private float desplazamiento(float vel, float acel, float tiempo)
        {
            float resultado;
            resultado = vel * tiempo;
            //float cuad = (float)Math.Pow(tiempo, 2); //El tiempo elevado al cuadrado da 0 por ser muy poco.
            resultado += (acel * tiempo)/ (float)2;
            return resultado;
        }
        public void rotar(float time, List<Dibujable> dibujables)
        {
            duenio.rotar(time, dibujables);
        }
        public void trasladar(float time, List<Dibujable> dibujables)
        {
            if (velocidadInstantanea < 0) velocidadInstantanea = 0;
            
            //Desplazamiento Gravitacional
            Vector3 auxiliar1 = calcularGravedad(dibujables);
            Vector3 dGravedad = auxiliar1;
            float gravedad = Vector3.Length(dGravedad);
            dGravedad.Normalize();
            float atraccion = desplazamiento(0,gravedad,time);
            dGravedad.Multiply(atraccion);
            
            //Desplazamiento Inercial
            Vector3 dTrayectoria= duenio.getTrayectoria();
            dTrayectoria.Normalize();
            float trayecto = desplazamiento(velocidadInstantanea, 0, time);
            dTrayectoria.Multiply(trayecto);
            
            //Desplazamiento Direccional
            Vector3 auxiliar2 = duenio.getDireccion();
            Vector3 dDireccion = auxiliar2;
            //direccion.Normalize();   //Ya viene normalizado.
            float desplazo = desplazamiento(0, duenio.traslacion * aceleracion, time);
            dDireccion.Multiply(desplazo);

            //Unimos y armamos la matriz
            dDireccion += dGravedad;
            dDireccion += dTrayectoria;
            Matrix translate = Matrix.Translation(dDireccion);

            Vector4 vector4 = Vector3.Transform(duenio.getPosicion(), translate);
            duenio.setPosicion(new Vector3(vector4.X, vector4.Y, vector4.Z));

            duenio.Transform *= translate;

            //Calculo de la Velocidad actual.
            auxiliar1.Normalize();
            auxiliar1.Multiply(gravedad);
            auxiliar2.Multiply(duenio.traslacion * aceleracion);
            auxiliar1 += auxiliar2;
            float acelGlobal = Vector3.Length(auxiliar1);
            velocidadInstantanea += acelGlobal * time;
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

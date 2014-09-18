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
        internal float aceleracion { set; get; }
        public float velocidadInstantanea;
        private float masa;
        public float Masa { get { return masa; } }
        internal bool frenado;
        internal float acelFrenado { set; get; }
        //-------------------
        public Fisica(Dibujable owner, float acel, float aFrenado, float masaCuerpo)    //La aceleracion de frenado recomiendo poner un valor mayor que acel.
        {
            duenio = owner;
            aceleracion = acel;
            masa = masaCuerpo;
            velocidadInstantanea = 0;
            frenado = false;
            acelFrenado = aFrenado;
        }
        private float desplazamiento(float vel, float acel, float tiempo)
        {
            float resultado;
            resultado = vel * tiempo;
            resultado += (acel * tiempo)/ (float)2;
            return resultado;
        }
        public void rotar(float time, List<Dibujable> dibujables)
        {
            //duenio.rotar(time, dibujables);
        }
        public void trasladar(float time, List<Dibujable> dibujables)
        {
            if (!frenado)
            {
                if (velocidadInstantanea < 0) velocidadInstantanea = 0;

                //Desplazamiento Gravitacional
                Vector3 auxiliar1 = calcularGravedad(dibujables);
                Vector3 dGravedad = auxiliar1;
                float gravedad = Vector3.Length(dGravedad);
                dGravedad.Normalize();
                float atraccion = desplazamiento(0, gravedad, time);
                dGravedad.Multiply(atraccion);

                //Desplazamiento Inercial
                Vector3 dTrayectoria = duenio.getTrayectoria();
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
                if (velocidadInstantanea < 0) velocidadInstantanea = 0;
            }
            else
            {
                if (velocidadInstantanea != 0)
                {
                    //Solo vamos a usar desplazamiento en trayectoria, es decir, el inercial.
                    Vector3 dTrayectoria = duenio.getTrayectoria();
                    dTrayectoria.Normalize();
                    float trayecto = desplazamiento(velocidadInstantanea, -acelFrenado, time);
                    if (trayecto < 0) trayecto = 0;
                    dTrayectoria.Multiply(trayecto);

                    //Armamos la matriz
                    Matrix translate = Matrix.Translation(dTrayectoria);
                    Vector4 vector4 = Vector3.Transform(duenio.getPosicion(), translate);
                    duenio.setPosicion(new Vector3(vector4.X, vector4.Y, vector4.Z));

                    duenio.Transform *= translate;

                    velocidadInstantanea -= acelFrenado * time;
                    if (velocidadInstantanea < 0) velocidadInstantanea = 0;
                }
            frenado = false;
            }
        }
        public Vector3 indicarGravedad(Vector3 posicionSolicitante, float mass)
        {
            posicionSolicitante -= duenio.getPosicion();
            posicionSolicitante.Multiply(-1);
            float distanciaCuad = Vector3.LengthSq(posicionSolicitante);
            float gravity = mass * masa;
            gravity = gravity / distanciaCuad;
            gravity *= (float) 0.0001; //Aca deberia ir el coeficiente de gravitacion universal.
            posicionSolicitante.Normalize();
            posicionSolicitante.Multiply(gravity);
            return posicionSolicitante;
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

using Microsoft.DirectX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AlumnoEjemplos.TheGRID.Helpers;

namespace AlumnoEjemplos.TheGRID
{
    class Fisica
    {
        //-----Atributos-----
        private Dibujable duenio;
        internal float aceleracion { set; get; }
        public float velocidadInstantanea;
        public float velocidadMaxima;
        public float velocidadCrucero;
        private float masa;
        public float Masa { get { return masa; } }
        public Vector3 ultimaDireccionCalculada = new Vector3(0,0,0);
        internal bool frenado;
        internal float acelFrenado { set; get; }
        //-------------------

        public Fisica(Dibujable owner, float acel, float aFrenado, float velMax, float masaCuerpo)    //La aceleracion de frenado recomiendo poner un valor mayor que acel.
        {
            duenio = owner;
            aceleracion = acel;
            masa = masaCuerpo;
            velocidadInstantanea = 0;
            frenado = false;
            acelFrenado = aFrenado;
            velocidadMaxima = velMax;
            velocidadCrucero = velMax;
        }
        private float calcularDistanciaDesplazada(float vel, float acel, float tiempo)
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
        public Vector3 calcularTraslado(float time, List<Dibujable> dibujables)
        {
            Vector3 devolucion = new Vector3();
            if (!frenado)
            {
                if (velocidadInstantanea < 0) velocidadInstantanea = 0;

                //Desplazamiento Gravitacional
                Vector3 auxiliar1 = calcularGravedad(dibujables);
                Vector3 dGravedad = auxiliar1;
                float gravedad = Vector3.Length(dGravedad);
                dGravedad.Normalize();
                float atraccion = calcularDistanciaDesplazada(0, gravedad, time);
                dGravedad.Multiply(atraccion);

                //Desplazamiento Inercial
                Vector3 dTrayectoria = duenio.getTrayectoriaInercial();
                dTrayectoria.Normalize();
                float trayecto = calcularDistanciaDesplazada(velocidadInstantanea, 0, time);
                dTrayectoria.Multiply(trayecto);

                //Desplazamiento Direccional
                Vector3 auxiliar2 = duenio.getDireccion();
                Vector3 dDireccion = auxiliar2;
                //direccion.Normalize();   //Ya viene normalizado.
                float desplazo = calcularDistanciaDesplazada(0, duenio.traslacion * aceleracion, time);
                dDireccion.Multiply(desplazo);

                //Unimos
                dDireccion += dGravedad;
                dDireccion += dTrayectoria;
                devolucion = dDireccion;

                //Calculo de la Velocidad actual.
                auxiliar1.Normalize();
                auxiliar1.Multiply(gravedad);
                auxiliar2.Multiply(duenio.traslacion * aceleracion);
                auxiliar1 += auxiliar2;
                float acelGlobal = Vector3.Length(auxiliar1);
                float temp = velocidadInstantanea + acelGlobal * time;
                //if (velocidadInstantanea < 0) velocidadInstantanea = 0;
                if (temp > velocidadCrucero) velocidadInstantanea = velocidadCrucero;
                else velocidadInstantanea = temp;
            }
            else
            {
                if (velocidadInstantanea != 0)
                {
                    //Solo vamos a usar desplazamiento en trayectoria, es decir, el inercial.
                    Vector3 dTrayectoria = duenio.getTrayectoriaInercial();
                    dTrayectoria.Normalize();
                    float trayecto = calcularDistanciaDesplazada(velocidadInstantanea, -acelFrenado, time);
                    if (trayecto < 0) trayecto = 0;
                    dTrayectoria.Multiply(trayecto);
                    devolucion = dTrayectoria;

                    velocidadInstantanea -= acelFrenado * time;
                    if (velocidadInstantanea < 0) velocidadInstantanea = 0;
                }
            frenado = false;
            velocidadCrucero = velocidadMaxima;
            }
            ultimaDireccionCalculada = devolucion;
            return devolucion;
        }
        public void activarCrucero() { velocidadCrucero = velocidadInstantanea; }
        public void desactivarCrucero() { velocidadCrucero = velocidadMaxima; }
        public void impulsar(Vector3 direccionDeImpulso, float velocidadDeImpulso, float elapsedTimed)
        {
            direccionDeImpulso.Normalize();
            direccionDeImpulso.Multiply(velocidadDeImpulso * elapsedTimed);
            duenio.desplazarUnaDistancia(direccionDeImpulso);
            velocidadInstantanea = velocidadDeImpulso;
        }
        public Vector3 indicarGravedad(Vector3 posicionSolicitante, float mass)
        {
            posicionSolicitante -= duenio.getPosicion();
            posicionSolicitante.Multiply(-1);
            float distanciaCuad = Vector3.Length(posicionSolicitante);
            if (distanciaCuad < 1) return new Vector3(0, 0, 0);
            distanciaCuad *= distanciaCuad;
            float gravity = mass * masa;
            gravity = gravity / distanciaCuad;
            gravity *= (float) 0.001; //Aca deberia ir el coeficiente de gravitacion universal.
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

        internal static Dupla<Vector3> CalcularChoqueElastico(Dibujable colisionado, Dibujable colisionador)
        {
            float X;
            float Z;
            Vector3 velocidad_colisionado = colisionado.ultimaTraslacion;       //.fisica.ultimaDireccionCalculada;
            X = velocidad_colisionado.X;
            Z = velocidad_colisionado.Z;
            Vector3 velocidad_colisionador = colisionador.ultimaTraslacion;     //.fisica.ultimaDireccionCalculada;
            velocidad_colisionado.X = velocidad_colisionador.X;
            velocidad_colisionado.Z = velocidad_colisionador.Z;
            velocidad_colisionador.X = X;
            velocidad_colisionador.Z = Z;
            return new Dupla<Vector3>(velocidad_colisionador, velocidad_colisionado);
        }
    }
}

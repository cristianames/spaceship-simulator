using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.DirectX;
using TgcViewer;
using TgcViewer.Utils.TgcSceneLoader;
using System.Drawing;


namespace AlumnoEjemplos.TheGRID
{
    public struct Doble
    {
        private float actual;
        private float anterior;
        public void setActual(float nuevo)
        {
            anterior = actual;
            actual = nuevo;
        }
        public float getActual() { return actual; }
        public float getAnterior() { return anterior; }
    }
    //------------------------------------------------------------------
    public struct Vector3Doble
    {
        private Vector3 actual;
        private Vector3 anterior;
        public void setActual(Vector3 nuevo){
            anterior = actual;
            actual = nuevo;
        }
        public void setActual(float X, float Y, float Z)
        {
            anterior = actual;
            actual.X = X;
            actual.Y = Y;
            actual.Z = Z;
        }
        public Vector3 getActual() { return actual; }
        public Vector3 getAnterior() { return anterior; }
        public Vector3 direccion()
        {
            Vector3 temp = new Vector3(actual.X, actual.Y, actual.Z);
            temp.Subtract(anterior);
            return temp;
        }
    }
    //------------------------------------------------------------------
    public interface IComun : IRenderObject, ITransformObject
    {
    }
    //------------------------------------------------------------------
    class Dibujable
    {
        private Vector3Doble posicion;
        private Doble aceleracion;
        private Doble velocidad;
        public Object objeto;
        
        public Dibujable()
        {
            posicion.setActual(0, 0, 0);
            aceleracion.setActual(0);
            velocidad.setActual(0);
        }   
    }
}

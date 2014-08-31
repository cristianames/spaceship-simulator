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
    public interface IComun : IRenderObject, ITransformObject {}
    //------------------------------------------------------------------
    class Dibujable
    {
        private Vector3Doble posicion;
        private Vector3Doble aceleracion;
        private Vector3Doble velocidad;
        public Object objeto;
        public Object fisica;
        public Object colisiones;        
        public Dibujable()
        {
            posicion.setActual(0, 0, 0);
            aceleracion.setActual(0, 0, 0);
            velocidad.setActual(0, 0, 0);
            fisica = null;
            colisiones = null;
        }
        //-----IRenderObject-----
        public void render() { ((IRenderObject)objeto).render(); }
        public void dispose() 
        { 
            ((IRenderObject)objeto).dispose();
            //fisica.dispose();
            //colisiones.dispose();            
        }
        bool AlphaBlendEnable
        {
            get { return ((IRenderObject)objeto).AlphaBlendEnable; }
            set { ((IRenderObject)objeto).AlphaBlendEnable = value; }
        }
        //-----IRenderObject-----
        //-----ITransformObject-----
        Matrix Transform
        {
            get { return ((ITransformObject)objeto).Transform; }
            set { ((ITransformObject)objeto).Transform = value; }
        }
        bool AutoTransformEnable
        {
            get { return ((ITransformObject)objeto).AutoTransformEnable; }
            set { ((ITransformObject)objeto).AutoTransformEnable = value; }
        }
        Vector3 Position
        {
            get { return ((ITransformObject)objeto).Position; }
            set { ((ITransformObject)objeto).Position = value; }
        }
        Vector3 Rotation
        {
            get { return ((ITransformObject)objeto).Rotation; }
            set { ((ITransformObject)objeto).Rotation = value; }
        }
        Vector3 Scale
        {
            get { return ((ITransformObject)objeto).Scale; }
            set { ((ITransformObject)objeto).Scale = value; }
        }
        void move(Vector3 v) { ((ITransformObject)objeto).move(v); }
        void move(float x, float y, float z) { ((ITransformObject)objeto).move(x,y,z); }
        void moveOrientedY(float movement) { ((ITransformObject)objeto).moveOrientedY(movement); }
        void getPosition(Vector3 pos) { ((ITransformObject)objeto).getPosition(pos); }
        void rotateX(float angle) { ((ITransformObject)objeto).rotateX(angle); }
        void rotateY(float angle) { ((ITransformObject)objeto).rotateY(angle); }
        void rotateZ(float angle) { ((ITransformObject)objeto).rotateZ(angle); }
        //-----ITransformObject-----
    }
}

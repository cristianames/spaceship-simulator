using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.DirectX;
using TgcViewer;
using TgcViewer.Utils.TgcSceneLoader;
using System.Drawing;
using Microsoft.DirectX.Direct3D;
using AlumnoEjemplos.TheGRID.Explosiones;
using AlumnoEjemplos.TheGRID.Colisiones;


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
        private bool sonIguales(Vector3 v1, Vector3 v2)
        {
            bool iguales=true;
            if (v1.X != v2.X) iguales = false;
            if (v1.Y != v2.Y) iguales = false;
            if (v1.Z != v2.Z) iguales = false;
            return iguales;
        }
        public Vector3 direccion(){
            Vector3 dir;
            if (sonIguales(actual, anterior)) dir = new Vector3(0, 0, -1);
            else
            {
                dir = new Vector3(actual.X, actual.Y, actual.Z);
                dir.Subtract(anterior);
            }
            dir.Normalize();
            return dir;
        }
    }
    //------------------------------------------------------------------
    public interface IComun : IRenderObject, ITransformObject {}
    //------------------------------------------------------------------
    class Dibujable
    {
        //-----Atributos-----
        private Vector3Doble posicion;
        public Vector3Doble normal;
        public Vector3Doble direccion;
        public float velocidad { set; get; }
        public float velocidadRadial { set; get; }
        public int traslacion { set; get; } // 0: Nada ; -1:frenado ; 1:acelerado
        public int inclinacion { set; get; } // 0: Nada ; -1:hacia abajo ; 1:hacia arriba //Pitch
        public int rotacion { set; get; } // 0: Nada ; -1:lateral izquierda ; 1:lateral derecha //Roll
        // Doblar hacia la derecha o izquierda se hace rotando e inclinando, como un avion. //Yaw
        public Object objeto { set; get; }
        private Fisica fisica; // Acá cargamos las consideraciones del movimiento especializado.
        private IColision colision; // Acá va la detecciones de colisiones según cada objeto lo necesite.
        private IExplosion explosion; // Acá va el manejo de un objeto cuando es chocado por otro.
        //-------------------
        
        public Dibujable()
        {
            posicion.setActual(0, 0, 0);
            normal.setActual(0, 1, 0);
            direccion.setActual(0, 0, -1);
            fisica = null;
            colision = null;
            explosion = null;
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
        //-----------------------

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
        public Vector3 Position
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
        void move(Vector3 v) { this.move(v.X, v.Y, v.Z); }
        void move(float x, float y, float z) 
        { 
            ((ITransformObject)objeto).move(x,y,z); 
            Vector3 temp = new Vector3();
            getPosition(temp);
            posicion.setActual(temp);
        }
        void moveOrientedY(float movement) 
        {
            ((ITransformObject)objeto).moveOrientedY(movement); 
            Vector3 temp = new Vector3();
            getPosition(temp);
            posicion.setActual(temp);
        }
        void getPosition(Vector3 pos) { ((ITransformObject)objeto).getPosition(pos); }
        void rotateX(float angle) { ((ITransformObject)objeto).rotateX(angle); }
        void rotateY(float angle) { ((ITransformObject)objeto).rotateY(angle); }
        void rotateZ(float angle) { ((ITransformObject)objeto).rotateZ(angle); }
        //--------------------------
        public void setObject(Object cosa) { objeto = cosa; }
        //public Vector3 direccion() { return posicion.direccion(); }

        public void rotar(float time)
        {
            if (fisica != null) fisica.rotar(time);
            else
            {
                float angulo = Geometry.DegreeToRadian(velocidadRadial * time);
                //if (inclinacion != 0)rotarX(angulo * inclinacion);
                if (rotacion != 0) rotarZ(angulo * rotacion);
                //rotateX(angulo * inclinacion);
                //rotateZ(angulo * rotacion);
                inclinacion = 0;
                rotacion = 0;
            }
        }

        /*
       Vector3 p = new Vector3(10, 5, 10);
       Vector4 transformedVec4 = Vector3.Transform(p, movimientoFinal); //Devuelve un Vector4 poque estan las coordenadas homogeneas
       Vector3 transformedVec3 = new Vector3(transformedVec4.X, transformedVec4.Y, transformedVec4.Z); //Ignoramos la componente W
       */
        private void rotarX(float angulo)
        {
            Vector3 x = Vector3.Cross(normal.getActual(), direccion.getActual());
            x.Normalize();
            Matrix rotation = Matrix.RotationYawPitchRoll(x.Y * angulo, x.X * angulo, x.Z * angulo);
            Vector4 normal4 = Vector3.Transform(x, rotation);
            normal.setActual(normal4.X, normal4.Y, normal4.Z);
            Vector4 dir4 = Vector3.Transform(x, rotation);
            direccion.setActual(normal4.X, normal4.Y, normal4.Z);
            Transform *= rotation;
            //rotateX(x.X * angulo);
            //rotateY(x.Y * angulo);
            //rotateZ(x.Z * angulo);
        }
        private void rotarZ(float angulo)
        {
            Vector3 z = direccion.getActual();
            z.Normalize();
            //Matrix rotation = Matrix.RotationZ(angulo);
            Matrix rotation = Matrix.RotationYawPitchRoll(0*z.Y* angulo,0* z.X * angulo, z.Z * angulo);
            Vector4 normal4 = Vector3.Transform(z, rotation);
            normal.setActual(normal4.X, normal4.Y, normal4.Z);
            Vector4 dir4 = Vector3.Transform(z, rotation);
            direccion.setActual(normal4.X, normal4.Y, normal4.Z);
            Transform *= rotation;
            //rotateX(z.X * angulo);
            //rotateY(z.Y * angulo);
            //rotateZ(z.Z * angulo);
        }
        public void rotar(float time, List<Dibujable> dibujables) { if (fisica != null) fisica.rotar(time, dibujables); }

        public void trasladar(float time)
        {
            if (fisica != null) fisica.trasladar(time);
            else
            {
                //Vector3 temp = posicion.direccion();
                Vector3 temp = direccion.getActual();
                ((TgcMesh)objeto).move(temp.X * traslacion * velocidad * time, temp.Y * traslacion * velocidad * time, temp.Z * traslacion * velocidad * time);
                //traslacion = 0;
                //moveOrientedY(traslacion * velocidad * time);
            }
        }
        public void trasladar(float time, List<Dibujable> dibujables) { if (fisica != null) fisica.trasladar(time, dibujables); }

    }
}

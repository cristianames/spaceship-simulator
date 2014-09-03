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
            if (sonIguales(actual, anterior)) dir = new Vector3(0, 0, 1);
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
        public float velocidad { set; get; }
        public float velocidadRadial { set; get; }
        public int traslacion { set; get; } // 0: Nada ; -1:frenado ; 1:acelerado
        public int inclinacion { set; get; } // 0: Nada ; -1:hacia abajo ; 1:hacia arriba //Pitch
        public int rotacion { set; get; } // 0: Nada ; -1:lateral izquierda ; 1:lateral derecha //Roll
        // Doblar hacia la derecha o izquierda se hace rotando e inclinando, como un avion. //Yaw
        public Object objeto { set; get; }
        public EjeCoordenadas vectorDireccion;
        private Fisica fisica; // Acá cargamos las consideraciones del movimiento especializado.
        private IColision colision; // Acá va la detecciones de colisiones según cada objeto lo necesite.
        private IExplosion explosion; // Acá va el manejo de un objeto cuando es chocado por otro.
        
        //-------------------
        
        public Dibujable()
        {
            posicion.setActual(0, 0, 0);
            fisica = null;
            colision = null;
            explosion = null;
            vectorDireccion = new EjeCoordenadas();
            vectorDireccion.centrar(0, 0, 0);
        }
        public Dibujable(float x, float y, float z)
        {
            posicion.setActual(0, 0, 0);
            fisica = null;
            colision = null;
            explosion = null;
            vectorDireccion = new EjeCoordenadas();
            vectorDireccion.centrar(x, y, z);
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
        public Matrix Transform
        {
            get { return ((ITransformObject)objeto).Transform; }
            set { ((ITransformObject)objeto).Transform = value; }
        }
        public bool AutoTransformEnable
        {
            get { return ((ITransformObject)objeto).AutoTransformEnable; }
            set { ((ITransformObject)objeto).AutoTransformEnable = value; }
        }
        /*   ACLARACION: No deberían ser necesarios.
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
        */
        //--------------------------

        //----------INSTANCIADOR----------
        public void setObject(Object cosa)
        {
            objeto = cosa;
            AutoTransformEnable = false;
        }
        public void setObject(Object cosa, float vLineal, float vRadial)
        {
            objeto = cosa;
            AutoTransformEnable = false;
            velocidad = vLineal;
            velocidadRadial = vRadial;
        }
        public void setObject(Object cosa, float vLineal, float vRadial, Vector3 rotacion, Vector3 escalado)
        {
            objeto = cosa;
            AutoTransformEnable = false;
            velocidad = vLineal;
            velocidadRadial = vRadial;
            rotacion.X = Geometry.DegreeToRadian(rotacion.X);
            rotacion.Y = Geometry.DegreeToRadian(rotacion.Y);
            rotacion.Z = Geometry.DegreeToRadian(rotacion.Z);
            Matrix matriz = Matrix.Scaling(escalado);
            matriz *= Matrix.RotationYawPitchRoll(rotacion.Y, rotacion.X, rotacion.Z);
            Transform *= matriz;
        }
        public void setCentro(float x, float y, float z) { vectorDireccion.centrar(x, y, z); }
        public Vector3 getCentro() 
        {
            Vector3 temp = getPosicion();
            temp.Subtract(vectorDireccion.getCentro());
            return temp;
        }

        public Vector3 direccion() { return vectorDireccion.direccion(); }
        //--------------------------------

        //----------MOVIMIENTOS----------
        public Vector3 getPosicion() { return posicion.getActual(); }
        public void rotar(float time, List<Dibujable> dibujables)
        {
            if (fisica != null) fisica.rotar(time, dibujables);
            else
            {
                float angulo = velocidadRadial * time;
                Matrix rotation;
                if (inclinacion != 0) //Rotar en X
                {
                    rotation = vectorDireccion.rotarX_desde(posicion.getActual(), angulo * inclinacion);
                    Transform *= rotation;
                }
                if (rotacion != 0) //Rotar en Z
                {
                    rotation = vectorDireccion.rotarZ_desde(posicion.getActual(), angulo * rotacion);
                    Transform *= rotation;
                }
                inclinacion = 0;
                rotacion = 0;
            }
        }
        public void trasladar(float time, List<Dibujable> dibujables)
        {
            if (fisica != null) fisica.trasladar(time, dibujables);
            else
            {
                Vector3 vectorResultante = fisica.calcularGravedad(dibujables);

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
        }

        public void escalar(Vector3 escalado)
        {
            Matrix matriz = Matrix.Scaling(escalado);
            Transform *= matriz;
        }

        public void escalar(float x, float y, float z)
        {
            Matrix matriz = Matrix.Scaling(x, y, z);
            Transform *= matriz;
        }

        public Vector3 indicarGravedad(Vector3 pos, float mass){
            return this.fisica.indicarGravedad(pos,mass);
        }

        public void renderBoundingBox() { colision.render();}
        public IColision getColision() { return this.colision; }
        public void setColision(IColision bb) { this.colision = bb; }
    }
}

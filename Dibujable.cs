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
        //----------------------------------------------------------------------------------------------------ATRIBUTOS-----
        private Vector3Doble posicion;
        public float velocidad { set; get; }
        public float velocidadRadial { set; get; }
        public int traslacion { set; get; } // 0: Nada ; -1:frenado ; 1:acelerado
        public int inclinacion { set; get; } // 0: Nada ; -1:hacia abajo ; 1:hacia arriba //Pitch
        public int rotacion { set; get; } // 0: Nada ; -1:lateral izquierda ; 1:lateral derecha //Roll
        public int giro { set; get; } // 0: Nada ; -1:lateral izquierda ; 1:lateral derecha //Yaw
        internal bool velocidadManual { set; get; }   //Indica si hay que mantener apretado para moverte o no.
        internal bool desplazamientoReal { set; get; }    //Se usa o no el modulo de Fisica para el desplazamiento.
        internal bool rotacionReal { set; get; }  //Se usa o no el modulo de Fisica para la rotacion.
        public Object objeto { set; get; }
        private EjeCoordenadas vectorDireccion;
        internal Fisica fisica; // Acá cargamos las consideraciones del movimiento especializado.
        private IColision colision; // Acá va la detecciones de colisiones según cada objeto lo necesite.
        private IExplosion explosion; // Acá va el manejo de un objeto cuando es chocado por otro.
        
        //----------------------------------------------------------------------------------------------------INSTANCIADOR-----
        public Dibujable()
        {
            posicion.setActual(0, 0, 0);
            fisica = null;
            colision = null;
            explosion = null;
            vectorDireccion = new EjeCoordenadas();
            vectorDireccion.centrar(0, 0, 0);
            velocidadManual = false;
            desplazamientoReal = false;
            rotacionReal = false;
        }
        public Dibujable(float x, float y, float z)
        {
            posicion.setActual(0, 0, 0);
            fisica = null;
            colision = null;
            explosion = null;
            vectorDireccion = new EjeCoordenadas();
            vectorDireccion.centrar(x, y, z);
            velocidadManual = false;
            desplazamientoReal = false;
            rotacionReal = false;
        }
        //-------------------------------------------------------------------------------------METODOS--------IRenderObject-----
        public void render() { ((IRenderObject)objeto).render(); }
        public void dispose() 
        {
            try { ((IRenderObject)objeto).dispose(); }
            catch { }
            //fisica.dispose();
            //colisiones.dispose();            
        }
        bool AlphaBlendEnable
        {
            get { return ((IRenderObject)objeto).AlphaBlendEnable; }
            set { ((IRenderObject)objeto).AlphaBlendEnable = value; }
        }
        //-------------------------------------------------------------------------------------METODOS--------ITransformObject-----
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
        //----------------------------------------------------------------------------------------------------SETTERS-----
        public void setObject(Object cosa)  //Solo le pasas el objeto renderizable.
        {
            objeto = cosa;
            AutoTransformEnable = false;
            Transform *= Matrix.Identity;
        }
        public void setObject(Object cosa, float vLineal, float vRadial)    //Le agregas la velocidad maxima lineal y radial.
        {
            objeto = cosa;
            AutoTransformEnable = false;
            velocidad = vLineal;
            velocidadRadial = vRadial;
            Transform *= Matrix.Identity;
        }
        public void setObject(Object cosa, float vLineal, float vRadial, Vector3 rotacion, Vector3 escalado)
        {  //Le agregas ademas un vector para la matriz de rotacion y otro para la de escalado. Para acomodar el objeto de forma inicial.
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
            Transform *= Matrix.Identity;
        }
        public void setFisica(float acel, float aFrenado, float masaCuerpo) { fisica = new Fisica(this, acel, aFrenado, masaCuerpo); }
                //Carga un nuevo módulo de fisica.
        public void SetPropiedades(bool velMan, bool despReal, bool rotReal)    //La velocidad vuelve a 0 cuando no ocurre un evento continuo.
        {       //Se usa el desplazamiento y la rotacion del modulo de fisica. Por defecto viene todo false.
            velocidadManual = velMan;
            desplazamientoReal = despReal;
            rotacionReal = rotReal;
        }
        public void setCentro(float x, float y, float z) { vectorDireccion.centrar(x, y, z); } //Acomoda el centro de giro del objeto.
        public Vector3 getCentro() 
        {
            Vector3 temp = getPosicion();
            temp.Subtract(vectorDireccion.getCentro());
            return temp;
        }
        public void setPosicion(Vector3 pos) 
        {
            posicion.setActual(pos);
        }   //No manipular a menos que sea necesario. Se pierde coherencia con la posicion que lleva el objeto renderizable.
        public Vector3 getPosicion() { return posicion.getActual(); }

        //----------------------------------------------------------------------------------------------------MOVIMIENTOS-----
        public Vector3 getTrayectoria() { return posicion.direccion(); }   //Direccion en la que se desplaza un objeto.
        public Vector3 getDireccion() { return vectorDireccion.direccion(); }   //Direccion en la que apunta el frente del objeto.
        public void rotar(float time, List<Dibujable> dibujables)   //Movimiento de rotacion base de un dibujable.
        {
            if (fisica != null && rotacionReal) fisica.rotar(time, dibujables);
            else
            {
                float angulo = velocidadRadial * time;
                Matrix rotation;
                if (inclinacion != 0) //Rotar en X
                {
                    rotation = vectorDireccion.rotarX_desde(posicion.getActual(), angulo * inclinacion);
                    Transform *= rotation;
                }
                if (giro != 0) //Rotar en Y
                {
                    rotation = vectorDireccion.rotarY_desde(posicion.getActual(), angulo * giro);
                    Transform *= rotation;
                }
                if (rotacion != 0) //Rotar en Z
                {
                    rotation = vectorDireccion.rotarZ_desde(posicion.getActual(), angulo * rotacion);
                    Transform *= rotation;
                }
            }
            if (velocidadManual)
            {
                inclinacion = 0;
                rotacion = 0;
                giro = 0;
            }
        }
        internal void acelerar() { traslacion = 1; }

        internal void frenar()
        {
            if (fisica != null && velocidadManual) fisica.frenado = true;
            else traslacion = 0;
        }
        public void trasladar(float time, List<Dibujable> dibujables)   //Movimiento de traslacion base de un dibujable.
        {
            if (fisica != null && desplazamientoReal) fisica.trasladar(time, dibujables);
            else
            {
                Vector3 director = vectorDireccion.direccion();
                director.Normalize();
                director.X *= traslacion * velocidad * time;
                director.Y *= traslacion * velocidad * time;
                director.Z *= traslacion * velocidad * time;
                Matrix translate = Matrix.Translation(director);

                Vector4 vector4 = Vector3.Transform(posicion.getActual(), translate);
                posicion.setActual(vector4.X, vector4.Y, vector4.Z);

                Transform *= translate;
            }
            if (velocidadManual) traslacion = 0;
        }
        //----------------------------------------------------------------------------------------------------TRANSFORMACIONES-----
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
        //----------------------------------------------------------------------------------------------------CONSULTAS-----
        public Vector3 indicarGravedad(Vector3 pos, float mass){
            if (fisica != null) return this.fisica.indicarGravedad(pos, mass);
            else return new Vector3(0, 0, 0);
        }

        public float getAceleracion() { if (fisica != null) return fisica.aceleracion; else return 0; }
        public float getAcelFrenado() { if (fisica != null) return fisica.acelFrenado; else return 0; }
        public void renderBoundingBox() { colision.render();}
        public IColision getColision() { return this.colision; }
        public void setColision(IColision bb) { this.colision = bb; }

        internal float velocidadActual()
        {
            if (fisica != null && desplazamientoReal) return fisica.velocidadInstantanea;
            else return velocidad;
        }
    }
}

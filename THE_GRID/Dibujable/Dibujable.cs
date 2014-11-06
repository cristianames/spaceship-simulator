using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.DirectX;
using TgcViewer;
using TgcViewer.Utils.TgcSceneLoader;
using System.Drawing;
using Microsoft.DirectX.Direct3D;
using AlumnoEjemplos.THE_GRID.Explosiones;
using AlumnoEjemplos.THE_GRID.Colisiones;
using TgcViewer.Utils.TgcGeometry;
using AlumnoEjemplos.THE_GRID.Shaders;


namespace AlumnoEjemplos.THE_GRID
{
    #region Estructuras previas
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
        public void addActual(Vector3 nuevo)
        {
            anterior = actual;
            actual.Add(new Vector3(0,FastMath.PI_HALF,0));
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
        public Vector3 direccionDesplazamiento(){
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
    #endregion

    public class Dibujable
    {
        #region Atributos
        private Vector3Doble posicion;
        public float velocidad { set; get; }
        public float velocidadRadial { set; get; }
        public int traslacion { set; get; } // 0: Nada ; -1:frenado ; 1:acelerado
        public float inclinacion { set; get; } // 0: Nada ; -1:hacia abajo ; 1:hacia arriba //Pitch
        public float rotacion { set; get; } // 0: Nada ; -1:lateral izquierda ; 1:lateral derecha //Roll
        public int giro { set; get; } // 0: Nada ; -1:lateral izquierda ; 1:lateral derecha //Yaw
        internal bool velocidadManual { set; get; }   //Indica si hay que mantener apretado para moverte o no.
        internal bool desplazamientoReal { set; get; }    //Se usa o no el modulo de Fisica para el desplazamiento.
        internal bool rotacionReal { set; get; }  //Se usa o no el modulo de Fisica para la rotacion.
        internal Vector3 escala;
        internal Vector3 ultimaTraslacion { set; get; }
        internal TgcMesh objeto { set; get; }
        private EjeCoordenadas vectorDireccion;
        internal Fisica fisica; // Acá cargamos las consideraciones del movimiento especializado.
        protected IColision colision; // Acá va la detecciones de colisiones según cada objeto lo necesite.
        internal Explosion explosion; // Acá va el manejo de un objeto cuando es chocado por otro.
        public int valor = 0;
        #endregion

        #region Constructor
        public Dibujable()
        {
            posicion.setActual(0, 0, 0);
            fisica = null;
            colision = null;
            vectorDireccion = new EjeCoordenadas();
            vectorDireccion.centrar(0, 0, 0);
            velocidadManual = false;
            desplazamientoReal = false;
            rotacionReal = false;
            ultimaTraslacion = new Vector3();
        }
        #endregion

        #region Setter Modular
        public void setObject(TgcMesh cosa, float vLineal, float vRadial, Vector3 escalado)
        {  //Le agregas ademas un vector para la matriz de rotacion y otro para la de escalado. Para acomodar el objeto de forma inicial.
            objeto = cosa;
            AutoTransformEnable = false;
            velocidad = vLineal;
            velocidadRadial = vRadial;
            escala = escalado;
            traslacion = 0;
            inclinacion = 0;
            rotacion = 0;
            giro = 0;
            Matrix matriz = Matrix.Scaling(escalado);
            Transform *= matriz;
        }
        public void setFisica(float acel, float aFrenado, float velMax, float masaCuerpo) { fisica = new Fisica(this, acel, aFrenado, velMax, masaCuerpo); }  //Carga un nuevo módulo de fisica.        
        public void SetPropiedades(bool velMan, bool despReal, bool rotReal)    //La velocidad vuelve a 0 cuando no ocurre un evento continuo.
        {   //Se usa el desplazamiento y la rotacion del modulo de fisica. Por defecto viene todo false.
            velocidadManual = velMan;
            desplazamientoReal = despReal;
            rotacionReal = rotReal;
        }
        public void setColision(IColision bb) { this.colision = bb; }
        #endregion

        #region Atributos Basicos - Mesh
        bool AlphaBlendEnable
        {
            get { return ((IRenderObject)objeto).AlphaBlendEnable; }
            set { ((IRenderObject)objeto).AlphaBlendEnable = value; }
        }
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
        public void activar() { objeto.Enabled = true; }
        public void desactivar() 
        { 
            objeto.Enabled = false; 
        }
        public void escalarSinBB(Vector3 escalado)  //Escala la mesh con el vector asociado.
        {
            Matrix matriz = Matrix.Scaling(escalado);
            Transform = matriz;
            escala = escalado;
        }
        #endregion

        #region Atributos Avanzados - Dibujable
        internal void setPosicion(Vector3 pos) { posicion.setActual(pos); }   //Fuerza la posicion del dibujable con la posicion indicada. Deberia ser la del mesh asociado.
        public Vector3 getPosicion() { return posicion.getActual(); }   //Devuelve la posicion actual del Dibujable.
        public Vector3 getTrayectoriaInercial() { return posicion.direccionDesplazamiento(); }   //Direccion en la que se desplaza un objeto. (Diferencia entre posAnterior y posActual)
        public Vector3 getDireccion() { return vectorDireccion.direccion(); }   //Direccion en la que apunta el frente del objeto. (Direccion del eje Z del Dibujable)
        public Vector3 getDireccionAnterior() { return vectorDireccion.direccionAnterior(); } //Direccion anterior a la que apuntaba el frente del objeto
        public Vector3 getDireccion_Y() { return vectorDireccion.direccion_Y(); }   //Direccion del eje Y del Dibujable
        public Vector3 getDireccion_X() { return vectorDireccion.direccion_X(); }   //Direccion del eje X del Dibujable
        public float getAceleracion() { if (fisica != null) return fisica.aceleracion; else return 0; }
        public float getAcelFrenado() { if (fisica != null) return fisica.acelFrenado; else return 0; }     //El objeto debe poder frenar mas rapido de lo que acelera.
        public IColision getColision() { return this.colision; }    //Devuelve el objeto asignado para las colisiones.
        public bool fisicaPresente() { if (fisica != null) return true; else return false; }
        public Vector3 indicarGravedad(Vector3 pos, float mass)     //Devuelve la fuerza gravitacional que ejerce este cuerpo respecto del que viene por parmetro.
        {
            if (fisica != null) return this.fisica.indicarGravedad(pos, mass);
            else return new Vector3(0, 0, 0);
        }
        internal float velocidadActual()    //Devuelve la velocidad asignada al Dibujable.
        {
            if (fisica != null && desplazamientoReal) return fisica.velocidadInstantanea;
            else return velocidad;
        }
        public void setEjes(EjeCoordenadas nuevoEje) { vectorDireccion = nuevoEje; }    //Asigna un nuevo grupo de ejes de direccion. Por si se quiere copiar los ejes de otro Dibujable.
        public EjeCoordenadas getEjes()     //Devuelve una copia del estado de los ejes del Dibujable.
        {
            EjeCoordenadas nuevoEje = new EjeCoordenadas();
            nuevoEje.vectorX = vectorDireccion.vectorX;
            nuevoEje.vectorY = vectorDireccion.vectorY;
            nuevoEje.vectorZ = vectorDireccion.vectorZ;
            nuevoEje.centrar(vectorDireccion.getCentro());
            nuevoEje.lRotor = new List<Vector3>(vectorDireccion.lRotor);
            nuevoEje.mRotor = vectorDireccion.mRotor;
            return nuevoEje;
        }
        public virtual bool soyAsteroide() { return false; }
        #endregion

        #region Rotacion
        public void rotarPorTiempo(float time, List<Dibujable> dibujables)   //Movimiento de rotacion base de un dibujable.
        {
            float angulo = velocidadRadial * time;
            Matrix rotation;
            Vector3 rotacionActual = new Vector3();
            if (inclinacion != 0) //Rotar en X
            {
                rotation = vectorDireccion.calculoRotarX_desde(posicion.getActual(), angulo * inclinacion, ref rotacionActual);// paso un vector por referencia para luego poder aplicarselo a la obb
                Transform *= rotation;
                if (colision != null) this.getColision().rotar(rotacionActual);
            }
            if (giro != 0) //Rotar en Y
            {
                rotation = vectorDireccion.calculoRotarY_desde(posicion.getActual(), angulo * giro, ref rotacionActual);
                Transform *= rotation;
                if (colision != null) this.getColision().rotar(rotacionActual);
            }
            if (rotacion != 0) //Rotar en Z
            {
                rotation = vectorDireccion.calculoRotarZ_desde(posicion.getActual(), angulo * rotacion * 2, ref rotacionActual);
                Transform *= rotation;
                if (colision != null) this.getColision().rotar(rotacionActual);
            }
            if (velocidadManual)
            {
                inclinacion = 0;
                rotacion = 0;
                giro = 0;
            }
        }
        public void rotarPorVectorDeAngulos(Vector3 rotacion)
        {
            Matrix rotation;
            Vector3 rotacionActual = new Vector3();
            rotation = vectorDireccion.calculoRotarManualmente(posicion.getActual(), rotacion, ref rotacionActual);
            Transform *= rotation;
            if (colision != null) this.getColision().rotar(rotacionActual);
        }
        #endregion

        #region Traslacion
        public void desplazarsePorTiempo(float time, List<Dibujable> dibujables)   //Movimiento de traslacion base de un dibujable.
        {
            Vector3 director;
            Matrix move = Matrix.Identity;
            if (fisica != null && desplazamientoReal) director = fisica.calcularTraslado(time, dibujables);
            else
            {
                director = vectorDireccion.direccion();
                director.Normalize();
                director.X *= traslacion * velocidad * time;
                director.Y *= traslacion * velocidad * time;
                director.Z *= traslacion * velocidad * time;
            }
            move = Matrix.Translation(director);
            desplazarUnaDistancia(director);
            if (velocidadManual) traslacion = 0;
        }
        public void ubicarEnUnaPosicion(Vector3 posicion)   //Ubica la mesh y la BB a la posicion indicada, a partir de la posicion actual del Dibujable.
        {
            Vector3 movimiento = Vector3.Subtract(posicion, getPosicion());
            Matrix matriz = Matrix.Translation(movimiento);
            Transform *= matriz;
            setPosicion(posicion);
            if (colision != null) this.getColision().transladar(movimiento);
            //ultimaTraslacion = new Vector3(0,0,0);
        }
        public void desplazarUnaDistancia(Vector3 VDesplazamiento)  //Desplaza la mesh y la BB en la direccion indicada, la distancia contenida en el modulo de dicho vector.
        {
            Matrix desplazamiento = Matrix.Translation(VDesplazamiento);
            Vector4 vector4 = Vector3.Transform(posicion.getActual(), desplazamiento);
            posicion.setActual(vector4.X, vector4.Y, vector4.Z);
            Transform *= desplazamiento;
            if (colision != null) this.getColision().transladar(VDesplazamiento);
            ultimaTraslacion = VDesplazamiento;
        }
        public void impulsate(Vector3 vector, float velocidad, float tiempo)
        {  //Genera un impulso por un instante de tiempo, para desplazar el cuerpo una pequeña distancia, asignandole una nueva velocidad.
            fisica.impulsar(vector, velocidad, tiempo);
        }
        #endregion

        #region Updating
        public void render()
        {
            objeto.render();
            if (colision != null) //Si tiene modulo de colisiones, renderiza la bounding del modulo
            {
                if (EjemploAlumno.workspace().boundingBoxes && objeto.Enabled)
                    colision.render();
            }
        }
        public void dispose()
        {
            try
            {
                ((IRenderObject)objeto).dispose();
                if (colision != null)
                    getColision().getBoundingBox().dispose();
            }
            catch { }           
        }
        internal void acelerar() { traslacion = 1; }    //Hace que el cuerpo active la fuerza para avanzar en la direccion en la que apunta dicho cuerpo.

        internal void frenar()
        {
            if (fisica != null && velocidadManual) fisica.frenado = true;
            else traslacion = 0;
        }
        #endregion

        #region Colisiones
        public virtual void teChoque(Dibujable colisionador, float moduloVelocidad)
        {
            //Metodo virtual que se utilizara para poder definir los choques
        }
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Linq;

using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using Microsoft.DirectX.DirectInput;

using TgcViewer;
using TgcViewer.Example;
using TgcViewer.Utils.Modifiers;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSceneLoader;
using TgcViewer.Utils.Input;
using TgcViewer.Utils.Terrain;

using AlumnoEjemplos.TheGRID;
using AlumnoEjemplos.TheGRID.Colisiones;
using AlumnoEjemplos.TheGRID.Explosiones;
using AlumnoEjemplos.TheGRID.Shaders;
using AlumnoEjemplos.TheGRID.Camara;

namespace AlumnoEjemplos.TheGRID
{
    public class EjemploAlumno : TgcExample
    {
        #region TGCVIEWER METADATA
        /// Categoría a la que pertenece el ejemplo.
        /// Influye en donde se va a ver en el árbol de la derecha de la pantalla.
        public override string getCategory(){ return "AlumnoEjemplos"; }
        /// Completar nombre del grupo en formato Grupo NN
        public override string getName(){ return "Grupo TheGRID"; }
        /// Completar con la descripción del TP
        public override string getDescription(){return "Viaje Interplanetario - Manejo: \nArriba/Abajo - Pitch                       \nIzq/Der - Roll                                  \nA/D - Yaw                 \nW - Acelerar                  \nS - Estabilizar                             \nEspacio - Disparo Principal";}
        #endregion

        #region ATRIBUTOS
        Escenario scheme;
        internal Escenario Escenario { get { return scheme; } }
        static EjemploAlumno singleton;
        Nave nave;
        float velocidadBlur;
        bool velocidadBlurBool = false;
        float tiempoBlur;
        private Dibujable objetoPrincipal;  //Este va a ser configurable con el panel de pantalla.
        public Dibujable ObjetoPrincipal { get { return objetoPrincipal; } }
        List<Dibujable> listaDibujable = new List<Dibujable>();
        float timeLaser = 0; //Inicializacion.
        const float betweenTime = 0.3f;    //Tiempo de espera entre cada disparo de laser.

        //lista de meshes para implementar el motion blur
        public List<TgcMesh> meshCollection = new List<TgcMesh>();

        //Modificador de la camara del proyecto
        CambioCamara camara;
        TgcArrow arrow;
        private TgcFrustum currentFrustrum;
        public TgcFrustum CurrentFrustrum { get { return currentFrustrum; } }
        private SkySphere skySphere;
        public SkySphere SkySphere { get { return skySphere; } }
        //TgcBox suelo;
        //TgcSkyBox skyBox;
        //ManagerLaser laserManager;
        //private ManagerAsteroide asteroidManager;
        ShaderTheGrid shader;
        internal ShaderTheGrid Shader { get { return shader; } }
        #endregion

        #region METODOS AUXILIARES
        public static EjemploAlumno workspace() { return singleton; }
        public static void addMesh(TgcMesh unMesh){
            singleton.meshCollection.Add(unMesh);
        }
        public TgcFrustum getCurrentFrustrum() { return currentFrustrum; }
        private void crearSkyBox()
        {
            /*
            //Crear SkyBox 
            //skyBox = new TgcSkyBox();
            skyBox.Center = new Vector3(0, 0, 0);
            skyBox.Size = new Vector3(15000, 15000, 15000);
            //Crear suelo
            TgcTexture pisoTexture = TgcTexture.createTexture(d3dDevice, alumnoMediaFolder + "TheGrid\\SkyBox\\adelante.jpg");
            suelo = TgcBox.fromSize(new Vector3(0, 0, 9500), new Vector3(1000, 1000, 0), pisoTexture);
            //Configurar color
            //skyBox.Color = Color.OrangeRed;
            //Configurar las texturas para cada una de las 6 caras
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Up, alumnoMediaFolder + "TheGrid\\SkyBox\\arriba.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Down, alumnoMediaFolder + "TheGrid\\SkyBox\\abajo.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Left, alumnoMediaFolder + "TheGrid\\SkyBox\\izquierda.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Right, alumnoMediaFolder + "TheGrid\\SkyBox\\derecha.jpg");
            //Hay veces es necesario invertir las texturas Front y Back si se pasa de un sistema RightHanded a uno LeftHanded
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Front, alumnoMediaFolder + "TheGrid\\SkyBox\\adelante.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Back, alumnoMediaFolder + "TheGrid\\SkyBox\\atras.jpg");
            //Actualizar todos los valores para crear el SkyBox
            skyBox.updateValues();
             */
        }
        private static string tg_Folder = GuiController.Instance.AlumnoEjemplosDir + "\\TheGrid\\ArchivosMedia\\";
        public static string TG_Folder { get { return tg_Folder; } }
        #endregion

        public override void init()
        {
            #region INICIALIZACIONES POCO IMPORTANTES
            
            EjemploAlumno.singleton = this;
            Microsoft.DirectX.Direct3D.Device d3dDevice = GuiController.Instance.D3dDevice;
            string alumnoMediaFolder = GuiController.Instance.AlumnoEjemplosMediaDir;
            GuiController.Instance.CustomRenderEnabled = true;
            //d3dDevice.Clear(ClearFlags.Target, Color.FromArgb(22, 22, 22), 1.0f, 0);
            //Crear manager Lasers
            //laserManager = new ManagerLaser(5);
            //Crear 5 asteroides
            //asteroidManager = new ManagerAsteroide(1000);
            //asteroidManager.creaUno(TamanioAsteroide.MUYGRANDE);
            //asteroidManager.fabricar(5, TamanioAsteroide.MEDIANO);
            //asteroidManager.fabricarCinturonAsteroides(new Vector3(-500,0,2000),10,50);

            #endregion

            currentFrustrum = new TgcFrustum();
            crearSkyBox();

            shader = new ShaderTheGrid();
            //shader.motionBlurActivado = true; //Descomentar para activar el motion---Ahora mismo esta en Escenario

            //Crear la nave
            nave = new Nave();

            skySphere = new SkySphere();

            //Creamos el escenario.
            scheme = new Escenario(nave);
            //scheme.loadChapter2();

            //Cargamos la nave como objeto principal.
            objetoPrincipal = nave;
            camara = new CambioCamara(nave);
            
            //Flecha direccion objetivo
            arrow = new TgcArrow();
            arrow.BodyColor = Color.FromArgb(230, Color.Cyan);
            arrow.HeadColor = Color.FromArgb(230, Color.Yellow);

            #region PANEL DERECHO

            //Cargamos valores en el panel lateral
            GuiController.Instance.UserVars.addVar("Vel-Actual:");
            GuiController.Instance.UserVars.addVar("Integtidad Nave:");
            GuiController.Instance.UserVars.addVar("Integridad Escudos:");
            GuiController.Instance.UserVars.addVar("Posicion X:");
            GuiController.Instance.UserVars.addVar("Posicion Y:");
            GuiController.Instance.UserVars.addVar("Posicion Z:");
            //Cargar valor en UserVar
            GuiController.Instance.UserVars.setValue("Vel-Actual:", objetoPrincipal.velocidadActual());
            GuiController.Instance.UserVars.setValue("Integtidad Nave:", objetoPrincipal.explosion.vida);
            GuiController.Instance.UserVars.setValue("Integridad Escudos:", objetoPrincipal.explosion.escudo);
            GuiController.Instance.UserVars.setValue("Posicion X:", objetoPrincipal.getPosicion().X);
            GuiController.Instance.UserVars.setValue("Posicion Y:", objetoPrincipal.getPosicion().Y);
            GuiController.Instance.UserVars.setValue("Posicion Z:", objetoPrincipal.getPosicion().Z);
            //Crear un modifier para un valor FLOAT
            //GuiController.Instance.Modifiers.addFloat("Aceleracion", 0f,500f, objetoPrincipal.getAceleracion());  De momento lo saco.
            //GuiController.Instance.Modifiers.addFloat("Frenado", 0f, 1000f, objetoPrincipal.getAcelFrenado());    De momento lo saco.
            //Crear un modifier para un ComboBox con opciones
            string[] opciones0 = new string[] { "THE OPENING", "IMPULSE DRIVE", "WELCOME HOME", "VACUUM" };
            GuiController.Instance.Modifiers.addInterval("Escenario Actual", opciones0, 3);
            string[] opciones1 = new string[] { "Tercera Persona", "Camara FPS", "Libre" };
            GuiController.Instance.Modifiers.addInterval("Tipo de Camara", opciones1, 0);
            string[] opciones2 = new string[] { "Desactivado", "Activado" };
            GuiController.Instance.Modifiers.addInterval("Velocidad Manual", opciones2, 1);
            string[] opciones3 = new string[] { "Activado", "Desactivado" };
            GuiController.Instance.Modifiers.addInterval("Desplaz. Avanzado", opciones3, 1);
            //string[] opciones4 = new string[] { "Activado", "Desactivado" };
            //GuiController.Instance.Modifiers.addInterval("Rotacion Avanzada", opciones4, 1);  De momento lo saco.
            string opcionElegida = (string)GuiController.Instance.Modifiers["Escenario Actual"];
            scheme.chequearCambio(opcionElegida);

            #endregion
        }   

        public override void render(float elapsedTime)
        {
            #region -----UPDATE-----
            TgcD3dInput input = GuiController.Instance.D3dInput;
            GuiController.Instance.FpsCounterEnable = true;

            //Flechas
            if (input.keyDown(Key.Left)) { nave.rotacion = 1; }
            if (input.keyDown(Key.Right)) { nave.rotacion = -1; }
            if (input.keyDown(Key.Up)) { nave.inclinacion = 1; }
            if (input.keyDown(Key.Down)) { nave.inclinacion = -1; }
            //Letras
            if (input.keyDown(Key.A)) { nave.giro = -1; }
            if (input.keyDown(Key.D)) { nave.giro = 1; }
            if (input.keyDown(Key.W)) { nave.acelerar(); }
            if (input.keyDown(Key.S)) { if (!velocidadBlurBool)nave.frenar(); }


            if (input.keyDown(Key.Z)) { nave.rotarPorVectorDeAngulos(new Vector3(0, 0, 15)); }

            if (input.keyPressed(Key.LeftShift)) 
            {
                if (velocidadBlurBool) velocidadBlurBool = false;
                else 
                {
                    if (objetoPrincipal.velocidadActual() == 200) 
                    {
                        velocidadBlurBool = true;
                        tiempoBlur = 0;
                        //velocidadBlur = objetoPrincipal.velocidadActual();
                    }
                    
                }
            }

            //Otros
            //if (input.keyDown(Key.LeftShift)) { nave.acelerar(1); }
            //if (input.keyDown(Key.F1)) { camara.modoFPS(); }
            //if (input.keyDown(Key.F2)) { camara.modoExterior(); }
            //if (input.keyDown(Key.F3)) { camara.modoTPS(); }

            if (input.keyDown(Key.P)) { scheme.asteroidManager.explotaAlPrimero(); }

            camara.cambiarPosicionCamara();
            currentFrustrum.updateMesh(GuiController.Instance.CurrentCamera.getPosition(),GuiController.Instance.CurrentCamera.getLookAt());
            
            if (input.keyDown(Key.Space))
            {
                timeLaser += elapsedTime;
                if (timeLaser > betweenTime)
                {
                    scheme.dispararLaser();
                    //laserManager.fabricar(nave.getEjes(),nave.getPosicion());                  
                    timeLaser = 0;
                }
            }
            #endregion
            


            Microsoft.DirectX.Direct3D.Device d3dDevice = GuiController.Instance.D3dDevice;
            //d3dDevice.Clear(ClearFlags.Target, Color.FromArgb(22, 22, 22), 1.0f, 0);
            //GuiController.Instance.FpsCounterEnable = true;
            scheme.refrescar(elapsedTime);

            //Cargar valores de la flecha
            Vector3 navePos = nave.getPosicion();
            Vector3 naveDir = Vector3.Subtract(new Vector3(0, 0, 10000), nave.getDireccion());
            naveDir.Normalize();
            naveDir.Multiply(75);
            arrow.PStart = navePos;
            arrow.PEnd = navePos + naveDir;
            arrow.Thickness = 0.5f;
            arrow.HeadSize = new Vector2(2,2);
            arrow.updateValues();
            arrow.render();
            skySphere.render();
            //suelo.render();

            nave.rotarPorTiempo(elapsedTime,listaDibujable);
            nave.desplazarsePorTiempo(elapsedTime,new List<Dibujable>(scheme.cuerpos()));
            if(!camara.soyFPS())
                nave.render(elapsedTime);
            shader.shadear((TgcMesh)nave.objeto, meshCollection, elapsedTime);
            #region Refrescar panel lateral
            string opcionElegida = (string)GuiController.Instance.Modifiers["Tipo de Camara"];
            camara.chequearCambio(opcionElegida);
            opcionElegida = (string)GuiController.Instance.Modifiers["Escenario Actual"];
            scheme.chequearCambio(opcionElegida);
            opcionElegida = (string)GuiController.Instance.Modifiers["Velocidad Manual"];
            if (String.Compare(opcionElegida, "Activado") == 0) objetoPrincipal.velocidadManual = true; else objetoPrincipal.velocidadManual = false;
            opcionElegida = (string)GuiController.Instance.Modifiers["Desplaz. Avanzado"];
            if (String.Compare(opcionElegida, "Activado") == 0) objetoPrincipal.desplazamientoReal = true; else objetoPrincipal.desplazamientoReal = false;
            //opcionElegida = (string)GuiController.Instance.Modifiers["Rotacion Avanzada"];
            //if (String.Compare(opcionElegida, "Activado") == 0) objetoPrincipal.rotacionReal = true; else objetoPrincipal.rotacionReal = false;   De momento lo saco.
            //Refrescar User Vars
            if (velocidadBlurBool)
            {
                tiempoBlur += elapsedTime;
                velocidadBlur = (float)Math.Pow(7D, tiempoBlur);
                if (velocidadBlur > 299800) velocidadBlur = 299800;
                GuiController.Instance.UserVars.setValue("Vel-Actual:", velocidadBlur + objetoPrincipal.velocidadActual());
            }
            else GuiController.Instance.UserVars.setValue("Vel-Actual:", objetoPrincipal.velocidadActual());            
            GuiController.Instance.UserVars.setValue("Posicion X:", objetoPrincipal.getPosicion().X);
            GuiController.Instance.UserVars.setValue("Posicion Y:", objetoPrincipal.getPosicion().Y);
            GuiController.Instance.UserVars.setValue("Posicion Z:", objetoPrincipal.getPosicion().Z);
            GuiController.Instance.UserVars.setValue("Integtidad Nave:", objetoPrincipal.explosion.vida);
            GuiController.Instance.UserVars.setValue("Integridad Escudos:", objetoPrincipal.explosion.escudo);
            //Obtener valores de Modifiers
            //objetoPrincipal.fisica.aceleracion = (float)GuiController.Instance.Modifiers["Aceleracion"];  De momento lo saco.
            //objetoPrincipal.fisica.acelFrenado = (float)GuiController.Instance.Modifiers["Frenado"];      De momento lo saco.
            #endregion
        }

        public override void close()
        {
            scheme.asteroidManager.destruirListas();
            scheme.laserManager.destruirListas();
            scheme.dispose();
            nave.dispose();
            arrow.dispose();
            skySphere.dispose();
            //suelo.dispose();
        }
    }
}


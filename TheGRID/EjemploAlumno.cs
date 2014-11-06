using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

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
using AlumnoEjemplos.TheGRID.InterfazGrafica;

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
        public override string getDescription() { return    "Welcome to TheGRID"+Environment.NewLine+
                                                            "C: Menú de Configuracion"+Environment.NewLine+
                                                            "P: Menú de Pausa"+Environment.NewLine+
                                                            "F1-F3: Cámara Tercera Persona / FPS / Fija"+Environment.NewLine+
                                                            "LeftShift: Efecto Blur"+Environment.NewLine+
                                                            "LeftCtrl: Modo Automático"+Environment.NewLine+
                                                            "Espacio: Disparo Principal"+Environment.NewLine+
                                                            "RightShift: Disparo Secundario"; }
        #endregion

        #region ATRIBUTOS
        Escenario scheme;
        internal Escenario Escenario { get { return scheme; } }
        static EjemploAlumno singleton;
        public Nave nave;
        public Dibujable sol;
        public Estrella estrellaControl;
        public List<TgcMesh> estrellas;
        public List<TgcMesh> estrellasNo;
        public float velocidadBlur = 0;
        bool velocidadAutomatica = false;
        public float tiempoBlur=0.3f;
        public Dibujable ObjetoPrincipal { get { return nave; } }
        List<Dibujable> listaDibujable = new List<Dibujable>();
        float timeLaser = 0; //Inicializacion.
        const float betweenTime = 0.15f;    //Tiempo de espera entre cada disparo de laser.
        public float tiempoPupila;
        //lista de meshes para implementar el motion blur
        public List<Dibujable> dibujableCollection = new List<Dibujable>();
        public List<IRenderObject> objectosNoMeshesCollection = new List<IRenderObject>();
        public List<TgcMesh> objetosBrillantes = new List<TgcMesh>();
        //Modificador de la camara del proyecto
        public CambioCamara camara;
        private TgcFrustum currentFrustrum;
        public TgcFrustum CurrentFrustrum { get { return currentFrustrum; } }
        private SkySphere skySphere;
        public SkySphere SkySphere { get { return skySphere; } }
        SuperRender superRender;
        internal SuperRender Shader { get { return superRender; } }
        public Musique music = new Musique();
        //Lazer Azul
        float pressed_time_lazer = 0;

        public Point mouseCenter;
        public int altoPantalla;
        public int anchoPantalla;
        //GUI
        public bool pausa = false;
        public bool config = false;
        public bool gravity = true;
        public bool mouse;
        public int invertirMira = -1;
        Pausa guiPausa = new Pausa();
        public Configuracion guiConfig;
        public Hud guiHud;
        #endregion

        #region Atributos Menu
        public bool glow = false;
        public bool luces_posicionales = false;
        //Variables para el parpadeo
            float tiempo_acum = 0;
            float periodo_parpadeo = 1.5f;
            public bool parpadeoIzq = true;
            public bool parpadeoDer = false;
            public bool linterna = false;
        public bool boundingBoxes = false;
        public bool musicaActivada = true;
        public bool despl_avanzado = true;
        public string escenarioActivado = "THE OPENING";
        public bool entreWarp = false;
        public bool helpActivado = false;
        #endregion

        #region METODOS AUXILIARES
        public static EjemploAlumno workspace() { return singleton; }
        public static void addMesh(Dibujable unDibujable){
            singleton.dibujableCollection.Add(unDibujable);
        }
        public static void addRenderizable(IRenderObject unObjeto)
        {
            singleton.objectosNoMeshesCollection.Add(unObjeto);
        }
        public static void addMesh(TgcMesh unObjeto)
        {
            singleton.objetosBrillantes.Add(unObjeto);
        }
        public TgcFrustum getCurrentFrustrum() { return currentFrustrum; }
        private static string tg_Folder = GuiController.Instance.AlumnoEjemplosMediaDir + "\\TheGrid\\";
        public static string TG_Folder { get { return tg_Folder; } }
        #endregion

        public override void init()
        {
            #region INICIALIZACIONES POCO IMPORTANTES
            
            EjemploAlumno.singleton = this;
            Microsoft.DirectX.Direct3D.Device d3dDevice = GuiController.Instance.D3dDevice;
            string alumnoMediaFolder = GuiController.Instance.AlumnoEjemplosMediaDir;
            GuiController.Instance.CustomRenderEnabled = true;
            #endregion

            //Informacion sobre los movimientos en la parte inferior de la pantalla.
            TgcViewer.Utils.Logger logger = GuiController.Instance.Logger;
            logger.clear();
            logger.log("Le damos la bienvenida a este simulador. A continuacion le indicaremos los controles que puede utilizar:");
            logger.log("Paso 1: Para rotar sobre los ejes Z y X utilice las FLECHAS de direccion. Para rotar sobre el eje Y utilice las teclas A y D.");
            logger.log("Paso 2: Para avanzar presione la W pero cuidado con el movimiento inercial. Seguramente se va a dar cuenta de lo que hablo. Para frenar puede presionar la tecla S. Esto estabiliza la nave sin importar que fuerzas tiren de ella.");
            logger.log("Paso 3: Para avanzar con cuidado, acelere o frene hasta la velocidad deseada, pulse una vez LeftCtrl y luego acelere. Esto activa el modo automatico. Para desactivarlo basta con frenar un poco o volver a pulsar LeftCtrl.");
            logger.log("Paso 4: Para activar el Motion Blur debe ir a la maxima velocidad y luego pulsar una vez LeftShift. La desactivacion es de la misma forma.");
            logger.log("Por ultimo pruebe disparar presionando SpaceBar o RightShift. -- Disfrute el ejemplo.");

            GuiController.Instance.FullScreenEnable = true;

            currentFrustrum = new TgcFrustum();           
            superRender = new SuperRender();
            guiConfig = new Configuracion(music);
            guiHud = new Hud();


            //Crear la nave
            nave = new Nave();

            skySphere = new SkySphere("SkyBox\\skysphere-TgcScene.xml");
            addMesh(skySphere.dibujable_skySphere);

            //Creamos el escenario.
            scheme = new Escenario(nave);

            //Cargamos la nave como objeto principal.
            camara = new CambioCamara(nave);

            //Inicializacion del Control por Mouse
            Control focusWindows = GuiController.Instance.D3dDevice.CreationParameters.FocusWindow;
            mouseCenter = focusWindows.PointToScreen(new Point(focusWindows.Width / 2, focusWindows.Height / 2));
            mouse = false;
            Cursor.Position = mouseCenter;
            altoPantalla = focusWindows.Height;
            anchoPantalla = focusWindows.Width;

            #region PANEL DERECHO
            string[] opciones2 = new string[] { "Lista Completa", "Castor", "Derezzed", "M4 Part 2", "ME Theme", "New Worlds", "Solar Sailer", "Spectre", "Tali", "The Son of Flynn", "Tron Ending", "Sin Musica" };
            GuiController.Instance.Modifiers.addInterval("Musica de fondo", opciones2, 0);
            string opcionElegida = (string)GuiController.Instance.Modifiers["Musica de fondo"];
            music.chequearCambio(opcionElegida);
            music.refrescar();
            #endregion
            scheme.chequearCambio("THE OPENING");
        }   

        public override void render(float elapsedTime)
        {
            guiHud.operar();
            if (mouse)
            {
                Cursor.Hide();
            }
            TgcD3dInput input = GuiController.Instance.D3dInput;
            if (pausa) //Entramos a la pantalla de pausa
            {
                guiPausa.render();
                return;
            }
            if (config) //Entramos al menu de configuracion
            {
                guiConfig.operar(elapsedTime);
                return;
            }
            #region -----KEYS-----
            if (input.keyPressed(Key.I)) { music.refrescar(); }
            if (input.keyPressed(Key.P)) { 
                pausa = true;
                music.playPauseBackgound();
            }
            if (input.keyPressed(Key.C))
            {
                EjemploAlumno.workspace().config = true;
                EjemploAlumno.workspace().pausa = false;
                EjemploAlumno.workspace().guiConfig.restart();
                EjemploAlumno.workspace().music.playPauseBackgound();
            }     //Configuracion.

            //Flechas
            
            float sensibilidad = 0.003f;
            float zonaMuerta = 10f;
            if (mouse)
            {
                float temp = input.Ypos - mouseCenter.Y;
                if (FastMath.Abs(temp) > zonaMuerta) nave.inclinacion = temp * sensibilidad * invertirMira;
                temp = input.Xpos - mouseCenter.X;
                if (FastMath.Abs(temp) > zonaMuerta) nave.rotacion = temp * -sensibilidad;
            }
            if (input.keyDown(Key.Left)) { nave.rotacion = 1; }
            if (input.keyDown(Key.Right)) { nave.rotacion = -1; }
            if (input.keyDown(Key.Up)) { nave.inclinacion = 1; }
            if (input.keyDown(Key.Down)) { nave.inclinacion = -1; }
            
            //Cambios de cámara con los F1-F3
            if (input.keyDown(Key.F1))
                camara.chequearCambio("Tercera Persona");
            else if (input.keyDown(Key.F2))
                camara.chequearCambio("Camara FPS");
            else if (input.keyDown(Key.F3))
                camara.chequearCambio("Libre");

            //Movimientos
            if (input.keyDown(Key.A)) { nave.giro = -1; }          
            if (input.keyDown(Key.D)) { nave.giro = 1; }
            if (input.keyDown(Key.W)) { nave.acelerar(); }
            if (input.keyDown(Key.S)) { if (!superRender.motionBlurActivado)nave.frenar(); }
            if (input.keyPressed(Key.S)) { nave.fisica.desactivarAutomatico(); velocidadAutomatica = false; }
            if (input.keyDown(Key.Z)) { nave.rotarPorVectorDeAngulos(new Vector3(0, 0, 15)); }
            if (input.keyPressed(Key.H)) //Muestra el HUD de ayuda en pantalla
            {
                if (helpActivado)
                    helpActivado = false;
                else
                    helpActivado = true;
            }
            if (input.keyPressed(Key.LeftControl)) //Modo Automatico
            {
                if (velocidadAutomatica)
                {
                    nave.fisica.desactivarAutomatico();
                    velocidadAutomatica = false;
                }
                else
                {
                    nave.fisica.activarAutomatico();
                    velocidadAutomatica = true;
                }
            }
            //Activamos el motionBlur si estamos en Impuse Drive y presionan LeftShift
            if (scheme.escenarioActual == Escenario.TipoModo.IMPULSE_DRIVE || scheme.escenarioActual == Escenario.TipoModo.MISION)
            {
                if (input.keyPressed(Key.LeftShift))
                {
                    if (superRender.motionBlurActivado)
                    {
                        superRender.motionBlurActivado = false;
                        tiempoBlur = 0.3f;
                        nave.fisica.velocidadMaxima = nave.velMaxNormal;
                        nave.fisica.velocidadCrucero = nave.velMaxNormal;
                        nave.fisica.aceleracion = nave.acelNormal;
                        nave.velocidadRadial = nave.rotNormal;
                        nave.fisica.velocidadInstantanea = nave.velMaxNormal;//Esto esta para cuando el blur se desactiva, desacelrar rapidamente la nave
                    }
                    else
                    {
                        if (nave.velocidadActual() >= nave.fisica.velocidadMaxima)
                        {
                            superRender.motionBlurActivado = true;
                            nave.fisica.velocidadMaxima = nave.velMaxBlur;
                            nave.fisica.desactivarAutomatico(); velocidadAutomatica = false;
                            nave.velocidadRadial = nave.rotBlur;
                        }

                    }
                }
            }
            if (scheme.escenarioActual == Escenario.TipoModo.THE_OPENING || scheme.escenarioActual == Escenario.TipoModo.MISION) //Habilita el disparo si estamos con los asteroides
            {
                if (input.keyDown(Key.Space) || (input.buttonDown(TgcD3dInput.MouseButtons.BUTTON_LEFT) && mouse))
                {
                    timeLaser += elapsedTime;
                    if (timeLaser > betweenTime)
                    {
                        scheme.dispararLaser();
                        timeLaser = 0;
                    }
                }
                if (input.keyDown(Key.RightControl) || input.keyDown(Key.RightShift) || (input.buttonDown(TgcD3dInput.MouseButtons.BUTTON_RIGHT) && mouse))
                {
                    pressed_time_lazer += elapsedTime;
                    music.playLazerCarga();
                }
                else
                    if (input.keyUp(Key.RightControl) || input.keyUp(Key.RightShift) || (input.buttonUp(TgcD3dInput.MouseButtons.BUTTON_RIGHT) && mouse))
                    {
                        scheme.dispararLaserAzul(pressed_time_lazer);
                        pressed_time_lazer = 0;
                        music.playLazer2();
                    }
            }

            #endregion

            #region -----Update------
            tiempoPupila = elapsedTime; //Para el HDRL

            #region Parpadeo de las luces
            tiempo_acum += elapsedTime;
            if(tiempo_acum >= periodo_parpadeo)
            {
                if (luces_posicionales)
                {
                    if (parpadeoIzq)
                    {
                        parpadeoIzq = false;
                        parpadeoDer = true;
                    }
                    else
                    {
                        parpadeoIzq = true;
                        parpadeoDer = false;
                    }
                }
                else
                {
                    parpadeoDer = false;
                    parpadeoIzq = false;
                }
                tiempo_acum = 0;
            }
            #endregion

            //Aceleracion Blur
            if (superRender.motionBlurActivado && tiempoBlur < 5f)
            {
                tiempoBlur += elapsedTime;
                nave.fisica.aceleracion += FastMath.Pow(tiempoBlur, 10);
                nave.acelerar();
            }

            //Update de la nave
            if (velocidadAutomatica) nave.acelerar();
            nave.rotarPorTiempo(elapsedTime, listaDibujable);
            if(gravity)nave.desplazarsePorTiempo(elapsedTime, new List<Dibujable>(scheme.CuerposGravitacionales));
            else nave.desplazarsePorTiempo(elapsedTime, new List<Dibujable>());
            nave.reajustarSiSuperoLimite();

            //Update del escnario
            scheme.refrescar(elapsedTime);
            //Update de la camara
            camara.cambiarPosicionCamara();
            currentFrustrum.updateMesh(GuiController.Instance.CurrentCamera.getPosition(),GuiController.Instance.CurrentCamera.getLookAt());
            
            //Si estamos en una vel de warp considerable, le damos sonido al warp
            if (nave.velocidadActual() > 25000f)
                music.playWarp();
            else
                music.stopWarp();
            
            skySphere.render(nave);     //Solo actualiza posicion de la skysphere. Tiene deshabiltiado los render, eso lo hace el SuperRender
            #endregion

            //Redirige todo lo que renderiza para aplicar los efectos
            superRender.render(nave, sol, dibujableCollection, objectosNoMeshesCollection, objetosBrillantes); 

            
            //scheme.chequearCambio(escenarioActivado); //Realiza el cambio de capitulo
            nave.desplazamientoReal = despl_avanzado; //Setea si se habilito el modo real o avanzado de desplazamiento (Con atraccion y choque)

            string opcionElegida = (string)GuiController.Instance.Modifiers["Musica de fondo"];
            music.chequearCambio(opcionElegida);

        }

        public override void close()
        {
            scheme.asteroidManager.destruirListas();
            scheme.laserManager.destruirListas();
            scheme.dispose();
            nave.dispose();
            skySphere.dispose();
            music.liberarRecursos();
        }
    }
}


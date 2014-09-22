using System;
using System.Collections.Generic;
using System.Text;
using TgcViewer.Example;
using TgcViewer;
using System.Linq;
using Microsoft.DirectX.Direct3D;
using System.Drawing;
using Microsoft.DirectX;
using TgcViewer.Utils.Modifiers;
using TgcViewer.Utils.TgcGeometry;
using AlumnoEjemplos.TheGRID;
using TgcViewer.Utils.TgcSceneLoader;
using TgcViewer.Utils.Input;
using AlumnoEjemplos.TheGRID.Camara;
using TgcViewer.Utils.Terrain;
using Microsoft.DirectX.DirectInput;
using AlumnoEjemplos.TheGRID.Colisiones;
using AlumnoEjemplos.TheGRID.Explosiones;
using AlumnoEjemplos.TheGRID.Shaders;


namespace AlumnoEjemplos.TheGrid
{
    public class EjemploAlumno : TgcExample
    {
        /// Categoría a la que pertenece el ejemplo.
        /// Influye en donde se va a ver en el árbol de la derecha de la pantalla.
        public override string getCategory(){ return "AlumnoEjemplos"; }
        /// Completar nombre del grupo en formato Grupo NN
        public override string getName(){ return "Grupo TheGRID"; }
        /// Completar con la descripción del TP
        public override string getDescription(){return "Viaje Interplanetario - Manejo: \nArriba/Abajo - Pitch                       \nIzq/Der - Roll                                  \nA/D - Yaw                 \nW - Acelerar                  \nS - Estabilizar                             \nEspacio - Disparo Principal";}
        //--------------------------------------------------------
        
        // ATRIBUTOS
        Escenario scheme;
        static EjemploAlumno singleton;
        //TgcBox suelo;
        Dibujable nave;
        Dibujable sol;
        Dibujable objetoPrincipal;  //Este va a ser configurable con el panel de pantalla.

        List<Dibujable> listaDibujable = new List<Dibujable>();
        float timeLaser = 0; //Inicializacion.
        const float betweenTime = 0.3f;    //Tiempo de espera entre cada disparo de laser.
        //ManagerLaser laserManager;
        //private ManagerAsteroide asteroidManager;
        internal Escenario Escenario { get { return scheme; } }

        //Modificador de la camara del proyecto
        CambioCamara camara;
        //TgcSkyBox skyBox;
        TgcArrow arrow;
        TgcFrustum currentFrustrum;

        //--------------------------------------------------------

        public static EjemploAlumno workspace() { return singleton; }
        public TgcFrustum getCurrentFrustrum() { return currentFrustrum; }

        public override void init()
        {
            EjemploAlumno.singleton = this;
            //GuiController.Instance: acceso principal a todas las herramientas del Framework
            Microsoft.DirectX.Direct3D.Device d3dDevice = GuiController.Instance.D3dDevice;
            //Carpeta de archivos Media del alumno
            string alumnoMediaFolder = GuiController.Instance.AlumnoEjemplosMediaDir;
            singleton = this;
            currentFrustrum = new TgcFrustum();
            GuiController.Instance.CustomRenderEnabled = true;

            //d3dDevice.Clear(ClearFlags.Target, Color.FromArgb(22, 22, 22), 1.0f, 0);
            
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

            //Crear manager Lasers
            //laserManager = new ManagerLaser(5);
            
            //Crear 5 asteroides
            //asteroidManager = new ManagerAsteroide(1000);
            
            //Crear la nave
            TgcSceneLoader loader = new TgcSceneLoader();
            TgcScene scene;
            nave = new Dibujable(0, 0, 0);
            scene = loader.loadSceneFromFile(GuiController.Instance.AlumnoEjemplosMediaDir + "TheGrid\\Nave\\nave3-TgcScene.xml");
            nave.setObject(scene.Meshes[0], 100, 25, new Vector3(0, 180, 0), new Vector3(0.5f, 0.5f, 0.5f));
            nave.setFisica(50, 400, 100);
            nave.SetPropiedades(true, false, false);
            ExplosionNave modulo;
            modulo = new ExplosionNave(nave,100,200);
            nave.explosion = modulo;
            //Cargamos su BB
            TgcBoundingBox naveBb = ((TgcMesh)nave.objeto).BoundingBox;
            naveBb.scaleTranslate(new Vector3(0, 0, 0), new Vector3(0.5f, 0.5f, 0.5f));
            TgcObb naveObb = TgcObb.computeFromAABB(naveBb);
            foreach (var vRotor in nave.getEjes().lRotor) { naveObb.rotate(vRotor); } //Le aplicamos TODAS las rotaciones que hasta ahora lleva la nave.
            nave.setColision(new ColisionNave());
            nave.getColision().setBoundingBox(naveObb);
            //nave.getColision().transladar(posicionNave);

            //Creamos el escenario.
            scheme = new Escenario(nave);
            scheme.loadChapter1();
            //Creamos.....EL SOL
            TgcSceneLoader loaderSol = new TgcSceneLoader();
            TgcScene sceneSol = loaderSol.loadSceneFromFile(GuiController.Instance.AlumnoEjemplosMediaDir + "TheGrid\\Sol\\sol-TgcScene.xml");
            TgcMesh mesh_Sol = sceneSol.Meshes[0];
            //Cargamos las cosas en el dibujable
            sol = new Dibujable();
            sol.setObject(mesh_Sol, 0, 100, new Vector3(0, 0, 0), new Vector3(1F, 1F, 1F));
            sol.trasladar(new Vector3(0, 0, 2500));
            sol.giro = 1;


            //Cargamos la nave como objeto principal.
            objetoPrincipal = nave;
            //Cargamos la camara
            camara = new CambioCamara(nave);

            //asteroidManager.creaUno(TamanioAsteroide.MUYGRANDE);
            //asteroidManager.fabricar(5, TamanioAsteroide.MEDIANO);
            //asteroidManager.fabricarCinturonAsteroides(new Vector3(-500,0,2000),10,50);

            //Flecha direccion objetivo
            arrow = new TgcArrow();
            arrow.BodyColor = Color.FromArgb(230, Color.Cyan);
            arrow.HeadColor = Color.FromArgb(230, Color.Yellow);

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
            string[] opciones0 = new string[] { "THE OPENIG", "IMPULSE DRIVE", "WELCOME HOME", "VACUUM" };
            GuiController.Instance.Modifiers.addInterval("Escenario Actual", opciones0, 0);
            string[] opciones1 = new string[] { "Tercera Persona", "Camara FPS", "Libre" };
            GuiController.Instance.Modifiers.addInterval("Tipo de Camara", opciones1, 0);
            string[] opciones2 = new string[] { "Desactivado", "Activado" };
            GuiController.Instance.Modifiers.addInterval("Velocidad Manual", opciones2, 1);
            string[] opciones3 = new string[] { "Activado", "Desactivado" };
            GuiController.Instance.Modifiers.addInterval("Desplaz. Avanzado", opciones3, 1);
            //string[] opciones4 = new string[] { "Activado", "Desactivado" };
            //GuiController.Instance.Modifiers.addInterval("Rotacion Avanzada", opciones4, 1);  De momento lo saco.

        }
        //--------------------------------------------------------RENDER-----

        /// <summary>
        /// 
        /// </summary>
        /// <param name="elapsedTime">Tiempo en segundos transcurridos desde el último frame</param>
        public override void render(float elapsedTime)
        {
            //-----UPDATE-----
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
            if (input.keyDown(Key.S)) { nave.frenar(); }

            //Otros
            //if (input.keyDown(Key.LeftShift)) { nave.acelerar(1); }
            //if (input.keyDown(Key.F1)) { camara.modoFPS(); }
            //if (input.keyDown(Key.F2)) { camara.modoExterior(); }
            //if (input.keyDown(Key.F3)) { camara.modoTPS(); }

            if (input.keyDown(Key.P)) { scheme.asteroidManager.explotaAlPrimero(); }
            if (input.keyDown(Key.O)) { scheme.asteroidManager.creaUno(TamanioAsteroide.CHICO); }

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


            //-----FIN-UPDATE-----


            //Device de DirectX para renderizar
            Microsoft.DirectX.Direct3D.Device d3dDevice = GuiController.Instance.D3dDevice;
            //d3dDevice.Clear(ClearFlags.Target, Color.FromArgb(22, 22, 22), 1.0f, 0);
            //GuiController.Instance.FpsCounterEnable = true;

            scheme.refrescar(elapsedTime);

            //Cargar valores de la flecha
            Vector3 navePos = nave.getCentro();
            Vector3 naveDir = Vector3.Subtract(new Vector3(0, 0, 10000), nave.getDireccion());
            naveDir.Normalize();
            naveDir.Multiply(75);
            arrow.PStart = navePos;
            arrow.PEnd = navePos + naveDir;
            arrow.Thickness = 0.5f;
            arrow.HeadSize = new Vector2(2,2);
            arrow.updateValues();
            arrow.render();
            sol.rotar(elapsedTime, listaDibujable);
            sol.render(elapsedTime);
            
            //skyBox.render();
            //suelo.render();
            
            nave.rotar(elapsedTime,listaDibujable);
            nave.desplazarse(elapsedTime,listaDibujable);
            if(!camara.soyFPS())
                nave.render(elapsedTime);
       
            //Refrescar panel lateral
            //case
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
            GuiController.Instance.UserVars.setValue("Vel-Actual:", objetoPrincipal.velocidadActual());
            GuiController.Instance.UserVars.setValue("Posicion X:", objetoPrincipal.getPosicion().X);
            GuiController.Instance.UserVars.setValue("Posicion Y:", objetoPrincipal.getPosicion().Y);
            GuiController.Instance.UserVars.setValue("Posicion Z:", objetoPrincipal.getPosicion().Z);
            GuiController.Instance.UserVars.setValue("Integtidad Nave:", objetoPrincipal.explosion.vida);
            GuiController.Instance.UserVars.setValue("Integridad Escudos:", objetoPrincipal.explosion.escudo);
            //Obtener valores de Modifiers
            //objetoPrincipal.fisica.aceleracion = (float)GuiController.Instance.Modifiers["Aceleracion"];  De momento lo saco.
            //objetoPrincipal.fisica.acelFrenado = (float)GuiController.Instance.Modifiers["Frenado"];      De momento lo saco.

            
        }

        public override void close()
        {
            scheme.asteroidManager.destruirLista();
            scheme.laserManager.destruirLista();
            nave.dispose();

            //suelo.dispose();

        }
    }
}


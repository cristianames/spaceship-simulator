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

namespace AlumnoEjemplos.MiGrupo
{
    public class EjemploAlumno : TgcExample
    {
        /// Categoría a la que pertenece el ejemplo.
        /// Influye en donde se va a ver en el árbol de la derecha de la pantalla.
        public override string getCategory(){ return "AlumnoEjemplos"; }
        /// Completar nombre del grupo en formato Grupo NN
        public override string getName(){ return "Grupo TheGRID"; }
        /// Completar con la descripción del TP
        public override string getDescription(){return "Viaje Interplanetario - Manejo: \nArriba/Abajo - Pitch                       \nIzq/Der - Roll                                  \nZ/X o AltGr/CtrlDer - Yaw                 \nSpaceBar - Acelerar                  \n CtrlIzq - Estabilizar                             \nA - Disparo Principal";}
        //--------------------------------------------------------
        // ATRIBUTOS
        TgcBox suelo;
        Dibujable asteroide;
        //Dibujable caja;
        Dibujable nave;
        Dibujable objetoPrincipal;  //Este va a ser configurable con el panel de pantalla.
        List<Dibujable> laserLista;
        List<Dibujable> listaDibujable = new List<Dibujable>();
        


        //--------------------------------------------------------
        public override void init()
        {
            laserLista=new List<Dibujable>();
            //GuiController.Instance: acceso principal a todas las herramientas del Framework
            Device d3dDevice = GuiController.Instance.D3dDevice;
            //Carpeta de archivos Media del alumno
            string alumnoMediaFolder = GuiController.Instance.AlumnoEjemplosMediaDir;           
            //Crear suelo
            TgcTexture pisoTexture = TgcTexture.createTexture(d3dDevice, GuiController.Instance.ExamplesMediaDir + "Texturas\\Quake\\TexturePack2\\rock_floor1.jpg");
            suelo = TgcBox.fromSize(new Vector3(0, -5, 0), new Vector3(500, 0, 500), pisoTexture);            
            //Crear 1 asteroide

            Factory fabrica_dibujables = new Factory();

            asteroide = fabrica_dibujables.crearAsteroide(new Vector3(1, 1, 1));
            fabrica_dibujables.trasladar(asteroide, new Vector3(200, 100, 50));
            asteroide.setPosicion(new Vector3(200, 100, 50));
            GuiController.Instance.RotCamera.targetObject(((TgcMesh)asteroide.objeto).BoundingBox);
            asteroide.setFisica(5, 10, 5000);
           
            TgcSceneLoader loader = new TgcSceneLoader();
            TgcScene scene = loader.loadSceneFromFile(GuiController.Instance.AlumnoEjemplosMediaDir + "Laser\\Laser_Box-TgcScene.xml"); 
          
            //Crear la nave

            nave = new Dibujable(0, -10, 15);
            scene = loader.loadSceneFromFile(GuiController.Instance.AlumnoEjemplosMediaDir + "Nave\\nave-TgcScene.xml");
            nave.setObject(scene.Meshes[0], 200, 100, new Vector3(0, 180, 0), new Vector3(1, 1, 1));
            nave.setFisica(100, 500, 100);
            //nave.SetPropiedades(true, true, false);

            //Cargamos la nave como objeto principal.
            objetoPrincipal = nave;

            GuiController.Instance.RotCamera.targetObject(suelo.BoundingBox);

          
            
            //Configurar camara en Tercer Persona
            /*GuiController.Instance.ThirdPersonCamera.Enable = true;
            GuiController.Instance.ThirdPersonCamera.setCamera(nave.Position, 30, -75);*/

            
            //Cargamos valores en el panel lateral
            GuiController.Instance.UserVars.addVar("Vel-Actual:");
            GuiController.Instance.UserVars.addVar("Integtidad Nave:");
            GuiController.Instance.UserVars.addVar("Integridad Escudos:");
            GuiController.Instance.UserVars.addVar("Posicion X:");
            GuiController.Instance.UserVars.addVar("Posicion Y:");
            GuiController.Instance.UserVars.addVar("Posicion Z:");
            //Cargar valor en UserVar
            GuiController.Instance.UserVars.setValue("Vel-Actual:", objetoPrincipal.velocidadActual());
            GuiController.Instance.UserVars.setValue("Integtidad Nave:", 100);
            GuiController.Instance.UserVars.setValue("Integridad Escudos:", 100);
            GuiController.Instance.UserVars.setValue("Posicion X:", objetoPrincipal.getPosicion().X);
            GuiController.Instance.UserVars.setValue("Posicion Y:", objetoPrincipal.getPosicion().Y);
            GuiController.Instance.UserVars.setValue("Posicion Z:", objetoPrincipal.getPosicion().Z);
            //Crear un modifier para un valor FLOAT
            GuiController.Instance.Modifiers.addFloat("Aceleracion", 0f,500f, objetoPrincipal.getAceleracion());
            GuiController.Instance.Modifiers.addFloat("Frenado", 0f, 1000f, objetoPrincipal.getAcelFrenado());
            //Crear un modifier para un ComboBox con opciones
            string[] opciones1 = new string[] { "Camara Fija", "Camara FPS", "Camara TPS" };
            GuiController.Instance.Modifiers.addInterval("Tipo de Camara", opciones1, 0);
            string[] opciones2 = new string[] { "Activado", "Desactivado" };
            GuiController.Instance.Modifiers.addInterval("Velocidad Manual", opciones2, 1);
            string[] opciones3 = new string[] { "Activado", "Desactivado" };
            GuiController.Instance.Modifiers.addInterval("Desplaz. Avanzado", opciones3, 1);
            string[] opciones4 = new string[] { "Activado", "Desactivado" };
            GuiController.Instance.Modifiers.addInterval("Rotacion Avanzada", opciones4, 1);
            
        }
        //--------------------------------------------------------RENDER-----

        // <param name="elapsedTime">Tiempo en segundos transcurridos desde el último frame</param>
        public override void render(float elapsedTime)
        {
            //-----UPDATE-----
            //Flechas
            if (GuiController.Instance.D3dInput.keyDown(Microsoft.DirectX.DirectInput.Key.Left)) { nave.rotacion = 1; }
            if (GuiController.Instance.D3dInput.keyDown(Microsoft.DirectX.DirectInput.Key.Right)) { nave.rotacion = -1; }
            if (GuiController.Instance.D3dInput.keyDown(Microsoft.DirectX.DirectInput.Key.Up)) { nave.inclinacion = 1; }
            if (GuiController.Instance.D3dInput.keyDown(Microsoft.DirectX.DirectInput.Key.Down)) { nave.inclinacion = -1; }
            //Letras
            if (GuiController.Instance.D3dInput.keyDown(Microsoft.DirectX.DirectInput.Key.A)) { }
            if (GuiController.Instance.D3dInput.keyDown(Microsoft.DirectX.DirectInput.Key.D)) { }
            if (GuiController.Instance.D3dInput.keyDown(Microsoft.DirectX.DirectInput.Key.W)) { }
            if (GuiController.Instance.D3dInput.keyDown(Microsoft.DirectX.DirectInput.Key.S)) { }
            //Otros
            //if (GuiController.Instance.D3dInput.keyDown(Microsoft.DirectX.DirectInput.Key.LeftShift)) { nave.acelerar(1); }
            if (GuiController.Instance.D3dInput.keyDown(Microsoft.DirectX.DirectInput.Key.LeftControl)) { nave.frenar(); }
            if (GuiController.Instance.D3dInput.keyDown(Microsoft.DirectX.DirectInput.Key.Space)) { nave.acelerar(); }

            if (GuiController.Instance.D3dInput.keyDown(Microsoft.DirectX.DirectInput.Key.Z)) { nave.giro = -1; }
            if (GuiController.Instance.D3dInput.keyDown(Microsoft.DirectX.DirectInput.Key.X)) { nave.giro = 1; }
            if (GuiController.Instance.D3dInput.keyDown(Microsoft.DirectX.DirectInput.Key.RightAlt)) { nave.giro = -1; }
            if (GuiController.Instance.D3dInput.keyDown(Microsoft.DirectX.DirectInput.Key.RightControl)) { nave.giro = 1; }


            Factory fabrica = new Factory();
            
            if (GuiController.Instance.D3dInput.keyDown(Microsoft.DirectX.DirectInput.Key.A))
            {
                laserLista.Add(fabrica.crearLaser(nave.getCentro())); //arreglar haz de laser infinito, agregar colisiones
            }            
            if (laserLista.Count != 0)
            {

                foreach (Dibujable laser in laserLista)
                {
                    fabrica.dispararLaser(laser, nave.getDireccion() ,elapsedTime);

                }
            }
                       
            //-----FIN-UPDATE-----


            //Device de DirectX para renderizar
            Device d3dDevice = GuiController.Instance.D3dDevice;

          
            //laser.trasladar(elapsedTime);

            if (laserLista.Count != 0)
            {

                foreach (Dibujable laser in laserLista)
                {
                    laser.render();

                }
            }
           
            asteroide.render();
            asteroide.renderBoundingBox();

            suelo.render();

            
            listaDibujable.Add(asteroide);
            //listaDibujable.Add(nave);

            nave.rotar(elapsedTime,listaDibujable);
            nave.trasladar(elapsedTime,listaDibujable);
            nave.render();
            
            //Refrescar panel lateral
            //Refrescar User Vars
            GuiController.Instance.UserVars.setValue("Vel-Actual:", objetoPrincipal.velocidadActual());
            GuiController.Instance.UserVars.setValue("Posicion X:", objetoPrincipal.getPosicion().X);
            GuiController.Instance.UserVars.setValue("Posicion Y:", objetoPrincipal.getPosicion().Y);
            GuiController.Instance.UserVars.setValue("Posicion Z:", objetoPrincipal.getPosicion().Z);
            //Obtener valores de Modifiers
            objetoPrincipal.fisica.aceleracion = (float)GuiController.Instance.Modifiers["Aceleracion"];
            objetoPrincipal.fisica.acelFrenado = (float)GuiController.Instance.Modifiers["Frenado"];
            string opcionElegida = (string)GuiController.Instance.Modifiers["Tipo de Camara"];
            //case
            opcionElegida = (string)GuiController.Instance.Modifiers["Velocidad Manual"];
            if (String.Compare(opcionElegida, "Activado") == 0) objetoPrincipal.velocidadManual = true; else objetoPrincipal.velocidadManual = false;
            opcionElegida = (string)GuiController.Instance.Modifiers["Desplaz. Avanzado"];
            if (String.Compare(opcionElegida, "Activado") == 0) objetoPrincipal.desplazamientoReal = true; else objetoPrincipal.desplazamientoReal = false;
            opcionElegida = (string)GuiController.Instance.Modifiers["Rotacion Avanzada"];
            if (String.Compare(opcionElegida, "Activado") == 0) objetoPrincipal.rotacionReal = true; else objetoPrincipal.rotacionReal = false;
        }

        public override void close()
        {

            nave.dispose();
           // laser.dispose();

            asteroide.dispose();
            nave.dispose();
            suelo.dispose();

        }

        public void metodoUselessInit()
        {
            ///////////////USER VARS//////////////////
            //Crear una UserVar
            GuiController.Instance.UserVars.addVar("variablePrueba");
            //Cargar valor en UserVar
            GuiController.Instance.UserVars.setValue("variablePrueba", 5451);

            ///////////////MODIFIERS//////////////////
            //Crear un modifier para un valor FLOAT
            GuiController.Instance.Modifiers.addFloat("valorFloat", -50f, 200f, 0f);
            //Crear un modifier para un ComboBox con opciones
            string[] opciones = new string[] { "opcion1", "opcion2", "opcion3" };
            GuiController.Instance.Modifiers.addInterval("valorIntervalo", opciones, 0);
            //Crear un modifier para modificar un vértice
            GuiController.Instance.Modifiers.addVertex3f("valorVertice", new Vector3(-100, -100, -100), new Vector3(50, 50, 50), new Vector3(0, 0, 0));

            ///////////////CONFIGURAR CAMARA ROTACIONAL//////////////////
            //Es la camara que viene por default, asi que no hace falta hacerlo siempre
            GuiController.Instance.RotCamera.Enable = true;
            //Configurar centro al que se mira y distancia desde la que se mira
            GuiController.Instance.RotCamera.setCamera(new Vector3(0, 0, 0), 100);

            /*
            ///////////////CONFIGURAR CAMARA PRIMERA PERSONA//////////////////
            //Camara en primera persona, tipo videojuego FPS
            //Solo puede haber una camara habilitada a la vez. Al habilitar la camara FPS se deshabilita la camara rotacional
            //Por default la camara FPS viene desactivada
            GuiController.Instance.FpsCamera.Enable = true;
            //Configurar posicion y hacia donde se mira
            GuiController.Instance.FpsCamera.setCamera(new Vector3(0, 0, -20), new Vector3(0, 0, 0));
            */

            ///////////////LISTAS EN C#//////////////////
            //crear
            List<string> lista = new List<string>();
            //agregar elementos
            lista.Add("elemento1");
            lista.Add("elemento2");
            //obtener elementos
            string elemento1 = lista[0];
            //bucle foreach
            foreach (string elemento in lista)
            {
                //Loggear por consola del Framework
                GuiController.Instance.Logger.log(elemento);
            }
            //bucle for
            for (int i = 0; i < lista.Count; i++)
            {
                string element = lista[i];
            }
        }

        public void metodoUselessRender()
        {
            //Obtener valor de UserVar (hay que castear)
            int valor = (int)GuiController.Instance.UserVars.getValue("variablePrueba");
            //Obtener valores de Modifiers
            float valorFloat = (float)GuiController.Instance.Modifiers["valorFloat"];
            string opcionElegida = (string)GuiController.Instance.Modifiers["valorIntervalo"];
            Vector3 valorVertice = (Vector3)GuiController.Instance.Modifiers["valorVertice"];
            ///////////////INPUT//////////////////
            //conviene deshabilitar ambas camaras para que no haya interferencia
            //Capturar Input teclado 
            if (GuiController.Instance.D3dInput.keyPressed(Microsoft.DirectX.DirectInput.Key.F))
            {
                //Tecla F apretada
            }
            //Capturar Input Mouse
            if (GuiController.Instance.D3dInput.buttonPressed(TgcViewer.Utils.Input.TgcD3dInput.MouseButtons.BUTTON_LEFT))
            {
                //Boton izq apretado
            }
        }
    }
}


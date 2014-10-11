using AlumnoEjemplos.TheGRID.Shaders;
using AlumnoEjemplos.TheGRID;
using Microsoft.DirectX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSceneLoader;
using TgcViewer;
using Microsoft.DirectX.Direct3D;

namespace AlumnoEjemplos.TheGRID
{
    class Escenario
    {
        public ManagerLaser laserManager = new ManagerLaser(10);
        public ManagerAsteroide asteroidManager; 
        public Dibujable principal;
        public TgcBoundingCylinder limite;
        private Boolean fuera_limite = false;
        enum TipoModo { THE_OPENING, IMPULSE_DRIVE, WELCOME_HOME, VACUUM };
        private TipoModo escenarioActual = TipoModo.VACUUM;
        //Objetos
        Dibujable sol;
        private List<Dibujable> estrellas;
        private List<TgcTexture> texturasEstrellas;

        public Escenario(Dibujable ppal) 
        {
            principal = ppal;
            asteroidManager = new ManagerAsteroide(180); //Siempre debe ser mucho mayor que la cantidad de asteroides que queremos tener, pero no tanto sino colapsa
            limite = new TgcBoundingCylinder(principal.getPosicion(), 10000, 100000);
            crearEstrellas();   
        }

        public List<Dibujable> cuerpos()
        {
            return asteroidManager.Controlados();
        }

        private void crearEstrellas()
        {
            #region Stars
            //Cargamos la lista de estrellas
            estrellas = new List<Dibujable>();

            //Creamos.....EL SOL
            TgcMesh mesh_Sol = Factory.cargarMesh(@"Sol\sol-TgcScene.xml");
            sol = new Dibujable();
            sol.setObject(mesh_Sol, 0, 200, new Vector3(1F, 1F, 1F));
            sol.giro = -1;
            sol.ubicarEnUnaPosicion(new Vector3(0,0,9000));
            sol.activar();
            
            EjemploAlumno.workspace().dibujableCollection.Add(sol);
            estrellas.Add(sol);

            //Cargamos la lista de texturas
            texturasEstrellas = new List<TgcTexture>();
            texturasEstrellas.Add(TgcTexture.createTexture(GuiController.Instance.D3dDevice, EjemploAlumno.TG_Folder + @"Estrella\Textures\Azul.jpg"));
            texturasEstrellas.Add(TgcTexture.createTexture(GuiController.Instance.D3dDevice, EjemploAlumno.TG_Folder + @"Estrella\Textures\Blanca.jpg"));
            texturasEstrellas.Add(TgcTexture.createTexture(GuiController.Instance.D3dDevice, EjemploAlumno.TG_Folder + @"Estrella\Textures\Brillante.jpg"));
            texturasEstrellas.Add(TgcTexture.createTexture(GuiController.Instance.D3dDevice, EjemploAlumno.TG_Folder + @"Estrella\Textures\Celeste.jpg"));
            texturasEstrellas.Add(TgcTexture.createTexture(GuiController.Instance.D3dDevice, EjemploAlumno.TG_Folder + @"Estrella\Textures\Marron.jpg"));
            texturasEstrellas.Add(TgcTexture.createTexture(GuiController.Instance.D3dDevice, EjemploAlumno.TG_Folder + @"Estrella\Textures\Negra.jpg"));
            texturasEstrellas.Add(TgcTexture.createTexture(GuiController.Instance.D3dDevice, EjemploAlumno.TG_Folder + @"Estrella\Textures\Roja.jpg"));

            int i;
            for (i = 0; i < 1000; i++)
            {
                //Estrella como esfera            
                TgcSphere star = new TgcSphere();
                star.Radius = 20;
                int resto;
                Math.DivRem(i,texturasEstrellas.Count(), out resto);
                star.setTexture(texturasEstrellas[resto]);         //(Factory.elementoRandom<TgcTexture>(texturasEstrellas));
                star.Position = new Vector3(0, 0, 0);
                star.BasePoly = TgcSphere.eBasePoly.ICOSAHEDRON;
                star.Inflate = true;
                star.LevelOfDetail = 0;
                star.updateValues();
                TgcMesh meshTemporal = star.toMesh("Estrellita");
                //Estrella como dibujable
                Dibujable estrella;
                estrella = new Dibujable();
                meshTemporal.AutoTransformEnable = false;
                estrella.setObject(meshTemporal, 0, 200, new Vector3(1F, 1F, 1F));
                List<int> opcionesRotacion = new List<int>();
                opcionesRotacion.Add(1);
                opcionesRotacion.Add(-1);
                estrella.giro = Factory.elementoRandom<int>(opcionesRotacion);
                estrella.activar();
                //Rotamos la posicion de la estrella
                List<Vector3> opcionesPosiciones = new List<Vector3>();
               opcionesPosiciones.Add(new Vector3(9500, 0, 0));
               opcionesPosiciones.Add(new Vector3(-9500,0,0));
               opcionesPosiciones.Add(new Vector3(0,9500,0));
               opcionesPosiciones.Add(new Vector3(0,-9500,0));
               opcionesPosiciones.Add(new Vector3(0,0,9500));
               opcionesPosiciones.Add(new Vector3(0,0,-9500));

               Vector3 posicionFinal = Factory.elementoRandom<Vector3>(opcionesPosiciones);
                Vector3 rotacion = Factory.VectorRandom(0,360);  // Aca va una rotacion random
                rotacion.X = Geometry.DegreeToRadian(rotacion.X + i);
                rotacion.Y = Geometry.DegreeToRadian(rotacion.Y * i);
                rotacion.Z = Geometry.DegreeToRadian(rotacion.Z + 3 * i);
                Matrix rotation = Matrix.RotationYawPitchRoll(rotacion.Y, rotacion.X, rotacion.Z);
                Vector4 normal4 = Vector3.Transform(posicionFinal, rotation);
                posicionFinal = new Vector3(normal4.X, normal4.Y, normal4.Z);
                //Llevamos a la estrella a su posicion final
                estrella.ubicarEnUnaPosicion(posicionFinal);
                //Añadimos la estrella a las listas
                EjemploAlumno.workspace().dibujableCollection.Add(estrella);
                estrellas.Add(estrella);
            }
            #endregion
        }


        public void dispose()
        {
            sol.dispose();
            dispose();
        }
        //-------------------------------------------------------------------------------------------CHAPTER-1
        public void loadChapter1()
        {
            disposeOld();
            asteroidManager.fabricarCinturonAsteroides(principal.getPosicion(), 10, 100);
            escenarioActual = TipoModo.THE_OPENING;
        }
        //-------------------------------------------------------------------------------------------CHAPTER-2
        public void loadChapter2() 
        {
            disposeOld();
            escenarioActual = TipoModo.IMPULSE_DRIVE;
            EjemploAlumno.workspace().Shader.motionBlurActivado = true;
            EjemploAlumno.workspace().tiempoBlur = 5F;// velocidadBlur = 299800;
        }
        //-------------------------------------------------------------------------------------------CHAPTER-3
        public void loadChapter3() 
        {
            disposeOld();
            escenarioActual = TipoModo.WELCOME_HOME;
        }
        //-------------------------------------------------------------------------------------------VACUUM
        public void loadVacuum() 
        {
            disposeOld();
            escenarioActual = TipoModo.VACUUM;
        }

        private void disposeOld()
        {
            asteroidManager.desactivarTodos();
            EjemploAlumno.workspace().Shader.motionBlurActivado = false;
            // Aca se deben borran todas las cosas para un reinicializado.
        }
        //-------------------------------------------------------------------------------------------
        internal void dispararLaser()
        {
            laserManager.cargarDisparo(principal.getEjes(), principal.getPosicion());
        }
        internal void refrescar(float elapsedTime)                                                  //RENDER DEL ESCENARIO
        {            
            laserManager.operar(elapsedTime);
            asteroidManager.operar(elapsedTime);
            sol.rotarPorTiempo(elapsedTime, new List<Dibujable>());             
            foreach (Dibujable estrella in estrellas) estrella.desplazarUnaDistancia(principal.ultimaTraslacion);

            //no chequeo si los asteroides chocaron con la nave

            //Chequeo si la nave choco con algun asteroide
            asteroidManager.chocoNave(principal);
            //Chequeo si los lasers chocaron con algun asteroide

            //Invierto el flujo de este mensaje por cuestiones tecnicas
            //asteroidManager.chocoLasers(laserManager);
            laserManager.chocoAsteroide(); //no hace falta pasar el manager de asteroides, lo saca del singleton

            //no chequeo si algun laser choco con algun otro

            //Chequeo colision entre asteroides 
            asteroidManager.colisionEntreAsteroides(0); //hay que pasarle el 0 como parametro para que empieze a preguntar desde el asteoride 0, es una funcion recursiva

            if (TgcCollisionUtils.testPointCylinder(principal.getPosicion(), limite))
            {
                fuera_limite = true;
                //Aca activaria el bombardeo de asteroides que te destrozarian la carroceria
            }
        }

        internal void chequearCambio(string opcion)
        {
            switch (opcion)
            {
                case "THE OPENING":
                    if (escenarioActual != TipoModo.THE_OPENING)
                        loadChapter1();
                    break;
                case "IMPULSE DRIVE":
                    if (escenarioActual != TipoModo.IMPULSE_DRIVE)
                        loadChapter2();
                    break;
                case "WELCOME HOME":
                    if (escenarioActual != TipoModo.WELCOME_HOME)
                        loadChapter3();
                    break;
                case "VACUUM":
                    if (escenarioActual != TipoModo.VACUUM)
                        loadVacuum();
                    break;
            }
        }
    }
}

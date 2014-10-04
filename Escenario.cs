using AlumnoEjemplos.TheGRID.Shaders;
using AlumnoEjemplos.TheGRID;
using Microsoft.DirectX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSceneLoader;

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
        private List<TgcMesh> texturasEstrellas;


        public Escenario(Dibujable ppal) 
        {
            principal = ppal;
            asteroidManager = new ManagerAsteroide(180); //Siempre debe ser mucho mayor que la cantidad de asteroides que queremos tener, pero no tanto sino colapsa
            limite = new TgcBoundingCylinder(principal.getCentro(), 10000, 100000);
            crearEstrellas();
           
        }

        private void crearEstrellas()
        {
            //Creamos.....EL SOL
            TgcMesh mesh_Sol = Factory.cargarMesh(@"Sol\sol-TgcScene.xml");
            sol = new Dibujable();
            sol.setObject(mesh_Sol, 0, 2500, new Vector3(0, 0, 0), new Vector3(0.5F, 0.5F, 0.5F));
            sol.giro = -1;
            sol.activar();
            EjemploAlumno.workspace().meshCollection.Add((TgcMesh)sol.objeto);

            texturasEstrellas = new List<TgcMesh>();

            
            texturasEstrellas.Add(Factory.cargarMesh(@"Estrella\Estrella-Azul-TgcScene.xml"));
            texturasEstrellas.Add(Factory.cargarMesh(@"Estrella\Estrella-Blanca-TgcScene.xml"));
            texturasEstrellas.Add(Factory.cargarMesh(@"Estrella\Estrella-Brillante-TgcScene.xml"));
            texturasEstrellas.Add(Factory.cargarMesh(@"Estrella\Estrella-Celeste-TgcScene.xml"));
            texturasEstrellas.Add(Factory.cargarMesh(@"Estrella\Estrella-Marron-TgcScene.xml"));
            texturasEstrellas.Add(Factory.cargarMesh(@"Estrella\Estrella-Negra-TgcScene.xml"));
            texturasEstrellas.Add(Factory.cargarMesh(@"Estrella\Estrella-Roja-TgcScene.xml"));
            
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
            asteroidManager.fabricarCinturonAsteroides(principal.getCentro(), 10, 100);
            escenarioActual = TipoModo.THE_OPENING;
        }
        //-------------------------------------------------------------------------------------------CHAPTER-2
        public void loadChapter2() 
        {
            disposeOld();
            escenarioActual = TipoModo.IMPULSE_DRIVE;
            EjemploAlumno.workspace().Shader.motionBlurActivado = true;
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
            laserManager.fabricar(principal.getEjes(), principal.getPosicion());
        }
        internal void refrescar(float elapsedTime)                                                  //RENDER DEL ESCENARIO
        {            
            laserManager.operar(elapsedTime);
            asteroidManager.operar(elapsedTime);
                        
            
            Vector3 temporal = principal.getPosicion();
            temporal.Z+=9000;
            sol.ubicarEn(temporal);
            sol.rotar(elapsedTime, new List<Dibujable>());
            sol.render(elapsedTime);

            //Chequeo de colision
            //Chequeo si la nave choco con algun asteroide

            //Eze: Codigo repetido, esto ya se realiza en asteroidManager.chocoNave(nave);

            /*bool naveColision = false;
            foreach (Dibujable asteroide in asteroidManager.lista())
            {

                if (nave.getColision().colisiono(((TgcBoundingSphere)asteroide.getColision().getBoundingBox())))
                {
                    ((TgcObb)nave.getColision().getBoundingBox()).setRenderColor(Color.Red);
                    ((TgcBoundingSphere)asteroide.getColision().getBoundingBox()).setRenderColor(Color.Red);
                    naveColision = true;
                }
                else
                {                  
                    ((TgcBoundingSphere)asteroide.getColision().getBoundingBox()).setRenderColor(Color.Yellow);
                }            
            }
            if (!naveColision) ((TgcObb)nave.getColision().getBoundingBox()).setRenderColor(Color.Yellow);
            */

            //no chequeo si los asteroides chocaron con la nave

            //Chequeo si la nave choco con algun asteroide
            asteroidManager.chocoNave(principal);
            //Chequeo si los lasers chocaron con algun asteroide

            //Invierto el flujo de este mensaje por cuestiones tecnicas
            //asteroidManager.chocoLasers(laserManager);
            laserManager.chocoAsteroide(); //no hace falta pasar el manager de asteroides, lo saca del singleton

            //no chequeo si algun laser choco con algun otro

            //Chequeo colision entre asteroides 
            //asteroidManager.colisionEntreAsteroides(0); //hay que pasarle el 0 como parametro para que empieze a preguntar desde el asteoride 0, es una funcion recursiva
            
            if(TgcCollisionUtils.testPointCylinder(principal.getCentro(),limite)){
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

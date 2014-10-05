﻿using AlumnoEjemplos.TheGRID.Shaders;
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
            limite = new TgcBoundingCylinder(principal.getCentro(), 10000, 100000);
            crearEstrellas();   
        }

        public List<Dibujable> cuerpos()
        {
            return asteroidManager.Controlados();
        }

        private void crearEstrellas()
        {
            //Creamos.....EL SOL
            TgcMesh mesh_Sol = Factory.cargarMesh(@"Sol\sol-TgcScene.xml");
            sol = new Dibujable();
            sol.setObject(mesh_Sol, 0, 200, new Vector3(0, 0, 0), new Vector3(0.5F, 0.5F, 0.5F));
            sol.giro = -1;
            sol.ubicarEnUnaPosicion(new Vector3(0,0,9000));
            sol.activar();
            EjemploAlumno.workspace().meshCollection.Add((TgcMesh)sol.objeto);

            //Cargamos la lista de texturas
            texturasEstrellas = new List<TgcTexture>();
            texturasEstrellas.Add(TgcTexture.createTexture(GuiController.Instance.D3dDevice, EjemploAlumno.TG_Folder + @"Estrella\Textures\Azul.jpg"));
            texturasEstrellas.Add(TgcTexture.createTexture(GuiController.Instance.D3dDevice, EjemploAlumno.TG_Folder + @"Estrella\Textures\Blanca.jpg"));
            texturasEstrellas.Add(TgcTexture.createTexture(GuiController.Instance.D3dDevice, EjemploAlumno.TG_Folder + @"Estrella\Textures\Brillante.jpg"));
            texturasEstrellas.Add(TgcTexture.createTexture(GuiController.Instance.D3dDevice, EjemploAlumno.TG_Folder + @"Estrella\Textures\Celeste.jpg"));
            texturasEstrellas.Add(TgcTexture.createTexture(GuiController.Instance.D3dDevice, EjemploAlumno.TG_Folder + @"Estrella\Textures\Marron.jpg"));
            texturasEstrellas.Add(TgcTexture.createTexture(GuiController.Instance.D3dDevice, EjemploAlumno.TG_Folder + @"Estrella\Textures\Negra.jpg"));
            texturasEstrellas.Add(TgcTexture.createTexture(GuiController.Instance.D3dDevice, EjemploAlumno.TG_Folder + @"Estrella\Textures\Roja.jpg"));

            //Cargamos la lista de estrellas
            estrellas = new List<Dibujable>();

            int i;
            for (i = 0; i < 10; i++)
            {
                //Estrella como esfera            
                TgcSphere star = new TgcSphere();
                star.Radius = 20;
                star.setTexture(Factory.elementoRandom<TgcTexture>(texturasEstrellas));
                star.Position = new Vector3(0, 0, 0);
                star.BasePoly = TgcSphere.eBasePoly.ICOSAHEDRON;
                star.Inflate = true;
                star.LevelOfDetail = 0;
                star.updateValues();
                TgcMesh meshTemporal = star.toMesh("Estrellita");
                //Estrella como dibujable
                Dibujable estrella;
                estrella = new Dibujable(0, 0, 0);
                meshTemporal.AutoTransformEnable = false;
                estrella.setObject(meshTemporal, 0, 200, new Vector3(0, 0, 0), new Vector3(1F, 1F, 1F));
                List<int> opcionesRotacion = new List<int>();
                opcionesRotacion.Add(1);
                opcionesRotacion.Add(-1);
                estrella.giro = Factory.elementoRandom<int>(opcionesRotacion);
                estrella.activar();
                //Rotamos la posicion de la estrella
                List<Vector3> opcionesPosiciones = new List<Vector3>();
               opcionesPosiciones.Add(new Vector3(200, 0, 0));
               opcionesPosiciones.Add(new Vector3(-200,0,0));
               opcionesPosiciones.Add(new Vector3(0,200,0));
               opcionesPosiciones.Add(new Vector3(0,-200,0));
               opcionesPosiciones.Add(new Vector3(0,0,200));
               opcionesPosiciones.Add(new Vector3(0,0,-200));

               Vector3 posicionFinal = Factory.elementoRandom<Vector3>(opcionesPosiciones);
                Vector3 rotacion = Factory.VectorRandom(-90,90);  // Aca va una rotacion random
                rotacion.X = Geometry.DegreeToRadian(rotacion.X);
                rotacion.Y = Geometry.DegreeToRadian(rotacion.Y);
                rotacion.Z = Geometry.DegreeToRadian(rotacion.Z);
                Matrix rotation = Matrix.RotationYawPitchRoll(rotacion.Y, rotacion.X, rotacion.Z);
                Vector4 normal4 = Vector3.Transform(posicionFinal, rotation);
                posicionFinal = new Vector3(normal4.X, normal4.Y, normal4.Z);
                //Llevamos a la estrella a su posicion final
                estrella.ubicarEnUnaPosicion(posicionFinal);
                //Añadimos la estrella a las listas
                EjemploAlumno.workspace().meshCollection.Add((TgcMesh)estrella.objeto);
                estrellas.Add(estrella);
            }
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

            sol.desplazarUnaDistancia(principal.ultimaTraslacion);
            sol.rotarPorTiempo(elapsedTime, new List<Dibujable>());

            foreach (Dibujable estrella in estrellas) estrella.rotarPorTiempo(elapsedTime, new List<Dibujable>());

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
            asteroidManager.colisionEntreAsteroides(0); //hay que pasarle el 0 como parametro para que empieze a preguntar desde el asteoride 0, es una funcion recursiva
            
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

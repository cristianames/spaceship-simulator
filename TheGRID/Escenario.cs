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
using AlumnoEjemplos.TheGRID.Colisiones;
using System.Drawing;
using AlumnoEjemplos.TheGRID.Helpers;

namespace AlumnoEjemplos.TheGRID
{
    class Escenario
    {
        #region Atributos
        public ManagerLaser laserManager = new ManagerLaser(100);
        public ManagerAsteroide asteroidManager; 
        public Dibujable principal;
        public TgcBoundingCylinder limite;
        //private Boolean fuera_limite = false;
        enum TipoModo { THE_OPENING, IMPULSE_DRIVE, WELCOME_HOME, VACUUM };
        private TipoModo escenarioActual = TipoModo.VACUUM;
        //Objetos
        public Dibujable sol;
        public Dibujable planet;
        private List<Dibujable> cuerposGravitacionales = new List<Dibujable>();
        public List<Dibujable> CuerposGravitacionales { get{ return cuerposGravitacionales; } }
        #endregion

        #region Constructor y Destructor
        public Escenario(Dibujable ppal) 
        {
            principal = ppal;
            asteroidManager = new ManagerAsteroide(2000); //Siempre debe ser mucho mayor que la cantidad de asteroides que queremos tener, pero no tanto sino colapsa
            limite = new TgcBoundingCylinder(principal.getPosicion(), 1500, 15000);

            //Creamos.....EL SOL
            TgcMesh mesh_Sol = Factory.cargarMesh(@"Sol\sol-TgcScene.xml");
            sol = new Dibujable();
            sol.setObject(mesh_Sol, 0, 200, new Vector3(2F, 2F, 2F));
            sol.giro = -1;
            sol.ubicarEnUnaPosicion(new Vector3(0, 0, 9000));
            sol.activar();
            EjemploAlumno.workspace().sol = sol;

            //Creamos.....THE PLANET
            TgcMesh mesh_Planet = Factory.cargarMesh(@"asteroid\theplanet-TgcScene.xml");
            planet = new Dibujable();
            planet.setObject(mesh_Planet, 0, 10, new Vector3(10F, 10F, 10F));
            planet.setFisica(0, 0, 0, 500000000);
            planet.giro = -1;
            planet.ubicarEnUnaPosicion(new Vector3(0, 0, 0));
            planet.desactivar();
            TgcBoundingSphere bounding_asteroide = new TgcBoundingSphere(new Vector3(0, 0, 0), 15000);
            planet.setColision(new ColisionAsteroide());
            planet.getColision().setBoundingBox(bounding_asteroide);
            EjemploAlumno.workspace().dibujableCollection.Add(planet);

            //crearEstrellas();   
        }
        public void dispose()
        {
            sol.dispose();
        }
        #endregion

        #region Carga de escenario
        //-------------------------------------------------------------------------------------------CHAPTER-1
        public void loadChapter1()
        {
            disposeOld();
            asteroidManager.fabricarMapaAsteroides(principal.getPosicion(), 50, 7000);
            cuerposGravitacionales = asteroidManager.Controlados();
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
            Dibujable ppal = EjemploAlumno.workspace().ObjetoPrincipal;
            List<int> opciones = new List<int>() { -15000, 15000 };
            Vector3 posicion = ppal.getPosicion();
            posicion.Add(Vector3.Multiply(ppal.getDireccion(),13000));
            posicion.Add(Vector3.Multiply(ppal.getDireccion_X(), Factory.elementoRandom<int>(opciones)));
            posicion.Add(Vector3.Multiply(ppal.getDireccion_Y(), (new Random()).Next(-3000,3000)));
            planet.ubicarEnUnaPosicion(posicion);
            planet.activar();
            cuerposGravitacionales = new List<Dibujable>() { planet };
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
            planet.desactivar();
            cuerposGravitacionales = new List<Dibujable>();
            EjemploAlumno.workspace().Shader.motionBlurActivado = false;
            // Aca se deben borran todas las cosas para un reinicializado.
        }
        #endregion
        
        #region Update segun escenario
        internal void dispararLaser()
        {
            if (escenarioActual != TipoModo.THE_OPENING) return;
            laserManager.cargarDisparo(principal.getEjes(), principal.getPosicion());
            EjemploAlumno.workspace().music.playLazer();
        }
        internal void refrescar(float elapsedTime)                                                  //RENDER DEL ESCENARIO
        {

            sol.rotarPorTiempo(elapsedTime, new List<Dibujable>());
            sol.desplazarUnaDistancia(principal.ultimaTraslacion);
            switch (escenarioActual)
            {
                case TipoModo.THE_OPENING:
                    laserManager.operar(elapsedTime);
                    asteroidManager.reinsertarSiEsNecesario();
                    asteroidManager.operar(elapsedTime);
                    asteroidManager.chocoNave(principal);
                    laserManager.chocoAsteroide();
                    asteroidManager.colisionEntreAsteroides(0);
                    break;
                case TipoModo.IMPULSE_DRIVE:
                    break;
                case TipoModo.WELCOME_HOME:
                    colisionNavePlaneta(EjemploAlumno.workspace().ObjetoPrincipal);
                    planet.rotarPorTiempo(elapsedTime, new List<Dibujable>());
                    break;
            }
            /*if (TgcCollisionUtils.testPointCylinder(principal.getPosicion(), limite))
            {
                fuera_limite = true;
                //Aca activaria el bombardeo de asteroides que te destrozarian la carroceria
            }*/
        }

        private void colisionNavePlaneta(Dibujable nave)
        {
            Color color = Color.Yellow;
            if (nave.getColision().colisiono(((TgcBoundingSphere)planet.getColision().getBoundingBox())))
            {
                color = Color.Red;
                //--------
                int flagReintento = 0;
                //Dupla<Vector3> velocidades = Fisica.CalcularChoqueElastico(nave, planet);
                Vector3 direccion = nave.getDireccion();
                Vector3 distancias = Vector3.Subtract(nave.getPosicion(), planet.getPosicion());
                //distancias.X *= distancias.X;
                //distancias.Y *= distancias.Y;
                //distancias.Z *= distancias.Z;
                distancias.Normalize();
                distancias.Multiply(1);
                direccion.X *= distancias.X;
                direccion.Y *= distancias.Y;
                direccion.Z *= distancias.Z;
                float velocidad = nave.fisica.velocidadInstantanea * 0.7f;// / 3f;
                //velocidad = (velocidad * distancias.X)+(velocidad * distancias.Y)+(velocidad * distancias.Z);
                float radioCuad = FastMath.Pow2(((TgcBoundingSphere)planet.getColision().getBoundingBox()).Radius) + 0;
                while (Vector3.LengthSq(Vector3.Subtract(nave.getPosicion(), planet.getPosicion())) < radioCuad)
                {
                    nave.impulsate(direccion, velocidad, 0.01f);
                }                
                if (flagReintento == 0) EjemploAlumno.workspace().music.playAsteroideColision();
                //--------
            }
            //((TgcBoundingSphere)planet.getColision().getBoundingBox()).setRenderColor(color);
            ((TgcObb)nave.getColision().getBoundingBox()).setRenderColor(color);
        }
        #endregion

        #region Chequeo de cambios
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
        #endregion

        #region Deprecados
        private void crearEstrellas()
        {
            //Cargamos la lista de texturas
            List<TgcTexture> texturasEstrellas = new List<TgcTexture>();
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
                Math.DivRem(i, texturasEstrellas.Count(), out resto);
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
                opcionesPosiciones.Add(new Vector3(-9500, 0, 0));
                opcionesPosiciones.Add(new Vector3(0, 9500, 0));
                opcionesPosiciones.Add(new Vector3(0, -9500, 0));
                opcionesPosiciones.Add(new Vector3(0, 0, 9500));
                opcionesPosiciones.Add(new Vector3(0, 0, -9500));

                Vector3 posicionFinal = Factory.elementoRandom<Vector3>(opcionesPosiciones);
                Vector3 rotacion = Factory.VectorRandom(0, 360);  // Aca va una rotacion random
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
            }
        }
        #endregion

    }
}

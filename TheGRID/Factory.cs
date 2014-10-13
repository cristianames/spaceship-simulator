using AlumnoEjemplos.TheGRID.Colisiones;
using AlumnoEjemplos.TheGRID.Shaders;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using TgcViewer;
using TgcViewer.Utils.Modifiers;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSceneLoader;
using AlumnoEjemplos.TheGRID.Helpers;

namespace AlumnoEjemplos.TheGRID
{
    public class Factory
    {
        static Device d3dDevice = GuiController.Instance.D3dDevice;
        static string alumnoMediaFolder = GuiController.Instance.AlumnoEjemplosMediaDir;

        TgcMesh mesh_asteroide_base;
        TgcTexture[] normalMapAsteroidArray;

        public Factory()
        {
            mesh_asteroide_base = cargarMesh("asteroid\\asteroid1-TgcScene.xml");
            TgcTexture normalMapAsteroid = TgcTexture.createTexture(EjemploAlumno.TG_Folder + "asteroid\\Textures\\3215_Bump.jpg");
            normalMapAsteroidArray = new TgcTexture[] { normalMapAsteroid };
        }
        
        #region Random
        public static int numeroRandom()
        {
            int resto;
            long restoL;
            Random random = new Random();
            random = new Random(random.Next());
            Math.DivRem(random.Next(), DateTime.Now.Millisecond + 1, out resto);
            random = new Random(resto);
            Math.DivRem(random.Next(), DateTime.Now.Second + 1, out resto);
            random = new Random(resto);
            Math.DivRem(random.Next(), DateTime.Now.Hour + 1, out resto);
            random = new Random(resto);
            Math.DivRem(DateTime.Now.Ticks, DateTime.Now.Millisecond + 2, out restoL);
            Math.DivRem(restoL, 65536, out restoL);
            resto = (int)restoL;
            random = new Random(resto);
            return random.Next();
        }
        public static T elementoRandom<T>(List<T> valores)
        {
            int resto, length;
            length = valores.Count;
            Math.DivRem(numeroRandom(), length, out resto);
            return valores[resto];
        }
        public static Vector3 VectorRandom(int valorMinimo, int valorMaximo){
            Vector3 victima = new Vector3();
            //Thread.Sleep(5);
            victima.X = numeroRandom()+1;
            //Thread.Sleep(5);
            victima.Y = numeroRandom()+1;
            //Thread.Sleep(5);
            victima.Z = numeroRandom()+1;
            return victima;
        }
        #endregion

        #region Carga y Descarga
        public static TgcMesh cargarMesh(string path)
        {
            TgcSceneLoader loader = new TgcSceneLoader();
            TgcScene scene = loader.loadSceneFromFile(EjemploAlumno.TG_Folder + path);
            return scene.Meshes[0];
        }

        public static void resetearDefault(ref Dibujable victima)   //Vuelve a cargar el dibujable, dejandolo sin Fisica ni Colision. Ademas queda desactivado.
        {
            Dibujable auxiliar = victima;
            victima = new Dibujable();
            auxiliar.objeto.Transform = Matrix.Identity;
            victima.setObject(auxiliar.objeto, auxiliar.velocidad, auxiliar.velocidadRadial, auxiliar.escala);
        }
        public static void resetearDefault_Asteroide(ref Asteroide victima)   //Vuelve a cargar el dibujable, dejandolo sin Fisica ni Colision. Ademas queda desactivado.
        {
            Asteroide auxiliar = victima;
            victima = new Asteroide();
            auxiliar.objeto.Transform = Matrix.Identity;
            victima.setObject(auxiliar.objeto, auxiliar.velocidad, auxiliar.velocidadRadial, auxiliar.escala);
            victima.traslacion = 0;
            victima.inclinacion = 0;
            victima.rotacion = 0;
            victima.giro = 0;
            victima.manager = auxiliar.manager;
        }
        #endregion

        #region Asteroide
        public Asteroide crearAsteroide(TamanioAsteroide tamanio, Vector3 posicion, ManagerAsteroide manager)
        {
            FormatoAsteroide formato = Asteroide.elegirAsteroidePor(tamanio);
            TgcMeshBumpMapping mesh_asteroide = TgcMeshBumpMapping.fromTgcMesh(mesh_asteroide_base, normalMapAsteroidArray);
            List<float> valores = new List<float>() { -4, -3, -2, -1, 1, 2, 3, 4 };
            Vector3 escalado = formato.getVolumen();
            Vector3 rotacion = new Vector3(elementoRandom(valores), elementoRandom(valores), elementoRandom(valores));
            EjeCoordenadas ejes = new EjeCoordenadas();
            //ejes.vectorX = ejeX;
            //ejes.vectorY = ejeY;
            //ejes.vectorZ = ejeZ;

            //Creamos su bounding Sphere
            mesh_asteroide.AutoUpdateBoundingBox = false;
            float radioMalla3DsMax = 11.633f;
            TgcBoundingSphere bounding_asteroide = new TgcBoundingSphere(posicion, radioMalla3DsMax * formato.getVolumen().X);

            //Cargamos las cosas en el dibujable
            Asteroide asteroide = new Asteroide();
            asteroide.setObject(mesh_asteroide, 2, 4, escalado);
            asteroide.AutoTransformEnable = false;
            asteroide.setColision(new ColisionAsteroide());
            asteroide.getColision().setBoundingBox(bounding_asteroide);
            asteroide.traslacion = 0;
            asteroide.rotacion = 0;
            asteroide.ubicarEnUnaPosicion(posicion);
            asteroide.setEjes(ejes);

            asteroide.setFisica(0, 0, 10, formato.getMasa());
            
            asteroide.velocidad = formato.getVelocidad();
            asteroide.tamanioAnterior = formato.tamanioAnterior();
            asteroide.Vida = formato.vidaInicial();
            asteroide.manager = manager;
            asteroide.SetPropiedades(false, true, false);

            EjemploAlumno.addMesh(asteroide);
            return asteroide;
        }

        public static Dibujable resetearAsteroide(Asteroide asteroide)   //Resetea el dibujable y le agrega ademas la OBB.
        {
            TgcBoundingSphere bb = (TgcBoundingSphere) asteroide.getColision().getBoundingBox();
            resetearDefault_Asteroide(ref asteroide);
            asignarBB_Asteroide(asteroide, bb);
            return asteroide;
        }
        private static void asignarBB_Asteroide(Dibujable asteroide, TgcBoundingSphere bb)
        {
            asteroide.setColision(new ColisionAsteroide());
            asteroide.getColision().setBoundingBox(bb);
        }
        #endregion

        #region Laser
        public static Dibujable crearLaserRojo()   //Carga la Mesh con los valores default + la OBB.
        {
            //Creemos la mesh
            TgcMesh mesh_laser = cargarMesh("Laser\\Laser_Box-TgcScene.xml");
            //Cargamos las cosas en el dibujable
            Dibujable laser = new Dibujable();
            laser.setObject(mesh_laser, 2000, 100, new Vector3(0.09F, 0.09F, 0.13F));
            laser.desactivar();
            asignarOBB_Laser(laser, new Vector3(0.1F, 0.1F, 0.15F));
            EjemploAlumno.addMesh(laser);
            return laser;
        }
        public static Dibujable resetearLaser(Dibujable laser)   //Resetea el dibujable y le agrega ademas la OBB.
        {
            resetearDefault(ref laser);
            asignarOBB_Laser(laser, new Vector3(0.1F, 0.1F, 0.15F));
            return laser;
        }
        private static void asignarOBB_Laser(Dibujable laser, Vector3 escalado)  //Asigna una nueva OBB al dibujable en cuestion.
        {
            TgcBoundingBox bb = laser.objeto.BoundingBox;
            bb.scaleTranslate(laser.getPosicion(), escalado);
            TgcObb obb = TgcObb.computeFromAABB(bb);
            laser.setColision(new ColisionLaser());
            laser.getColision().setBoundingBox(obb);
            laser.getColision().transladar(laser.getPosicion());
        }
        private static void rotarOBB(List<Vector3> rotaciones, IColision colisionOBB)
        {
            TgcObb obb = (TgcObb)colisionOBB.getBoundingBox();
            foreach (var vRotor in rotaciones) { obb.rotate(vRotor); } //Le aplicamos TODAS las rotaciones que hasta ahora lleva la nave.            
        }
        public static void reubicarLaserAPosicion(Dibujable laser, EjeCoordenadas ejes, Vector3 posicionNave)
        {
            //Ubicamos el laser en el cañon
            laser.setEjes(ejes);
            laser.Transform *= ejes.mRotor;
            laser.ubicarEnUnaPosicion(posicionNave);
            //Carga sentido de traslacion y rotacion
            laser.rotacion = 0;
            laser.traslacion = 1;
            rotarOBB(laser.getEjes().lRotor, laser.getColision());
        }
        #endregion
    }
}

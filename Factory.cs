using Microsoft.DirectX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TgcViewer.Utils.TgcSceneLoader;
using TgcViewer;
using Microsoft.DirectX.Direct3D;
using System.Drawing;
using TgcViewer.Utils.Modifiers;
using TgcViewer.Utils.TgcGeometry;
using AlumnoEjemplos.TheGRID.Colisiones;

namespace AlumnoEjemplos.TheGRID
{
    public static class Factory
    {
        static Device d3dDevice = GuiController.Instance.D3dDevice;
        static string alumnoMediaFolder = GuiController.Instance.AlumnoEjemplosMediaDir;        
        public static T numeroRandom<T>(List<T> valores)
        {
            Random random = new Random();
            int numero = random.Next();
            int resto, length;
            length = valores.Count;
            Math.DivRem(numero, length, out resto);
            return valores[resto];
        }

        private static TgcMesh cargarMesh(string path){
            TgcSceneLoader loader = new TgcSceneLoader();
            TgcScene scene = loader.loadSceneFromFile(GuiController.Instance.AlumnoEjemplosMediaDir + path);
            return scene.Meshes[0];
        }

        public static void trasladar(Dibujable asteroide, Vector3 vector)
        {
            Matrix traslacion = Matrix.Translation(vector);
            asteroide.Transform *= traslacion;
            asteroide.getColision().transladar(vector);
        }

        // public static FormatoAsteroide elegirAsteroidePor(TamanioAsteroide tamanio)
        // movido a la clase Asteroide

        public static Dibujable crearAsteroide(TamanioAsteroide tamanio, Vector3 posicion)
        {
            FormatoAsteroide formato = Asteroide.elegirAsteroidePor(tamanio);
            TgcMesh mesh_asteroide = cargarMesh("TheGrid\\asteroid\\asteroid-TgcScene.xml");
            List<float> valores = new List<float>() { -4, -3, -2, -1, 1, 2, 3, 4 };
            Vector3 escalado = formato.getVolumen();
            Vector3 rotacion = new Vector3(numeroRandom(valores), numeroRandom(valores), numeroRandom(valores));
            Vector3 ejeX = new Vector3(numeroRandom(valores), numeroRandom(valores), numeroRandom(valores));
            Vector3 ejeY = new Vector3(numeroRandom(valores), numeroRandom(valores), numeroRandom(valores));
            Vector3 ejeZ = new Vector3(numeroRandom(valores), numeroRandom(valores), numeroRandom(valores));
            EjeCoordenadas ejes = new EjeCoordenadas();
            ejes.vectorX = ejeX;
            ejes.vectorY = ejeY;
            ejes.vectorZ = ejeZ;

            //Creamos su bounding Sphere
            mesh_asteroide.AutoUpdateBoundingBox = false;
            float radioMalla3DsMax = 11.633f;
            TgcBoundingSphere bounding_asteroide = new TgcBoundingSphere(posicion, radioMalla3DsMax * formato.getVolumen().X);

            //Cargamos las cosas en el dibujable
            Asteroide asteroide = new Asteroide();
            asteroide.setObject(mesh_asteroide, 10, 4, rotacion, escalado);
            asteroide.AutoTransformEnable = false;
            asteroide.setColision(new ColisionAsteroide());
            asteroide.getColision().setBoundingBox(bounding_asteroide);
            asteroide.traslacion = 1;
            asteroide.rotacion = 1;
            trasladar(asteroide, posicion);
            asteroide.setPosicion(posicion);
            asteroide.setEjes(ejes);
            asteroide.setFisica(0, 0, formato.getMasa());
            asteroide.velocidad = formato.getVelocidad();
            asteroide.Formato = formato;
            return asteroide;
        }

        /* public Asteroide(Vector3 tamanio)
        {
            transform.Scale(tamanio);
            Device d3dDevice = GuiController.Instance.D3dDevice;
            //Carpeta de archivos Media del alumno
            string alumnoMediaFolder = GuiController.Instance.AlumnoEjemplosMediaDir;   

            TgcSceneLoader loader = new TgcSceneLoader();
            changeDiffuseMaps(new TgcTexture[] { TgcTexture.createTexture(d3dDevice, GuiController.Instance.ExamplesDir + "Transformations\\SistemaSolar\\SunTexture.jpg") });

        }*/

        public static Dibujable crearLaser(EjeCoordenadas ejes, Vector3 posicionNave)
        {
            //Creemos la mesh
            TgcMesh mesh_laser = cargarMesh("TheGrid\\Laser\\Laser_Box-TgcScene.xml");
            //Cargamos las cosas en el dibujable
            Dibujable laser = new Dibujable();
            laser.setObject(mesh_laser, 5000, 100, new Vector3(0, 0, 0), new Vector3(0.09F, 0.09F, 0.13F));
            //Ubicamos el laser en el cañon
            laser.setEjes(ejes);
            laser.Transform *= ejes.mRotor;
            laser.trasladar(posicionNave);
            laser.setPosicion(((TgcMesh)laser.objeto).Position);
            //Carga sentido de traslacion y rotacion
            List<int> valores = new List<int>(2);
            valores.Add(1);
            valores.Add(-1);
            laser.rotacion = 0;//numeroRandom<int>(valores);
            laser.traslacion = 1;            
            //Le seteamos la Collision box.
            TgcBoundingBox bb = ((TgcMesh)laser.objeto).BoundingBox;
            bb.scaleTranslate(laser.getPosicion(), new Vector3(0.1F, 0.1F, 0.15F));
            TgcObb obb = TgcObb.computeFromAABB(bb);
            foreach (var vRotor in ejes.lRotor) { obb.rotate(vRotor); } //Le aplicamos TODAS las rotaciones que hasta ahora lleva la nave.            
            laser.setColision(new ColisionLaser());
            laser.getColision().setBoundingBox(obb);
            laser.getColision().transladar(posicionNave);

            return laser;
        }
    }
    
    // Formato del asteroide movido a Asteroide.cs
}

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
        private static T numeroRandom<T>(List<T> valores)
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

        public static Dibujable crearAsteroide()
        {
            TgcMesh mesh_asteroide = cargarMesh("asteroid\\asteroid-TgcScene.xml");
            List<float> factoresEscalado = new List<float>() { 5, 5.5F, 6, 6.5F, 7 };
            List<float> valores = new List<float>() { -4, -3, -2, -1, 1, 2, 3, 4 };
            List<float> valoresPosXY = new List<float>() { -100, 0, 100 };
            List<float> valoresPosZ = new List<float>() { 500, 800 };
            float factorEscalado = numeroRandom(factoresEscalado);
            Vector3 escalado = new Vector3(factorEscalado, factorEscalado, factorEscalado);
            Vector3 rotacion = new Vector3(numeroRandom(valores), numeroRandom(valores), numeroRandom(valores));
            Vector3 posicion = new Vector3(numeroRandom(valoresPosXY), numeroRandom(valoresPosXY), numeroRandom(valoresPosZ));
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
            TgcBoundingSphere bounding_asteroide = new TgcBoundingSphere(posicion, radioMalla3DsMax * factorEscalado);

            //Cargamos las cosas en el dibujable
            Dibujable asteroide = new Dibujable();
            asteroide.setObject(mesh_asteroide, 10, 4, rotacion, escalado);
            asteroide.AutoTransformEnable = false;
            asteroide.setColision(new ColisionAsteroide());
            asteroide.getColision().setBoundingBox(bounding_asteroide);
            asteroide.traslacion = 1;
            asteroide.rotacion = 1;
            trasladar(asteroide, posicion);
            asteroide.setPosicion(posicion);
            asteroide.setEjes(ejes);
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
        /*      LASER DANTE
        public Dibujable crearLaser(Vector3 origen) //se le pasa como parametro al laser el punto de origen 
        {
            //Carguemos el DirectX y la carpeta de media
            Device d3dDevice = GuiController.Instance.D3dDevice;
            string alumnoMediaFolder = GuiController.Instance.AlumnoEjemplosMediaDir;
            //Creemos la mesh
            TgcSceneLoader loader = new TgcSceneLoader();
            TgcScene scene = loader.loadSceneFromFile(GuiController.Instance.AlumnoEjemplosMediaDir + "Laser\\Laser_Box-TgcScene.xml");
            TgcMesh mesh_laser = scene.Meshes[0];
            mesh_laser.AutoTransformEnable = false;
            mesh_laser.Transform = Matrix.Scaling(new Vector3(0.1F, 0.1F, 1F)) * Matrix.Translation(origen);            
            //Cargamos las cosas en el dibujable
            Dibujable laser = new Dibujable();
            laser.objeto = mesh_laser;
            return laser;
        }
        public void dispararLaser(Dibujable laser, Vector3 direccion,float time) 
        {
            Matrix traslacion = Matrix.Translation(direccion*time*4000);// extraer velocidad
            ((TgcMesh)laser.objeto).Transform *= traslacion;
        }
        */

        public static Dibujable crearLaser(Matrix transformacion,EjeCoordenadas ejes) 
        {
            //Creemos la mesh
            TgcMesh mesh_laser = cargarMesh("Laser\\Laser_Box-TgcScene.xml");
            //mesh_laser.Transform = Matrix.Scaling(new Vector3(0.1F, 0.1F, 1F));
            //Cargamos las cosas en el dibujable
            Dibujable laser = new Dibujable();
            laser.setObject(mesh_laser, 5000, 0, new Vector3(0, 0, 0), new Vector3(0.1F, 0.1F, 0.5F));
            laser.AutoTransformEnable = false;
            //Ubicamos el laser en el cañon
            laser.setEjes(ejes);
            laser.trasladar(ejes.getCentro());
            laser.Transform *= transformacion;
            //Carga direccion de traslacion
            List<int> posibilidades = new List<int>(2);
            posibilidades.Add(-1);
            posibilidades.Add(1);
            laser.rotacion = 0;     //DE MOMENTO EL LASER NO SE PUEDE ROTAR.
            laser.traslacion = 1;
            return laser;
        }
    }
}

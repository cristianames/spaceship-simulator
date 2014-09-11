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
        public static FormatoAsteroide elegirAsteroidePor(TamanioAsteroide tamanio){
            switch(tamanio){
                case TamanioAsteroide.MUYGRANDE:
                    return new AsteroideMuyGrande();
                case TamanioAsteroide.GRANDE:
                    return new AsteroideGrande();
                case TamanioAsteroide.MEDIANO:
                    return new AsteroideMediano();
                case TamanioAsteroide.CHICO:
                    return new AsteroideChico();
            }
            return null;
        }

        public static Dibujable crearAsteroide(TamanioAsteroide tamanio, Vector3 posicion)
        {
            FormatoAsteroide formato = elegirAsteroidePor(tamanio);
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
            asteroide.fisica = new Fisica(asteroide,0,0,formato.getMasa());
            asteroide.velocidad = formato.getVelocidad();
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
            laser.AutoTransformEnable = false;
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
    public enum TamanioAsteroide {MUYGRANDE, GRANDE, MEDIANO,CHICO}

    public interface FormatoAsteroide
    {
        float getMasa(); //En toneladas
        Vector3 getVolumen(); //En realidad un factor de escalado
        float getVelocidad();
    }

    public class AsteroideMuyGrande : FormatoAsteroide
    {
        private float masa = 1000; 
        private float longitud = 8;
        public float getMasa() { return masa; }
        public Vector3 getVolumen(){ return new Vector3(longitud,longitud,longitud); }
        public float getVelocidad() { return 20; }
    }
    public class AsteroideGrande : FormatoAsteroide
    {
        private float masa = 800; 
        private float longitud = 6;
        public float getMasa() { return masa; }
        public Vector3 getVolumen() { return new Vector3(longitud, longitud, longitud); }
        public float getVelocidad() { return 25; }
    }
    public class AsteroideMediano : FormatoAsteroide
    {
        private float masa = 500; 
        private float longitud = 3;
        public float getMasa() { return masa; }
        public Vector3 getVolumen() { return new Vector3(longitud, longitud, longitud); }
        public float getVelocidad() { return 30; }
    }
    public class AsteroideChico : FormatoAsteroide
    {
        private float masa = 200;
        private float longitud =0.7f;
        public float getMasa() { return masa; }
        public Vector3 getVolumen() { return new Vector3(longitud, longitud, longitud); }
        public float getVelocidad() { return 40; }
    }
}

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

        /*  DANTE, Basicamente copie todo el metodo por si me mandaba cagada. Corregi o cambie algunas cosas, pero la rotacion aun no la consigo.
         * Si puedo conseguir guardarme la MATRIZ de rotacion desde el origen de los tiempos, pero parece que las obb aplican mal las matrices.
         * Cuando guardo la rotacion en forma de vector, se guarda mal. Lo hago en rotor y mRotor de los ejes.
         * 
         * 
        public static Dibujable crearLaser(Matrix transformacion,EjeCoordenadas ejes,Vector3 posicionNave,Vector3 rotacionNave) 
        {
            //Creemos la mesh
            TgcMesh mesh_laser = cargarMesh("Laser\\Laser_Box-TgcScene.xml");
            //mesh_laser.Transform = Matrix.Scaling(new Vector3(0.1F, 0.1F, 1F));
            //Cargamos las cosas en el dibujable
            Dibujable laser = new Dibujable();
            laser.setObject(mesh_laser, 5, 0, new Vector3(0, 0, 0), new Vector3(0.1F, 0.1F, 0.5F));
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
            TgcBoundingBox bb = ((TgcMesh)laser.objeto).BoundingBox;
            bb.scaleTranslate(posicionNave, new Vector3(0.1F, 0.1F, 0.5F));
            bb.move(ejes.getCentro());

            //CALCULO ROTACION LASER EN EJE X
            Vector3 versorZ = new Vector3();
            versorZ = laser.getEjes().vectorZ;
            versorZ.X = 0;
            //calculo angulo entre versorZ y el eje Z
            float angX = FastMath.Acos(versorZ.Z / (FastMath.Sqrt((FastMath.Pow2(versorZ.X))) + FastMath.Sqrt((FastMath.Pow2(versorZ.Y))) + FastMath.Sqrt((FastMath.Pow2(versorZ.Z + 1)))));
            Matrix rotacionX = new Matrix();
            rotacionX.RotateX(angX);
            //bb.transform(rotacionX);

            TgcObb obb = TgcObb.computeFromAABB(bb);
            laser.setColision(new ColisionLaser());
            laser.getColision().setBoundingBox(obb);
            //((TgcObb)laser.getColision().getBoundingBox()).setRotation(new Vector3 (1,0,0));
            //laser.getColision().transladar(ejes.getCentro());
           
            return laser;
        }*/
        public static Dibujable crearLaser(Matrix transformacion, EjeCoordenadas ejes, Vector3 posicionNave, Vector3 rotacionNave)
        {
            //Creemos la mesh
            TgcMesh mesh_laser = cargarMesh("TheGrid\\Laser\\Laser_Box-TgcScene.xml");
            //Cargamos las cosas en el dibujable
            Dibujable laser = new Dibujable();
            laser.setObject(mesh_laser, 10, 100, new Vector3(0, 0, 0), new Vector3(0.09F, 0.09F, 0.13F));
            laser.AutoTransformEnable = false;
            //Ubicamos el laser en el cañon
            laser.setEjes(ejes);
            //-------  Prueba para acomodar bien el laser
            laser.trasladar(posicionNave);
            //laser.Transform *= transformacion;    //Funcional
            //laser.Transform *= Matrix.RotationYawPitchRoll(ejes.rotor.Y,ejes.rotor.X,ejes.rotor.Z);   //Prueba 1
            laser.Transform *= ejes.mRotor; //Prueba 2 -- Aparentemente funcional
            //-------
            laser.setPosicion(((TgcMesh)laser.objeto).Position);
            //Carga sentido de traslacion y rotacion
            List<int> valores = new List<int>(2);
            valores.Add(1);
            valores.Add(-1);
            laser.rotacion = 0;//numeroRandom<int>(valores);
            laser.traslacion = 1;
            TgcBoundingBox bb = ((TgcMesh)laser.objeto).BoundingBox;

            bb.scaleTranslate(laser.getPosicion(), new Vector3(0.1F, 0.1F, 0.15F));
            TgcObb obb = TgcObb.computeFromAABB(bb);
             
            Vector3 prueba = new Vector3(ejes.rotor.X, ejes.rotor.Y, ejes.rotor.Z);
            obb.setRotation(prueba);
            laser.setColision(new ColisionLaser());
            laser.getColision().setBoundingBox(obb);


            /*
            //CALCULO ROTACION LASER EN EJE X
            Vector3 versorZ = new Vector3();
            versorZ = laser.getEjes().vectorZ;
            versorZ.X = 0;
            //calculo angulo entre versorZ y el eje Z
            float angX = FastMath.Acos(versorZ.Z / (FastMath.Sqrt((FastMath.Pow2(versorZ.X))) + FastMath.Sqrt((FastMath.Pow2(versorZ.Y))) + FastMath.Sqrt((FastMath.Pow2(versorZ.Z + 1)))));
            Matrix rotacionX = new Matrix();
            rotacionX.RotateX(angX);
            //bb.transform(rotacionX);
             */

            //((TgcObb)laser.getColision().getBoundingBox()).setRotation(new Vector3 (1,0,0));
            //laser.getColision().transladar(ejes.getCentro());

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
        public float getVelocidad() { return 5; }
    }
    public class AsteroideGrande : FormatoAsteroide
    {
        private float masa = 800; 
        private float longitud = 6;
        public float getMasa() { return masa; }
        public Vector3 getVolumen() { return new Vector3(longitud, longitud, longitud); }
        public float getVelocidad() { return 7; }
    }
    public class AsteroideMediano : FormatoAsteroide
    {
        private float masa = 500; 
        private float longitud = 3;
        public float getMasa() { return masa; }
        public Vector3 getVolumen() { return new Vector3(longitud, longitud, longitud); }
        public float getVelocidad() { return 10; }
    }
    public class AsteroideChico : FormatoAsteroide
    {
        private float masa = 200;
        private float longitud =0.7f;
        public float getMasa() { return masa; }
        public Vector3 getVolumen() { return new Vector3(longitud, longitud, longitud); }
        public float getVelocidad() { return 14; }
    }
}

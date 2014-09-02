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
    class Factory
    {
        public Dibujable crearAsteroide(Vector3 tamanio)
        {
            //Carguemos el DirectX y la carpeta de media
            Device d3dDevice = GuiController.Instance.D3dDevice;
            string alumnoMediaFolder = GuiController.Instance.AlumnoEjemplosMediaDir; 

            //Creemos la mesh
            TgcSceneLoader loader = new TgcSceneLoader();
            TgcScene scene = loader.loadSceneFromFile(GuiController.Instance.AlumnoEjemplosMediaDir + "Asteroide\\esferita-TgcScene.xml");
            TgcMesh mesh_asteroide = scene.Meshes[0];
            mesh_asteroide.AutoTransformEnable = false;
            mesh_asteroide.Transform = Matrix.Scaling(tamanio);
            //Creamos su bounding Sphere
            mesh_asteroide.AutoUpdateBoundingBox = false;
            TgcBoundingSphere bounding_asteroide = new TgcBoundingSphere(mesh_asteroide.BoundingBox.calculateBoxCenter(), mesh_asteroide.BoundingBox.calculateAxisRadius().X);
           
            //Cargamos las cosas en el dibujable
            Dibujable asteroide = new Dibujable();
            asteroide.objeto = mesh_asteroide;
            asteroide.setColision(new ColisionAsteroide());
            asteroide.getColision().setBoundingBox(bounding_asteroide);

            return asteroide;
        }
        public void trasladar(Dibujable asteroide, Vector3 vector)
        {
            Matrix traslacion = Matrix.Translation(vector);
            ((TgcMesh)asteroide.objeto).Transform *= traslacion;
            asteroide.getColision().transladar(vector);
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
        public Dibujable crearLaser() 
        {
            //Carguemos el DirectX y la carpeta de media
            Device d3dDevice = GuiController.Instance.D3dDevice;
            string alumnoMediaFolder = GuiController.Instance.AlumnoEjemplosMediaDir;
            //Creemos la mesh
            TgcSceneLoader loader = new TgcSceneLoader();
            TgcScene scene = loader.loadSceneFromFile(GuiController.Instance.AlumnoEjemplosMediaDir + "Laser\\Laser_Box-TgcScene.xml");
            TgcMesh mesh_laser = scene.Meshes[0];
            mesh_laser.AutoTransformEnable = false;
            mesh_laser.Transform = Matrix.Scaling(new Vector3(0.1F, 0.1F, 1F));
            //Cargamos las cosas en el dibujable
            Dibujable laser = new Dibujable();
            laser.objeto = mesh_laser;
            return laser;

        }
    }
}

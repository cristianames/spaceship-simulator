using AlumnoEjemplos.TheGRID.Camara;
using AlumnoEjemplos.TheGRID.Shaders;
using Microsoft.DirectX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TgcViewer;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSceneLoader;

namespace AlumnoEjemplos.TheGRID.InterfazGrafica
{
    class AlternativeDimension
    {
        string alumnoMediaFolder = GuiController.Instance.AlumnoEjemplosMediaDir;
        SuperRender shader;
        TgcFrustum currentFrustrum;
        Nave ppal;
        Dibujable sol;
        SkySphere skySphere;
        //CambioCamara camara;
        List<Dibujable> dibujableCollection = new List<Dibujable>();
        List<IRenderObject> objectosNoMeshesCollection = new List<IRenderObject>();
        List<TgcMesh> objetosBrillantes = new List<TgcMesh>();
        public AlternativeDimension()
        {
            shader = EjemploAlumno.workspace().Shader;
            init();
        }
        public void init()
        {
            currentFrustrum = new TgcFrustum(); 
            ppal = new Nave();
            skySphere = new SkySphere("SkyBox\\skysphere2-TgcScene.xml");
            sol = crearSol();
            dibujableCollection.Add(skySphere.dibujable_skySphere);
            ppal.rotarPorVectorDeAngulos(Factory.VectorRandom(-50, 50));
            GuiController.Instance.RotCamera.CameraDistance = 75;
            //camara = new CambioCamara(ppal);

        }
        public void render(float elapsedTime)
        {
            currentFrustrum.updateMesh(GuiController.Instance.CurrentCamera.getPosition(), GuiController.Instance.CurrentCamera.getLookAt());
            skySphere.render(ppal);
            shader.render(ppal, sol, dibujableCollection, objectosNoMeshesCollection, objetosBrillantes);
        }
        private Dibujable crearSol()
        {
            TgcMesh mesh_Sol = Factory.cargarMesh(@"Sol\sol-TgcScene.xml");
            sol = new Dibujable();
            sol.setObject(mesh_Sol, 0, 200, new Vector3(2F, 2F, 2F));
            sol.giro = -1;
            sol.ubicarEnUnaPosicion(new Vector3(0, 0, 9000));
            sol.activar();
            return sol;
        }
    }
}

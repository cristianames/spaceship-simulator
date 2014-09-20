using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using TgcViewer;
using TgcViewer.Utils.Shaders;
using TgcViewer.Utils.TgcSceneLoader;

namespace AlumnoEjemplos.TheGRID.Shaders
{
    public class ShaderDefault : ShaderInterface
    {
        Effect effect;
        TgcScene scene;
        TgcMesh mesh;
        float time;
        private string ShaderDirectory;
        private string technique;


        public ShaderDefault()
        {
            ShaderDirectory = GuiController.Instance.AlumnoEjemplosMediaDir + "\\TheGrid\\Shaders\\";
            effect = TgcShaders.loadEffect(ShaderDirectory + "\\BasicShader.fx");
            technique = "RenderScene";
        }
        public void setShader(Dibujable dibujable)
        {
            //Configurar Technique dentro del shader
            ((TgcMesh)dibujable.objeto).Effect = effect;
            ((TgcMesh)dibujable.objeto).Technique = technique;
        }

        public void shadear(TgcViewer.Utils.TgcSceneLoader.TgcMesh mesh, float elapsedTime)
        {
            Device device = GuiController.Instance.D3dDevice;

            time += elapsedTime;


            // Cargar variables de shader, por ejemplo el tiempo transcurrido.
            effect.SetValue("time", time);

            // dibujo la malla pp dicha
            mesh.render();
            //throw new NotImplementedException();
        }

        public void close()
        {
            effect.Dispose();
            //throw new NotImplementedException();
        }
    }
}

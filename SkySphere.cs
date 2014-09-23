using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.DirectX;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSceneLoader;

namespace AlumnoEjemplos.TheGRID
{
    public class SkySphere
    {
        public TgcMesh horizonteVision;

        public SkySphere()
        {
            horizonteVision = Factory.cargarMesh(@"TheGRID\SkyBox\skysphere-TgcScene.xml");
            horizonteVision.Position = new Vector3(0, 0, 0);
            horizonteVision.Scale = new Vector3(86, 86, 86);
        }

        internal void render()
        {
            horizonteVision.render();
        }

        public void actualizaPos(Vector3 pos)
        {
            horizonteVision.Position = pos;
        }
    }
}

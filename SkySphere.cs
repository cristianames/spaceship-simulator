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
        public TgcBoundingSphere bordeSky;

        public SkySphere()
        {
            //horizonteVision = Factory.cargarMesh(@"SkyBox\skysphere-2-TgcScene.xml");
            horizonteVision = Factory.cargarMesh("SkyBox\\skysphere-TgcScene.xml");
            horizonteVision.Position = new Vector3(0, 0, 0);
            horizonteVision.Scale = new Vector3(86, 86, 86);
            horizonteVision.Enabled = false;
            EjemploAlumno.addMesh(horizonteVision);
            bordeSky = new TgcBoundingSphere(new Vector3(0, 0, 0), 500);
        }

        internal void render(){
            actualizaPos(EjemploAlumno.workspace().ObjetoPrincipal.getPosicion());
            horizonteVision.render();
            bordeSky.render();
        }
        public void dispose(){ dispose(); }

        public void actualizaPos(Vector3 pos)
        {
            horizonteVision.Position = pos;
            bordeSky.setCenter(pos);
        }
        
    }
}

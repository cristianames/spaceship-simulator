using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.DirectX;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSceneLoader;
using AlumnoEjemplos.TheGRID.Colisiones;

namespace AlumnoEjemplos.TheGRID
{
    public class SkySphere
    {
        public TgcMesh horizonteVision;
        public TgcBoundingSphere bordeSky;
        public Dibujable dibujable_skySphere;

        public SkySphere()
        {
            horizonteVision = Factory.cargarMesh("SkyBox\\skysphere-TgcScene.xml");
            horizonteVision.Position = new Vector3(0, 0, 0);
            horizonteVision.Enabled = true;
            bordeSky = new TgcBoundingSphere(new Vector3(0, 0, 0), 9500);
            dibujable_skySphere = new Dibujable();
            dibujable_skySphere.setObject(horizonteVision, 0, 0, new Vector3(1, 1, 1));
            dibujable_skySphere.setColision(new ColisionAsteroide());
            dibujable_skySphere.getColision().setBoundingBox(bordeSky);
            dibujable_skySphere.valor = 1;
            EjemploAlumno.addMesh(dibujable_skySphere);
        }

        internal void render(){
            dibujable_skySphere.ubicarEnUnaPosicion(EjemploAlumno.workspace().ObjetoPrincipal.getPosicion());
        }
        public void dispose()
        {
            dibujable_skySphere.dispose();
        } 
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.DirectX;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSceneLoader;
using AlumnoEjemplos.THE_GRID.Colisiones;

namespace AlumnoEjemplos.THE_GRID
{
    public class SkySphere
    {
        public TgcMesh horizonteVision;
        public TgcBoundingSphere bordeSky; //Borde de la skysphere para desactivar los objetos fuera de la misma
        public Dibujable dibujable_skySphere;

        public SkySphere(string path)
        {
            horizonteVision = Factory.cargarMesh(path);
            horizonteVision.Position = new Vector3(0, 0, 0);
            horizonteVision.Enabled = true;
            bordeSky = new TgcBoundingSphere(new Vector3(0, 0, 0), 11000);
            dibujable_skySphere = new Dibujable();
            dibujable_skySphere.setObject(horizonteVision, 0, 0, new Vector3(1, 1, 1));
            dibujable_skySphere.setColision(new ColisionAsteroide());
            dibujable_skySphere.getColision().setBoundingBox(bordeSky);
            dibujable_skySphere.valor = 1;
        }

        internal void render(Dibujable ppal){
            dibujable_skySphere.ubicarEnUnaPosicion(ppal.getPosicion());
        }
        public void dispose()
        {
            dibujable_skySphere.dispose();
        } 
    }
}

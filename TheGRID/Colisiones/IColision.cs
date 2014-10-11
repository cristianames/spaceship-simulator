using Microsoft.DirectX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TgcViewer.Utils.TgcSceneLoader;
using TgcViewer.Utils.TgcGeometry;


namespace AlumnoEjemplos.TheGRID.Colisiones
{
    public interface IColision
    {
        IRenderObject getBoundingBox();
        void setBoundingBox(IRenderObject bb);
        void render();
        void transladar(Vector3 posicion);
        void rotar(Vector3 rotacion); //Solo implementada en nave
        void escalar(Vector3 tam);
        bool colisiono(TgcObb objeto);
        bool colisiono(TgcBoundingSphere objeto);
        
    }
}

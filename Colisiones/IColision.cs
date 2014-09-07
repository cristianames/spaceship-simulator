using Microsoft.DirectX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TgcViewer.Utils.TgcSceneLoader;

namespace AlumnoEjemplos.TheGRID.Colisiones
{
    public interface IColision
    {
        IRenderObject getBoundingBox();
        void setBoundingBox(IRenderObject bb);
        void render();
        void transladar(Vector3 posicion);
        void rotar(); //Not Implemented
        void escalar(Vector3 tam);
    }
}

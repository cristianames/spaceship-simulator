    using Microsoft.DirectX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TgcViewer.Utils.TgcSceneLoader;
using TgcViewer.Utils.TgcGeometry;


namespace AlumnoEjemplos.THE_GRID.Colisiones
{
    public interface IColision
    {
        /* Esta es la interfaz para tratar el modulo de colisiones
         * donde se trabaja con las boundings. 
         * Tiene la particularidad de ser medianamente polimorfico,
         * ya que los chequeos entre boundings NO son polimorficos
         */
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

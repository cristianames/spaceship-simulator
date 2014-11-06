using Microsoft.DirectX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSceneLoader;

namespace AlumnoEjemplos.THE_GRID.Colisiones
{
    class ColisionNave : IColision
    {
        private TgcObb Obb; //Como es una nave medianamente cuadrada, la implementamos con una Obb para simplificar el chequeo
        //Para ver notas sobre las Colisiones, ir a la interfaz de donde extiende
        public IRenderObject getBoundingBox()
        {
            return this.Obb;
        }

        public void setBoundingBox(IRenderObject bb)
        {
            this.Obb = (TgcObb)bb;
        }

        public void transladar(Vector3 posicion)
        {
            this.Obb.move(posicion);
        }

        public void render()
        {
            Obb.render();
        }

        public void rotar(Vector3 rotacion) 
        {
            Obb.rotate(rotacion);
        }
        public void escalar(Vector3 tamanio)
        {
            ; //La nave siempre se mantiene del mismo tamaño
        }

        public bool colisiono(TgcBoundingSphere objeto) 
        {
           return TgcCollisionUtils.testSphereOBB(objeto,this.Obb);
            
        }

        public bool colisiono(TgcObb objeto)
        {
            return TgcCollisionUtils.testObbObb(objeto, this.Obb);

        }
    }
}

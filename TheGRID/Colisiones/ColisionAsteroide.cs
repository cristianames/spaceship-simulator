using Microsoft.DirectX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSceneLoader;

namespace AlumnoEjemplos.TheGRID.Colisiones
{
    class ColisionAsteroide: IColision
    {
        private TgcBoundingSphere bounding_sphere;


        public IRenderObject getBoundingBox()
        {
            return this.bounding_sphere;
        }

        public void setBoundingBox(IRenderObject bb)
        {
            this.bounding_sphere = (TgcBoundingSphere) bb;
        }

        public void transladar(Vector3 posicion)
        {
            this.bounding_sphere.moveCenter(posicion);
        }

        public void render(){
            bounding_sphere.render();
        }


        public void rotar(Vector3 rotacion) { ; }
        public void escalar(Vector3 tamanio)
        {
            this.bounding_sphere.setValues(this.bounding_sphere.Center, tamanio.Length());
        }

        public bool colisiono(TgcBoundingSphere objeto)
        {
            return TgcCollisionUtils.testSphereSphere(this.bounding_sphere,objeto);

        }

        public bool colisiono(TgcObb objeto)
        {
            return TgcCollisionUtils.testSphereOBB(this.bounding_sphere,objeto);

        }
    }
}

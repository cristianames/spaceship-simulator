using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.DirectX;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSceneLoader;
using AlumnoEjemplos.TheGRID.Explosiones;
using AlumnoEjemplos.TheGRID.Colisiones;
using Microsoft.DirectX.Direct3D;

namespace AlumnoEjemplos.TheGRID
{
    public class Nave : Dibujable
    {
        public Nave() : base() 
        {
            TgcMesh meshNave = Factory.cargarMesh("Nave\\naveTrooper-TgcScene.xml");
            meshNave.Transform *= Matrix.RotationY(Geometry.DegreeToRadian(180));
            setObject(meshNave, 100, 50, new Vector3(0.5f, 0.5f, 0.5f));
            setFisica(50, 100, 200, 10000);
            SetPropiedades(true, false, false);
            explosion = new ExplosionNave(this, 100, 200);

            //Cargamos su BB
            TgcBoundingBox naveBb = ((TgcMesh)objeto).BoundingBox;
            naveBb.scaleTranslate(new Vector3(0, 0, 0), new Vector3(0.5f, 0.5f, 0.5f));
            TgcObb naveObb = TgcObb.computeFromAABB(naveBb);
            foreach (var vRotor in getEjes().lRotor) { naveObb.rotate(vRotor); } //Le aplicamos TODAS las rotaciones que hasta ahora lleva la nave.
            setColision(new ColisionNave());
            getColision().setBoundingBox(naveObb);
            //nave.getColision().transladar(posicionNave);
        }

        private float vida;

        public override void teChoque(Dibujable colisionador, float velocidadColisionador)
        {
            //if (vida <= 0) Explosion.explosionNave(this);
            //else restaVida(colisionador);
        }

        private void restaVida(Dibujable colisionador)
        {
            //throw new NotImplementedException();
        }

    }
}

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
using AlumnoEjemplos.TheGRID.Helpers;

namespace AlumnoEjemplos.TheGRID
{
    public class Nave : Dibujable
    {
        public Nave() : base() 
        {
            TgcTexture normalMap = TgcTexture.createTexture(EjemploAlumno.TG_Folder + "Nave\\Textures\\Bump.jpg");
            TgcTexture[] normalMapArray = new TgcTexture[] { normalMap };
            TgcMesh meshNave_base =  Factory.cargarMesh("Nave\\naveTrooper-TgcScene.xml");
            TgcMeshBumpMapping meshNave = TgcMeshBumpMapping.fromTgcMesh(meshNave_base, normalMapArray);
            acelNormal = 50;
            acelBlur = 500;
            velMaxNormal = 200;
            velMaxBlur = 3000;
            meshNave.Transform *= Matrix.RotationY(Geometry.DegreeToRadian(180));
            setObject(meshNave, 100, 50, new Vector3(0.5f, 0.5f, 0.5f));
            setFisica(acelNormal, 100, velMaxNormal, 10000);
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

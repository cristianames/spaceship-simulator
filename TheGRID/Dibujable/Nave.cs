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
        public float acelNormal;//Estos atributos se setean en las clases hijas con los valores correspondientes
        public float velMaxNormal;
        public float rotNormal;
        public float velMaxBlur;
        public float rotBlur;

        public Nave() : base() 
        {
            TgcTexture normalMap = TgcTexture.createTexture(EjemploAlumno.TG_Folder + "Nave\\Textures\\Bump.jpg");
            TgcTexture[] normalMapArray = new TgcTexture[] { normalMap };
            TgcMesh meshNave_base =  Factory.cargarMesh("Nave\\naveTrooper-TgcScene.xml");
            TgcMeshBumpMapping meshNave = TgcMeshBumpMapping.fromTgcMesh(meshNave_base, normalMapArray);
            acelNormal = 600;
            velMaxNormal = 2500;
            rotNormal = 50;
            velMaxBlur = 300000;
            rotBlur = 5;

            meshNave.Transform *= Matrix.RotationY(Geometry.DegreeToRadian(180));
            setObject(meshNave, 100, 50, new Vector3(0.5f, 0.5f, 0.5f));
            setFisica(acelNormal, 1000, velMaxNormal, 100000);
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


        public override void teChoque(Dibujable colisionador, float velocidadColisionador)
        {
            //if (vida <= 0) Explosion.explosionNave(this);
            //else restaVida(colisionador);
        }

        private void restaVida(Dibujable colisionador)
        {
            //throw new NotImplementedException();
        }

        public void setearAceleracion(float acel)
        {
            fisica.aceleracion = acel;
        }

        public Vector3 puntoLuzIzq()
        {
            /*
            Vector3 posNave = getPosicion();
            Vector3 desp_X = Vector3.Multiply(getDireccion_X(),9);
            Vector3 desp_Z = Vector3.Multiply(getDireccion(),15);
            Vector3 desp_Y = Vector3.Multiply(getDireccion_Y(),2);
            return Vector3.Add(Vector3.Add(Vector3.Add(posNave, desp_X), desp_Y), desp_Z);
             */
            //Ahora la version mas optimizada y menos descriptiva.
            return Vector3.Add(Vector3.Add(Vector3.Add(getPosicion(), Vector3.Multiply(getDireccion_X(), -9.7f)),
                Vector3.Multiply(getDireccion_Y(), 0)), Vector3.Multiply(getDireccion(), 6));
        }
        public Vector3 puntoLuzDer()
        {
            return Vector3.Add(Vector3.Add(Vector3.Add(getPosicion(), Vector3.Multiply(getDireccion_X(), 9.7f)),
                Vector3.Multiply(getDireccion_Y(), 0)), Vector3.Multiply(getDireccion(), 6));
        }
        public Vector3 puntoLuzCent()
        {
            return Vector3.Add(Vector3.Add(Vector3.Add(getPosicion(), Vector3.Multiply(getDireccion_X(), 0)),
                Vector3.Multiply(getDireccion_Y(), -1)), Vector3.Multiply(getDireccion(), 48));
        }
        public Vector3 dirLuzIzq()
        {
            //return Vector3.Add(puntoLuzIzq(), Vector3.Multiply(getDireccion(), 50)) - getPosicion();
            return Vector3.Subtract(puntoLuzIzq(), getPosicion());
        }
        public Vector3 dirLuzDer()
        {
            //return Vector3.Add(puntoLuzDer(), Vector3.Multiply(getDireccion(), 50)) - getPosicion();
            return Vector3.Subtract(puntoLuzDer(), getPosicion());
        }        //Ya estan los puntos de las luces. Para obtener la direccion de la luz usar el metodo getDireccion();
    }
}

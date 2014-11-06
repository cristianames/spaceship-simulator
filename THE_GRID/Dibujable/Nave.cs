using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.DirectX;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSceneLoader;
using AlumnoEjemplos.THE_GRID.Explosiones;
using AlumnoEjemplos.THE_GRID.Colisiones;
using Microsoft.DirectX.Direct3D;
using AlumnoEjemplos.THE_GRID.Helpers;

namespace AlumnoEjemplos.THE_GRID
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
            setObject(meshNave, 800, 50, new Vector3(0.5f, 0.5f, 0.5f));
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
        }

        float max_eje = 1000000000000;
        public bool reajustarSiSuperoLimite()
        {
            Vector3 pos = getPosicion();
            bool flag = false;
            if (Math.Pow(pos.X, 2).CompareTo(max_eje) > 0)
                flag = true;
            if (Math.Pow(pos.Y, 2).CompareTo(max_eje) > 0)
                flag = true;
            if (Math.Pow(pos.Z, 2).CompareTo(max_eje) > 0)
                flag = true;

            if (flag) ubicarEnUnaPosicion(new Vector3(0, 0, 0));//Chequeamos que no nos hayamos pasado del mapa
            return flag;
        }
        public override void teChoque(Dibujable colisionador, float velocidadColisionador)
        {
            //Aqui se bajaria la vida de la nave o explotaria, pero para mayor experiencia, lo removimos asi la nave no muere.
            //if (vida <= 0) Explosion.explosionNave(this);
            //else restaVida(colisionador);
        }

        private void restaVida(Dibujable colisionador)
        {
            //Idem arriba
        }

        public void setearAceleracion(float acel)
        {
            fisica.aceleracion = acel;
        }
        //Luces de la nave
        public Vector3 puntoLuzIzq() //Posicion de la luz izquierda de la nave
        {
            return Vector3.Add(Vector3.Add(Vector3.Add(getPosicion(), Vector3.Multiply(getDireccion_X(), -9.7f)),
                Vector3.Multiply(getDireccion_Y(), 0)), Vector3.Multiply(getDireccion(), 6));
        }
        public Vector3 puntoLuzDer() //Posicion de la luz derecha de la nave
        {
            return Vector3.Add(Vector3.Add(Vector3.Add(getPosicion(), Vector3.Multiply(getDireccion_X(), 9.7f)),
                Vector3.Multiply(getDireccion_Y(), 0)), Vector3.Multiply(getDireccion(), 6));
        }
        public Vector3 puntoLuzCent() //Posicion de la luz frontal de la nave
        {
            return Vector3.Add(Vector3.Add(Vector3.Add(getPosicion(), Vector3.Multiply(getDireccion_X(), 0)),
                Vector3.Multiply(getDireccion_Y(), -1)), Vector3.Multiply(getDireccion(), 48));
        }
        public Vector3 dirLuzIzq() //Direccion de la luz izquierda de la nave
        {
            return Vector3.Subtract(puntoLuzIzq(), getPosicion());
        }
        public Vector3 dirLuzDer() //Posicion de la luz derecha de la nave
        {
            return Vector3.Subtract(puntoLuzDer(), getPosicion());
        }
    }
}

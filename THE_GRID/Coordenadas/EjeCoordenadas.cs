using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlumnoEjemplos.THE_GRID
{
    public class EjeCoordenadas
    {
        /* Esta clase funciona como un eje de coordenadas relativo
         * para trabajar las rotaciones en funcion a un pivote.
         * Ademas permite una rotacion mejorada en funcion de los desplazamientos,
         * no de los angulos resplazados.
         */ 
        public Vector3 vectorX;
        public Vector3 vectorY;
        public Vector3 vectorZ;
        public Vector3 vectorK;
        public Matrix mRotor { get; set; }          //Matriz de todas las rotaciones hasta el momento, sin las traslaciones del cuerpo al (0,0,0).
        public List<Vector3> lRotor { get; set; }       //Lista de todas las rotaciones que se fueron haciendo. (Las BB no usan matriz de rotacion sino que un vector... Solo por eso.)

        private Vector3 centroObjeto;

        public EjeCoordenadas()
        {
            vectorX = new Vector3(1, 0, 0);
            vectorY = new Vector3(0, 1, 0);
            vectorZ = new Vector3(0, 0, 1);
            vectorK = new Vector3(0, 0, 1);
            centroObjeto = new Vector3(0, 0, 0);
            mRotor = Matrix.Identity;
            lRotor = new List<Vector3>();
        }
        public void actualizarX() { vectorX = Vector3.Cross(vectorY, vectorZ); }

        public void centrar(float x, float y, float z) { centroObjeto = new Vector3(x, y, z); }
        public void centrar(Vector3 centro) { centroObjeto = centro; }

        public Vector3 getCentro() 
        {
            Vector3 temp = centroObjeto;
            temp.Multiply(-1);
            return temp; 
        }

        public Matrix calculoRotarManualmente(Vector3 posActual, Vector3 rotacion, ref Vector3 rotacionActual)
        {
            rotacion.X = Geometry.DegreeToRadian(rotacion.X);
            rotacion.Y = Geometry.DegreeToRadian(rotacion.Y);
            rotacion.Z = Geometry.DegreeToRadian(rotacion.Z);
            rotacionActual = rotacion;
            lRotor.Add(rotacionActual);
            rotation = Matrix.RotationYawPitchRoll(rotacion.Y, rotacion.X, rotacion.Z);

            normal4 = Vector3.Transform(vectorZ, rotation);
            vectorK = vectorZ;
            vectorZ = new Vector3(normal4.X, normal4.Y, normal4.Z);

            normal4 = Vector3.Transform(vectorY, rotation);
            vectorY = new Vector3(normal4.X, normal4.Y, normal4.Z);

            mRotor *= rotation;

            actualizarX();

            rototraslation = Matrix.Translation(centroObjeto - posActual);
            rototraslation *= rotation;
            rototraslation *= Matrix.Translation(posActual - centroObjeto);

            return rototraslation;
        }

        Matrix rotation, rototraslation;
        Vector4 normal4;
        public Matrix calculoRotarX_desde(Vector3 posActual, float grados, ref Vector3 rotacionActual)
        {
            float angulo = Geometry.DegreeToRadian(grados);
            rotacionActual = new Vector3(vectorX.X * angulo, vectorX.Y * angulo, vectorX.Z * angulo);
            lRotor.Add(rotacionActual);
            rotation = Matrix.RotationYawPitchRoll(vectorX.Y * angulo, vectorX.X * angulo, vectorX.Z * angulo);
            
            normal4 = Vector3.Transform(vectorZ, rotation);
            vectorK = vectorZ;
            vectorZ = new Vector3(normal4.X, normal4.Y, normal4.Z);

            normal4 = Vector3.Transform(vectorY, rotation);
            vectorY = new Vector3(normal4.X, normal4.Y, normal4.Z);

            mRotor *= rotation;

            actualizarX();

            rototraslation = Matrix.Translation(centroObjeto - posActual);
            rototraslation *= rotation;
            rototraslation *= Matrix.Translation(posActual - centroObjeto);

            return rototraslation;
        }

        public Matrix calculoRotarY_desde(Vector3 posActual, float grados, ref Vector3 rotacionActual)
        {
            float angulo = Geometry.DegreeToRadian(grados);
            rotacionActual = new Vector3(vectorY.X * angulo, vectorY.Y * angulo, vectorY.Z * angulo);
            lRotor.Add(rotacionActual);           
            rotation = Matrix.RotationYawPitchRoll(vectorY.Y * angulo, vectorY.X * angulo, vectorY.Z * angulo);

            normal4 = Vector3.Transform(vectorY, rotation);
            vectorY = new Vector3(normal4.X, normal4.Y, normal4.Z);

            normal4 = Vector3.Transform(vectorZ, rotation);
            vectorK = vectorZ;
            vectorZ = new Vector3(normal4.X, normal4.Y, normal4.Z);

            mRotor *= rotation;

            actualizarX();

            rototraslation = Matrix.Translation(centroObjeto - posActual);
            rototraslation *= rotation;
            rototraslation *= Matrix.Translation(posActual - centroObjeto);

            return rototraslation;
        }

        public Matrix calculoRotarZ_desde(Vector3 posActual, float grados, ref Vector3 rotacionActual)
        {
            float angulo = Geometry.DegreeToRadian(grados);
            rotacionActual = new Vector3(vectorZ.X * angulo, vectorZ.Y * angulo, vectorZ.Z * angulo);
            lRotor.Add(rotacionActual);
            rotation = Matrix.RotationYawPitchRoll(vectorZ.Y * angulo, vectorZ.X * angulo, vectorZ.Z * angulo);
            
            normal4 = Vector3.Transform(vectorY, rotation);
            vectorY = new Vector3(normal4.X, normal4.Y, normal4.Z);

            normal4 = Vector3.Transform(vectorZ, rotation);
            vectorK = vectorZ;
            vectorZ = new Vector3(normal4.X, normal4.Y, normal4.Z);

            mRotor *= rotation;

            actualizarX(); 
            
            rototraslation = Matrix.Translation(centroObjeto - posActual);
            rototraslation *= rotation;
            rototraslation *= Matrix.Translation(posActual - centroObjeto);

            return rototraslation;
        }
        //Ejes del sistema de coordenadas
        public Vector3 direccion() { return vectorZ; }
        public Vector3 direccion_Y() { return vectorY; }
        public Vector3 direccion_X() { return vectorX; }
        public Vector3 direccionAnterior() { return vectorK; } //Eje Z anterior a la rotacion para poder determinar cuanto se roto.

    }
}

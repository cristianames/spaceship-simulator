using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlumnoEjemplos.TheGRID
{
    class EjeCoordenadas
    {
        public Vector3 vectorX;
        public Vector3 vectorY;
        public Vector3 vectorZ;
        public Vector3 vectorK;

        public Vector3 centroObjeto;

        public EjeCoordenadas()
        {
            vectorX = new Vector3(1, 0, 0);
            vectorY = new Vector3(0, 1, 0);
            vectorZ = new Vector3(0, 0, 1);

            vectorK = new Vector3(0, 0, 1);

            centroObjeto = new Vector3(0, 0, 0);
        }
        public void actualizarX() { vectorX = Vector3.Cross(vectorY, vectorZ); }

        public void centrar(float x, float y, float z) { centroObjeto = new Vector3(x, y, z); }

        public Vector3 getCentro() 
        {
            Vector3 temp = centroObjeto;
            temp.Multiply(-1);
            return temp; 
        }

        Matrix rotation, rototraslation;
        Vector4 normal4;
        public Matrix rotarX_desde(Vector3 posActual, float grados)
        {
            float angulo = Geometry.DegreeToRadian(grados);
            rotation = Matrix.RotationYawPitchRoll(vectorX.Y * angulo, vectorX.X * angulo, vectorX.Z * angulo);
            
            normal4 = Vector3.Transform(vectorZ, rotation);
            vectorK = vectorZ;
            vectorZ = new Vector3(normal4.X, normal4.Y, normal4.Z);

            normal4 = Vector3.Transform(vectorY, rotation);
            vectorY = new Vector3(normal4.X, normal4.Y, normal4.Z);

            actualizarX();

            rototraslation = Matrix.Translation(centroObjeto - posActual);
            rototraslation *= rotation;
            rototraslation *= Matrix.Translation(posActual - centroObjeto);

            return rototraslation;
        }

        public Matrix rotarY_desde(Vector3 posActual, float grados)
        {
            float angulo = Geometry.DegreeToRadian(grados);
            rotation = Matrix.RotationYawPitchRoll(vectorY.Y * angulo, vectorY.X * angulo, vectorY.Z * angulo);

            normal4 = Vector3.Transform(vectorY, rotation);
            vectorY = new Vector3(normal4.X, normal4.Y, normal4.Z);

            normal4 = Vector3.Transform(vectorZ, rotation);
            vectorK = vectorZ;
            vectorZ = new Vector3(normal4.X, normal4.Y, normal4.Z);

            actualizarX();

            rototraslation = Matrix.Translation(centroObjeto - posActual);
            rototraslation *= rotation;
            rototraslation *= Matrix.Translation(posActual - centroObjeto);

            return rototraslation;
        }

        public Matrix rotarZ_desde(Vector3 posActual, float grados)
        {
            float angulo = Geometry.DegreeToRadian(grados);
            rotation = Matrix.RotationYawPitchRoll(vectorZ.Y * angulo, vectorZ.X * angulo, vectorZ.Z * angulo);
            
            normal4 = Vector3.Transform(vectorY, rotation);
            vectorY = new Vector3(normal4.X, normal4.Y, normal4.Z);

            normal4 = Vector3.Transform(vectorZ, rotation);
            vectorK = vectorZ;
            vectorZ = new Vector3(normal4.X, normal4.Y, normal4.Z);

            actualizarX(); 
            
            rototraslation = Matrix.Translation(centroObjeto - posActual);
            rototraslation *= rotation;
            rototraslation *= Matrix.Translation(posActual - centroObjeto);

            return rototraslation;
        }
        //De la subclase
        public Vector3 direccion() { return vectorZ; }
        public Vector3 direccion_Y() { return vectorY; }
        public Vector3 direccion_X() { return vectorX; }
        public Vector3 direccionAnterior() { return vectorK; }

    }
}

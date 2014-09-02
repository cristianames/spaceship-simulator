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

        public Vector3 centroObjeto;

        public EjeCoordenadas()
        {
            vectorX = new Vector3(1, 0, 0);
            vectorY = new Vector3(0, 1, 0);
            vectorZ = new Vector3(0, 0, 1);

            centroObjeto = new Vector3(0, 0, 0);
        }
        public void actualizarX() { vectorX = Vector3.Cross(vectorY, vectorZ); }

        public void centrar(float x, float y, float z) { centroObjeto = new Vector3(x, y, z); }

        Matrix rotation, rototranslation;
        Vector4 normal4;
        public Matrix rotarX_desde(Vector3 posActual, float grados)
        {
            float angulo = Geometry.DegreeToRadian(grados);
            rotation = Matrix.RotationYawPitchRoll(vectorX.Y * angulo, vectorX.X * angulo, vectorX.Z * angulo);
            
            normal4 = Vector3.Transform(vectorZ, rotation);
            vectorZ = new Vector3(normal4.X, normal4.Y, normal4.Z);

            normal4 = Vector3.Transform(vectorY, rotation);
            vectorY = new Vector3(normal4.X, normal4.Y, normal4.Z);

            actualizarX();

            rototranslation = Matrix.Translation(centroObjeto - posActual);
            rototranslation *= rotation;
            rototranslation *= Matrix.Translation(posActual - centroObjeto);

            return rototranslation;
        }

        public Matrix rotarY_desde(Vector3 posActual, float grados)
        {
            float angulo = Geometry.DegreeToRadian(grados);
            rotation = Matrix.RotationYawPitchRoll(vectorY.Y * angulo, vectorY.X * angulo, vectorY.Z * angulo);

            normal4 = Vector3.Transform(vectorY, rotation);
            vectorY = new Vector3(normal4.X, normal4.Y, normal4.Z);

            normal4 = Vector3.Transform(vectorZ, rotation);
            vectorZ = new Vector3(normal4.X, normal4.Y, normal4.Z);

            actualizarX();

            rototranslation = Matrix.Translation(centroObjeto - posActual);
            rototranslation *= rotation;
            rototranslation *= Matrix.Translation(posActual - centroObjeto);

            return rototranslation;
        }

        public Matrix rotarZ_desde(Vector3 posActual, float grados)
        {
            float angulo = Geometry.DegreeToRadian(grados);
            rotation = Matrix.RotationYawPitchRoll(vectorZ.Y * angulo, vectorZ.X * angulo, vectorZ.Z * angulo);
            
            normal4 = Vector3.Transform(vectorY, rotation);
            vectorY = new Vector3(normal4.X, normal4.Y, normal4.Z);

            normal4 = Vector3.Transform(vectorZ, rotation);
            vectorZ = new Vector3(normal4.X, normal4.Y, normal4.Z);

            actualizarX(); 
            
            rototranslation = Matrix.Translation(centroObjeto - posActual);
            rototranslation *= rotation;
            rototranslation *= Matrix.Translation(posActual - centroObjeto);

            return rototranslation;
        }
        //De la subclase
        public Vector3 direccion()
        {
            return vectorZ;
        }
    }
}

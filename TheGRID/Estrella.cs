using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Microsoft.DirectX;
using TgcViewer.Utils.TgcGeometry;
using AlumnoEjemplos.TheGRID.Explosiones;
using TgcViewer;
using TgcViewer.Utils.TgcSceneLoader;
using Microsoft.DirectX.Direct3D;

namespace AlumnoEjemplos.TheGRID
{
    public class Estrella : TgcMesh
    {
        public bool recur;
        public int cuadrante;

        public void insertarEstrellas(List<TgcMesh> estrellas, List<TgcMesh> estrellasUsadas, Vector3 posicionNave, Vector3 direccionNave,Vector3 movNave,Matrix rotacion, float elapsedTime)
        {
            int a, b, c;
            Vector3 generatriz;
            generatriz = posicionNave + 3000 * direccionNave*elapsedTime;
            Vector3 posicionEstrella;
            int j;
            for (j = 0; j < 50; j++)
            {
                switch (cuadrante)
                {
                    case 1: a = 1; b = 1; break;
                    case 2: a = 1; b = -1; break;
                    case 3: a = -1; b = -1; break;
                    case 4: a = -1; b = 1; break;
                    default: a = 1; b = 1; break;
                }
                if (cuadrante < 4) cuadrante++;
                else cuadrante = 1;
                if (Factory.numeroRandom((int)posicionNave.Z) % 2 == 1) c = 1;
                else c = -1;
                //Genera la posicion al azar
                posicionEstrella.X = (generatriz.X + a * (Factory.numeroRandom((int)posicionNave.Z) % 50));
                posicionEstrella.Y = (generatriz.Y + b * (Factory.numeroRandom((int)posicionNave.Y) % 50));
                posicionEstrella.Z = (generatriz.Z + c * (Factory.numeroRandom((int)posicionNave.X) % 500));
                    if (estrellas.Count != 0)
                    {
                        estrellas[0].Position = posicionEstrella;
                        estrellasUsadas.Add(estrellas[0]);
                        estrellas.Remove(estrellas[0]);
                        estrellas.Add(estrellasUsadas[0]);
                        estrellasUsadas.Remove(estrellasUsadas[0]);
                    }
            }
        }
    }

     
}

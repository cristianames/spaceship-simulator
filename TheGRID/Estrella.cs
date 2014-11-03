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
        float time;
        public bool recur;
        #region Constructor
       /* public Estrella(Vector3 origen) 
        {
            
            
            /*
            this.A = origen + new Vector3(10,0,0);
            this.B = origen + new Vector3(-10,0,0);
            this.A = origen + new Vector3(0, 10, 0);
            string currentTexurePah = GuiController.Instance.ExamplesMediaDir + "Texturas" + "\\" + "baldosaFacultad.jpg";
            Texture texture = TextureLoader.FromFile(d3dDevice, currentTexurePah);
            d3dDevice.SetTexture(0, texture);
           
            //Size = tamaño;
            //Normal = normal;
            //Color = Color.White;
        }*/
        #endregion

        public void insertarEstrellas(List<TgcMesh> estrellas, List<TgcMesh> estrellasUsadas, Vector3 posicionNave, Vector3 direccionNave, float elapsedTime)
        {

           
            int a, b, c;
            Vector3 generatriz;
            generatriz = posicionNave + 100000 * direccionNave*elapsedTime;
            Vector3 posicionEstrella;
            if (Factory.numeroRandom((int)posicionNave.X) % 2 > 0) a = 1;
            else a = -1;
            if (Factory.numeroRandom((int)posicionNave.Y) % 2 > 0) b = 1;
            else b = -1;
            if (Factory.numeroRandom((int)posicionNave.Z) % 2 > 0) c = 1;
            else c = -1;
            posicionEstrella.X = a * (generatriz.X + (Factory.numeroRandom((int)posicionNave.Z) % 100));
            posicionEstrella.Y = b * (generatriz.Y + (Factory.numeroRandom((int)posicionNave.Y) % 100));
            posicionEstrella.Z = c * (generatriz.Z + (Factory.numeroRandom((int)posicionNave.X) % 100));
            if (!recur)time = time + elapsedTime;
            recur = false;
            if (time > 0.01)
            {
                time = time - 0.01f;
                if (estrellas.Count != 0)
                {
                    estrellas[0].Position = posicionEstrella;
                    estrellasUsadas.Add(estrellas[0]);
                    estrellas.Remove(estrellas[0]);
                    estrellas.Add(estrellasUsadas[0]);
                    estrellasUsadas.Remove(estrellasUsadas[0]);

                }
                recur = true;
                insertarEstrellas(estrellas,estrellasUsadas,posicionNave,direccionNave,elapsedTime);
            }
            
          /*  else 
            {
                estrellasUsadas[0].Position = posicionEstrella;
                estrellas.Add(estrellasUsadas[0]);
                estrellasUsadas.Remove(estrellasUsadas[0]);
            }*/

        }
    }

     
}

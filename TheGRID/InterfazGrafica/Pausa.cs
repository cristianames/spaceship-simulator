using Microsoft.DirectX;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using TgcViewer;
using TgcViewer.Utils._2D;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSceneLoader;

namespace AlumnoEjemplos.TheGRID
{
    class Pausa
    {
        TgcSprite sprite;
        public Pausa()
        {
            //Crear Sprite
            sprite = new TgcSprite();
            sprite.Texture = TgcTexture.createTexture(EjemploAlumno.TG_Folder + "Sprites\\Pausa.png");

            //Ubicarlo centrado en la pantalla
            Size screenSize = GuiController.Instance.Panel3d.Size;
            Size textureSize = sprite.Texture.Size;
            sprite.Position = new Vector2(FastMath.Max(screenSize.Width / 2 - textureSize.Width / 2, 0), FastMath.Max(screenSize.Height / 2 - textureSize.Height / 2, 0));
            sprite.Scaling = new Vector2((float)screenSize.Width / (float)textureSize.Width, (float)screenSize.Height / (float)textureSize.Height);
            //sprite.Scaling = new Vector2(0.5f, 0.5f);
        }

        public void pausa()
        {
            //Iniciar dibujado de todos los Sprites de la escena (en este caso es solo uno)
            GuiController.Instance.Drawer2D.beginDrawSprite();
            //Dibujar sprite (si hubiese mas, deberian ir todos aquí)
            sprite.render();
            //Finalizar el dibujado de Sprites
            GuiController.Instance.Drawer2D.endDrawSprite();
        }
    }
}

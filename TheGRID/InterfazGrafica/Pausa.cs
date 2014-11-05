using Microsoft.DirectX;
using Microsoft.DirectX.DirectInput;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using TgcViewer;
using TgcViewer.Utils._2D;
using TgcViewer.Utils.Input;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSceneLoader;

namespace AlumnoEjemplos.TheGRID
{
    public class Pausa
    {
        TgcSprite sprite;
        TgcD3dInput input = GuiController.Instance.D3dInput;
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

        public void render()
        {
            if (input.keyPressed(Key.P))
            {
                EjemploAlumno.workspace().pausa = false;
                EjemploAlumno.workspace().music.playPauseBackgound();
            }

            GuiController.Instance.Drawer2D.beginDrawSprite();
            sprite.render();
            GuiController.Instance.Drawer2D.endDrawSprite();
        }
    }
}

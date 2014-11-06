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
        int posicionSlide = 0;
        TgcD3dInput input = GuiController.Instance.D3dInput;
        List<TgcSprite> papers = new List<TgcSprite>();
        public Pausa()
        {
            cargarDiapositivas();

            foreach (TgcSprite item in papers)
            {
                Size screenSize = GuiController.Instance.Panel3d.Size;
                Size textureSize = item.Texture.Size;
                item.Position = new Vector2(FastMath.Max(screenSize.Width / 2 - textureSize.Width / 2, 0), FastMath.Max(screenSize.Height / 2 - textureSize.Height / 2, 0));
                item.Scaling = new Vector2((float)screenSize.Width / (float)textureSize.Width, (float)screenSize.Height / (float)textureSize.Height);
            }
        }

        private void cargarDiapositivas()
        {
            TgcSprite sprite = new TgcSprite();
            sprite.Texture = TgcTexture.createTexture(EjemploAlumno.TG_Folder + "Sprites\\Tutorial\\Paper1.png");
            papers.Add(sprite);
            sprite = new TgcSprite();
            sprite.Texture = TgcTexture.createTexture(EjemploAlumno.TG_Folder + "Sprites\\Tutorial\\Paper2.png");
            papers.Add(sprite);
            sprite = new TgcSprite();
            sprite.Texture = TgcTexture.createTexture(EjemploAlumno.TG_Folder + "Sprites\\Tutorial\\Paper3.png");
            papers.Add(sprite);
            sprite = new TgcSprite();
            sprite.Texture = TgcTexture.createTexture(EjemploAlumno.TG_Folder + "Sprites\\Tutorial\\Paper4.png");
            papers.Add(sprite);
            sprite = new TgcSprite();
            sprite.Texture = TgcTexture.createTexture(EjemploAlumno.TG_Folder + "Sprites\\Tutorial\\Paper5.png");
            papers.Add(sprite);
            sprite = new TgcSprite();
            sprite.Texture = TgcTexture.createTexture(EjemploAlumno.TG_Folder + "Sprites\\Tutorial\\Paper6.png");
            papers.Add(sprite);
            sprite = new TgcSprite();
            sprite.Texture = TgcTexture.createTexture(EjemploAlumno.TG_Folder + "Sprites\\Tutorial\\Paper7.png");
            papers.Add(sprite);
            sprite = new TgcSprite();
            sprite.Texture = TgcTexture.createTexture(EjemploAlumno.TG_Folder + "Sprites\\Tutorial\\Paper8.png");
            papers.Add(sprite);
        }

        public void render()
        {
            if (input.keyPressed(Key.Return) || input.keyPressed(Key.Right))
            {
                if (posicionSlide >= 7)
                {
                    EjemploAlumno.workspace().pausa = false;
                    EjemploAlumno.workspace().music.playPauseBackgound();
                    posicionSlide = 0;
                    EjemploAlumno.workspace().music.playExitMenu();
                    return;
                }
                else
                {
                    posicionSlide++;
                    EjemploAlumno.workspace().music.playSlideButton();
                }
            }
            if (input.keyPressed(Key.Left)) { if (posicionSlide > 0) { posicionSlide--; EjemploAlumno.workspace().music.playSlideButton(); } }

            GuiController.Instance.Drawer2D.beginDrawSprite();
            papers[posicionSlide].render();
            GuiController.Instance.Drawer2D.endDrawSprite();
        }
    }
}

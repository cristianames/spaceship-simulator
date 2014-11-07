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

namespace AlumnoEjemplos.THE_GRID
{
    public class Ending
    {
        #region Atributos
        float desplazamiento;
        float offset_pantalla;
        public TgcSprite cris;
        public TgcSprite crisTareas;
        public TgcSprite eze;
        public TgcSprite ezeTareas;
        public TgcSprite dante;
        public TgcSprite danteTareas;
        public TgcSprite tomas;
        public TgcSprite tomasTareas;
        public int incremento = 0;
        public float timeSprite = 0;
        public bool credit1 = false;
        public bool credit2 = false;
        public bool credit3 = false;
        public bool credit4 = false;
        #endregion
        
        public Ending()
        {
            //Crear Sprite Cris
            cris = new TgcSprite();
            cris.Texture = TgcTexture.createTexture(EjemploAlumno.TG_Folder + "Sprites\\Cris.png");

            Size screenSize = GuiController.Instance.Panel3d.Size;
            Size textureSize = cris.Texture.Size;


            desplazamiento = textureSize.Width; //Desplazamiento fuera de la plantalla. Viene de que las imagenes de los sprites son de 640x400
            offset_pantalla = screenSize.Width * 0.01f; //El offset en la pantalla es el 10% de la pantalla total

            cris.Position = new Vector2(-desplazamiento, 0);

            //Crear Sprite CrisTareas
            crisTareas = new TgcSprite();
            crisTareas.Texture = TgcTexture.createTexture(EjemploAlumno.TG_Folder + "Sprites\\CrisTareas.png");
            crisTareas.Position = new Vector2(screenSize.Width, screenSize.Height - 200);

            //Crear Sprite Eze
            eze = new TgcSprite();
            eze.Texture = TgcTexture.createTexture(EjemploAlumno.TG_Folder + "Sprites\\Eze.png");
            eze.Position = new Vector2(screenSize.Width, 0);

            //Crear Sprite EzeTareas
            ezeTareas = new TgcSprite();
            ezeTareas.Texture = TgcTexture.createTexture(EjemploAlumno.TG_Folder + "Sprites\\EzeTareas.png");
            ezeTareas.Position = new Vector2(-desplazamiento, screenSize.Height - 200);

            //Crear Sprite Dante
            dante = new TgcSprite();
            dante.Texture = TgcTexture.createTexture(EjemploAlumno.TG_Folder + "Sprites\\Dante.png");
            dante.Position = new Vector2(-desplazamiento, 0);

            //Crear Sprite DanteTareas
            danteTareas = new TgcSprite();
            danteTareas.Texture = TgcTexture.createTexture(EjemploAlumno.TG_Folder + "Sprites\\DanteTareas.png");
            danteTareas.Position = new Vector2(screenSize.Width, screenSize.Height - 200);

            //Crear Sprite Tomas
            tomas = new TgcSprite();
            tomas.Texture = TgcTexture.createTexture(EjemploAlumno.TG_Folder + "Sprites\\Tomas.png");
            tomas.Position = new Vector2(screenSize.Width, 0);

            //Crear Sprite TomasTareas
            tomasTareas = new TgcSprite();
            tomasTareas.Texture = TgcTexture.createTexture(EjemploAlumno.TG_Folder + "Sprites\\TomasTareas.png");
            tomasTareas.Position = new Vector2(-desplazamiento, screenSize.Height - 200);
        }
        public void update(float elapsedTime)
        {
            timeSprite += elapsedTime;
            Size screenSize = GuiController.Instance.Panel3d.Size;
            if (credit1)
            {
                if (cris.Position.X < offset_pantalla) //Mientras que no este completamente dentro de la plantalla, aumenta rapido
                {
                    cris.Position += new Vector2(elapsedTime * 1000, 0);
                    crisTareas.Position -= new Vector2(elapsedTime * 1000, 0);
                }
                if (cris.Position.X.IsBetween(offset_pantalla, screenSize.Width - desplazamiento) && timeSprite <= 2) //Va mas lento durante 2 segundos
                {
                    timeSprite += elapsedTime;
                    cris.Position += new Vector2(elapsedTime * 10, 0);
                    crisTareas.Position -= new Vector2(elapsedTime * 10, 0);
                }
                if (timeSprite > 2)
                {
                    timeSprite += elapsedTime;
                    incremento++;
                    cris.Position += new Vector2(elapsedTime * 50 * incremento, 0);
                    crisTareas.Position -= new Vector2(elapsedTime * 50 * incremento, 0);
                }
                if (timeSprite > 4)
                {
                    credit1 = false;
                    credit2 = true;
                    incremento = 0;
                    timeSprite = 0;
                }

            }
            if (credit2)
            {
                if (ezeTareas.Position.X < offset_pantalla)
                {
                    eze.Position -= new Vector2(elapsedTime * 1000, 0);
                    ezeTareas.Position += new Vector2(elapsedTime * 1000, 0);
                }
                if (ezeTareas.Position.X.IsBetween(offset_pantalla, screenSize.Width - desplazamiento) && timeSprite <= 2)
                {
                    timeSprite += elapsedTime;
                    eze.Position -= new Vector2(elapsedTime * 10, 0);
                    ezeTareas.Position += new Vector2(elapsedTime * 10, 0);
                }
                if (timeSprite > 2)
                {
                    timeSprite += elapsedTime;
                    incremento++;
                    eze.Position -= new Vector2(elapsedTime * 50 * incremento, 0);
                    ezeTareas.Position += new Vector2(elapsedTime * 50 * incremento, 0);
                }
                if (timeSprite > 4)
                {
                    credit2 = false;
                    credit3 = true;
                    incremento = 0;
                    timeSprite = 0;
                }

            }
            if (credit3)
            {

                if (dante.Position.X < offset_pantalla)
                {
                    dante.Position += new Vector2(elapsedTime * 1000, 0);
                    danteTareas.Position -= new Vector2(elapsedTime * 1000, 0);
                }
                if (dante.Position.X.IsBetween(offset_pantalla, screenSize.Width - desplazamiento) && timeSprite <= 2)
                {
                    timeSprite += elapsedTime;
                    dante.Position += new Vector2(elapsedTime * 10, 0);
                    danteTareas.Position -= new Vector2(elapsedTime * 10, 0);
                }
                if (timeSprite > 2)
                {
                    timeSprite += elapsedTime;
                    incremento++;
                    dante.Position += new Vector2(elapsedTime * 50 * incremento, 0);
                    danteTareas.Position -= new Vector2(elapsedTime * 50 * incremento, 0);
                }
                if (timeSprite > 4)
                {
                    credit3 = false;
                    credit4 = true;
                    incremento = 0;
                    timeSprite = 0;
                }

            }
            if (credit4)
            {
                if (tomasTareas.Position.X < offset_pantalla)
                {
                    tomas.Position -= new Vector2(elapsedTime * 1000, 0);
                    tomasTareas.Position += new Vector2(elapsedTime * 1000, 0);
                }
                if (tomasTareas.Position.X.IsBetween(offset_pantalla, screenSize.Width - desplazamiento) && timeSprite <= 2)
                {
                    timeSprite += elapsedTime;
                    tomas.Position -= new Vector2(elapsedTime * 10, 0);
                    tomasTareas.Position += new Vector2(elapsedTime * 10, 0);
                }
                if (timeSprite > 2)
                {
                    timeSprite += elapsedTime;
                    incremento++;
                    tomas.Position -= new Vector2(elapsedTime * 50 * incremento, 0);
                    tomasTareas.Position += new Vector2(elapsedTime * 50 * incremento, 0);
                }
                if (timeSprite > 4)
                {
                    credit4 = false;
                    incremento = 0;
                    timeSprite = 0;
                }
            }
        }

        public void render()
        {
            //Iniciar dibujado de todos los Sprites de la escena (en este caso es solo uno)
            GuiController.Instance.Drawer2D.beginDrawSprite();

            //Dibujar sprite (si hubiese mas, deberian ir todos aquí)
            if (credit1)
            {
                cris.render();
                crisTareas.render();
            }
            if (credit2)
            {
                eze.render();
                ezeTareas.render();
            }
            if (credit3)
            {
                dante.render();
                danteTareas.render();
            }
            if (credit4)
            {
                tomas.render();
                tomasTareas.render();
            }

            //Finalizar el dibujado de Sprites
            GuiController.Instance.Drawer2D.endDrawSprite();
        }
    }
    public static class ExtensionsForFloat //Para trabajar con betweens
    {
        public static bool IsBetween(this float val, float low, float high)
        {
               return val > low && val < high;
        }
    }
}

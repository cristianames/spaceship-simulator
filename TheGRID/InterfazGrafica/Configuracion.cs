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

namespace AlumnoEjemplos.TheGRID.InterfazGrafica
{
    #region Estructuras Previas
    public enum Clase { NULL, VOLVER, OPCIONES, JUEGO, GRAFICOS, AUDIO, CONTINUAR, CREDITOS,
        CAPITULO, GRAVEDAD, REALISMO,
        CAPITULO1, CAPITULO2, CAPITULO3, MISION,
        EFECTO_GLOW, FARO_DELANTERO, FARO_POSICIONAL, BOUNDING_BOX, FULLSCREEN,
        MUSICA_FONDO, SELECCIONAR_TEMA};
    public class Sprite
    {
        internal TgcSprite spritePrincipal;
        internal TgcSprite spriteAlternativo;
        internal TgcSprite spriteDeshabilitado;
        internal Clase nombre;
        internal bool habilitado;

        public Sprite(Clase a) 
        { 
            spritePrincipal = new TgcSprite(); 
            spriteAlternativo = new TgcSprite();
            spriteDeshabilitado = new TgcSprite();
            habilitado = true;
            nombre = a; }

        public void alternarSprite()
        {
            TgcSprite temp = spritePrincipal;
            spritePrincipal = spriteAlternativo;
            spriteAlternativo = temp;
        }
        public void posicionarSprite(Vector2 posicion)
        {
            spritePrincipal.Position = posicion;
            spriteAlternativo.Position = posicion;
        }
    }
    public struct Menu
    {
        internal List<Sprite> listaOpciones;
        internal Clase anterior;

        public Menu(List<Sprite> a, Clase b) { listaOpciones = a; anterior = b; }
    }
    #endregion

    public class Configuracion
    {
        #region Atributos
        int opcionElegida;
        TgcSprite puntero;
        Size tamanioOriginalPuntero;
        Menu menuActivo;     //Este es el menu que se esta desplegando en pantalla. Va a ser alguno de los de abajo.
        Menu menuTransicion;   //Este es el menu que se acaba de ir.
        Menu opciones0;      //Menu Principal.
        Menu opciones1;      //Juego.
        Menu opciones11;     //Capitulos.
        Menu opciones2;      //Graficos.
        Menu opciones3;      //Musica.
        Menu opciones31;     //Audio.
        Size screenSize = GuiController.Instance.Panel3d.Size;
        Vector2 puntoEntrada = new Vector2(GuiController.Instance.Panel3d.Size.Width / 2f, -20f);
        Vector2 puntoSalida = new Vector2(-1000, GuiController.Instance.Panel3d.Size.Height / 2f - 5);
        float velocidadEfecto;
        bool salidaFin = false;
        bool entradaFin = false;
        int contadorSalida = 0;
        int contadorEntrada = 0;
        bool transicion = false;
        TgcD3dInput input = GuiController.Instance.D3dInput;
        Musique sonido;
        AlternativeDimension miMundo;
        #endregion

        #region Inicializacion
        public Configuracion(Musique media)
        {
            sonido = media;
            puntero = new TgcSprite();
            puntero.Texture = TgcTexture.createTexture(EjemploAlumno.TG_Folder + "Sprites\\Menu\\Puntero.png");
            tamanioOriginalPuntero = puntero.Texture.Size;
            opciones0 = cargarOpciones();
            opciones1 = cargarJuego();
            opciones2 = cargarGraficos();
            opciones3 = cargarAudio();
            opciones11 = cargarCapitulos();
            //opciones31 = cargarMusica();
            restart();
            miMundo = new AlternativeDimension();
        }
        private Menu cargarOpciones()
        {
            List<Sprite> listaTemp = new List<Sprite>();
            Sprite spriteTemp = new Sprite(Clase.OPCIONES);
            spriteTemp.spritePrincipal.Texture = TgcTexture.createTexture(EjemploAlumno.TG_Folder + "Sprites\\Menu\\Opcion\\Opciones.png");
            spriteTemp.spritePrincipal.Scaling = new Vector2(0.5f,0.5f);
            listaTemp.Add(spriteTemp);
            spriteTemp = new Sprite(Clase.JUEGO);
            spriteTemp.spritePrincipal.Texture = TgcTexture.createTexture(EjemploAlumno.TG_Folder + "Sprites\\Menu\\Opcion\\Juego.png");
            spriteTemp.spritePrincipal.Scaling = new Vector2(0.5f,0.5f);
            listaTemp.Add(spriteTemp);
            spriteTemp = new Sprite(Clase.GRAFICOS);
            spriteTemp.spritePrincipal.Texture = TgcTexture.createTexture(EjemploAlumno.TG_Folder + "Sprites\\Menu\\Opcion\\Graficos.png");
            spriteTemp.spritePrincipal.Scaling = new Vector2(0.5f,0.5f);
            listaTemp.Add(spriteTemp);
            spriteTemp = new Sprite(Clase.AUDIO);
            spriteTemp.spritePrincipal.Texture = TgcTexture.createTexture(EjemploAlumno.TG_Folder + "Sprites\\Menu\\Opcion\\Audio.png");
            spriteTemp.spritePrincipal.Scaling = new Vector2(0.5f,0.5f);
            listaTemp.Add(spriteTemp);
            spriteTemp = new Sprite(Clase.CREDITOS);
            spriteTemp.spritePrincipal.Texture = TgcTexture.createTexture(EjemploAlumno.TG_Folder + "Sprites\\Menu\\Opcion\\Creditos.png");
            spriteTemp.spritePrincipal.Scaling = new Vector2(0.5f, 0.5f);
            listaTemp.Add(spriteTemp);
            spriteTemp = new Sprite(Clase.CONTINUAR);
            spriteTemp.spritePrincipal.Texture = TgcTexture.createTexture(EjemploAlumno.TG_Folder + "Sprites\\Menu\\Opcion\\Continuar.png");
            spriteTemp.spritePrincipal.Scaling = new Vector2(0.5f, 0.5f);
            listaTemp.Add(spriteTemp);
            Menu menuTemp = new Menu(listaTemp, Clase.NULL);
            resetearPosicionLista(menuTemp);
            return menuTemp;      
        }
        private Menu cargarJuego()
        {
            List<Sprite> listaTemp = new List<Sprite>();
            Sprite spriteTemp = new Sprite(Clase.JUEGO);
            spriteTemp.spritePrincipal.Texture = TgcTexture.createTexture(EjemploAlumno.TG_Folder + "Sprites\\Menu\\Juego\\Juego.png");
            spriteTemp.spritePrincipal.Scaling = new Vector2(0.5f, 0.5f);
            listaTemp.Add(spriteTemp);
            spriteTemp = new Sprite(Clase.CAPITULO);
            spriteTemp.spritePrincipal.Texture = TgcTexture.createTexture(EjemploAlumno.TG_Folder + "Sprites\\Menu\\Juego\\Capitulo.png");
            spriteTemp.spritePrincipal.Scaling = new Vector2(0.5f, 0.5f);
            listaTemp.Add(spriteTemp);
            spriteTemp = new Sprite(Clase.GRAVEDAD);
            spriteTemp.spritePrincipal.Texture = TgcTexture.createTexture(EjemploAlumno.TG_Folder + "Sprites\\Menu\\Juego\\GravedadON.png");
            spriteTemp.spritePrincipal.Scaling = new Vector2(0.5f, 0.5f);
            spriteTemp.spriteAlternativo.Texture = TgcTexture.createTexture(EjemploAlumno.TG_Folder + "Sprites\\Menu\\Juego\\GravedadOFF.png");
            spriteTemp.spriteAlternativo.Scaling = new Vector2(0.5f, 0.5f);
            listaTemp.Add(spriteTemp);
            spriteTemp = new Sprite(Clase.REALISMO);
            spriteTemp.spritePrincipal.Texture = TgcTexture.createTexture(EjemploAlumno.TG_Folder + "Sprites\\Menu\\Juego\\RealismoON.png");
            spriteTemp.spritePrincipal.Scaling = new Vector2(0.5f, 0.5f);
            spriteTemp.spriteAlternativo.Texture = TgcTexture.createTexture(EjemploAlumno.TG_Folder + "Sprites\\Menu\\Juego\\RealismoOFF.png");
            spriteTemp.spriteAlternativo.Scaling = new Vector2(0.5f, 0.5f);
            listaTemp.Add(spriteTemp);
            spriteTemp = new Sprite(Clase.VOLVER);
            spriteTemp.spritePrincipal.Texture = TgcTexture.createTexture(EjemploAlumno.TG_Folder + "Sprites\\Menu\\Juego\\Volver.png");
            spriteTemp.spritePrincipal.Scaling = new Vector2(0.5f, 0.5f);
            listaTemp.Add(spriteTemp);
            Menu menuTemp = new Menu(listaTemp, Clase.OPCIONES);
            resetearPosicionLista(menuTemp);
            return menuTemp;      
        }
        private Menu cargarGraficos()
        {
            List<Sprite> listaTemp = new List<Sprite>();
            Sprite spriteTemp = new Sprite(Clase.GRAFICOS);
            spriteTemp.spritePrincipal.Texture = TgcTexture.createTexture(EjemploAlumno.TG_Folder + "Sprites\\Menu\\Graficos\\Graficos.png");
            spriteTemp.spritePrincipal.Scaling = new Vector2(0.5f, 0.5f);
            listaTemp.Add(spriteTemp);
            spriteTemp = new Sprite(Clase.EFECTO_GLOW);
            spriteTemp.spritePrincipal.Texture = TgcTexture.createTexture(EjemploAlumno.TG_Folder + "Sprites\\Menu\\Graficos\\GlowOFF.png");
            spriteTemp.spritePrincipal.Scaling = new Vector2(0.5f, 0.5f);
            spriteTemp.spriteAlternativo.Texture = TgcTexture.createTexture(EjemploAlumno.TG_Folder + "Sprites\\Menu\\Graficos\\GlowON.png");
            spriteTemp.spriteAlternativo.Scaling = new Vector2(0.5f, 0.5f);
            listaTemp.Add(spriteTemp);
            spriteTemp = new Sprite(Clase.FARO_DELANTERO);
            spriteTemp.spritePrincipal.Texture = TgcTexture.createTexture(EjemploAlumno.TG_Folder + "Sprites\\Menu\\Graficos\\FaroOFF.png");
            spriteTemp.spritePrincipal.Scaling = new Vector2(0.5f, 0.5f);
            spriteTemp.spriteAlternativo.Texture = TgcTexture.createTexture(EjemploAlumno.TG_Folder + "Sprites\\Menu\\Graficos\\FaroON.png");
            spriteTemp.spriteAlternativo.Scaling = new Vector2(0.5f, 0.5f);
            listaTemp.Add(spriteTemp);
            spriteTemp = new Sprite(Clase.FARO_POSICIONAL);
            spriteTemp.spritePrincipal.Texture = TgcTexture.createTexture(EjemploAlumno.TG_Folder + "Sprites\\Menu\\Graficos\\PosicionOFF.png");
            spriteTemp.spritePrincipal.Scaling = new Vector2(0.5f, 0.5f);
            spriteTemp.spriteAlternativo.Texture = TgcTexture.createTexture(EjemploAlumno.TG_Folder + "Sprites\\Menu\\Graficos\\PosicionON.png");
            spriteTemp.spriteAlternativo.Scaling = new Vector2(0.5f, 0.5f);
            listaTemp.Add(spriteTemp);
            spriteTemp = new Sprite(Clase.BOUNDING_BOX);
            spriteTemp.spritePrincipal.Texture = TgcTexture.createTexture(EjemploAlumno.TG_Folder + "Sprites\\Menu\\Graficos\\BoundingOFF.png");
            spriteTemp.spritePrincipal.Scaling = new Vector2(0.5f, 0.5f);
            spriteTemp.spriteAlternativo.Texture = TgcTexture.createTexture(EjemploAlumno.TG_Folder + "Sprites\\Menu\\Graficos\\BoundingON.png");
            spriteTemp.spriteAlternativo.Scaling = new Vector2(0.5f, 0.5f);
            listaTemp.Add(spriteTemp);
            spriteTemp = new Sprite(Clase.FULLSCREEN);
            spriteTemp.spritePrincipal.Texture = TgcTexture.createTexture(EjemploAlumno.TG_Folder + "Sprites\\Menu\\Graficos\\FullscreenOFF.png");
            spriteTemp.spritePrincipal.Scaling = new Vector2(0.5f, 0.5f);
            spriteTemp.spriteAlternativo.Texture = TgcTexture.createTexture(EjemploAlumno.TG_Folder + "Sprites\\Menu\\Graficos\\FullscreenON.png");
            spriteTemp.spriteAlternativo.Scaling = new Vector2(0.5f, 0.5f);
            listaTemp.Add(spriteTemp);
            spriteTemp = new Sprite(Clase.VOLVER);
            spriteTemp.spritePrincipal.Texture = TgcTexture.createTexture(EjemploAlumno.TG_Folder + "Sprites\\Menu\\Graficos\\Volver.png");
            spriteTemp.spritePrincipal.Scaling = new Vector2(0.5f, 0.5f);
            listaTemp.Add(spriteTemp);
            Menu menuTemp = new Menu(listaTemp, Clase.OPCIONES);
            resetearPosicionLista(menuTemp);
            return menuTemp;      
        }
        private Menu cargarAudio()
        {
            List<Sprite> listaTemp = new List<Sprite>();
            Sprite spriteTemp = new Sprite(Clase.AUDIO);
            spriteTemp.spritePrincipal.Texture = TgcTexture.createTexture(EjemploAlumno.TG_Folder + "Sprites\\Menu\\Audio\\Audio.png");
            spriteTemp.spritePrincipal.Scaling = new Vector2(0.5f, 0.5f);
            listaTemp.Add(spriteTemp);
            spriteTemp = new Sprite(Clase.MUSICA_FONDO);
            spriteTemp.spritePrincipal.Texture = TgcTexture.createTexture(EjemploAlumno.TG_Folder + "Sprites\\Menu\\Audio\\MusicaON.png");
            spriteTemp.spritePrincipal.Scaling = new Vector2(0.5f, 0.5f);
            spriteTemp.spriteAlternativo.Texture = TgcTexture.createTexture(EjemploAlumno.TG_Folder + "Sprites\\Menu\\Audio\\MusicaOFF.png");
            spriteTemp.spriteAlternativo.Scaling = new Vector2(0.25f, 0.5f);
            listaTemp.Add(spriteTemp);
            spriteTemp = new Sprite(Clase.VOLVER);
            spriteTemp.spritePrincipal.Texture = TgcTexture.createTexture(EjemploAlumno.TG_Folder + "Sprites\\Menu\\Audio\\Volver.png");
            spriteTemp.spritePrincipal.Scaling = new Vector2(0.5f, 0.5f);
            listaTemp.Add(spriteTemp);
            Menu menuTemp = new Menu(listaTemp, Clase.OPCIONES);
            resetearPosicionLista(menuTemp);
            return menuTemp;      
        }
        private Menu cargarCapitulos()
        {
            List<Sprite> listaTemp = new List<Sprite>();
            Sprite spriteTemp = new Sprite(Clase.CAPITULO);
            spriteTemp.spritePrincipal.Texture = TgcTexture.createTexture(EjemploAlumno.TG_Folder + "Sprites\\Menu\\Capitulo\\Capitulo.png");
            spriteTemp.spritePrincipal.Scaling = new Vector2(0.5f, 0.5f);
            listaTemp.Add(spriteTemp);
            spriteTemp = new Sprite(Clase.CAPITULO1);
            spriteTemp.spritePrincipal.Texture = TgcTexture.createTexture(EjemploAlumno.TG_Folder + "Sprites\\Menu\\Capitulo\\TheOpening.png");
            spriteTemp.spritePrincipal.Scaling = new Vector2(0.5f, 0.5f);
            listaTemp.Add(spriteTemp);
            spriteTemp = new Sprite(Clase.CAPITULO2);
            spriteTemp.spritePrincipal.Texture = TgcTexture.createTexture(EjemploAlumno.TG_Folder + "Sprites\\Menu\\Capitulo\\ImpulseDrive.png");
            spriteTemp.spritePrincipal.Scaling = new Vector2(0.5f, 0.5f);
            listaTemp.Add(spriteTemp);
            spriteTemp = new Sprite(Clase.CAPITULO3);
            spriteTemp.spritePrincipal.Texture = TgcTexture.createTexture(EjemploAlumno.TG_Folder + "Sprites\\Menu\\Capitulo\\WelcomeHome.png");
            spriteTemp.spritePrincipal.Scaling = new Vector2(0.5f, 0.5f);
            listaTemp.Add(spriteTemp);
            spriteTemp = new Sprite(Clase.MISION);
            spriteTemp.spritePrincipal.Texture = TgcTexture.createTexture(EjemploAlumno.TG_Folder + "Sprites\\Menu\\Capitulo\\Mision.png");
            spriteTemp.spritePrincipal.Scaling = new Vector2(0.5f, 0.5f);
            listaTemp.Add(spriteTemp);
            spriteTemp = new Sprite(Clase.VOLVER);
            spriteTemp.spritePrincipal.Texture = TgcTexture.createTexture(EjemploAlumno.TG_Folder + "Sprites\\Menu\\Capitulo\\Volver.png");
            spriteTemp.spritePrincipal.Scaling = new Vector2(0.5f, 0.5f);
            listaTemp.Add(spriteTemp);
            Menu menuTemp = new Menu(listaTemp, Clase.JUEGO);
            resetearPosicionLista(menuTemp);
            return menuTemp;      
        }
        private Menu cargarMusica()
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Herramientas
        internal void restart()
        {
            velocidadEfecto = 15f;
            opcionElegida = 1;
            menuActivo = opciones0;
            resetearPosicionLista(menuActivo);
            contadorEntrada = menuActivo.listaOpciones.Count;
            transicion = true;
            salidaFin = true;
        }
        private void resetearPosicionLista(Menu menu)
        {
            foreach (var item in menu.listaOpciones) { item.posicionarSprite(reajustarPosicionTextura(item.spritePrincipal, puntoEntrada)); }
        }
        private void reajustarPuntero()
        {
            Size chosenSize = menuActivo.listaOpciones[opcionElegida].spritePrincipal.Texture.Size;
            float temp = (float)chosenSize.Width * menuActivo.listaOpciones[opcionElegida].spritePrincipal.Scaling.X;
            chosenSize.Width = (int)temp;
            temp = (float)chosenSize.Height * menuActivo.listaOpciones[opcionElegida].spritePrincipal.Scaling.Y;
            chosenSize.Height = (int)temp;
            puntero.Position = menuActivo.listaOpciones[opcionElegida].spritePrincipal.Position;
            puntero.Scaling = new Vector2((float)chosenSize.Width / tamanioOriginalPuntero.Width, (float)chosenSize.Height / tamanioOriginalPuntero.Height);
        }
        private Vector2 reajustarPosicionTextura(TgcSprite sprite, Vector2 posicionSolicitada)
        {
            posicionSolicitada.X -= ((float)sprite.Texture.Width * sprite.Scaling.X) / 2f;
            posicionSolicitada.Y -= ((float)sprite.Texture.Height * sprite.Scaling.Y) / 2f;            
            return posicionSolicitada;
        }
        #endregion

        #region Operar y Chequeos
        public void operar(float elapsedTime)
        {
            miMundo.render(elapsedTime);
            if (transicion)
            {
                if (!salidaFin) { operarSalida(elapsedTime); return; }
                if (!entradaFin) { operarEntrada(elapsedTime); return; }
                opcionElegida = 1;
                reajustarPuntero();
                salidaFin = false;
                entradaFin = false;
                transicion = false;
                return;
            }
            GuiController.Instance.Drawer2D.beginDrawSprite();
            puntero.render();
            foreach (var item in menuActivo.listaOpciones) 
            {
                if (item.habilitado) item.spritePrincipal.render();
                else item.spriteDeshabilitado.render(); 
            }
            //foreach (var item in menuTransicion.listaOpciones) { item.sprite.render(); }
            GuiController.Instance.Drawer2D.endDrawSprite();
            if (input.keyPressed(Key.Up)) { if (opcionElegida > 1) { opcionElegida--; reajustarPuntero(); sonido.playSlideButton(); } }
            if (input.keyPressed(Key.Down)) { if (opcionElegida < menuActivo.listaOpciones.Count - 1) { opcionElegida++; reajustarPuntero(); sonido.playSlideButton(); } }
            if (input.keyPressed(Key.Return)) { chequearReturn();}
            if (input.keyPressed(Key.BackSpace)) { chequearBackSpace(); sonido.playSlidePanel(); }
        }
        private void chequearReturn()
        {
            switch (menuActivo.listaOpciones[opcionElegida].nombre)
            {
                case Clase.VOLVER:
                    chequearBackSpace();
                    sonido.playSlidePanel(); 
                    break;
                case Clase.JUEGO:
                    transicion = true;
                    menuTransicion = menuActivo;
                    menuActivo = opciones1;
                    contadorSalida = menuTransicion.listaOpciones.Count;
                    contadorEntrada = menuActivo.listaOpciones.Count;
                    transicion = true;
                    sonido.playSlidePanel(); 
                    break;
                case Clase.GRAFICOS:
                    transicion = true;
                    menuTransicion = menuActivo;
                    menuActivo = opciones2;
                    contadorSalida = menuTransicion.listaOpciones.Count;
                    contadorEntrada = menuActivo.listaOpciones.Count;
                    transicion = true;
                    sonido.playSlidePanel(); 
                    break;
                case Clase.AUDIO:
                    transicion = true;
                    menuTransicion = menuActivo;
                    menuActivo = opciones3;
                    contadorSalida = menuTransicion.listaOpciones.Count;
                    contadorEntrada = menuActivo.listaOpciones.Count;
                    transicion = true;
                    sonido.playSlidePanel(); 
                    break;
                case Clase.CREDITOS:
                    //NADA de momento
                    break;
                case Clase.CONTINUAR:
                    EjemploAlumno.workspace().config = false;
                    sonido.playPauseBackgound();
                    break;
                case Clase.CAPITULO:
                    transicion = true;
                    menuTransicion = menuActivo;
                    menuActivo = opciones11;
                    contadorSalida = menuTransicion.listaOpciones.Count;
                    contadorEntrada = menuActivo.listaOpciones.Count;
                    transicion = true;
                    sonido.playSlidePanel(); 
                    break;
                case Clase.GRAVEDAD:
                    menuActivo.listaOpciones[opcionElegida].alternarSprite();
                    sonido.playChangePause(); 
                    break;
                case Clase.REALISMO:
                    menuActivo.listaOpciones[opcionElegida].alternarSprite();
                    sonido.playChangePause(); 
                    break;
                case Clase.EFECTO_GLOW:
                    menuActivo.listaOpciones[opcionElegida].alternarSprite();
                    sonido.playChangePause(); 
                    break;
                case Clase.FARO_DELANTERO:
                    menuActivo.listaOpciones[opcionElegida].alternarSprite();
                    sonido.playChangePause(); 
                    break;
                case Clase.FARO_POSICIONAL:
                    menuActivo.listaOpciones[opcionElegida].alternarSprite();
                    sonido.playChangePause(); 
                    break;
                case Clase.BOUNDING_BOX:
                    menuActivo.listaOpciones[opcionElegida].alternarSprite();
                    sonido.playChangePause(); 
                    break;
                case Clase.FULLSCREEN:
                    menuActivo.listaOpciones[opcionElegida].alternarSprite();
                    sonido.playChangePause(); 
                    break;
                case Clase.MUSICA_FONDO:
                    menuActivo.listaOpciones[opcionElegida].alternarSprite();
                    sonido.playChangePause(); 
                    break;
                case Clase.SELECCIONAR_TEMA:
                    //NADA de momento
                    break;
                case Clase.CAPITULO1:
                    //COMPLETAR DESPUES
                    break;
                case Clase.CAPITULO2:
                    //COMPLETAR DESPUES
                    break;
                case Clase.CAPITULO3:
                    //COMPLETAR DESPUES
                    break;
                case Clase.MISION:
                    //COMPLETAR DESPUES
                    break;
            }
        }
        private void chequearBackSpace()
        {
            switch (menuActivo.anterior)
            {
                case Clase.NULL:
                    //No hacer nada.
                    break;
                case Clase.OPCIONES:
                    transicion = true;
                    menuTransicion = menuActivo;
                    menuActivo = opciones0;
                    contadorSalida = menuTransicion.listaOpciones.Count;
                    contadorEntrada = menuActivo.listaOpciones.Count;
                    transicion = true;
                    break;
                case Clase.JUEGO:
                    transicion = true;
                    menuTransicion = menuActivo;
                    menuActivo = opciones1;
                    contadorSalida = menuTransicion.listaOpciones.Count;
                    contadorEntrada = menuActivo.listaOpciones.Count;
                    transicion = true;
                    break;
                case Clase.AUDIO:
                    transicion = true;
                    menuTransicion = menuActivo;
                    menuActivo = opciones3;
                    contadorSalida = menuTransicion.listaOpciones.Count;
                    contadorEntrada = menuActivo.listaOpciones.Count;
                    transicion = true;
                    break;
            }
        }
        private void operarSalida(float elapsedTime)
        {
            if (velocidadEfecto < 20f) velocidadEfecto += elapsedTime * 30f;
            GuiController.Instance.Drawer2D.beginDrawSprite();
            for (int i = 0; i < menuTransicion.listaOpciones.Count; i++)
            {
                Vector2 pos = menuTransicion.listaOpciones[i].spritePrincipal.Position;
                if (pos != puntoSalida)
                {
                    Vector2 temp = Vector2.Subtract(puntoSalida, pos);
                    temp.Normalize();
                    temp.Multiply(velocidadEfecto);
                    pos += temp;
                    if (pos.X < -800)//Vector2.LengthSq(Vector2.Subtract(pos, puntoSalida)) < 10)
                    {
                        pos = puntoSalida;
                        contadorSalida--;
                        if (contadorSalida == 0) velocidadEfecto = 15f;
                    }
                    menuTransicion.listaOpciones[i].posicionarSprite(pos);
                    menuTransicion.listaOpciones[i].spritePrincipal.render();
                }
            }
            GuiController.Instance.Drawer2D.endDrawSprite();
            if (contadorSalida <= 0)
            {
                salidaFin = true;
                resetearPosicionLista(menuTransicion);                
            }
        }
        private void operarEntrada(float elapsedTime)
        {
            if (velocidadEfecto > 3) velocidadEfecto -= elapsedTime * 2f;
            GuiController.Instance.Drawer2D.beginDrawSprite();
            for (int i = 0; i < menuActivo.listaOpciones.Count; i++)
            {
                Vector2 temp = menuActivo.listaOpciones[i].spritePrincipal.Position;
                float yRef = screenSize.Height / menuActivo.listaOpciones.Count * i + 20;
                if (temp.Y != yRef)
                {
                    if (temp.Y < yRef) temp.Y += velocidadEfecto;
                    //if (temp.Y > yRef) temp.Y -= velocidadEfecto;
                    if (FastMath.Abs(temp.Y - yRef) < 8f)
                    {
                        temp.Y = yRef;
                        contadorEntrada--;
                    }
                    menuActivo.listaOpciones[i].posicionarSprite(temp);
                }
                menuActivo.listaOpciones[i].spritePrincipal.render();
            }
            GuiController.Instance.Drawer2D.endDrawSprite();
            if (contadorEntrada <= 0) { entradaFin = true; velocidadEfecto = 0.1f; }
        }
        #endregion
    }
}

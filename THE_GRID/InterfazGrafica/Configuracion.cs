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

namespace AlumnoEjemplos.THE_GRID.InterfazGrafica
{
    #region Estructuras Previas
    public enum Clase 
    { 
        NULL, VOLVER, 
        OPCIONES, JUEGO, GRAFICOS, AUDIO, CONTINUAR, FONDO,
        CAPITULO, GRAVEDAD, REALISMO, MOUSE,
        CAPITULO1, CAPITULO2, CAPITULO3, MISION,
        EFECTO_GLOW, FARO_DELANTERO, FARO_POSICIONAL, BOUNDING_BOX, FULLSCREEN,
        MUSICA_FONDO, SELECCIONAR_TEMA,
        CASTOR, DEREZZED, M4PART2, TRONENDING, TSOF, TALI, SPECTRE, SOLARSAILER, NEWWORLDS, METHEME, COMPLETE 
    };
    public class Sprite
    {
        internal TgcSprite spritePrincipal;
        internal TgcSprite spriteAlternativo;
        internal TgcSprite spriteDeshabilitado;
        internal Clase nombre;
        internal bool on;
        internal bool habilitado;

        public Sprite(Clase a) 
        { 
            spritePrincipal = new TgcSprite(); 
            spriteAlternativo = new TgcSprite();
            spriteDeshabilitado = new TgcSprite();
            habilitado = true;
            on = true;
            nombre = a; }

        public void alternsarSprite()
        {
            TgcSprite temp = spritePrincipal;
            spritePrincipal = spriteAlternativo;
            spriteAlternativo = temp;
        }
        public void posicionarSprite(Vector2 posicion)
        {
            spritePrincipal.Position = posicion;
            spriteAlternativo.Position = posicion;
            spriteDeshabilitado.Position = posicion;
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
        bool fondoAnimado = false;
        TgcSprite fondoAlternativo;
        TgcSprite fondoPrincipal;
        bool verFondo = false;
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
private   AlumnoEjemplos.THE_GRID.Musique music;
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
            opciones31 = cargarMusica();
            crearFondos();
            restart();
            if (fondoAnimado) miMundo = new AlternativeDimension();
        }


        private void crearFondos()
        {
            //Crear Sprite
            fondoAlternativo = new TgcSprite();
            fondoAlternativo.Texture = TgcTexture.createTexture(EjemploAlumno.TG_Folder + "Sprites\\Menu\\FondoAlternativo.png");
            //Ubicarlo centrado en la pantalla
            Size screenSize = GuiController.Instance.Panel3d.Size;
            Size textureSize = fondoAlternativo.Texture.Size;
            fondoAlternativo.Position = new Vector2(FastMath.Max(screenSize.Width / 2 - textureSize.Width / 2, 0), FastMath.Max(screenSize.Height / 2 - textureSize.Height / 2, 0));
            fondoAlternativo.Scaling = new Vector2((float)screenSize.Width / (float)textureSize.Width, (float)screenSize.Height / (float)textureSize.Height);
            //fondoAlternativo.Scaling = new Vector2(0.5f, 0.5f);

            //Crear Sprite
            fondoPrincipal = new TgcSprite();
            fondoPrincipal.Texture = TgcTexture.createTexture(EjemploAlumno.TG_Folder + "Sprites\\Menu\\FondoPrincipal.png");
            //Ubicarlo centrado en la pantalla
            screenSize = GuiController.Instance.Panel3d.Size;
            textureSize = fondoPrincipal.Texture.Size;
            fondoPrincipal.Scaling = new Vector2((float)screenSize.Width / (float)textureSize.Width * 0.7f, (float)screenSize.Height / (float)textureSize.Height * 0.95f);
            fondoPrincipal.Position = new Vector2(FastMath.Max(screenSize.Width / 2 - (textureSize.Width * fondoPrincipal.Scaling.X)/ 2, 0), FastMath.Max(screenSize.Height / 2 - (textureSize.Height * fondoPrincipal.Scaling.Y) / 2, 0));
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
            spriteTemp = new Sprite(Clase.FONDO);
            spriteTemp.spritePrincipal.Texture = TgcTexture.createTexture(EjemploAlumno.TG_Folder + "Sprites\\Menu\\Opcion\\Fondo.png");
            spriteTemp.spritePrincipal.Scaling = new Vector2(0.5f, 0.5f);
            spriteTemp.spriteDeshabilitado.Texture = TgcTexture.createTexture(EjemploAlumno.TG_Folder + "Sprites\\Menu\\Opcion\\FondoIN.png");
            spriteTemp.spriteDeshabilitado.Scaling = new Vector2(0.4f, 0.4f);
            if (!fondoAnimado) spriteTemp.habilitado = false;
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
            spriteTemp.spritePrincipal.Scaling = new Vector2(0.6f, 0.55f);
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
            spriteTemp.on = EjemploAlumno.workspace().gravity;
            listaTemp.Add(spriteTemp);
            spriteTemp = new Sprite(Clase.REALISMO);
            spriteTemp.spritePrincipal.Texture = TgcTexture.createTexture(EjemploAlumno.TG_Folder + "Sprites\\Menu\\Juego\\RealismoON.png");
            spriteTemp.spritePrincipal.Scaling = new Vector2(0.5f, 0.5f);
            spriteTemp.spriteAlternativo.Texture = TgcTexture.createTexture(EjemploAlumno.TG_Folder + "Sprites\\Menu\\Juego\\RealismoOFF.png");
            spriteTemp.spriteAlternativo.Scaling = new Vector2(0.5f, 0.5f);
            spriteTemp.on = EjemploAlumno.workspace().despl_avanzado;
            listaTemp.Add(spriteTemp);
            spriteTemp = new Sprite(Clase.MOUSE);
            spriteTemp.spritePrincipal.Texture = TgcTexture.createTexture(EjemploAlumno.TG_Folder + "Sprites\\Menu\\Juego\\MouseON.png");
            spriteTemp.spritePrincipal.Scaling = new Vector2(0.8f, 0.5f);
            spriteTemp.spriteAlternativo.Texture = TgcTexture.createTexture(EjemploAlumno.TG_Folder + "Sprites\\Menu\\Juego\\MouseOFF.png");
            spriteTemp.spriteAlternativo.Scaling = new Vector2(0.4f, 0.5f);
            spriteTemp.spriteDeshabilitado.Texture = TgcTexture.createTexture(EjemploAlumno.TG_Folder + "Sprites\\Menu\\Juego\\MouseIN.png");
            spriteTemp.spriteDeshabilitado.Scaling = new Vector2(0.5f, 0.5f);
            spriteTemp.habilitado = GuiController.Instance.FullScreenEnable;
            spriteTemp.on = EjemploAlumno.workspace().mouse;
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
            spriteTemp.spritePrincipal.Texture = TgcTexture.createTexture(EjemploAlumno.TG_Folder + "Sprites\\Menu\\Graficos\\GlowON.png");
            spriteTemp.spritePrincipal.Scaling = new Vector2(0.5f, 0.5f);
            spriteTemp.spriteAlternativo.Texture = TgcTexture.createTexture(EjemploAlumno.TG_Folder + "Sprites\\Menu\\Graficos\\GlowOFF.png");
            spriteTemp.spriteAlternativo.Scaling = new Vector2(0.5f, 0.5f);
            spriteTemp.on = EjemploAlumno.workspace().glow;
            listaTemp.Add(spriteTemp);
            spriteTemp = new Sprite(Clase.FARO_DELANTERO);
            spriteTemp.spritePrincipal.Texture = TgcTexture.createTexture(EjemploAlumno.TG_Folder + "Sprites\\Menu\\Graficos\\FaroON.png");
            spriteTemp.spritePrincipal.Scaling = new Vector2(0.5f, 0.5f);
            spriteTemp.spriteAlternativo.Texture = TgcTexture.createTexture(EjemploAlumno.TG_Folder + "Sprites\\Menu\\Graficos\\FaroOFF.png");
            spriteTemp.spriteAlternativo.Scaling = new Vector2(0.5f, 0.5f);
            spriteTemp.on = EjemploAlumno.workspace().linterna;
            listaTemp.Add(spriteTemp);
            spriteTemp = new Sprite(Clase.FARO_POSICIONAL);
            spriteTemp.spritePrincipal.Texture = TgcTexture.createTexture(EjemploAlumno.TG_Folder + "Sprites\\Menu\\Graficos\\PosicionON.png");
            spriteTemp.spritePrincipal.Scaling = new Vector2(0.5f, 0.5f);
            spriteTemp.spriteAlternativo.Texture = TgcTexture.createTexture(EjemploAlumno.TG_Folder + "Sprites\\Menu\\Graficos\\PosicionOFF.png");
            spriteTemp.spriteAlternativo.Scaling = new Vector2(0.5f, 0.5f);
            spriteTemp.on = EjemploAlumno.workspace().luces_posicionales;
            listaTemp.Add(spriteTemp);
            spriteTemp = new Sprite(Clase.BOUNDING_BOX);
            spriteTemp.spritePrincipal.Texture = TgcTexture.createTexture(EjemploAlumno.TG_Folder + "Sprites\\Menu\\Graficos\\BoundingON.png");
            spriteTemp.spritePrincipal.Scaling = new Vector2(0.5f, 0.5f);
            spriteTemp.spriteAlternativo.Texture = TgcTexture.createTexture(EjemploAlumno.TG_Folder + "Sprites\\Menu\\Graficos\\BoundingOFF.png");
            spriteTemp.spriteAlternativo.Scaling = new Vector2(0.5f, 0.5f);
            spriteTemp.on = EjemploAlumno.workspace().boundingBoxes;
            listaTemp.Add(spriteTemp);
            /*spriteTemp = new Sprite(Clase.FULLSCREEN);
            spriteTemp.spritePrincipal.Texture = TgcTexture.createTexture(EjemploAlumno.TG_Folder + "Sprites\\Menu\\Graficos\\FullscreenOFF.png");
            spriteTemp.spritePrincipal.Scaling = new Vector2(0.5f, 0.5f);
            spriteTemp.spriteAlternativo.Texture = TgcTexture.createTexture(EjemploAlumno.TG_Folder + "Sprites\\Menu\\Graficos\\FullscreenON.png");
            spriteTemp.spriteAlternativo.Scaling = new Vector2(0.5f, 0.5f);
            listaTemp.Add(spriteTemp);*/
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
            spriteTemp.spritePrincipal.Scaling = new Vector2(0.6f, 0.55f);
            listaTemp.Add(spriteTemp);
            spriteTemp = new Sprite(Clase.MUSICA_FONDO);
            spriteTemp.spritePrincipal.Texture = TgcTexture.createTexture(EjemploAlumno.TG_Folder + "Sprites\\Menu\\Audio\\MusicaON.png");
            spriteTemp.spritePrincipal.Scaling = new Vector2(0.5f, 0.5f);
            spriteTemp.spriteAlternativo.Texture = TgcTexture.createTexture(EjemploAlumno.TG_Folder + "Sprites\\Menu\\Audio\\MusicaOFF.png");
            spriteTemp.spriteAlternativo.Scaling = new Vector2(0.25f, 0.5f);
            listaTemp.Add(spriteTemp);
            spriteTemp = new Sprite(Clase.COMPLETE);
            spriteTemp.spritePrincipal.Texture = TgcTexture.createTexture(EjemploAlumno.TG_Folder + "Sprites\\Menu\\Audio\\ListaCompleta.png");
            spriteTemp.spritePrincipal.Scaling = new Vector2(0.5f, 0.5f);
            spriteTemp.spriteDeshabilitado.Texture = TgcTexture.createTexture(EjemploAlumno.TG_Folder + "Sprites\\Menu\\Audio\\ListaCompletaIN.png");
            spriteTemp.spriteDeshabilitado.Scaling = new Vector2(0.5f, 0.5f);
            spriteTemp.habilitado = EjemploAlumno.workspace().musicaActivada;
            listaTemp.Add(spriteTemp);
            spriteTemp = new Sprite(Clase.SELECCIONAR_TEMA);
            spriteTemp.spritePrincipal.Texture = TgcTexture.createTexture(EjemploAlumno.TG_Folder + "Sprites\\Menu\\Audio\\Seleccionar.png");
            spriteTemp.spritePrincipal.Scaling = new Vector2(0.5f, 0.5f);
            spriteTemp.spriteDeshabilitado.Texture = TgcTexture.createTexture(EjemploAlumno.TG_Folder + "Sprites\\Menu\\Audio\\SeleccionarIN.png");
            spriteTemp.spriteDeshabilitado.Scaling = new Vector2(0.5f, 0.5f);
            spriteTemp.habilitado = EjemploAlumno.workspace().musicaActivada;
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
            List<Sprite> listaTemp = new List<Sprite>();
            Sprite spriteTemp = new Sprite(Clase.SELECCIONAR_TEMA);
            spriteTemp.spritePrincipal.Texture = TgcTexture.createTexture(EjemploAlumno.TG_Folder + "Sprites\\Menu\\Seleccionar\\Seleccionar.png");
            spriteTemp.spritePrincipal.Scaling = new Vector2(0.5f, 0.5f);
            listaTemp.Add(spriteTemp);
            spriteTemp = new Sprite(Clase.CASTOR);
            spriteTemp.spritePrincipal.Texture = TgcTexture.createTexture(EjemploAlumno.TG_Folder + "Sprites\\Menu\\Seleccionar\\Castor.png");
            spriteTemp.spritePrincipal.Scaling = new Vector2(0.3f, 0.3f);
            listaTemp.Add(spriteTemp);
            spriteTemp = new Sprite(Clase.DEREZZED);
            spriteTemp.spritePrincipal.Texture = TgcTexture.createTexture(EjemploAlumno.TG_Folder + "Sprites\\Menu\\Seleccionar\\Derezzed.png");
            spriteTemp.spritePrincipal.Scaling = new Vector2(0.3f, 0.3f);
            listaTemp.Add(spriteTemp);
            spriteTemp = new Sprite(Clase.M4PART2);
            spriteTemp.spritePrincipal.Texture = TgcTexture.createTexture(EjemploAlumno.TG_Folder + "Sprites\\Menu\\Seleccionar\\M4part2.png");
            spriteTemp.spritePrincipal.Scaling = new Vector2(0.3f, 0.3f);
            listaTemp.Add(spriteTemp);
            /*spriteTemp = new Sprite(Clase.METHEME);
            spriteTemp.spritePrincipal.Texture = TgcTexture.createTexture(EjemploAlumno.TG_Folder + "Sprites\\Menu\\Seleccionar\\MEtheme.png");
            spriteTemp.spritePrincipal.Scaling = new Vector2(0.3f, 0.3f);
            listaTemp.Add(spriteTemp);*/
            /*spriteTemp = new Sprite(Clase.NEWWORLDS);
            spriteTemp.spritePrincipal.Texture = TgcTexture.createTexture(EjemploAlumno.TG_Folder + "Sprites\\Menu\\Seleccionar\\NewWorlds.png");
            spriteTemp.spritePrincipal.Scaling = new Vector2(0.3f, 0.3f);
            listaTemp.Add(spriteTemp);*/
            spriteTemp = new Sprite(Clase.SOLARSAILER);
            spriteTemp.spritePrincipal.Texture = TgcTexture.createTexture(EjemploAlumno.TG_Folder + "Sprites\\Menu\\Seleccionar\\SolarSailer.png");
            spriteTemp.spritePrincipal.Scaling = new Vector2(0.3f, 0.3f);
            listaTemp.Add(spriteTemp);
            spriteTemp = new Sprite(Clase.SPECTRE);
            spriteTemp.spritePrincipal.Texture = TgcTexture.createTexture(EjemploAlumno.TG_Folder + "Sprites\\Menu\\Seleccionar\\Spectre.png");
            spriteTemp.spritePrincipal.Scaling = new Vector2(0.3f, 0.3f);
            listaTemp.Add(spriteTemp);
            spriteTemp = new Sprite(Clase.TALI);
            spriteTemp.spritePrincipal.Texture = TgcTexture.createTexture(EjemploAlumno.TG_Folder + "Sprites\\Menu\\Seleccionar\\Tali.png");
            spriteTemp.spritePrincipal.Scaling = new Vector2(0.3f, 0.3f);
            listaTemp.Add(spriteTemp);
            spriteTemp = new Sprite(Clase.TSOF);
            spriteTemp.spritePrincipal.Texture = TgcTexture.createTexture(EjemploAlumno.TG_Folder + "Sprites\\Menu\\Seleccionar\\TheSonofFlynn.png");
            spriteTemp.spritePrincipal.Scaling = new Vector2(0.25f, 0.3f);
            listaTemp.Add(spriteTemp);
            /*spriteTemp = new Sprite(Clase.TRONENDING);
            spriteTemp.spritePrincipal.Texture = TgcTexture.createTexture(EjemploAlumno.TG_Folder + "Sprites\\Menu\\Seleccionar\\TronEnding.png");
            spriteTemp.spritePrincipal.Scaling = new Vector2(0.3f, 0.3f);
            listaTemp.Add(spriteTemp);*/
            spriteTemp = new Sprite(Clase.VOLVER);
            spriteTemp.spritePrincipal.Texture = TgcTexture.createTexture(EjemploAlumno.TG_Folder + "Sprites\\Menu\\Seleccionar\\Volver.png");
            spriteTemp.spritePrincipal.Scaling = new Vector2(0.3f, 0.3f);
            listaTemp.Add(spriteTemp);
            Menu menuTemp = new Menu(listaTemp, Clase.AUDIO);
            resetearPosicionLista(menuTemp);
            return menuTemp;      
        }
        #endregion


        #region Herramientas
        internal void restart()
        {
            velocidadEfecto = 15f;
            opcionElegida = 1;
            if (menuActivo.listaOpciones != null) resetearPosicionLista(menuActivo);
            menuActivo = opciones0;
            resetearPosicionLista(menuActivo);
            contadorEntrada = menuActivo.listaOpciones.Count;
            transicion = true;
            salidaFin = true;
        }
        private void resetearPosicionLista(Menu menu)
        {
            foreach (var item in menu.listaOpciones) 
            {
                if (item.habilitado)
                {
                    if(item.on)item.posicionarSprite(reajustarPosicionTextura(item.spritePrincipal, puntoEntrada));
                    else item.posicionarSprite(reajustarPosicionTextura(item.spriteAlternativo, puntoEntrada));
                }
                else item.posicionarSprite(reajustarPosicionTextura(item.spriteDeshabilitado, puntoEntrada));
            }
        }
        private void reajustarPuntero()
        {
            TgcSprite chosenSprite;
            if (menuActivo.listaOpciones[opcionElegida].habilitado)
            {
                if (menuActivo.listaOpciones[opcionElegida].on) chosenSprite = menuActivo.listaOpciones[opcionElegida].spritePrincipal;
                else chosenSprite = menuActivo.listaOpciones[opcionElegida].spriteAlternativo;
            }
            else chosenSprite = menuActivo.listaOpciones[opcionElegida].spriteDeshabilitado;

            Size chosenSize = chosenSprite.Texture.Size;
            float temp = (float)chosenSize.Width * chosenSprite.Scaling.X;
            chosenSize.Width = (int)temp;
            temp = (float)chosenSize.Height * chosenSprite.Scaling.Y;
            chosenSize.Height = (int)temp;
            puntero.Position = chosenSprite.Position;
            puntero.Scaling = new Vector2((float)chosenSize.Width / tamanioOriginalPuntero.Width, (float)chosenSize.Height / tamanioOriginalPuntero.Height);
        }
        private Vector2 reajustarPosicionTextura(TgcSprite sprite, Vector2 posicionSolicitada)
        {
            posicionSolicitada.X -= ((float)sprite.Texture.Width * sprite.Scaling.X) / 2f;
            posicionSolicitada.Y -= ((float)sprite.Texture.Height * sprite.Scaling.Y) / 2f;            
            return posicionSolicitada;
        }
        private static void renderSegunEstado(Sprite item)
        {
            if (item.habilitado)
            {
                if (item.on) item.spritePrincipal.render();
                else item.spriteAlternativo.render();
            }
            else item.spriteDeshabilitado.render();
        }
        public void alternarBooleano(ref bool variable)
        {
            if (variable)
                variable = false;
            else
                variable = true;
        }
        #endregion


        #region Chequeos
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
                case Clase.FONDO:
                    if (menuActivo.listaOpciones[opcionElegida].habilitado)verFondo = true;
                    else sonido.playDeniedPress();
                    break;
                case Clase.CONTINUAR:
                    EjemploAlumno.workspace().config = false;
                    sonido.onlyResumeBackground();
                    sonido.playExitMenu();
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
                    menuActivo.listaOpciones[opcionElegida].on = !menuActivo.listaOpciones[opcionElegida].on;
                    EjemploAlumno.workspace().gravity = !EjemploAlumno.workspace().gravity;
                    sonido.playChangePause(); 
                    break;
                case Clase.REALISMO:
                    menuActivo.listaOpciones[opcionElegida].on = !menuActivo.listaOpciones[opcionElegida].on;
                    EjemploAlumno.workspace().despl_avanzado = !EjemploAlumno.workspace().despl_avanzado;
                    sonido.playChangePause(); 
                    break;
                case Clase.MOUSE:
                    if (menuActivo.listaOpciones[opcionElegida].habilitado)
                    {
                        menuActivo.listaOpciones[opcionElegida].on = !menuActivo.listaOpciones[opcionElegida].on;
                        EjemploAlumno.workspace().mouse = !EjemploAlumno.workspace().mouse;
                        sonido.playChangePause();
                    }
                    else sonido.playDeniedPress();
                    break;
                case Clase.EFECTO_GLOW:
                    menuActivo.listaOpciones[opcionElegida].on = !menuActivo.listaOpciones[opcionElegida].on;
                    alternarBooleano(ref EjemploAlumno.workspace().glow);
                    sonido.playChangePause(); 
                    break;
                case Clase.FARO_DELANTERO:
                    menuActivo.listaOpciones[opcionElegida].on = !menuActivo.listaOpciones[opcionElegida].on;
                    alternarBooleano(ref EjemploAlumno.workspace().linterna);
                    sonido.playChangePause(); 
                    break;
                case Clase.FARO_POSICIONAL:
                    menuActivo.listaOpciones[opcionElegida].on = !menuActivo.listaOpciones[opcionElegida].on;
                    alternarBooleano(ref EjemploAlumno.workspace().luces_posicionales);
                    sonido.playChangePause(); 
                    break;
                case Clase.BOUNDING_BOX:
                    menuActivo.listaOpciones[opcionElegida].on = !menuActivo.listaOpciones[opcionElegida].on;
                    alternarBooleano(ref EjemploAlumno.workspace().boundingBoxes);
                    sonido.playChangePause(); 
                    break;
                case Clase.FULLSCREEN:
                    /*if (menuActivo.listaOpciones[opcionElegida].on && opciones11.listaOpciones[4].on) opciones11.listaOpciones[4].on = false;
                    menuActivo.listaOpciones[opcionElegida].on = !menuActivo.listaOpciones[opcionElegida].on;
                    opciones11.listaOpciones[4].habilitado = !opciones11.listaOpciones[4].habilitado;
                    GuiController.Instance.FullScreenEnable = !GuiController.Instance.FullScreenEnable;
                    sonido.playChangePause(); */
                    break;
                case Clase.MUSICA_FONDO:
                    menuActivo.listaOpciones[opcionElegida].on = !menuActivo.listaOpciones[opcionElegida].on;
                    menuActivo.listaOpciones[2].habilitado = !menuActivo.listaOpciones[2].habilitado;
                    menuActivo.listaOpciones[3].habilitado = !menuActivo.listaOpciones[3].habilitado;
                    alternarBooleano(ref EjemploAlumno.workspace().musicaActivada);
                    sonido.onlyStopBackground();
                    sonido.playChangePause(); 
                    break;
                case Clase.SELECCIONAR_TEMA:
                    if (menuActivo.listaOpciones[opcionElegida].habilitado)
                    {
                        transicion = true;
                        menuTransicion = menuActivo;
                        menuActivo = opciones31;
                        contadorSalida = menuTransicion.listaOpciones.Count;
                        contadorEntrada = menuActivo.listaOpciones.Count;
                        transicion = true;
                        sonido.playSlidePanel();
                    }
                    else sonido.playDeniedPress(); 
                    break;
                case Clase.CAPITULO1:
                    EjemploAlumno.workspace().Escenario.chequearCambio("THE OPENING");
                    sonido.playChangePause(); 
                    EjemploAlumno.workspace().config = false;
                    sonido.playPauseBackgound();
                    break;
                case Clase.CAPITULO2:
                    EjemploAlumno.workspace().Escenario.chequearCambio("IMPULSE DRIVE");
                    sonido.playChangePause(); 
                    EjemploAlumno.workspace().config = false;
                    sonido.playPauseBackgound();
                    break;
                case Clase.CAPITULO3:
                    EjemploAlumno.workspace().Escenario.chequearCambio("WELCOME HOME");           
                    sonido.playChangePause(); 
                    EjemploAlumno.workspace().config = false;
                    sonido.playPauseBackgound();
                    break;
                case Clase.MISION:
                    EjemploAlumno.workspace().Escenario.chequearCambio("MISION");
                    sonido.playChangePause(); 
                    EjemploAlumno.workspace().config = false;
                    sonido.playPauseBackgound();
                    break;
                case Clase.COMPLETE:
                    if (menuActivo.listaOpciones[opcionElegida].habilitado)
                    {
                        EjemploAlumno.workspace().music.chequearCambio("Lista Completa");
                        sonido.playChangePause();
                    }
                    else sonido.playDeniedPress(); 
                    break;
                case Clase.CASTOR:
                    EjemploAlumno.workspace().music.chequearCambio("Castor");
                    sonido.playChangePause();
                    break;
                case Clase.DEREZZED:
                    EjemploAlumno.workspace().music.chequearCambio("Derezzed");
                    sonido.playChangePause();
                    break;
                case Clase.M4PART2:
                    EjemploAlumno.workspace().music.chequearCambio("M4 Part 2");
                    sonido.playChangePause();
                    break;
                case Clase.METHEME:
                    EjemploAlumno.workspace().music.chequearCambio("ME Theme");
                    sonido.playChangePause();
                    break;
                case Clase.NEWWORLDS:
                    EjemploAlumno.workspace().music.chequearCambio("New Worlds");
                    sonido.playChangePause();
                    break;
                case Clase.SOLARSAILER:
                    EjemploAlumno.workspace().music.chequearCambio("Solar Sailer");
                    sonido.playChangePause();
                    break;
                case Clase.SPECTRE:
                    EjemploAlumno.workspace().music.chequearCambio("Spectre");
                    sonido.playChangePause();
                    break;
                case Clase.TALI:
                    EjemploAlumno.workspace().music.chequearCambio("Tali");
                    sonido.playChangePause();
                    break;
                case Clase.TSOF:
                    EjemploAlumno.workspace().music.chequearCambio("The Son of Flynn");
                    sonido.playChangePause();
                    break;
                case Clase.TRONENDING:
                    EjemploAlumno.workspace().music.chequearCambio("Tron Ending");
                    sonido.playChangePause();
                    break;
            }
        }
        private void chequearBackSpace()
        {
            switch (menuActivo.anterior)
            {
                case Clase.NULL:
                    //Retornamos al ejemplo
                    EjemploAlumno.workspace().config = false;
                    sonido.playPauseBackgound();
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
        #endregion


        #region Operar
        public void operar(float elapsedTime)
        {
            if (fondoAnimado) miMundo.render(elapsedTime);
         
            GuiController.Instance.Drawer2D.beginDrawSprite();
            if (fondoAnimado & !verFondo) fondoPrincipal.render();
            if (!fondoAnimado) fondoAlternativo.render();
            GuiController.Instance.Drawer2D.endDrawSprite();
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
            if (!verFondo)
            {
                GuiController.Instance.Drawer2D.beginDrawSprite();
                puntero.render();
                foreach (var item in menuActivo.listaOpciones) { renderSegunEstado(item); }
                GuiController.Instance.Drawer2D.endDrawSprite();
            }
            if (input.keyPressed(Key.Up)) { if (!verFondo) { if (opcionElegida > 1) { opcionElegida--; reajustarPuntero(); sonido.playSlideButton(); } } else verFondo = false; }
            if (input.keyPressed(Key.Down)) { if(!verFondo){if (opcionElegida < menuActivo.listaOpciones.Count - 1) { opcionElegida++; reajustarPuntero(); sonido.playSlideButton(); } } else verFondo = false;  }
            if (input.keyPressed(Key.Return)) { if(!verFondo){chequearReturn(); } else verFondo = false;  }
            if (input.keyPressed(Key.BackSpace)) { if (!verFondo) { chequearBackSpace(); sonido.playSlidePanel(); } else verFondo = false; }
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
                    renderSegunEstado(menuTransicion.listaOpciones[i]);
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
                renderSegunEstado(menuActivo.listaOpciones[i]);
            }
            GuiController.Instance.Drawer2D.endDrawSprite();
            if (contadorEntrada <= 0) { entradaFin = true; velocidadEfecto = 0.1f; }
        }
        #endregion
    }
}

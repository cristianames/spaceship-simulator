using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TgcViewer;
using TgcViewer.Utils.Sound;

namespace AlumnoEjemplos.THE_GRID
{
    public class Musique
    {
        enum TipoModo { CASTOR, DEREZZED, M4PART2, TRONENDING, TSOF, TALI, SPECTRE, SOLARSAILER, NEWWORLDS, METHEME, NONE, COMPLETE };
        private TipoModo escenarioActual = TipoModo.NONE;
        private List<string> listaTemas = new List<string>() 
        {   "Castor.mp3", 
            "Derezzed.mp3",
            "M4Part2.mp3",
            "METheme.mp3",
            "NewWorlds.mp3",
            "SolarSailer.mp3",
            "Spectre.mp3",
            "Tali.mp3",
            "TheSonofFlynn.mp3",
            "TronEnding.mp3"
        };
        private int pistaActual;
        private string cancionActual = "Sin Musica";
        private List<int> listaPistas;

        TgcMp3Player playerMP3 = GuiController.Instance.Mp3Player;
        TgcStaticSound lazer = new TgcStaticSound();
        TgcStaticSound lazer2_carga = new TgcStaticSound();
        TgcStaticSound lazer2_disparo = new TgcStaticSound();
        TgcStaticSound asteroideColision = new TgcStaticSound();
        TgcStaticSound asteroideFragmentacion = new TgcStaticSound();
        TgcStaticSound asteroideImpacto = new TgcStaticSound();
        TgcStaticSound warp_time = new TgcStaticSound();
        TgcStaticSound slideButton = new TgcStaticSound();
        TgcStaticSound slidePanel = new TgcStaticSound();
        TgcStaticSound changePause = new TgcStaticSound();
        TgcStaticSound navePropulsion = new TgcStaticSound();
        TgcStaticSound navePropulsionContinua = new TgcStaticSound();
        TgcStaticSound exitMenu = new TgcStaticSound();
        TgcStaticSound failButton = new TgcStaticSound();

        public Musique()
        {
            lazer.loadSound(EjemploAlumno.TG_Folder + "Music\\laser_shot.wav");
            lazer2_carga.loadSound(EjemploAlumno.TG_Folder + "Music\\laser2_carga.wav");
            lazer2_disparo.loadSound(EjemploAlumno.TG_Folder + "Music\\laser2.wav");
            asteroideColision.loadSound(EjemploAlumno.TG_Folder + "Music\\Asteroide\\colision_1.wav");
            asteroideFragmentacion.loadSound(EjemploAlumno.TG_Folder + "Music\\Asteroide\\fragmentacion_1.wav");
            asteroideImpacto.loadSound(EjemploAlumno.TG_Folder + "Music\\Asteroide\\impacto_1.wav");
            warp_time.loadSound(EjemploAlumno.TG_Folder + "Music\\warp.wav");
            slideButton.loadSound(EjemploAlumno.TG_Folder + "Music\\Menu\\Slide.wav");
            slidePanel.loadSound(EjemploAlumno.TG_Folder + "Music\\Menu\\Select.wav");
            changePause.loadSound(EjemploAlumno.TG_Folder + "Music\\Menu\\Pause.wav");
            navePropulsion.loadSound(EjemploAlumno.TG_Folder + "Music\\propulsion.wav");
            navePropulsionContinua.loadSound(EjemploAlumno.TG_Folder + "Music\\propulsionContinua.wav");
            exitMenu.loadSound(EjemploAlumno.TG_Folder + "Music\\Menu\\ExitMenu.wav");
            failButton.loadSound(EjemploAlumno.TG_Folder + "Music\\Menu\\FailButton.wav");
        }

        /// <summary>
        /// Retorna el titulo de la musica de fondo
        /// </summary>
        public string getActual()
        {
            return cancionActual;
        }
        /// <summary>
        /// Reproducir la musica de fondo en loop si esta activada
        /// </summary>
        public void playBackgound(bool loop)
        {
            if (EjemploAlumno.workspace().musicaActivada) playerMP3.play(loop);
        }
        /// <summary>
        /// Reproducir la musica de fondo
        /// </summary>
        public void playPauseBackgound()
        {
            lazer2_carga.SoundBuffer.Stop();
            lazer2_carga.SoundBuffer.SetCurrentPosition(0); //Para solucionar un bug de cuando carga el laser azul
            stopPropulsion();
            if (!EjemploAlumno.workspace().musicaActivada) { playerMP3.pause(); return; }
            if (playerMP3.getStatus() == TgcMp3Player.States.Playing) { playerMP3.pause(); cancionActual = "Sin Musica"; return; }
            if (playerMP3.getStatus() == TgcMp3Player.States.Paused) { playerMP3.resume(); cancionActual = listaTemas[pistaActual]; return; }
        }
        /// <summary>
        /// Reproducir la musica de fondo si estaba en Pausa
        /// </summary>
        public void onlyResumeBackground()
        {
            if (!EjemploAlumno.workspace().musicaActivada) { playerMP3.pause(); return; }
            if (playerMP3.getStatus() == TgcMp3Player.States.Paused) { playerMP3.resume(); return; }
        }
        public void onlyStopBackground()
        {
            if (!EjemploAlumno.workspace().musicaActivada) { playerMP3.pause(); return; }
            if (playerMP3.getStatus() == TgcMp3Player.States.Playing) { playerMP3.pause(); return; }
        }
        /// <summary>
        /// Reproducir la musica de carga del laser rojo
        /// </summary>
        public void playLazer()
        {
            lazer.SoundBuffer.Stop();
            lazer.SoundBuffer.SetCurrentPosition(0);
            if (EjemploAlumno.workspace().musicaActivada) { lazer.play(); };
        }
        /// <summary>
        /// Reproducir la musica de carga del laser azul
        /// </summary>
        public void playLazerCarga()
        {
            if (EjemploAlumno.workspace().musicaActivada) { lazer2_carga.play(true); }
        }
        /// <summary>
        /// Reproducir la musica de disparo del laser azul deteniendo la de carga
        /// </summary>
        public void playLazer2()
        {
            lazer2_carga.SoundBuffer.Stop();
            lazer2_carga.SoundBuffer.SetCurrentPosition(0);
            lazer2_disparo.SoundBuffer.Stop();
            lazer2_disparo.SoundBuffer.SetCurrentPosition(0);
            if (EjemploAlumno.workspace().musicaActivada) { lazer2_disparo.play(); }
        }
        /// <summary>
        /// Reproducir la musica del warp
        /// </summary>
        public void playWarp()
        {
            if (EjemploAlumno.workspace().musicaActivada) { warp_time.play(true); }
        }
        /// <summary>
        /// Detener la musica del warp
        /// </summary>
        public void stopWarp()
        {
            warp_time.SoundBuffer.Stop();
            warp_time.SoundBuffer.SetCurrentPosition(0);
        }
        /// <summary>
        /// Reproducir el sonido de colision
        /// </summary>
        public void playAsteroideColision()
        {
            asteroideColision.SoundBuffer.Stop();
            asteroideColision.SoundBuffer.SetCurrentPosition(0);
            if (EjemploAlumno.workspace().musicaActivada) { asteroideColision.play(); }
        }
        /// <summary>
        /// Reproducir el sonido de fragmentacion de asteroides
        /// </summary>
        public void playAsteroideFragmentacion()
        {
            asteroideFragmentacion.SoundBuffer.Stop();
            asteroideFragmentacion.SoundBuffer.SetCurrentPosition(0);
            if (EjemploAlumno.workspace().musicaActivada) { asteroideFragmentacion.play(); }
        }
        /// <summary>
        /// Reproducir el sonido de impacto de asteroides
        /// </summary>
        public void playAsteroideImpacto()
        {
            asteroideImpacto.SoundBuffer.Stop();
            asteroideImpacto.SoundBuffer.SetCurrentPosition(0);
            if (EjemploAlumno.workspace().musicaActivada) { asteroideImpacto.play(); }
        }
        internal void playDeniedPress()
        {
            failButton.SoundBuffer.Stop();
            failButton.SoundBuffer.SetCurrentPosition(0);
            if (EjemploAlumno.workspace().musicaActivada) { failButton.play(); }
        }
        internal void playExitMenu()
        {
            exitMenu.SoundBuffer.Stop();
            exitMenu.SoundBuffer.SetCurrentPosition(50000);
            if (EjemploAlumno.workspace().musicaActivada) { exitMenu.play(); }
        }
        /// <summary>
        /// Reproducir el sonido del boton del config
        /// </summary>
        public void playSlideButton()
        {
            slideButton.SoundBuffer.Stop();
            slideButton.SoundBuffer.SetCurrentPosition(0);
            if (EjemploAlumno.workspace().musicaActivada) { slideButton.play(); }
        }
        /// <summary>
        /// Reproducir el sonido del cambio de panel del config
        /// </summary>
        public void playSlidePanel()
        {
            slidePanel.SoundBuffer.Stop();
            slidePanel.SoundBuffer.SetCurrentPosition(0);
            if (EjemploAlumno.workspace().musicaActivada) { slidePanel.play(); }
        }
        /// </summary>
        /// Reproducir sonido de cambio de pausa
        /// </summary>
        public void playChangePause()
        {
            changePause.SoundBuffer.Stop();
            changePause.SoundBuffer.SetCurrentPosition(0);
            if (EjemploAlumno.workspace().musicaActivada) { changePause.play(); }
        }
        float propulsion_t = 0;
        /// <summary>
        /// Reproducir el sonido de los propulsores de la nave
        /// </summary>
        public void playPropulsion(float tiempo)
        {
            if (EjemploAlumno.workspace().musicaActivada)
            {
                propulsion_t += tiempo;
                if (propulsion_t < 2)
                {
                    navePropulsion.play();
                }
                else
                {
                    navePropulsionContinua.play();
                }
            }
        }
        /// <summary>
        /// Detener el sonido de los propulsores de la nave
        /// </summary>
        public void stopPropulsion()
        {
            navePropulsion.SoundBuffer.Stop();
            navePropulsion.SoundBuffer.SetCurrentPosition(0);
            navePropulsionContinua.SoundBuffer.Stop();
            navePropulsionContinua.SoundBuffer.SetCurrentPosition(0);
            propulsion_t = 0;

        }
        /// </summary>
        /// Dispose a todos los sonidos
        /// </summary>
        internal void liberarRecursos()
        {
            playerMP3.closeFile();
            lazer.dispose();
            lazer2_carga.dispose();
            lazer2_disparo.dispose();
            asteroideColision.dispose();
            asteroideFragmentacion.dispose();
            asteroideImpacto.dispose();
            warp_time.dispose();
        }
        /// </summary>
        /// Reproduce una musica de fondo aleatoria
        /// </summary>
        internal void refrescar()
        {
            listaPistas = new List<int>();
            for (int i = 0; i < listaTemas.Count; i++)
            {
                if (i != pistaActual) listaPistas.Add(i);
            }
            nuevaPista(Factory.elementoRandom<int>(listaPistas),false);

        }
        /// </summary>
        /// Carga una nueva pista
        /// </summary>
        internal void nuevaPista(int pista, bool loop) 
        {
            playerMP3.closeFile();
            playerMP3.FileName = EjemploAlumno.TG_Folder + "Music\\Main\\" + listaTemas[pista];
            playBackgound(loop);
            pistaActual = pista;
            cancionActual = listaTemas[pista];
        }
        /// </summary>
        /// Segun un string con el nombre de la cancion, cambia el tema
        /// </summary>
        internal void chequearCambio(string opcionElegida) //Switch de comportamiento de los diferentes temas
        {
            switch (opcionElegida)
            {
                case "Lista Completa":
                    refrescar();
                    if (escenarioActual != TipoModo.COMPLETE) escenarioActual = TipoModo.COMPLETE;
                    break;
                case "Sin Musica":
                        playerMP3.stop();
                        escenarioActual = TipoModo.NONE;
                    cancionActual = "Sin Musica";
                    return;
                case "Castor":
                        nuevaPista(0,true);
                        escenarioActual = TipoModo.CASTOR;
                    break;
                case "Derezzed":
                        nuevaPista(1, true);
                        escenarioActual = TipoModo.DEREZZED;
                    break;
                case "M4 Part 2":
                        nuevaPista(2, true);
                        escenarioActual = TipoModo.M4PART2;
                    break;
                case "ME Theme":
                        nuevaPista(3, true);
                        escenarioActual = TipoModo.METHEME;
                    break;
                case "New Worlds":
                        nuevaPista(4, true);
                        escenarioActual = TipoModo.NEWWORLDS;
                    break;
                case "Solar Sailer":
                        nuevaPista(5, true);
                        escenarioActual = TipoModo.SOLARSAILER;
                    break;
                case "Spectre":
                        nuevaPista(6, true);
                        escenarioActual = TipoModo.SPECTRE;
                    break;
                case "Tali":
                        nuevaPista(7, true);
                        escenarioActual = TipoModo.TALI;
                    break;
                case "The Son of Flynn":
                        nuevaPista(8, true);
                        escenarioActual = TipoModo.TSOF;
                    break;
                case "Tron Ending":
                        nuevaPista(9, true);
                        escenarioActual = TipoModo.TRONENDING;
                    break;
            }
            cancionActual = listaTemas[pistaActual];
        }
    }
}

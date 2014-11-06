using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TgcViewer;
using TgcViewer.Utils.Sound;

namespace AlumnoEjemplos.TheGRID
{
    public class Musique
    {
        enum TipoModo { CASTOR, DEREZZED, M4PART2, TRONENDING, TSOF, TALI, SPECTRE, SOLARSAILER, NEWWORLDS, METHEME, NONE, COMPLETE };
        private TipoModo escenarioActual = TipoModo.NONE;
        private List<string> listaTemas = new List<string>() 
        {   "Music\\Main\\Castor.mp3", 
            "Music\\Main\\Derezzed.mp3",
            "Music\\Main\\M4Part2.mp3",
            "Music\\Main\\METheme.mp3",
            "Music\\Main\\NewWorlds.mp3",
            "Music\\Main\\SolarSailer.mp3",
            "Music\\Main\\Spectre.mp3",
            "Music\\Main\\Tali.mp3",
            "Music\\Main\\TheSonofFlynn.mp3",
            "Music\\Main\\TronEnding.mp3"
        };
        private int pistaActual;
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
            exitMenu.loadSound(EjemploAlumno.TG_Folder + "Music\\Menu\\ExitMenu.wav");
            failButton.loadSound(EjemploAlumno.TG_Folder + "Music\\Menu\\FailButton.wav");
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
            if (!EjemploAlumno.workspace().musicaActivada) { playerMP3.pause(); return; }
            if (playerMP3.getStatus() == TgcMp3Player.States.Playing) { playerMP3.pause(); return; }
            if (playerMP3.getStatus() == TgcMp3Player.States.Paused) { playerMP3.resume(); return; }
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
        public void playAsteroideColision()
        {
            asteroideColision.SoundBuffer.Stop();
            asteroideColision.SoundBuffer.SetCurrentPosition(0);
            if (EjemploAlumno.workspace().musicaActivada) { asteroideColision.play(); }
        }
        public void playAsteroideFragmentacion()
        {
            asteroideFragmentacion.SoundBuffer.Stop();
            asteroideFragmentacion.SoundBuffer.SetCurrentPosition(0);
            if (EjemploAlumno.workspace().musicaActivada) { asteroideFragmentacion.play(); }
        }
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
        public void playSlideButton()
        {
            slideButton.SoundBuffer.Stop();
            slideButton.SoundBuffer.SetCurrentPosition(0);
            if (EjemploAlumno.workspace().musicaActivada) { slideButton.play(); }
        }
        public void playSlidePanel()
        {
            slidePanel.SoundBuffer.Stop();
            slidePanel.SoundBuffer.SetCurrentPosition(0);
            if (EjemploAlumno.workspace().musicaActivada) { slidePanel.play(); }
        }
        public void playChangePause()
        {
            changePause.SoundBuffer.Stop();
            changePause.SoundBuffer.SetCurrentPosition(0);
            if (EjemploAlumno.workspace().musicaActivada) { changePause.play(); }
        }

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

        internal void refrescar()
        {
            listaPistas = new List<int>();
            for (int i = 0; i < listaTemas.Count; i++)
            {
                if (i != pistaActual) listaPistas.Add(i);
            }
            nuevaPista(Factory.elementoRandom<int>(listaPistas),false);

        }
        internal void nuevaPista(int pista, bool loop) //Cargar nueva cancion
        {
            playerMP3.closeFile();
            playerMP3.FileName = EjemploAlumno.TG_Folder + listaTemas[pista];
            playBackgound(loop);
            pistaActual = pista;
        }

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
                    break;
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
        }

    }
}

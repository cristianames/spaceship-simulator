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

        public Musique()
        {
            lazer.loadSound(EjemploAlumno.TG_Folder + "Music\\laser_shot.wav");
            lazer2_carga.loadSound(EjemploAlumno.TG_Folder + "Music\\laser2_carga.wav");
            lazer2_disparo.loadSound(EjemploAlumno.TG_Folder + "Music\\laser2.wav");
            asteroideColision.loadSound(EjemploAlumno.TG_Folder + "Music\\Asteroide\\colision_1.wav");
            asteroideFragmentacion.loadSound(EjemploAlumno.TG_Folder + "Music\\Asteroide\\fragmentacion_1.wav");
            asteroideImpacto.loadSound(EjemploAlumno.TG_Folder + "Music\\Asteroide\\impacto_1.wav");
            warp_time.loadSound(EjemploAlumno.TG_Folder + "Music\\warp.wav");
        }


        public void playBackgound(bool loop)
        {
            playerMP3.play(loop);
        }
        public void playPauseBackgound()
        {
            if (playerMP3.getStatus() == TgcMp3Player.States.Playing) { playerMP3.pause(); return; }
            if (playerMP3.getStatus() == TgcMp3Player.States.Paused) { playerMP3.resume(); return; }
        }
        public void playLazer()
        {
            lazer.SoundBuffer.Stop();
            lazer.SoundBuffer.SetCurrentPosition(0);
            lazer.play();
        }
        public void playLazerCarga()
        {
            lazer2_carga.play(true);
        }
        public void playLazer2()
        {
            lazer2_carga.SoundBuffer.Stop();
            lazer2_carga.SoundBuffer.SetCurrentPosition(0);
            lazer2_disparo.SoundBuffer.Stop();
            lazer2_disparo.SoundBuffer.SetCurrentPosition(0);
            lazer2_disparo.play();
        }

        public void playWarp()
        {
            warp_time.play(true);
        }
        public void stopWarp()
        {
            warp_time.SoundBuffer.Stop();
            warp_time.SoundBuffer.SetCurrentPosition(0);
        }
        public void playAsteroideColision()
        {
            asteroideColision.SoundBuffer.Stop();
            asteroideColision.SoundBuffer.SetCurrentPosition(0);
            asteroideColision.play();
        }
        public void playAsteroideFragmentacion()
        {
            asteroideFragmentacion.SoundBuffer.Stop();
            asteroideFragmentacion.SoundBuffer.SetCurrentPosition(0);
            asteroideFragmentacion.play();
        }
        public void playAsteroideImpacto()
        {
            asteroideImpacto.SoundBuffer.Stop();
            asteroideImpacto.SoundBuffer.SetCurrentPosition(0);
            asteroideImpacto.play();
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
        internal void nuevaPista(int pista, bool loop)
        {
            playerMP3.closeFile();
            playerMP3.FileName = EjemploAlumno.TG_Folder + listaTemas[pista];
            playBackgound(loop);
            pistaActual = pista;
        }

        internal void chequearCambio(string opcionElegida)
        {
            switch (opcionElegida)
            {
                case "Lista Completa":
                    if (playerMP3.getStatus() == TgcMp3Player.States.Stopped) refrescar();
                    if (escenarioActual != TipoModo.COMPLETE) escenarioActual = TipoModo.COMPLETE;
                    break;
                case "Sin Musica":
                    if (escenarioActual != TipoModo.NONE)
                    {
                        playerMP3.stop();
                        escenarioActual = TipoModo.NONE;
                    }
                    break;
                case "Castor":
                    if (escenarioActual != TipoModo.CASTOR)
                    {
                        nuevaPista(0,true);
                        escenarioActual = TipoModo.CASTOR;
                    }
                    break;
                case "Derezzed":
                    if (escenarioActual != TipoModo.DEREZZED)
                    {
                        nuevaPista(1, true);
                        escenarioActual = TipoModo.DEREZZED;
                    }
                    break;
                case "M4 Part 2":
                    if (escenarioActual != TipoModo.M4PART2)
                    {
                        nuevaPista(2, true);
                        escenarioActual = TipoModo.M4PART2;
                    }
                    break;
                case "ME Theme":
                    if (escenarioActual != TipoModo.METHEME)
                    {
                        nuevaPista(3, true);
                        escenarioActual = TipoModo.METHEME;
                    }
                    break;
                case "New Worlds":
                    if (escenarioActual != TipoModo.NEWWORLDS)
                    {
                        nuevaPista(4, true);
                        escenarioActual = TipoModo.NEWWORLDS;
                    }
                    break;
                case "Solar Sailer":
                    if (escenarioActual != TipoModo.SOLARSAILER)
                    {
                        nuevaPista(5, true);
                        escenarioActual = TipoModo.SOLARSAILER;
                    }
                    break;
                case "Spectre":
                    if (escenarioActual != TipoModo.SPECTRE)
                    {
                        nuevaPista(6, true);
                        escenarioActual = TipoModo.SPECTRE;
                    }
                    break;
                case "Tali":
                    if (escenarioActual != TipoModo.TALI)
                    {
                        nuevaPista(7, true);
                        escenarioActual = TipoModo.TALI;
                    }
                    break;
                case "The Son of Flynn":
                    if (escenarioActual != TipoModo.TSOF)
                    {
                        nuevaPista(8, true);
                        escenarioActual = TipoModo.TSOF;
                    }
                    break;
                case "Tron Ending":
                    if (escenarioActual != TipoModo.TRONENDING)
                    {
                        nuevaPista(9, true);
                        escenarioActual = TipoModo.TRONENDING;
                    }
                    break;
            }
        }
    }
}

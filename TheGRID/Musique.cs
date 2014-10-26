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
        enum TipoModo { CASTOR, DEREZZED, M4PART2, TRONENDING, TSOF, TALI, SPECTRE, SOLARSAILER, NEWWORLDS, METHEME, NONE };
        private TipoModo escenarioActual = TipoModo.NONE;
        List<TgcMp3Player> listaMainMusic = new List<TgcMp3Player>(10);

        TgcMp3Player playerMP3 = GuiController.Instance.Mp3Player;
        TgcStaticSound lazer = new TgcStaticSound();
        TgcStaticSound asteroideColision = new TgcStaticSound();
        TgcStaticSound asteroideFragmentacion = new TgcStaticSound();
        TgcStaticSound asteroideImpacto = new TgcStaticSound();
        public Musique()
        {
            //playerMP3.closeFile();
            //playerMP3.FileName = EjemploAlumno.TG_Folder + "Music\\Main\\main.mp3";
            lazer.loadSound(EjemploAlumno.TG_Folder + "Music\\laser_shot.wav");
            asteroideColision.loadSound(EjemploAlumno.TG_Folder + "Music\\Asteroide\\colision_1.wav");
            asteroideFragmentacion.loadSound(EjemploAlumno.TG_Folder + "Music\\Asteroide\\fragmentacion_1.wav");
            asteroideImpacto.loadSound(EjemploAlumno.TG_Folder + "Music\\Asteroide\\impacto_1.wav");
        }


        public void playBackgound()
        {
            playerMP3.play(true);
        }
        public void playLazer()
        {
            lazer.SoundBuffer.Stop();
            lazer.SoundBuffer.SetCurrentPosition(0);
            lazer.play();
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
            asteroideColision.dispose();
            asteroideFragmentacion.dispose();
        }

        internal void chequearCambio(string opcionElegida)
        {
            switch (opcionElegida)
            {
                case "Castor":
                    if (escenarioActual != TipoModo.CASTOR)
                    {
                        playerMP3.closeFile();
                        playerMP3.FileName = EjemploAlumno.TG_Folder + "Music\\Main\\Castor.mp3";
                        playBackgound();
                        escenarioActual = TipoModo.CASTOR;
                    }
                    break;
                case "Derezzed":
                    if (escenarioActual != TipoModo.DEREZZED)
                    {
                        playerMP3.closeFile();
                        playerMP3.FileName = EjemploAlumno.TG_Folder + "Music\\Main\\Derezzed.mp3";
                        playBackgound();
                        escenarioActual = TipoModo.DEREZZED;
                    }
                    break;
                case "M4 Part 2":
                    if (escenarioActual != TipoModo.M4PART2)
                    {
                        playerMP3.closeFile();
                        playerMP3.FileName = EjemploAlumno.TG_Folder + "Music\\Main\\M4Part2.mp3";
                        playBackgound();
                        escenarioActual = TipoModo.M4PART2;
                    }
                    break;
                case "ME Theme":
                    if (escenarioActual != TipoModo.METHEME)
                    {
                        playerMP3.closeFile();
                        playerMP3.FileName = EjemploAlumno.TG_Folder + "Music\\Main\\METheme.mp3";
                        playBackgound();
                        escenarioActual = TipoModo.METHEME;
                    }
                    break;
                case "New Worlds":
                    if (escenarioActual != TipoModo.NEWWORLDS)
                    {
                        playerMP3.closeFile();
                        playerMP3.FileName = EjemploAlumno.TG_Folder + "Music\\Main\\NewWorlds.mp3";
                        playBackgound();
                        escenarioActual = TipoModo.NEWWORLDS;
                    }
                    break;
                case "Solar Sailer":
                    if (escenarioActual != TipoModo.SOLARSAILER)
                    {
                        playerMP3.closeFile();
                        playerMP3.FileName = EjemploAlumno.TG_Folder + "Music\\Main\\SolarSailer.mp3";
                        playBackgound();
                        escenarioActual = TipoModo.SOLARSAILER;
                    }
                    break;
                case "Spectre":
                    if (escenarioActual != TipoModo.SPECTRE)
                    {
                        playerMP3.closeFile();
                        playerMP3.FileName = EjemploAlumno.TG_Folder + "Music\\Main\\Spectre.mp3";
                        playBackgound();
                        escenarioActual = TipoModo.SPECTRE;
                    }
                    break;
                case "Tali":
                    if (escenarioActual != TipoModo.TALI)
                    {
                        playerMP3.closeFile();
                        playerMP3.FileName = EjemploAlumno.TG_Folder + "Music\\Main\\Tali.mp3";
                        playBackgound();
                        escenarioActual = TipoModo.TALI;
                    }
                    break;
                case "The Son of Flynn":
                    if (escenarioActual != TipoModo.TSOF)
                    {
                        playerMP3.closeFile();
                        playerMP3.FileName = EjemploAlumno.TG_Folder + "Music\\Main\\TheSonofFlynn.mp3";
                        playBackgound();
                        escenarioActual = TipoModo.TSOF;
                    }
                    break;
                case "Tron Ending":
                    if (escenarioActual != TipoModo.TRONENDING)
                    {
                        playerMP3.closeFile();
                        playerMP3.FileName = EjemploAlumno.TG_Folder + "Music\\Main\\TronEnding.mp3";
                        playBackgound();
                        escenarioActual = TipoModo.TRONENDING;
                    }
                    break;
            }
        }
    }
}

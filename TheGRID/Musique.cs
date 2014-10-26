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
        TgcMp3Player playerMP3 = GuiController.Instance.Mp3Player;
        TgcStaticSound lazer = new TgcStaticSound();
        TgcStaticSound asteroideColision = new TgcStaticSound();
        TgcStaticSound asteroideFragmentacion = new TgcStaticSound();
        TgcStaticSound asteroideImpacto = new TgcStaticSound();
        public Musique()
        {
            playerMP3.closeFile();
            playerMP3.FileName = EjemploAlumno.TG_Folder + "Music\\Main\\main.mp3";
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
    }
}

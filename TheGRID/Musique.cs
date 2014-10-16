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
        public Musique()
        {
            playerMP3.closeFile();
            playerMP3.FileName = EjemploAlumno.TG_Folder + "Music\\main.mp3";
            lazer.loadSound(EjemploAlumno.TG_Folder + "Music\\laser_shot.wav");
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

        internal void liberarRecursos()
        {
            playerMP3.closeFile();
            lazer.dispose();
            //throw new NotImplementedException();
        }
    }
}

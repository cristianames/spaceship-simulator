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
        public Musique()
        {
            playerMP3.closeFile();
            playerMP3.FileName = EjemploAlumno.TG_Folder + "Music\\main.mp3";
        }

        public void playBackgound()
        {
            playerMP3.play(true);
        }
    }
}

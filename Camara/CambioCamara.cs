using Microsoft.DirectX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TgcViewer;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSceneLoader;

namespace AlumnoEjemplos.TheGRID.Camara
{
    class CambioCamara
    {
        enum TipoModo {FPS, TPS, Exterior};
        private TipoModo modo = TipoModo.TPS; 
        private Dibujable objeto_foco;

        public Boolean soyFPS(){ if(modo == TipoModo.FPS) return true; else return false;}
        public CambioCamara(Dibujable dibujable)
        {
            objeto_foco = dibujable;
            cambiarPosicionCamara();

        }
        public void modoFPS()
        {
            modo = TipoModo.FPS;
            moverFirst();
        }
        public void modoTPS()
        {
            modo = TipoModo.TPS;
            moverThird();
        }
        public void modoExterior()
        {
            habilitarExterior();
        }
        private void moverThird()
        {
            Vector3 posicionDeCamara = objeto_foco.getPosicion();
            Vector3 temp = objeto_foco.getDireccion();
            temp.Multiply(100);
            posicionDeCamara -= temp;
            temp = new Vector3(0, 50, 0);
            posicionDeCamara += temp;
            temp = objeto_foco.getDireccion_Y();
            temp.Multiply(15);
            GuiController.Instance.setCamera(posicionDeCamara, objeto_foco.getPosicion() + temp);
        }
        private void moverFirst()
        {
            GuiController.Instance.setCamera(objeto_foco.getPosicion() - objeto_foco.getDireccion(), objeto_foco.getPosicion());

        }
        private void habilitarExterior()
        {

        }
        public void cambiarFoco(Dibujable dibujable){
            objeto_foco = dibujable;
            cambiarPosicionCamara();
        }

        public void cambiarPosicionCamara()
        {
            switch (modo)
            {
                case TipoModo.FPS:
                    moverFirst();
                    break;
                case TipoModo.TPS:
                    moverThird();
                    break;
                case TipoModo.Exterior:
                    habilitarExterior();
                    break;
            }
            GuiController.Instance.CurrentCamera.updateCamera();
        }
    }
}

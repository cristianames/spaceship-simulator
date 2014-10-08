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
    public class CambioCamara
    {
        enum TipoModo {FPS, TPS, Exterior};
        private TipoModo modo = TipoModo.TPS; 
        private Dibujable objeto_foco;

        public Boolean soyFPS(){ if(modo == TipoModo.FPS) return true; else return false;}
        public CambioCamara(Dibujable dibujable)
        {
            cambiarFoco(dibujable);
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
            modo = TipoModo.Exterior;
            GuiController.Instance.CurrentCamera.Enable = false;
        }
        private void moverThird()
        {
            Vector3 posicionDeCamara = objeto_foco.getPosicion();
            Vector3 temp = objeto_foco.getDireccion();
            temp.Multiply(100);
            posicionDeCamara -= temp;
            posicionDeCamara += new Vector3(0, 20, 0);
            GuiController.Instance.setCamera(posicionDeCamara, objeto_foco.getPosicion() + temp);
        }
        private void moverFirst()
        {
            Vector3 posicionDeCamara = objeto_foco.getPosicion();
            Vector3 temp = objeto_foco.getDireccion_Y();
            temp.Multiply(5);
            posicionDeCamara += temp;
            Vector3 focoCamara = objeto_foco.getPosicion();
            temp = objeto_foco.getDireccion();
            temp.Multiply(500);
            focoCamara += temp;

            GuiController.Instance.setCamera(posicionDeCamara, focoCamara);
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
                    break;
            }
            GuiController.Instance.CurrentCamera.updateCamera();
        }
        public void chequearCambio(string opcion) {
            switch (opcion)
            {
                case "Tercera Persona":
                    if(modo != TipoModo.TPS)
                        modoTPS();
                    break;
                case "Camara FPS":
                    if (modo != TipoModo.FPS)
                        modoFPS();
                    break;
                case "Libre":
                    if (modo != TipoModo.Exterior)
                        modoExterior();
                    break;
            }
        }
        public string estado()
        {
            switch (modo)
            {
                case TipoModo.TPS:
                    return "Tercera Persona";
                case TipoModo.FPS:
                    return "Camara FPS";
                case TipoModo.Exterior:
                    return "Libre";
            }
            return "";
        }
    }
}

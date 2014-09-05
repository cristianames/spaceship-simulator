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

        private Boolean FPSon = false;
        private Dibujable objecto_foco;

        public Boolean getMode(){ return FPSon;}
        public CambioCamara(Dibujable dibujable)
        {
            objecto_foco = dibujable;
            if (FPSon) {habilitarFirst();}
            else {habilitarThird();}
        }
        public void modoFPS()
        {
            GuiController.Instance.ThirdPersonCamera.Enable = false;
            habilitarFirst();
            FPSon = true;
        }
        public void modoTPS()
        {
            GuiController.Instance.FpsCamera.Enable = false;
            habilitarThird();
            FPSon = false;
        }
        private void habilitarThird()
        {
            GuiController.Instance.ThirdPersonCamera.Enable = true;
            GuiController.Instance.ThirdPersonCamera.setCamera(objecto_foco.getPosicion(), 75, -300);
        }
        private void habilitarFirst()
        {
            GuiController.Instance.FpsCamera.Enable = true;
            GuiController.Instance.FpsCamera.setCamera(objecto_foco.getPosicion(), -objecto_foco.getDireccion());
        }
        public void cambiarFoco(Dibujable dibujable){
            objecto_foco = dibujable;
            cambiarPosicionCamara(dibujable);
        }

        public void cambiarPosicionCamara(Dibujable dibujable)
        {
            if (FPSon)
            {
                GuiController.Instance.FpsCamera.setCamera(dibujable.getPosicion(), -dibujable.getDireccion());
            }
            else
            {
                GuiController.Instance.ThirdPersonCamera.Target = dibujable.getPosicion();

            }
        }

    }
}

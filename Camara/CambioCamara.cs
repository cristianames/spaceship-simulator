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
            switch (modo)
            {
                case TipoModo.FPS:
                    habilitarFirst();
                    break;
                case TipoModo.TPS:
                    habilitarThird();
                    break;
                case TipoModo.Exterior:
                    habilitarExterior();
                    break;
            }
        }
        public void modoFPS()
        {
            GuiController.Instance.RotCamera.Enable = false;
            habilitarFirst();
        }
        public void modoTPS()
        {
            GuiController.Instance.RotCamera.Enable = false;
            habilitarThird();
        }
        public void modoExterior()
        {
            GuiController.Instance.ThirdPersonCamera.Enable = false;
            habilitarExterior();
        }
        private void habilitarThird()
        {
            modo = TipoModo.TPS;
            GuiController.Instance.ThirdPersonCamera.Enable = true;
            GuiController.Instance.ThirdPersonCamera.setCamera(objeto_foco.getCentro(), 200, -300);
        }
        private void habilitarFirst()
        {   
            modo = TipoModo.FPS;
            GuiController.Instance.ThirdPersonCamera.Enable = true;
            GuiController.Instance.ThirdPersonCamera.setCamera(objeto_foco.getCentro(), 0, -0.1f);
        }
        private void habilitarExterior()
        {
            modo = TipoModo.Exterior;
            GuiController.Instance.RotCamera.Enable = true;
            GuiController.Instance.RotCamera.CameraCenter = objeto_foco.getCentro();
        }
        public void cambiarFoco(Dibujable dibujable){
            objeto_foco = dibujable;
            cambiarPosicionCamara();
        }

        public void cambiarPosicionCamara()
        {
            if (modo != TipoModo.Exterior)
            {
                GuiController.Instance.ThirdPersonCamera.Target = objeto_foco.getCentro();
                if (objeto_foco.giro != 0)
                {
                    //float angulo = FastMath.Acos(dotProduct(objeto_foco.getDireccion(), objeto_foco.getDireccionAnterior()));
                    float dot = dotProduct(objeto_foco.getDireccion(), objeto_foco.getDireccionAnterior());
                    float cross = crossProduct(objeto_foco.getDireccion(), objeto_foco.getDireccionAnterior()).Length();
                    float angulo = (float) Math.Atan2(cross, dot);
                    if (objeto_foco.giro < 0) GuiController.Instance.ThirdPersonCamera.rotateY(-angulo);
                    if (objeto_foco.giro > 0) GuiController.Instance.ThirdPersonCamera.rotateY(angulo);
                }
            }
        }
        private float dotProduct(Vector3 v1, Vector3 v2) { return ((v1.X * v2.X) + (v1.Y * v2.Y) + (v1.Z * v2.Z))/(v1.Length()*v2.Length()); }
        private Vector3 crossProduct(Vector3 v1, Vector3 v2)
        {
            float x = v1.Y * v2.Z - v1.Z * v2.Y;
            float y = v1.Z * v2.X - v1.X * v2.Z;
            float z = v1.X * v2.Y - v1.Y * v2.X;
            return new Vector3(x, y, z);
        }
        //No se divide por los modulos xq se utiliza para versores
    }
}

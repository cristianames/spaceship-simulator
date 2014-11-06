using AlumnoEjemplos.THE_GRID.InterfazGrafica;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using TgcViewer;
using TgcViewer.Example;
using TgcViewer.Utils;
using TgcViewer.Utils.Input;
using TgcViewer.Utils.Modifiers;
using TgcViewer.Utils.Shaders;
using TgcViewer.Utils.Terrain;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSceneLoader;

namespace AlumnoEjemplos.THE_GRID.Shaders
{
    public class EstructuraRender
    {
        public Nave nave;
        public Dibujable sol;
        public List<Dibujable> meshes;
        public List<IRenderObject> elementosRenderizables;
        public List<TgcMesh> objetosBrillantes;

        public EstructuraRender(Nave n, Dibujable s, List<Dibujable> m, List<IRenderObject> r, List<TgcMesh> e)
        {
            nave = n;
            sol = s;
            meshes = m;
            elementosRenderizables = r;
            objetosBrillantes = e;
        }
    }

    class SuperRender
    {
        private MotionBlur motionShader;
        private HDRL hdrlShader;
        private BumpMapping bumpShader;
        public enum tipo { MOTION, HDRL, BUMP };

        public bool motionBlurActivado = false;

        public TgcBox[] lightMeshes;

        //public Hud hud = new Hud();

        public SuperRender()
        {
            motionShader = new MotionBlur(this);
            hdrlShader = new HDRL(this);
            bumpShader = new BumpMapping(this);                
        }

        public bool fueraFrustrum(Dibujable dibujable)
        {
            //Chequea si esta dentro del frustrum
            TgcFrustum frustrum = EjemploAlumno.workspace().getCurrentFrustrum();
            if (TgcCollisionUtils.classifyFrustumSphere(frustrum, (TgcBoundingSphere)dibujable.getColision().getBoundingBox()) != TgcCollisionUtils.FrustumResult.OUTSIDE)
                return false;
            return true;
        }

        public void render(Nave nave, Dibujable sol, List<Dibujable> meshes, List<IRenderObject> elementosRenderizables, List<TgcMesh> objetosBrillantes)
        {
            motionShader.renderEffect(new EstructuraRender(nave, sol, meshes, elementosRenderizables, objetosBrillantes));
            //if(EjemploAlumno.workspace().camara.soyFPS())hud.operar(); //Se muestran los sprites de la HUD si estamos en FPS

            EjemploAlumno.workspace().creditos.render(); //Renderizamos los creditos, si estan habilitados
            //Dibujamos las FPS y la Velocidad actual
            GuiController.Instance.Text3d.drawText( "FPS: " + HighResolutionTimer.Instance.FramesPerSecond + Environment.NewLine + 
                                                    "Velocidad: " +EjemploAlumno.workspace().nave.velocidadActual(), 0, 0, Color.White);
            GuiController.Instance.Text3d.drawText("♪ " + EjemploAlumno.workspace().music.getActual(),
                 GuiController.Instance.D3dDevice.Viewport.Width - 180,
                 GuiController.Instance.D3dDevice.Viewport.Height - 20,Color.White);
            GuiController.Instance.AxisLines.render();//Dibujamos los ejes

            if (EjemploAlumno.workspace().helpActivado) helpHUD(); //Si se pidio ayuda, se muestran los comandos
        }

        public void close()
        {
            motionShader.close();
            hdrlShader.close();
            bumpShader.close();
        }

        public Texture renderAnterior(EstructuraRender parametros, SuperRender.tipo tipoEfecto)
        {
            Texture texturaRetorno = null;
            switch(tipoEfecto)
            {
                case tipo.MOTION:
                    texturaRetorno = hdrlShader.renderEffect(parametros);
                    break;
                case tipo.HDRL:
                    texturaRetorno = bumpShader.renderEffect(parametros);
                    break;
                case tipo.BUMP:
                    break;
            }

            return texturaRetorno;
        }
        public void helpHUD()
        {
            GuiController.Instance.Text3d.drawText( "C: Menú de Configuración"+Environment.NewLine+
                                                    "P: Menú de Pausa/Tutorial"+Environment.NewLine+
                                                    "F1-F3: Cámara Tercera Persona / FPS / Fija"+Environment.NewLine +
                                                    "I: Siguiente Canción" + Environment.NewLine +
                                                    "LeftShift: Efecto Blur"+Environment.NewLine+
                                                    "LeftCtrl: Modo Automático"+Environment.NewLine+
                                                    "Espacio: Disparo Principal"+Environment.NewLine+
                                                    "RightShift: Disparo Secundario"
                                                    , GuiController.Instance.D3dDevice.Viewport.Width - 300, 0, Color.White);

        }
    }
}
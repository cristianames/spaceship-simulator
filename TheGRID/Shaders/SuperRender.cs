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

namespace AlumnoEjemplos.TheGRID.Shaders
{
    public class EstructuraRender
    {
        public Nave nave;
        public Dibujable sol;
        public List<Dibujable> meshes;
        public List<IRenderObject> elementosRenderizables; 

        public EstructuraRender(Nave n, Dibujable s, List<Dibujable> m, List<IRenderObject> r)
        {
            nave = n;
            sol = s;
            meshes = m;
            elementosRenderizables = r;
        }
    }

    class SuperRender
    {
        private MotionBlur motionShader;
        private HDRL hdrlShader;
        private DynamicLights adeShader;
        private BumpMapping bumpShader;
        public enum tipo { MOTION, HDRL, DYNAMIC, BUMP };

        public bool motionBlurActivado = false;


        public SuperRender()
        {
            motionShader = new MotionBlur(this);
            hdrlShader = new HDRL(this);
            adeShader = new DynamicLights(this);
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

        public void render(Nave nave, Dibujable sol, List<Dibujable> meshes, List<IRenderObject> elementosRenderizables)
        {
            motionShader.renderEffect(new EstructuraRender(nave, sol, meshes, elementosRenderizables));
        }

        public void close()
        {
            motionShader.close();
            hdrlShader.close();
            adeShader.close();
            bumpShader.close();
        }

        public Texture renderAnterior(EstructuraRender parametros, SuperRender.tipo tipoEfecto)
        {
            Texture texturaRetorno = null;
            switch(tipoEfecto)
            {
                case tipo.MOTION:
                    texturaRetorno = bumpShader.renderEffect(parametros);
                    //texturaRetorno = hdrlShader.renderEffect(parametros);
                    break;
                case tipo.HDRL:
                    texturaRetorno = adeShader.renderEffect(parametros);
                    break;
                case tipo.DYNAMIC:
                    texturaRetorno = bumpShader.renderEffect(parametros);
                    break;
                case tipo.BUMP:
                    break;
            }

            return texturaRetorno;
        }
    }
}
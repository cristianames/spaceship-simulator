using Microsoft.DirectX.Direct3D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TgcViewer.Utils.TgcSceneLoader;

namespace AlumnoEjemplos.TheGRID.Shaders
{
    class DynamicLights : IShader
    {
        private string ShaderDirectory = EjemploAlumno.TG_Folder + "Shaders\\";
        private SuperRender mainShader;

        public DynamicLights(SuperRender main)
        {
            mainShader = main;
        }

        public Texture renderDefault(EstructuraRender parametros)
        {
            throw new NotImplementedException();
        }

        public Texture renderEffect(EstructuraRender parametros)
        {
            throw new NotImplementedException();
        }

        public void renderScene(Nave nave, string technique)
        {
            throw new NotImplementedException();
        }

        public void renderScene(Dibujable sol, string technique)
        {
            throw new NotImplementedException();
        }

        public void renderScene(List<Dibujable> dibujables, string technique)
        {
            throw new NotImplementedException();
        }

        public void renderScene(List<IRenderObject> elementosRenderizables)
        {
            throw new NotImplementedException();
        }

        public bool fueraFrustrum(Dibujable dibujable)
        {
            throw new NotImplementedException();
        }

        public SuperRender.tipo tipoShader()
        {
            throw new NotImplementedException();
        }

        public void close()
        {
            throw new NotImplementedException();
        }
    }
}

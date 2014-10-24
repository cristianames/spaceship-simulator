using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using TgcViewer;
using TgcViewer.Utils.Shaders;
using TgcViewer.Utils.TgcSceneLoader;

namespace AlumnoEjemplos.TheGRID.Shaders
{
    class BumpMapping :IShader
    {
        private string ShaderDirectory = EjemploAlumno.TG_Folder + "Shaders\\BumpMapping.fx";
        private SuperRender mainShader;
        private Surface g_pDepthStencil;     // Depth-stencil buffer 
        private Texture g_pRenderTarget;

        private Vector3 lightPosition;
        private Vector3 eyePosition;

        private Effect bumpEffect_asteroides;
        private Effect bumpEffect_nave;
        private Effect bumpEffect_sol;

        public BumpMapping(SuperRender main)
        {
            mainShader = main;
            Device d3dDevice = GuiController.Instance.D3dDevice;
            //Inicializo el depth stencil
            g_pDepthStencil = d3dDevice.CreateDepthStencilSurface(d3dDevice.PresentationParameters.BackBufferWidth,
                                                             d3dDevice.PresentationParameters.BackBufferHeight,
                                                             DepthFormat.D24S8,
                                                             MultiSampleType.None,
                                                             0,
                                                             true);

            // inicializo el render target
            g_pRenderTarget = new Texture(d3dDevice, d3dDevice.PresentationParameters.BackBufferWidth
                    , d3dDevice.PresentationParameters.BackBufferHeight, 1, Usage.RenderTarget,
                        Format.X8R8G8B8, Pool.Default);
            cargarBumpEffect();
        }

        public Texture renderDefault(EstructuraRender parametros)
        {
            throw new NotImplementedException();
        }

        public Texture renderEffect(EstructuraRender parametros)
        {
            Device device = GuiController.Instance.D3dDevice;
            // guardo el Render target anterior y seteo la textura como render target
            Surface pOldRT = device.GetRenderTarget(0);
            Surface pSurf = g_pRenderTarget.GetSurfaceLevel(0);
            device.SetRenderTarget(0, pSurf);
            // hago lo mismo con el depthbuffer, necesito el que no tiene multisampling
            Surface pOldDS = device.DepthStencilSurface;
            device.DepthStencilSurface = g_pDepthStencil;
            device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black, 1.0f, 0);

            device.BeginScene();
            renderScene(parametros.meshes, "");
            renderScene(parametros.sol, "");
            renderScene(parametros.elementosRenderizables);
            if (!EjemploAlumno.workspace().camara.soyFPS())
                renderScene(parametros.nave, "");
            device.EndScene();
            pSurf.Dispose();
            //Vuelvo a Setear el depthbuffer y el Render target
            device.SetRenderTarget(0, pOldRT);
            device.DepthStencilSurface = pOldDS;
            //Retorno la textura
            return g_pRenderTarget;
        }

        #region RenderScene

        public void renderScene(Nave nave, string technique)
        {
            lightPosition = EjemploAlumno.workspace().Escenario.sol.getPosicion();
            eyePosition = EjemploAlumno.workspace().camara.PosicionDeCamara;
            actualizarBumpEffect_Nave();
            ((TgcMesh)nave.objeto).Effect = bumpEffect_nave;
            ((TgcMesh)nave.objeto).Technique = "BumpMappingTechnique";
            nave.render();
        }

        public void renderScene(Dibujable sol, string technique)
        {
            lightPosition = EjemploAlumno.workspace().Escenario.sol.getPosicion();
            lightPosition.X += 500;
            lightPosition.Y += 900;
            lightPosition.Z -= 900;
            eyePosition = EjemploAlumno.workspace().camara.PosicionDeCamara;
            actualizarBumpEffect_Sol();
            ((TgcMesh)sol.objeto).Effect = bumpEffect_sol;
            ((TgcMesh)sol.objeto).Technique = "BumpMappingTechnique";
            sol.render();
        }

        public void renderScene(List<Dibujable> meshes, string technique)
        {
            lightPosition = EjemploAlumno.workspace().Escenario.sol.getPosicion();
            lightPosition += new Vector3(0, 0, -18000);
            eyePosition = EjemploAlumno.workspace().camara.PosicionDeCamara;
            actualizarBumpEffect_asteroides();
            foreach (Dibujable dibujable in meshes)
            {
                if (dibujable.soyAsteroide())
                {
                    //GuiController.Instance.Logger.log(dibujable.valor.ToString());
                    if (mainShader.fueraFrustrum(dibujable)) continue;
                    ((TgcMesh)dibujable.objeto).Effect = bumpEffect_asteroides;
                    ((TgcMesh)dibujable.objeto).Technique = "BumpMappingTechnique";
                }
                else resetearRenderDefault(dibujable);
                dibujable.render();
            }
        }

        public void renderScene(List<IRenderObject> elementosRenderizables)
        {
            foreach (IRenderObject elemento in elementosRenderizables)
            {
                elemento.render();
            }
        }

        #endregion

        #region BumpEffect
        private void cargarBumpEffect()
        {
            //ASTEROIDES
            bumpEffect_asteroides = TgcShaders.loadEffect(ShaderDirectory);
            bumpEffect_asteroides.Technique = "BumpMappingTechnique";
            bumpEffect_asteroides.SetValue("lightColor", ColorValue.FromColor(Color.DarkCyan));
            bumpEffect_asteroides.SetValue("lightIntensity", 2000f);
            bumpEffect_asteroides.SetValue("lightAttenuation", 0.6f);
            bumpEffect_asteroides.SetValue("bumpiness", 1f);
            //Material
            bumpEffect_asteroides.SetValue("materialEmissiveColor", ColorValue.FromColor(Color.Black));
            bumpEffect_asteroides.SetValue("materialAmbientColor", ColorValue.FromColor(Color.White));
            bumpEffect_asteroides.SetValue("materialDiffuseColor", ColorValue.FromColor(Color.White));
            bumpEffect_asteroides.SetValue("materialSpecularColor", ColorValue.FromColor(Color.Black));
            bumpEffect_asteroides.SetValue("materialSpecularExp", 9f);

            //NAVE
            bumpEffect_nave = TgcShaders.loadEffect(ShaderDirectory);
            bumpEffect_nave.Technique = "BumpMappingTechnique";
            bumpEffect_nave.SetValue("lightColor", ColorValue.FromColor(Color.LightCyan));
            bumpEffect_nave.SetValue("lightIntensity", 2000f);
            bumpEffect_nave.SetValue("lightAttenuation", 0.3f);
            bumpEffect_nave.SetValue("bumpiness", 1f);
            //Material
            bumpEffect_nave.SetValue("materialEmissiveColor", ColorValue.FromColor(Color.Black));
            bumpEffect_nave.SetValue("materialAmbientColor", ColorValue.FromColor(Color.White));
            bumpEffect_nave.SetValue("materialDiffuseColor", ColorValue.FromColor(Color.White));
            bumpEffect_nave.SetValue("materialSpecularColor", ColorValue.FromColor(Color.White));
            bumpEffect_nave.SetValue("materialSpecularExp", 9f);

            //SOL
            bumpEffect_sol = TgcShaders.loadEffect(ShaderDirectory);
            bumpEffect_sol.Technique = "BumpMappingTechnique";
            bumpEffect_sol.SetValue("lightColor", ColorValue.FromColor(Color.LightCyan));
            bumpEffect_sol.SetValue("lightIntensity", 550f);
            bumpEffect_sol.SetValue("lightAttenuation", 0.6f);
            bumpEffect_sol.SetValue("bumpiness", 1f);
            //Material
            bumpEffect_sol.SetValue("materialEmissiveColor", ColorValue.FromColor(Color.Black));
            bumpEffect_sol.SetValue("materialAmbientColor", ColorValue.FromColor(Color.White));
            bumpEffect_sol.SetValue("materialDiffuseColor", ColorValue.FromColor(Color.White));
            bumpEffect_sol.SetValue("materialSpecularColor", ColorValue.FromColor(Color.White));
            bumpEffect_sol.SetValue("materialSpecularExp", 9f);
        }

        private void actualizarBumpEffect_asteroides()
        {
            bumpEffect_asteroides.SetValue("lightPosition", TgcParserUtils.vector3ToFloat4Array(lightPosition));
            bumpEffect_asteroides.SetValue("eyePosition", TgcParserUtils.vector3ToFloat4Array(eyePosition));
        }

        private void actualizarBumpEffect_Nave()
        {
            bumpEffect_nave.SetValue("lightPosition", TgcParserUtils.vector3ToFloat4Array(lightPosition));
            bumpEffect_nave.SetValue("eyePosition", TgcParserUtils.vector3ToFloat4Array(eyePosition));
        }

        private void actualizarBumpEffect_Sol()
        {
            bumpEffect_sol.SetValue("lightPosition", TgcParserUtils.vector3ToFloat4Array(lightPosition));
            bumpEffect_sol.SetValue("eyePosition", TgcParserUtils.vector3ToFloat4Array(eyePosition));
        }
        #endregion

        #region Auxiliares

        private void resetearRenderDefault(Dibujable dibujable)
        {
            dibujable.objeto.Effect = GuiController.Instance.Shaders.TgcMeshShader;
            dibujable.objeto.Technique = GuiController.Instance.Shaders.getTgcMeshTechnique(((TgcMesh)dibujable.objeto).RenderType);
        }

        public SuperRender.tipo tipoShader()
        {
            return SuperRender.tipo.BUMP;
        }

        public void close()
        {
            throw new NotImplementedException();
        }

        #endregion

    }
}

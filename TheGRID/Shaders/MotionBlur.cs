using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using TgcViewer;
using TgcViewer.Utils;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSceneLoader;

namespace AlumnoEjemplos.TheGRID.Shaders
{
    class MotionBlur : IShader
    {
        private string ShaderDirectory = EjemploAlumno.TG_Folder + "Shaders\\MotionBlur.fx";
        private SuperRender mainShader;
        private Effect effect;
        private VertexBuffer g_pVBV3D;
        private Surface g_pDepthStencil;     // Depth-stencil buffer 
        private Texture g_pRenderTarget;    //Textura
        private Texture g_pVel1, g_pVel2;   // velocidad
        private Texture g_blank;            // Testura sin nada
        private Matrix antMatView;

        public MotionBlur(SuperRender main)
        {
            mainShader = main;
            Device d3dDevice = GuiController.Instance.D3dDevice;

            //Cargar Shader del motionBlur
            string compilationErrors;
            effect = Effect.FromFile(GuiController.Instance.D3dDevice, ShaderDirectory,
                null, null, ShaderFlags.PreferFlowControl, null, out compilationErrors);
            if (effect == null)
            {
                throw new Exception("Error al cargar shader. Errores: " + compilationErrors);
            }
            //Configurar Technique Default dentro del shader
            effect.Technique = "DefaultTechnique";
            // stencil
            g_pDepthStencil = d3dDevice.CreateDepthStencilSurface(d3dDevice.PresentationParameters.BackBufferWidth,
                                                                         d3dDevice.PresentationParameters.BackBufferHeight,
                                                                         DepthFormat.D24S8,
                                                                         MultiSampleType.None,
                                                                         0,
                                                                         true);
            /*// Inicializo la Textura donde vamos a volcar la pantalla
            g_pRenderTarget = new Texture(d3dDevice, d3dDevice.PresentationParameters.BackBufferWidth
                    , d3dDevice.PresentationParameters.BackBufferHeight, 1, Usage.RenderTarget,
                        Format.X8R8G8B8, Pool.Default);*/

            // Inicializo las Texturas de Velocidad
            g_pVel1 = new Texture(d3dDevice, d3dDevice.PresentationParameters.BackBufferWidth
                    , d3dDevice.PresentationParameters.BackBufferHeight, 1, Usage.RenderTarget,
                        Format.A16B16G16R16F, Pool.Default);
            g_pVel2 = new Texture(d3dDevice, d3dDevice.PresentationParameters.BackBufferWidth
                    , d3dDevice.PresentationParameters.BackBufferHeight, 1, Usage.RenderTarget,
                        Format.A16B16G16R16F, Pool.Default);
            g_blank = new Texture(d3dDevice, d3dDevice.PresentationParameters.BackBufferWidth
                    , d3dDevice.PresentationParameters.BackBufferHeight, 1, Usage.RenderTarget,
                        Format.A16B16G16R16F, Pool.Default); 
            // Resolucion de pantalla
            effect.SetValue("screen_dx", d3dDevice.PresentationParameters.BackBufferWidth);
            effect.SetValue("screen_dy", d3dDevice.PresentationParameters.BackBufferHeight);

            CustomVertex.PositionTextured[] vertices = new CustomVertex.PositionTextured[]
		    {
    			new CustomVertex.PositionTextured( -1, 1, 1, 0,0), 
			    new CustomVertex.PositionTextured(1,  1, 1, 1,0),
			    new CustomVertex.PositionTextured(-1, -1, 1, 0,1),
			    new CustomVertex.PositionTextured(1,-1, 1, 1,1)
    		};
            //vertex buffer de los triangulos
            g_pVBV3D = new VertexBuffer(typeof(CustomVertex.PositionTextured),
                    4, d3dDevice, Usage.Dynamic | Usage.WriteOnly,
                        CustomVertex.PositionTextured.Format, Pool.Default);
            g_pVBV3D.SetData(vertices, 0, LockFlags.None);
            //Inicializo la matriz de vision
            antMatView = d3dDevice.Transform.View;
        }

        public Texture renderDefault(EstructuraRender parametros)
        {
            Device device = GuiController.Instance.D3dDevice;
            
            //Pedimos que nos renderizen la pantalla default
            g_pRenderTarget = mainShader.renderAnterior(parametros, tipoShader());

            //Hacemos el post Procesado
            device.BeginScene();
            effect.Technique = "PostProcessMotionBlur";
            device.VertexFormat = CustomVertex.PositionTextured.Format;
            device.SetStreamSource(0, g_pVBV3D, 0);
            effect.SetValue("g_RenderTarget", g_pRenderTarget);
            if (mainShader.motionBlurActivado)
            {
                effect.SetValue("texVelocityMap", g_pVel1);
                effect.SetValue("texVelocityMapAnt", g_pVel2);
            }
            else 
            {
                effect.SetValue("texVelocityMap", g_blank);
                effect.SetValue("texVelocityMapAnt", g_blank);

            }
            device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black, 1.0f, 0);
            effect.Begin(FX.None);
            effect.BeginPass(0);
            device.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 2);
            effect.EndPass();
            effect.End();
            device.EndScene();
            GuiController.Instance.Text3d.drawText("FPS: " + HighResolutionTimer.Instance.FramesPerSecond, 0, 0, Color.Yellow);
            return null;
        }

        public Texture renderEffect(EstructuraRender parametros)
        {
            //La simplicidad radica en que si la textura de velocidad esta en blanco, no hace el efecto :P
            if(mainShader.motionBlurActivado)
            {
                dibujarVelocidad(parametros);
            }
            renderDefault(parametros);
            return null;
        }

        private void dibujarVelocidad(EstructuraRender parametros)
        {
            Device device = GuiController.Instance.D3dDevice;
            float velActual = EjemploAlumno.workspace().velocidadBlur;
            float pixel_blur_variable = 0.2f * ((velActual - 200) / (300000 - 200));    //Calcula el porcentual de aplicacion sobre el blur.
            effect.SetValue("PixelBlurConst", pixel_blur_variable);

            // guardo el Render target anterior y seteo la textura como render target
            Surface pOldRT = device.GetRenderTarget(0);
            Surface pSurf = g_pVel1.GetSurfaceLevel(0);
            device.SetRenderTarget(0, pSurf);
            // hago lo mismo con el depthbuffer, necesito el que no tiene multisampling
            Surface pOldDS = device.DepthStencilSurface;
            device.DepthStencilSurface = g_pDepthStencil;

            //Seteo la Technique
            effect.Technique = "VelocityMap";
            // necesito mandarle la matrix de view actual y la anterior
            effect.SetValue("matView", device.Transform.View);
            effect.SetValue("matViewAnt", antMatView);
            device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black, 1.0f, 0);
            device.BeginScene();
            renderScene(parametros.meshes, "VelocityMap");
            if (!EjemploAlumno.workspace().camara.soyFPS())
                renderScene(parametros.nave, "VelocityMap");
            device.EndScene();
            pSurf.Dispose();

            //Vuelvo a Setear el depthbuffer y el Render target
            device.SetRenderTarget(0, pOldRT);
            device.DepthStencilSurface = pOldDS;

            // actualizo los valores para el proximo frame
            antMatView = device.Transform.View;
            Texture aux = g_pVel2;
            g_pVel2 = g_pVel1;
            g_pVel1 = aux;

        }

        #region RenderScene

        public void renderScene(Nave nave, string technique)
        {
            ((TgcMesh)nave.objeto).Effect = effect;
            ((TgcMesh)nave.objeto).Technique = technique;
            nave.render();
        }

        public void renderScene(Dibujable dibujable, string technique)
        {
            if (dibujable.soyAsteroide() && mainShader.fueraFrustrum(dibujable)) return;
            ((TgcMesh)dibujable.objeto).Effect = effect;
            ((TgcMesh)dibujable.objeto).Technique = technique;
            dibujable.render();
        }

        public void renderScene(List<Dibujable> dibujables, string technique)
        {
            foreach (Dibujable dibujable in dibujables)
            {
                renderScene(dibujable, technique);
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

        #region Metodos Auxiliares

        public SuperRender.tipo tipoShader()
        {
            return SuperRender.tipo.MOTION; ;
        }

        public void close()
        {
            g_pDepthStencil.Dispose();
            g_pVBV3D.Dispose();
            g_pVel1.Dispose();
            g_pVel2.Dispose();
        }

        #endregion

        
    }
}

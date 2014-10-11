using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using TgcViewer.Example;
using TgcViewer;
using Microsoft.DirectX.Direct3D;
using System.Drawing;
using Microsoft.DirectX;
using TgcViewer.Utils.Modifiers;
using TgcViewer.Utils.Terrain;
using TgcViewer.Utils.Shaders;
using TgcViewer.Utils.Input;
using TgcViewer.Utils;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSceneLoader;

namespace AlumnoEjemplos.TheGRID.Shaders
{
    class SuperRender
    {
        private string ShaderDirectory;
        private Effect effect;
        public bool motionBlurActivado = false;
        private VertexBuffer g_pVBV3D;
        private Surface g_pDepthStencil;     // Depth-stencil buffer 
        private Texture g_pRenderTarget;
        private Texture g_pVel1, g_pVel2;   // velocidad
        private Matrix antMatView;

        public SuperRender()
        {
            Device d3dDevice = GuiController.Instance.D3dDevice;

            ShaderDirectory = EjemploAlumno.TG_Folder + "Shaders\\";

            //Cargar Shader personalizado
            string compilationErrors;
            effect = Effect.FromFile(GuiController.Instance.D3dDevice, ShaderDirectory + "MotionBlur.fx",
                null, null, ShaderFlags.PreferFlowControl, null, out compilationErrors);
            if (effect == null)
            {
                throw new Exception("Error al cargar shader. Errores: " + compilationErrors);
            }
            
            //Configurar Technique dentro del shader
            effect.Technique = "DefaultTechnique";

            // stencil
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


            // velocidad del pixel
            g_pVel1 = new Texture(d3dDevice, d3dDevice.PresentationParameters.BackBufferWidth
                    , d3dDevice.PresentationParameters.BackBufferHeight, 1, Usage.RenderTarget,
                        Format.A16B16G16R16F, Pool.Default);
            g_pVel2 = new Texture(d3dDevice, d3dDevice.PresentationParameters.BackBufferWidth
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
            antMatView = d3dDevice.Transform.View;
        }

        public void renderScene(List<Dibujable> dibujables, String technique)
        {
            foreach (Dibujable dibujable in dibujables)
            {
                ((TgcMesh)dibujable.objeto).Effect = effect;
                ((TgcMesh)dibujable.objeto).Technique = technique;
                dibujable.render();
            }
        }
        public void renderScene(Dibujable dibujable, String technique)
        {
            ((TgcMesh)dibujable.objeto).Effect = effect;
            ((TgcMesh)dibujable.objeto).Technique = technique;
            dibujable.render();
        }
        public void renderScene(List<IRenderObject> elementosRenderizables) 
        {
            foreach (IRenderObject elemento in elementosRenderizables)
            {
                elemento.render();
            }
        }

        private void defaultRender(Dibujable nave, List<Dibujable> meshes, List<IRenderObject> elementosRenderizables)
        {
            Device device = GuiController.Instance.D3dDevice;
            effect.Technique = "DefaultTechnique";
            device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black, 1.0f, 0);
            device.BeginScene();
            renderScene(meshes, "DefaultTechnique");
            renderScene(elementosRenderizables);
            if (!EjemploAlumno.workspace().camara.soyFPS())
                renderScene(nave, "DefaultTechnique");
            device.EndScene();
        }

        private void motionBlur(Dibujable nave, List<Dibujable> meshes, List<IRenderObject> elementosRenderizables)
        {
            Device device = GuiController.Instance.D3dDevice;
            float velActual = EjemploAlumno.workspace().velocidadBlur;
            float pixel_blur_variable = 0.2f * ((velActual - 200) / (300000 - 200));    //Calcula el porcentual de aplicacion sobre el blur.
            effect.SetValue("PixelBlurConst", pixel_blur_variable); //Despues veo como hacerlo mas global

            // guardo el Render target anterior y seteo la textura como render target
            Surface pOldRT = device.GetRenderTarget(0);
            Surface pSurf = g_pVel1.GetSurfaceLevel(0);
            device.SetRenderTarget(0, pSurf);
            // hago lo mismo con el depthbuffer, necesito el que no tiene multisampling
            Surface pOldDS = device.DepthStencilSurface;
            device.DepthStencilSurface = g_pDepthStencil;

            // 1 - Genero un mapa de velocidad 
            effect.Technique = "VelocityMap";
            // necesito mandarle la matrix de view actual y la anterior
            effect.SetValue("matView", device.Transform.View);
            effect.SetValue("matViewAnt", antMatView);
            device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black, 1.0f, 0);
            device.BeginScene();
            renderScene(meshes, "VelocityMap");
            if (!EjemploAlumno.workspace().camara.soyFPS())
                renderScene(nave, "VelocityMap");
            device.EndScene();
            pSurf.Dispose();

            // 2- Genero la imagen pp dicha 
            effect.Technique = "DefaultTechnique";
            pSurf = g_pRenderTarget.GetSurfaceLevel(0);
            device.SetRenderTarget(0, pSurf);
            device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black, 1.0f, 0);
            device.BeginScene();
            renderScene(meshes, "DefaultTechnique");
            renderScene(elementosRenderizables);
            if (!EjemploAlumno.workspace().camara.soyFPS())
                renderScene(nave, "DefaultTechnique");
            device.EndScene();
            pSurf.Dispose();

            // Ultima pasada vertical va sobre la pantalla pp dicha
            device.SetRenderTarget(0, pOldRT);
            device.DepthStencilSurface = pOldDS;
            device.BeginScene();
            effect.Technique = "PostProcessMotionBlur";
            device.VertexFormat = CustomVertex.PositionTextured.Format;
            device.SetStreamSource(0, g_pVBV3D, 0);
            effect.SetValue("g_RenderTarget", g_pRenderTarget);
            effect.SetValue("texVelocityMap", g_pVel1);
            effect.SetValue("texVelocityMapAnt", g_pVel2);
            device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black, 1.0f, 0);
            effect.Begin(FX.None);
            effect.BeginPass(0);
            device.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 2);
            effect.EndPass();
            effect.End();
            device.EndScene();

            // actualizo los valores para el proximo frame
            antMatView = device.Transform.View;
            Texture aux = g_pVel2;
            g_pVel2 = g_pVel1;
            g_pVel1 = aux;
        }

        public void render(Dibujable nave, List<Dibujable> meshes, List<IRenderObject> elementosRenderizables)
        {
            if (!motionBlurActivado)
            {
                // dibujar sin motion blur
                defaultRender(nave, meshes, elementosRenderizables);
                GuiController.Instance.Text3d.drawText("FPS: " + HighResolutionTimer.Instance.FramesPerSecond, 0, 0, Color.Yellow);
                return;
            }
            motionBlur(nave, meshes, elementosRenderizables);
            GuiController.Instance.Text3d.drawText("FPS: " + HighResolutionTimer.Instance.FramesPerSecond, 0, 0, Color.Yellow);
        }

        public void close()
        {
            g_pRenderTarget.Dispose();
            g_pDepthStencil.Dispose();
            g_pVBV3D.Dispose();
            g_pVel1.Dispose();
            g_pVel2.Dispose();
        }
    }
}
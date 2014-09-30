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
    class ShaderTheGrid
    {
        private string ShaderDirectory;
        private Effect effect;
        private float time = 0;
        public bool motionBlurActivado = false;
        private VertexBuffer g_pVBV3D;
        private Surface g_pDepthStencil;     // Depth-stencil buffer 
        private Texture g_pRenderTarget;
        private Texture g_pVel1, g_pVel2;   // velocidad
        private Matrix antMatWorldView;

        public ShaderTheGrid()
        {
            Device d3dDevice = GuiController.Instance.D3dDevice;

            ShaderDirectory = GuiController.Instance.AlumnoEjemplosMediaDir + "\\TheGrid\\Shaders\\";

            //Cargar Shader personalizado
            string compilationErrors;
            effect = Effect.FromFile(GuiController.Instance.D3dDevice, ShaderDirectory + "MotionBlur.fx",
                null, null, ShaderFlags.PreferFlowControl, null, out compilationErrors);
            if (effect == null)
            {
                throw new Exception("Error al cargar shader. Errores: " + compilationErrors);
            }
            
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

            antMatWorldView = Matrix.Identity;
        }

        public void renderScene(List<TgcMesh> meshes, String technique)
        {
            foreach(TgcMesh mesh in meshes)
            {
                mesh.Effect = effect;
                mesh.Technique = technique;
                mesh.render();
            }
        }

        public void shadear(TgcMesh nave ,List<TgcMesh> meshes, float elapsedTime)
        {
            time += elapsedTime;
            Device device = GuiController.Instance.D3dDevice;

            // guardo el Render target anterior y seteo la textura como render target
            Surface pOldRT = device.GetRenderTarget(0);
            Surface pSurf = g_pRenderTarget.GetSurfaceLevel(0);
            device.SetRenderTarget(0, pSurf);
            // hago lo mismo con el depthbuffer, necesito el que no tiene multisampling
            Surface pOldDS = device.DepthStencilSurface;
            device.DepthStencilSurface = g_pDepthStencil;

            // 1- Genero la imagen pp dicha 
            effect.Technique = "DefaultTechnique";
            device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black, 1.0f, 0);
            device.BeginScene();
            //Renderizemos
            renderScene(meshes, "DefaultTechnique");
            //Renderizamos la nave
            nave.Effect = effect;
            nave.Technique = "DefaultTechnique";
            nave.render();
            device.EndScene();
            pSurf.Dispose();

            if (motionBlurActivado)
            {
                // 2 - Genero un mapa de velocidad 

                effect.Technique = "VelocityMap";
                pSurf = g_pVel1.GetSurfaceLevel(0);
                device.SetRenderTarget(0, pSurf);
                // necesito mandarle la matrix de view proj anterior
                effect.SetValue("matWorldViewProjAnt", antMatWorldView * device.Transform.Projection);
                device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black, 1.0f, 0);
                device.BeginScene();
                //Renderizemos
                renderScene(meshes, "VelocityMap");
                //Renderizamos la nave
                nave.Effect = effect;
                nave.Technique = "VelocityMap";
                nave.render();
                device.EndScene();
                pSurf.Dispose();

                // Ultima pasada vertical va sobre la pantalla pp dicha
                device.SetRenderTarget(0, pOldRT);
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
            }
            //Cosas a mostrar por pantalla, como el FPS y demaces
            GuiController.Instance.Text3d.drawText("FPS: " + HighResolutionTimer.Instance.FramesPerSecond, 0, 0, Color.Yellow);
            //GuiController.Instance.Text3d.drawText("Pos: " + GuiController.Instance.CurrentCamera.getPosition(), 0, 0, Color.Yellow);
            //GuiController.Instance.Text3d.drawText("Look At: " + GuiController.Instance.CurrentCamera.getLookAt(), 500, 0, Color.Yellow);
            
            // actualizo los valores para el proximo frame
            antMatWorldView = nave.Transform * device.Transform.View;
            Texture aux = g_pVel2;
            g_pVel2 = g_pVel1;
            g_pVel1 = aux;
            //throw new NotImplementedException();
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
using Microsoft.DirectX.Direct3D;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TgcViewer;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSceneLoader;

namespace AlumnoEjemplos.TheGRID.Shaders
{
    class HDRL : IShader
    {
        #region Atributes
        private string ShaderDirectory = EjemploAlumno.TG_Folder + "Shaders\\GaussianBlur.fx";
        private SuperRender mainShader;
        Effect effect;
        Surface g_pDepthStencil;     // Depth-stencil buffer 
        Texture g_pRenderTarget, g_pGlowMap, g_pRenderTarget4, g_pRenderTarget4Aux, g_pRenderFinal;

        const int NUM_REDUCE_TX = 5;
        Texture[] g_pLuminance = new Texture[NUM_REDUCE_TX];
        Texture g_pLuminance_ant;

        VertexBuffer g_pVBV3D;
        int cant_pasadas = 5;

        float pupila_time = 0;
        float adaptacion_pupila = 2;
        float MAX_PUPILA_TIME = 3;
        bool glow = true;
        #endregion

        public HDRL(SuperRender main)
        {
            mainShader = main;
            Device d3dDevice = GuiController.Instance.D3dDevice;
            string compilationErrors;
            effect = Effect.FromFile(GuiController.Instance.D3dDevice, ShaderDirectory,
                null, null, ShaderFlags.PreferFlowControl, null, out compilationErrors);
            if (effect == null)
            {
                throw new Exception("Error al cargar shader. Errores: " + compilationErrors);
            }
            //Configurar Technique dentro del shader
            effect.Technique = "DefaultTechnique";
            g_pDepthStencil = d3dDevice.CreateDepthStencilSurface(d3dDevice.PresentationParameters.BackBufferWidth,
                                                                         d3dDevice.PresentationParameters.BackBufferHeight,
                                                                         DepthFormat.D24S8,
                                                                         MultiSampleType.None,
                                                                         0,
                                                                         true);

            // inicializo el render target
            g_pRenderFinal = new Texture(d3dDevice, d3dDevice.PresentationParameters.BackBufferWidth
                    , d3dDevice.PresentationParameters.BackBufferHeight, 1, Usage.RenderTarget,
                        Format.A16B16G16R16F, Pool.Default);

            g_pGlowMap = new Texture(d3dDevice, d3dDevice.PresentationParameters.BackBufferWidth
                    , d3dDevice.PresentationParameters.BackBufferHeight, 1, Usage.RenderTarget,
                        Format.A16B16G16R16F, Pool.Default);

            g_pRenderTarget4 = new Texture(d3dDevice, d3dDevice.PresentationParameters.BackBufferWidth / 4
                    , d3dDevice.PresentationParameters.BackBufferHeight / 4, 1, Usage.RenderTarget,
                        Format.A16B16G16R16F, Pool.Default);

            g_pRenderTarget4Aux = new Texture(d3dDevice, d3dDevice.PresentationParameters.BackBufferWidth / 4
                    , d3dDevice.PresentationParameters.BackBufferHeight / 4, 1, Usage.RenderTarget,
                        Format.A16B16G16R16F, Pool.Default);


            // Para computar el promedio de Luminance
            int tx_size = 1;
            for (int i = 0; i < NUM_REDUCE_TX; ++i)
            {
                g_pLuminance[i] = new Texture(d3dDevice, tx_size, tx_size, 1,
                    Usage.RenderTarget, Format.A16B16G16R16F, Pool.Default);
                tx_size *= 4;
            }

            g_pLuminance_ant = new Texture(d3dDevice, 1, 1, 1,
                Usage.RenderTarget, Format.A16B16G16R16F, Pool.Default);


            effect.SetValue("g_RenderTarget", g_pRenderTarget);

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
        }

        public Texture renderDefault(EstructuraRender parametros)
        {
            throw new NotImplementedException();
        }

        public Texture renderEffect(EstructuraRender parametros)
        {
            float elapsedTime = EjemploAlumno.workspace().tiempoPupila;
            glow = (bool)GuiController.Instance.Modifiers["glow"];

            Device device = GuiController.Instance.D3dDevice;
            Control panel3d = GuiController.Instance.Panel3d;
            float aspectRatio = (float)panel3d.Width / (float)panel3d.Height;

            // Resolucion de pantalla
            float screen_dx = device.PresentationParameters.BackBufferWidth;
            float screen_dy = device.PresentationParameters.BackBufferHeight;
            effect.SetValue("screen_dx", screen_dx);
            effect.SetValue("screen_dy", screen_dy);

            // guardo el Render target anterior y seteo la textura como render target
            Surface pOldRT = device.GetRenderTarget(0);
            Surface pSurf;
            // hago lo mismo con el depthbuffer, necesito el que no tiene multisampling
            Surface pOldDS = device.DepthStencilSurface;
            device.DepthStencilSurface = g_pDepthStencil;

            //Obtenemos el render anterior
            g_pRenderTarget = mainShader.renderAnterior(parametros, tipoShader());

            MAX_PUPILA_TIME = adaptacion_pupila;
            effect.SetValue("glow", glow);
            if (glow)
            {
                #region GlowMap
                effect.SetValue("KLum", 1.3f);
                effect.Technique = "DefaultTechnique";
                pSurf = g_pGlowMap.GetSurfaceLevel(0);
                device.SetRenderTarget(0, pSurf);
                device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black, 1.0f, 0);
                device.BeginScene();
                    renderScene(parametros.meshes, "DibujarObjetosOscuros");
                    renderScene(parametros.sol, "DefaultTechnique");
                        if (!EjemploAlumno.workspace().camara.soyFPS())
                            renderScene(parametros.nave, "DibujarObjetosOscuros");
                    //renderScene(parametros.elementosRenderizables);
                    renderLuces(mainShader.lightMeshes, "DefaultTechnique");
                device.EndScene();
                pSurf.Dispose();

                // Hago un blur sobre el glow map
                // 1er pasada: downfilter x 4
                // -----------------------------------------------------
                pSurf = g_pRenderTarget4.GetSurfaceLevel(0);
                device.SetRenderTarget(0, pSurf);
                device.BeginScene();
                    effect.Technique = "DownFilter4";
                    device.VertexFormat = CustomVertex.PositionTextured.Format;
                    device.SetStreamSource(0, g_pVBV3D, 0);
                    effect.SetValue("g_RenderTarget", g_pGlowMap);
                    device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black, 1.0f, 0);
                    effect.Begin(FX.None);
                        effect.BeginPass(0);
                            device.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 2);
                        effect.EndPass();
                    effect.End();
                    pSurf.Dispose();
                device.EndScene();
                device.DepthStencilSurface = pOldDS;
                #endregion

                for (int P = 0; P < cant_pasadas; ++P)
                {
                    #region Blur
                    // Gaussian blur Horizontal
                    // -----------------------------------------------------
                    pSurf = g_pRenderTarget4Aux.GetSurfaceLevel(0);
                    device.SetRenderTarget(0, pSurf);
                    // dibujo el quad pp dicho :
                    device.BeginScene();
                        effect.Technique = "GaussianBlurSeparable";
                        device.VertexFormat = CustomVertex.PositionTextured.Format;
                        device.SetStreamSource(0, g_pVBV3D, 0);
                        effect.SetValue("g_RenderTarget", g_pRenderTarget4);

                        device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black, 1.0f, 0);
                        effect.Begin(FX.None);
                            effect.BeginPass(0);
                                device.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 2);
                            effect.EndPass();
                        effect.End();
                        pSurf.Dispose();
                    device.EndScene();

                    pSurf = g_pRenderTarget4.GetSurfaceLevel(0);
                    device.SetRenderTarget(0, pSurf);
                    pSurf.Dispose();

                    //  Gaussian blur Vertical
                    // -----------------------------------------------------
                    device.BeginScene();
                        effect.Technique = "GaussianBlurSeparable";
                        device.VertexFormat = CustomVertex.PositionTextured.Format;
                        device.SetStreamSource(0, g_pVBV3D, 0);
                        effect.SetValue("g_RenderTarget", g_pRenderTarget4Aux);

                        device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black, 1.0f, 0);
                        effect.Begin(FX.None);
                            effect.BeginPass(1);
                                device.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 2);
                            effect.EndPass();
                        effect.End();
                    device.EndScene();
                    #endregion
                }
                
            }

            #region Promedio
            pSurf = g_pLuminance[NUM_REDUCE_TX - 1].GetSurfaceLevel(0);
            screen_dx = pSurf.Description.Width;
            screen_dy = pSurf.Description.Height;
            device.SetRenderTarget(0, pSurf);
            device.BeginScene();
                effect.Technique = "DownFilter4";
                device.VertexFormat = CustomVertex.PositionTextured.Format;
                device.SetStreamSource(0, g_pVBV3D, 0);
                effect.SetValue("g_RenderTarget", g_pRenderTarget);
                device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black, 1.0f, 0);
                effect.Begin(FX.None);
                    effect.BeginPass(0);
                        device.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 2);
                    effect.EndPass();
                effect.End();
                pSurf.Dispose();
            device.EndScene();
            device.DepthStencilSurface = pOldDS;
            string fname2 = string.Format("Pass{0:D}.bmp", NUM_REDUCE_TX);

            // Reduce
            for (int i = NUM_REDUCE_TX - 1; i > 0; i--)
            {

                pSurf = g_pLuminance[i - 1].GetSurfaceLevel(0);
                effect.SetValue("screen_dx", screen_dx);
                effect.SetValue("screen_dy", screen_dy);

                device.SetRenderTarget(0, pSurf);
                effect.SetValue("g_RenderTarget", g_pLuminance[i]);
                device.BeginScene();
                    device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black, 1.0f, 0);
                    effect.Begin(FX.None);
                        effect.BeginPass(0);
                            device.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 2);
                        effect.EndPass();
                    effect.End();
                    pSurf.Dispose();
                device.EndScene();

                string fname = string.Format("Pass{0:D}.bmp", i);
                //SurfaceLoader.Save(fname, ImageFileFormat.Bmp, pSurf);

                screen_dx /= 4.0f;
                screen_dy /= 4.0f;
            }
            #endregion

            #region ToneMapping Effect
            effect.SetValue("pantalla_completa", true);
            effect.SetValue("screen_dx", device.PresentationParameters.BackBufferWidth);
            effect.SetValue("screen_dy", device.PresentationParameters.BackBufferHeight);
            pSurf = g_pRenderFinal.GetSurfaceLevel(0);
            device.SetRenderTarget(0, pSurf);
            device.BeginScene();
                effect.Technique = "ToneMapping";
                device.VertexFormat = CustomVertex.PositionTextured.Format;
                device.SetStreamSource(0, g_pVBV3D, 0);
                effect.SetValue("g_RenderTarget", g_pRenderTarget);
                effect.SetValue("g_GlowMap", g_pRenderTarget4Aux);
                pupila_time += elapsedTime;
                if (pupila_time >= MAX_PUPILA_TIME)
                {
                    pupila_time = 0;
                    effect.SetValue("g_Luminance_ant", g_pLuminance[0]);
                    Texture aux = g_pLuminance[0];
                    g_pLuminance[0] = g_pLuminance_ant;
                    g_pLuminance_ant = aux;
                }
                else
                {
                    effect.SetValue("g_Luminance", g_pLuminance[0]);
                }

                effect.SetValue("pupila_time", pupila_time / MAX_PUPILA_TIME);      // 0..1
                device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black, 1.0f, 0);
                effect.Begin(FX.None);
                    effect.BeginPass(0);
                        device.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 2);
                    effect.EndPass();
                effect.End();
            device.EndScene();
            #endregion

            //Volvemos los targets a la normalidad
            device.SetRenderTarget(0, pOldRT);
            device.DepthStencilSurface = pOldDS;
            //Devolvemos la textura
            return g_pRenderFinal;
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
        public void renderLuces(TgcBox[] lightMeshes, string technique)
        {
                if(EjemploAlumno.workspace().camara.soyFPS())
                {
                    lightMeshes[0].Effect = effect;
                    lightMeshes[0].Technique = technique;
                    lightMeshes[0].render();
                }
                lightMeshes[1].Effect = effect;
                lightMeshes[1].Technique = technique;
                lightMeshes[1].render();
                lightMeshes[2].Effect = effect;
                lightMeshes[2].Technique = technique;
                lightMeshes[2].render();

        }
        #endregion

        public SuperRender.tipo tipoShader()
        {
            return SuperRender.tipo.HDRL;
        }

        public void close()
        {
            g_pRenderFinal.Dispose();
            g_pGlowMap.Dispose();
            g_pRenderTarget4Aux.Dispose();
            g_pRenderTarget4.Dispose();
            g_pVBV3D.Dispose();
            g_pDepthStencil.Dispose();
            for (int i = 0; i < NUM_REDUCE_TX; i++)
            {
                g_pLuminance[i].Dispose();
            }
            g_pLuminance_ant.Dispose();
            effect.Dispose();
        }
    }
}

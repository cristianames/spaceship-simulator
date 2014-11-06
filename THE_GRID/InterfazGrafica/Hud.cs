using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TgcViewer;
using TgcViewer.Utils.Gui;
using TgcViewer.Utils.TgcSkeletalAnimation;

namespace AlumnoEjemplos.THE_GRID.InterfazGrafica
{
    public class Hud
    {
        DXGui gui = new DXGui();
        public Hud()
        {
            Device d3dDevice = GuiController.Instance.D3dDevice;
            // levanto el GUI
            float W = GuiController.Instance.Panel3d.Width;
            float H = GuiController.Instance.Panel3d.Height;
            gui.Create();
            gui.InitDialog(false);
            //gui.InsertFrame("Combo Rata", 10, 10, 200, 200, Color.FromArgb(32, 120, 255, 132), frameBorder.sin_borde);
            //gui.InsertFrame("", 10, (int)H - 150, 200, 140, Color.FromArgb(62, 120, 132, 255), frameBorder.sin_borde);
            gui.cursor_izq = gui.cursor_der = tipoCursor.sin_cursor;

            // le cambio el font
            gui.font.Dispose();
            // Fonts
            gui.font = new Microsoft.DirectX.Direct3D.Font(d3dDevice, 12, 0, FontWeight.Bold, 0, false, CharacterSet.Default,
                    Precision.Default, FontQuality.Default, PitchAndFamily.DefaultPitch, "Lucida Console");
            gui.font.PreloadGlyphs('0', '9');
            gui.font.PreloadGlyphs('a', 'z');
            gui.font.PreloadGlyphs('A', 'Z');

            gui.RTQ = gui.rectToQuad(0, 0, W, H, 0, 0, W - 150, 160, W - 200, H - 150, 0, H);
        }



        public void operar()
        {
            Device device = GuiController.Instance.D3dDevice;

            float mid_x = GuiController.Instance.D3dDevice.Viewport.Width / 2;
            float mid_y = GuiController.Instance.D3dDevice.Viewport.Height / 2;
                        
            //radar de proximidad
            float max_dist = 4000;
            float min_dist = 800;
            foreach (Dibujable item in EjemploAlumno.workspace().Escenario.CuerposGravitacionales)
            {
                if (item.soyAsteroide() && !EjemploAlumno.workspace().Shader.fueraFrustrum(item))
                {
                    Vector3 pos_personaje = EjemploAlumno.workspace().nave.getPosicion();
                    Vector3 pos_enemigo = item.getPosicion();
                    float dist = (pos_personaje - pos_enemigo).Length();
                    if (dist < max_dist && dist > min_dist)
                    {
                        pos_enemigo.Project(device.Viewport, device.Transform.Projection, device.Transform.View, device.Transform.World);
                        if (pos_enemigo.Z > 0 && pos_enemigo.Z < 1)
                        {
                            float an = (max_dist - dist) / max_dist * 3.1415f * 2.0f;
                            int d = (int)dist;
                                gui.DrawArc(new Vector2(pos_enemigo.X /*+ 20*/, pos_enemigo.Y), 40, 0, an, 10, dist < 2000 ? Color.Tomato : Color.WhiteSmoke);
                                gui.DrawLine(pos_enemigo.X, pos_enemigo.Y, pos_enemigo.X /*+ 20*/, pos_enemigo.Y, 3, Color.PowderBlue);
                                gui.DrawLine(pos_enemigo.X /*+ 20*/, pos_enemigo.Y, pos_enemigo.X /*+ 40*/, pos_enemigo.Y /*- 20*/, 3, Color.PowderBlue);
                                gui.TextOut((int)pos_enemigo.X /*+ 50*/, (int)pos_enemigo.Y /*- 20*/, "Proximidad " + d, Color.PowderBlue);
                        }
                    }
                }                
            }
        }
    }
}

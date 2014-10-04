using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using TgcViewer.Utils.TgcGeometry;
using Microsoft.DirectX;
using TgcViewer.Utils.TgcSceneLoader;
using TgcViewer;
using AlumnoEjemplos.TheGRID.Shaders;

namespace AlumnoEjemplos.TheGRID
{
    public abstract class ManagerDibujable
    {
        protected List<Dibujable> controlados;
        protected List<Dibujable> inactivos;
        protected int limiteControlados;

        public ManagerDibujable(int limite)
        {
            inactivos = new List<Dibujable>(limite);
            controlados = new List<Dibujable>(limite);
            limiteControlados = limite;
        }

        internal void addNew(Dibujable nuevo)
        {
            if (inactivos.Count == limiteControlados) controlados.RemoveAt(0);
            controlados.Add(nuevo);
        }

        public virtual void operar(float time)
        {
            foreach (var item in controlados)
            {
                trasladar(item, time);
                rotar(item, time);
                item.render(time);
            }
        }

        protected void trasladar(Dibujable objeto, float time)
        {
            List<Dibujable> lista = new List<Dibujable>(0);
            objeto.desplazarse(time, lista);
        }

        protected void rotar(Dibujable objeto, float time)
        {
            List<Dibujable> lista = new List<Dibujable>(0);
            objeto.rotar(time, lista);
        }

        public List<Dibujable> lista() { return controlados;}

        public void eliminarElemento(Dibujable aEliminar)
        {
            controlados.Remove(aEliminar);
            aEliminar.dispose();
        }

        public void destruirLista()
        {
            foreach (var item in controlados) item.dispose();
            foreach (var item in inactivos) item.dispose();
        }

        public void activar()
        {
            Dibujable objeto = inactivos[0];
            inactivos.RemoveAt(0);
            controlados.Add(objeto);
            objeto.activar();
        }

        public void desactivar(Dibujable objeto)
        {
            objeto.traslacion = 1;
            objeto.rotacion = 1;
            controlados.Remove(objeto);
            inactivos.Add(objeto);
            objeto.desactivar();
        }
    }

    public class ManagerLaser : ManagerDibujable
    {
        public ManagerLaser(int limite) : base(limite) { }
        
        public void fabricar(EjeCoordenadas ejes, Vector3 posicionNave)
        {
            addNew(Factory.crearLaser(ejes,posicionNave));
        }

        public void chocoAsteroide()
        {
            foreach (Dibujable laser in controlados)
                EjemploAlumno.workspace().Escenario.asteroidManager.chocoLaser(laser);
        }
    }

    public class ManagerAsteroide : ManagerDibujable
    {
        private List<Dibujable> buffer = new List<Dibujable>();
        
        public ManagerAsteroide(int limite) : base(limite) 
        {
            for(int i=0;i< limite;i++)
            {
                inactivos.Add(Factory.crearAsteroide(TamanioAsteroide.CHICO, new Vector3(0, 0, 0), this));
            }
        }

        public void explotaAlPrimero(){
            Dibujable colisionador = controlados[1];
            controlados.First().teChoque(colisionador,50);
        }

        public override void operar(float time)
        {
            TgcFrustum frustrum = EjemploAlumno.workspace().getCurrentFrustrum();
            foreach (var item in controlados)
            {
                trasladar(item, time);
                rotar(item, time);
                ((TgcBoundingSphere)item.getColision().getBoundingBox()).setCenter(item.getPosicion());
                //Chequea si esta dentro del frustrum
                TgcCollisionUtils.FrustumResult resultado = 
                    TgcCollisionUtils.classifyFrustumSphere(frustrum, (TgcBoundingSphere)item.getColision().getBoundingBox());
                if (resultado != TgcCollisionUtils.FrustumResult.OUTSIDE) item.render(time);
            }

            //reciclajeAsteroidesFueraDelSky();
        }

        private void reciclajeAsteroidesFueraDelSky()
        {
            SkySphere skysphere = EjemploAlumno.workspace().SkySphere;
            bool breakForzoso = true;
            while (breakForzoso && controlados.Count > 0)
            {
                breakForzoso = false;
                foreach (var asteroide in controlados)
                    if (!asteroide.getColision().colisiono(skysphere.bordeSky))
                    {
                        desactivar(asteroide);
                        breakForzoso = true;
                        break;
                    }
            }
        }

        public void desactivarTodos()
        {
            while (controlados.Count() > 0)
            {
                desactivar(controlados.First());
            }
        }

        public void activarAsteroide(Formato formato)
        {
            if(inactivos.Count > 0)
            {      
                Asteroide asteroide = (Asteroide)inactivos[0];
                inactivos.RemoveAt(0);

                //Darle el formato al asteroide
                formato.actualizarAsteroide(asteroide);
                controlados.Add(asteroide);
                asteroide.activar();                                    //ATENCION - No se realmente si habria que activarlo o no.
            }
        }

        public void fabricar(int cuantos, TamanioAsteroide tam)
        {
            for (int i = 0; i < cuantos; i++ ) addNew(Factory.crearAsteroide(tam, new Vector3(10*i, 20*i, 100),this));
        }

        public void fabricarMiniAsteroides(int cuantos, TamanioAsteroide tam, Vector3 pos)
        {
            Formato format = new Formato();
            format.tamanio = tam;
            format.posicion = pos;
            for(int i=0; i<cuantos;i++)  activarAsteroide(format); 
        }

        public void fabricarCinturonAsteroides(Vector3 pos_base, int raizCantidadAsteroides, int distanciaEntreAsteroides)
        {
            /*foreach (var asteroide in controlados)  //No se realmente si conviene desactivar los que ya estaban activos.
            {
                desactivar(asteroide);
            }*/
            int distancia = raizCantidadAsteroides * distanciaEntreAsteroides; //150 de separacion entre cada asteroides lo usual
            float pos_x;
            float pos_y = pos_base.Y;
            float pos_z = pos_base.Z - (distancia / 2);
            Formato formatoAsteroide = new Formato();

            for (int i = 0; i < raizCantidadAsteroides; i++)
            {
                pos_x = pos_base.X - distancia;
                for (int j = 0; j < raizCantidadAsteroides; j++)
                {
                    pos_x += distanciaEntreAsteroides * j;
                    //Setear Formato
                    formatoAsteroide.tamanio = TamanioAsteroide.MUYGRANDE;
                    formatoAsteroide.posicion = new Vector3(pos_x, pos_y, pos_z);

                    //Pasaje de asteroides con formato
                    activarAsteroide(formatoAsteroide);
                    //addNew(Factory.crearAsteroide(TamanioAsteroide.MUYGRANDE, new Vector3(pos_x, pos_y, pos_z),this));
                }
                pos_z += distanciaEntreAsteroides * i;
            }
        }

        public void chocoNave(Dibujable nave)
        {
            bool naveColision = false;
            foreach (Dibujable asteroide in controlados)
            {
                Color color = Color.Yellow;
                if (nave.getColision().colisiono(((TgcBoundingSphere)asteroide.getColision().getBoundingBox())))
                {
                    color = Color.Red;
                    ((TgcObb)nave.getColision().getBoundingBox()).setRenderColor(color);
                    naveColision = true;
                    nave.teChoque(asteroide, asteroide.velocidadActual());
                    asteroide.teChoque(nave, nave.velocidadActual());
                    break;
                }
                ((TgcBoundingSphere)asteroide.getColision().getBoundingBox()).setRenderColor(color);
            }
            //controlados.Concat(buffer);
            //buffer.Clear();
            if (!naveColision) ((TgcObb)nave.getColision().getBoundingBox()).setRenderColor(Color.Yellow);
        }

        public void chocoLaser(Dibujable laser)
        {
            //if (controlados.Count() < 1) return;      No hace falta
            foreach (Asteroide asteroide in controlados)
            {
                if (laser.getColision().colisiono(((TgcBoundingSphere)asteroide.getColision().getBoundingBox())))
                {
                    ((TgcObb)laser.getColision().getBoundingBox()).setRenderColor(Color.Blue);
                    ((TgcBoundingSphere)asteroide.getColision().getBoundingBox()).setRenderColor(Color.Blue);
                    asteroide.teChoque(laser,laser.velocidadActual());
                    break;
                }
            }
        }

        public void colisionEntreAsteroides(int i) 
        {
            int pos = i;
            int cant = controlados.Count();
            if (cant<2) return;
            if (i + 1 == cant/*controlados.Count()*/) //osea es el ultimo
            {
                //no hace nada
            }

            else
            {
                for (++i; i < cant; i++)
                {
                    if (controlados[pos].getColision().colisiono(((TgcBoundingSphere)controlados[i].getColision().getBoundingBox()))) 
                    {
                        //controlados[pos].teChoque(controlados[i]);
                        ((TgcBoundingSphere)controlados[pos].getColision().getBoundingBox()).setRenderColor(Color.DarkGreen);
                        ((TgcBoundingSphere)controlados[i].getColision().getBoundingBox()).setRenderColor(Color.DarkGreen);
                    }
                }
                colisionEntreAsteroides(++pos);
            }
        }
    }
    public class Formato
    {
        public TamanioAsteroide tamanio;
        public Vector3 posicion;

        public void actualizarAsteroide(Asteroide asteroide)
        {
            FormatoAsteroide formatoAUsar = Asteroide.elegirAsteroidePor(tamanio);

            asteroide.escalar(formatoAUsar.getVolumen());
            asteroide.setFisica(0, 0, formatoAUsar.getMasa());
            asteroide.velocidad = formatoAUsar.getVelocidad();
            asteroide.tamanioAnterior = formatoAUsar.tamanioAnterior();
            asteroide.Vida = formatoAUsar.vidaInicial();

            asteroide.traslacion = 1;
            asteroide.rotacion = 1;

            float radioMalla3DsMax = 7.633f;
            TgcBoundingSphere bounding = (TgcBoundingSphere) asteroide.getColision().getBoundingBox();
            bounding.setValues(bounding.Center, radioMalla3DsMax * formatoAUsar.getVolumen().X);
                
            asteroide.Transform *= Matrix.Translation(posicion);
            asteroide.Transform *= Matrix.Identity;

            asteroide.getColision().transladar(posicion);
            asteroide.setPosicion(posicion);
        }
    }
}

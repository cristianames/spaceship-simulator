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
using AlumnoEjemplos.TheGRID.Helpers;

namespace AlumnoEjemplos.TheGRID
{
    #region Manager Base
    public abstract class ManagerDibujable
    {
        protected List<Dibujable> controlados;
        protected List<Dibujable> inactivos;
        protected int limiteControlados;
        List<int> opciones = new List<int>() { -1, 1, 0 };

        public ManagerDibujable(int limite)
        {
            inactivos = new List<Dibujable>(limite);
            controlados = new List<Dibujable>(limite);
            limiteControlados = limite;
        }

        internal void addNew(Dibujable nuevo)
        {
            if (inactivos.Count == limiteControlados) controlados.RemoveAt(0);      //¿¿NO DEBERIA SER EL COUNT EN CONTROLADOS TMB??
            controlados.Add(nuevo);
        }

        public virtual void operar(float time)
        {
            foreach (var item in controlados)
            {
                trasladar(item, time);
                rotar(item, time);
            }
        }

        protected void trasladar(Dibujable objeto, float time)
        {
            List<Dibujable> lista = new List<Dibujable>(0);
            objeto.desplazarsePorTiempo(time, lista);
        }

        protected void rotar(Dibujable objeto, float time)
        {
            List<Dibujable> lista = new List<Dibujable>(0);
            objeto.rotarPorTiempo(time, lista);
        }
        public void eliminarInactivo(Dibujable aEliminar)
        {
            controlados.Remove(aEliminar);
            aEliminar.dispose();
        }

        public void destruirListas()
        {
            foreach (var item in controlados) item.dispose();
            foreach (var item in inactivos) item.dispose();
        }

        public Dibujable activar()
        {
            Dibujable objeto = inactivos[0];
            inactivos.RemoveAt(0);
            controlados.Add(objeto);
            objeto.activar();
            return objeto;
        }

        public virtual void desactivar(Dibujable objeto)
        {
            controlados.Remove(objeto);
            inactivos.Add(objeto);
            objeto.desactivar();
        }
    }
    #endregion

    #region Manager Laser
    public class ManagerLaser : ManagerDibujable
    {
        bool alternado;
        public ManagerLaser(int limite) : base(limite) 
        {
            for (int i = 0; i < limiteControlados; i++) inactivos.Add(Factory.crearLaserRojo());
        }        
        public void cargarDisparo(EjeCoordenadas ejes, Vector3 posicionNave)
        {
            Vector3 lateral = ejes.vectorX;
            Vector3 atras = ejes.vectorZ;
            lateral.Multiply(5);
            atras.Multiply(-20);
            if (alternado)      //Ubicamos de forma alternada los lasers.
            {
                posicionNave += lateral;
                alternado = false;
            }
            else
            {
                lateral.Multiply(-1);
                posicionNave += lateral;
                alternado = true;
            }
            posicionNave += atras;
            if (inactivos.Count == 0)
            {
                Dibujable dead = controlados[0];
                desactivar(dead);
            }
            Dibujable laser = activar();
            Factory.reubicarLaserAPosicion(laser, ejes, posicionNave);
        }
        public override void desactivar(Dibujable objeto)
        {
            controlados.Remove(objeto);            
            inactivos.Add(Factory.resetearLaser(objeto));
            objeto.desactivar();
        }
        public void chocoAsteroide()
        {
            foreach (Dibujable laser in controlados)
                EjemploAlumno.workspace().Escenario.asteroidManager.chocoLaser(laser);
        }
    }
    #endregion

    #region Manager Asteroide
    public class ManagerAsteroide : ManagerDibujable
    {

        public ManagerAsteroide(int limite) : base(limite) 
        {
            for(int i=0;i< limite;i++)
            {
                Asteroide asteroide = Factory.crearAsteroide(TamanioAsteroide.CHICO, new Vector3(0, 0, 0), this);
                asteroide.desactivar();
                inactivos.Add(asteroide);
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
                /*TgcCollisionUtils.FrustumResult resultado = 
                    TgcCollisionUtils.classifyFrustumSphere(frustrum, (TgcBoundingSphere)item.getColision().getBoundingBox());
                if (resultado != TgcCollisionUtils.FrustumResult.OUTSIDE) 
                    item.render(time);*/
                //No borrar, lo dejo por aca para despues usarlo en el Shader(09/10/14)
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
                Dibujable asteroide = inactivos[0];
                inactivos.RemoveAt(0);

                //Darle el formato al asteroide
                formato.actualizarAsteroide(asteroide);
                controlados.Add(asteroide);
                
                List<float> valores = new List<float>() { -4, -3, -2, -1, 1, 2, 3, 4 };
                Vector3 direccionImpulso = Factory.VectorRandom(-500, 500); // new Vector3(Factory.elementoRandom(valores), Factory.elementoRandom(valores), Factory.elementoRandom(valores));
                float velocidadImpulso = new Random().Next(1, 3);
                asteroide.fisica.impulsar(direccionImpulso, velocidadImpulso, 0.01f);                
                asteroide.activar();                                   
            }
        }

        public override void desactivar(Dibujable objeto)
        {
            controlados.Remove(objeto);
            inactivos.Add(Factory.resetearAsteroide((Asteroide) objeto));
            objeto.desactivar();
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
            foreach (var asteroide in controlados)  //No se realmente si conviene desactivar los que ya estaban activos.
            {
                desactivar(asteroide);
            }
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
                for (++i; i < controlados.Count(); i++)
                {
                    //float velocidad;
                    Dupla<Vector3> velocidades;
                    if (controlados[pos].getColision().colisiono(((TgcBoundingSphere)controlados[i].getColision().getBoundingBox()))) 
                    {
                        //controlados[pos].teChoque(controlados[i]);
                        ((TgcBoundingSphere)controlados[pos].getColision().getBoundingBox()).setRenderColor(Color.DarkGreen);
                        ((TgcBoundingSphere)controlados[i].getColision().getBoundingBox()).setRenderColor(Color.DarkGreen);
                        velocidades = Fisica.CalcularChoqueElastico(controlados[i], controlados[pos]);
                        //controlados[pos].teChoque(controlados[i], controlados[i].velocidad);
                        //controlados[i].teChoque(controlados[pos], velocidad);
                        controlados[pos].impulsate(velocidades.fst, controlados[pos].velocidad*12);
                        controlados[i].impulsate(velocidades.snd, controlados[i].velocidad*6);
                    }
                }
                colisionEntreAsteroides(++pos);
            }
        }

        internal List<Dibujable> Controlados()
        {
            return controlados;
        }
    }
    #endregion

    #region Formato
    public class Formato
    {
        public TamanioAsteroide tamanio;
        public Vector3 posicion;
        List<int> opciones = new List<int>() { -1, 1, 0 };

        public void actualizarAsteroide(Dibujable asteroide)
        {
            FormatoAsteroide formatoAUsar = Asteroide.elegirAsteroidePor(tamanio);

            asteroide.escalarSinBB(formatoAUsar.getVolumen());
            asteroide.setFisica(0, 0, 10, formatoAUsar.getMasa());
            asteroide.velocidad = formatoAUsar.getVelocidad();
            ((Asteroide)asteroide).tamanioAnterior = formatoAUsar.tamanioAnterior();
            ((Asteroide)asteroide).Vida = formatoAUsar.vidaInicial();

            asteroide.traslacion = 1;
            asteroide.rotacion = Factory.elementoRandom<int>(opciones);
            asteroide.giro = Factory.elementoRandom<int>(opciones);
            asteroide.inclinacion = Factory.elementoRandom<int>(opciones);

            float radioMalla3DsMax = 7.633f;
            TgcBoundingSphere bounding = (TgcBoundingSphere) asteroide.getColision().getBoundingBox();
            bounding.setValues(bounding.Center, radioMalla3DsMax * formatoAUsar.getVolumen().X);

            asteroide.ubicarEnUnaPosicion(posicion);
        }
    }
    #endregion
}

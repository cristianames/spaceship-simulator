using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using TgcViewer.Utils.TgcGeometry;
using Microsoft.DirectX;
using TgcViewer.Utils.TgcSceneLoader;
using TgcViewer;

namespace AlumnoEjemplos.TheGRID
{
    abstract class ManagerDibujable
    {
        protected List<Dibujable> controlados;
        protected int limiteControlados;

        public ManagerDibujable(int limite)
        {
            controlados = new List<Dibujable>(limite);
            limiteControlados = limite;
        }

        internal void addNew(Dibujable nuevo)
        {
            if (controlados.Count == limiteControlados) controlados.RemoveAt(0);
            controlados.Add(nuevo);
        }

        public virtual void operar(float time)
        {
            foreach (var item in controlados)
            {
                trasladar(item, time);
                rotar(item, time);
                item.render();
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

        public List<Dibujable> lista() 
        {
            return controlados;
        }

        public void destruirLista()
        {
            foreach (var item in controlados)
            {
                item.dispose();
            }
        }
    }

    class ManagerLaser : ManagerDibujable
    {
        public ManagerLaser(int limite) : base(limite) { }
        
        public void fabricar(EjeCoordenadas ejes, Vector3 posicionNave)
        {
            addNew(Factory.crearLaser(ejes,posicionNave));
        }

        public void chocoAsteroide(Dibujable asteroide)
        {
            foreach (Dibujable laser in controlados)
            {
                 if (laser.getColision().colisiono(((TgcBoundingSphere)asteroide.getColision().getBoundingBox())))
                    {
                        ((TgcObb)laser.getColision().getBoundingBox()).setRenderColor(Color.Blue);
                        ((TgcBoundingSphere)asteroide.getColision().getBoundingBox()).setRenderColor(Color.Blue);

                    }
            }
        }
    }

    class ManagerAsteroide : ManagerDibujable
    {
        public ManagerAsteroide(int limite) : base(limite) { }

        public void explotaAlPrimero(){
            Dibujable colisionador = controlados[1];
            controlados.First().teChoque(colisionador);
        }

        public void creaUno(TamanioAsteroide tam)
        {
            addNew(Factory.crearAsteroide(tam,new Vector3(10,40,50)));
        }

        public void fabricar(int cuantos, TamanioAsteroide tam)
        {
            for (int i = 0; i < cuantos; i++ ) addNew(Factory.crearAsteroide(tam, new Vector3(10*i, 20*i, 100)));
        }

        public override void operar(float time)
        {
            foreach (var item in controlados)
            {
                trasladar(item, time);
                rotar(item, time);
                ((TgcBoundingSphere)item.getColision().getBoundingBox()).setCenter(item.getPosicion());
                //Chequea si esta dentro del frustrum
                TgcFrustum frustrum = GuiController.Instance.Frustum;
                TgcViewer.Utils.TgcGeometry.TgcCollisionUtils.FrustumResult resultado = TgcCollisionUtils.classifyFrustumSphere(frustrum, (TgcBoundingSphere)item.getColision().getBoundingBox());
                //if (resultado != TgcViewer.Utils.TgcGeometry.TgcCollisionUtils.FrustumResult.OUTSIDE)
                    item.render();
            }
        }

        public void fabricarCinturonAsteroides(Vector3 pos_nave, int raizCantidadAsteroides, int distanciaEntreAsteroides)
        {
            int distancia = raizCantidadAsteroides * distanciaEntreAsteroides; //150 de separacion entre cada asteroides
            float pos_x;
            float pos_y = pos_nave.Y;
            float pos_z = pos_nave.Z - (distancia/2);

            for (int i = 0; i < raizCantidadAsteroides; i++)
            {
                pos_x = pos_nave.X - (distancia / 2);
                for (int j = 0; j < raizCantidadAsteroides; j++)
                {
                    pos_x += distanciaEntreAsteroides * j;
                    addNew(Factory.crearAsteroide(TamanioAsteroide.MUYGRANDE, new Vector3(pos_x, pos_y, pos_z)));
                }
                pos_z += distanciaEntreAsteroides * i;
            }
        }
        public void mostrarAsteroides(Vector3 pos_nave)
        {

        }

        public void chocoNave(Dibujable nave)
        {
            bool naveColision = false;
            foreach (Dibujable asteroide in controlados)
            {
                if (nave.getColision().colisiono(((TgcBoundingSphere)asteroide.getColision().getBoundingBox())))
                {
                    ((TgcObb)nave.getColision().getBoundingBox()).setRenderColor(Color.Red);
                    ((TgcBoundingSphere)asteroide.getColision().getBoundingBox()).setRenderColor(Color.Red);
                    naveColision = true;
                }
                else
                {
                    ((TgcBoundingSphere)asteroide.getColision().getBoundingBox()).setRenderColor(Color.Yellow);
                }            
            }
            if (!naveColision) ((TgcObb)nave.getColision().getBoundingBox()).setRenderColor(Color.Yellow);
        }

        public void chocoLasers(ManagerLaser lasers)
        {
            foreach (Dibujable asteroide in controlados)
            {
                lasers.chocoAsteroide(asteroide);
            }
        }

        //public void chocoAsteroide(Dibuajb)

        public void colisionEntreAsteroides(int i) 
        {            
            int pos = i;
            int cant = controlados.Count();
            if (i + 1 == controlados.Count()) //osea es el ultimo
            {
                //no hace nada
            }

            else
            {
                for (++i; i < cant; i++)
                {
                    if (controlados[pos].getColision().colisiono(((TgcBoundingSphere)controlados[i].getColision().getBoundingBox()))) 
                    {
                        ((TgcBoundingSphere)controlados[pos].getColision().getBoundingBox()).setRenderColor(Color.DarkGreen);
                        ((TgcBoundingSphere)controlados[i].getColision().getBoundingBox()).setRenderColor(Color.DarkGreen);
                    }
                }
                colisionEntreAsteroides(++pos);
            }
        }
    }
}

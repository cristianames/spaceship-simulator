using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TgcViewer.Utils.TgcGeometry;
using Microsoft.DirectX;
using TgcViewer.Utils.TgcSceneLoader;

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
                item.getColision().transladar(item.getPosicion() - ((TgcObb)item.getColision().getBoundingBox()).Position + new Vector3(0,0,30));    //por q Z +30 es necesario?             
                item.render();
                item.getColision().render();
                
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
    }

    class ManagerLaser : ManagerDibujable
    {
        public ManagerLaser(int limite) : base(limite) { }
        
        public void fabricar(Matrix transformacion, EjeCoordenadas ejes, Vector3 posicionNave,Vector3 rotacionNave)
        {
            addNew(Factory.crearLaser(transformacion, ejes,posicionNave,rotacionNave));
        }
    }

    class ManagerAsteroide : ManagerDibujable
    {
        public ManagerAsteroide(int limite) : base(limite) { }

        public void explotaAlPrimero(){
            Dibujable colisionador = controlados[1];
            controlados.First().teChoque(colisionador);
        }

        public void creaUno()
        {
            addNew(Factory.crearAsteroide(TamanioAsteroide.GRANDE,new Vector3(1,0,2)));
        }

        public void fabricar(int cuantos)
        {
            int i;
            for (i = 0; i < cuantos; i++ ) addNew(Factory.crearAsteroide(TamanioAsteroide.CHICO, new Vector3(10*i, 20*i, 100)));
        }

        public override void operar(float time)
        {
            foreach (var item in controlados)
            {
                trasladar(item, time);
                rotar(item, time);
                item.render();
                ((TgcBoundingSphere)item.getColision().getBoundingBox()).setCenter(item.getPosicion());
                item.renderBoundingBox();
            }
        }

        public void fabricar()
        {

        }
    }
}

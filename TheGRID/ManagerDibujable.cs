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
            foreach (var item in controlados) { item.dispose(); }
            foreach (var item in inactivos) { item.dispose(); }
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
                if(laser.objeto.Enabled) EjemploAlumno.workspace().Escenario.asteroidManager.chocoLaser(laser);
        }
    }
    #endregion

    #region Manager Asteroide
    public class ManagerAsteroide : ManagerDibujable
    {
        int cantidadEnMapa = 0;
        public ManagerAsteroide(int limite) : base(limite) 
        {
            Factory fabrica = new Factory();
            for(int i=0;i< limite;i++)
            {                                                                                                       
                 Asteroide asteroide = fabrica.crearAsteroide(TamanioAsteroide.CHICO, new Vector3(0, 0, 0), this);
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
            foreach (var item in controlados)
            {
                trasladar(item, time);
                rotar(item, time);
                //((TgcBoundingSphere)item.getColision().getBoundingBox()).setCenter(item.getPosicion());
            }

            reciclajeAsteroidesFueraDelSky();            
        }

        public void reinsertarSiEsNecesario(){
            if (controlados.Count < cantidadEnMapa) reinsertar(cantidadEnMapa - controlados.Count);
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
                        //reinsertar(asteroide);
                        desactivar(asteroide);    //Probaremos en vez de desactivar, reinsertar.
                        breakForzoso = true;
                        break;
                    }
            }
        }

        private void reinsertar(int cantidadAReinsertar)
        {
            List<TamanioAsteroide> opciones = new List<TamanioAsteroide>() { TamanioAsteroide.CHICO, TamanioAsteroide.MEDIANO, TamanioAsteroide.GRANDE, TamanioAsteroide.MUYGRANDE };
            Random random = new Random();
            Formato setearFormato = new Formato();
            Vector3 posicion = EjemploAlumno.workspace().ObjetoPrincipal.getPosicion();
            Vector3 ejeZ = EjemploAlumno.workspace().ObjetoPrincipal.getDireccion();
            Vector3 ejeX = EjemploAlumno.workspace().ObjetoPrincipal.getDireccion_X();
            Vector3 ejeY = EjemploAlumno.workspace().ObjetoPrincipal.getDireccion_Y();
            //desactivar(asteroide);
            for (int i = 0; i < cantidadAReinsertar; i++)
            {                
                setearFormato.tamanio = Factory.elementoRandom<TamanioAsteroide>(opciones);
                //Seteamos posicion de reinsercion
                setearFormato.posicion = Vector3.Add(Vector3.Add(Vector3.Add(posicion, Vector3.Multiply(ejeZ, 9500)), Vector3.Multiply(ejeX, random.Next(-3000, 3000))), Vector3.Multiply(ejeY, random.Next(-3000, 3000)));
                //Pasaje de asteroides con formato
                activarAsteroide(setearFormato);
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
                
                Vector3 direccionImpulso = Factory.VectorRandom(-200, 200);
                float velocidadImpulso = new Random().Next(150, 750);     //IMPORTANTE: Sin importar la velocidad cargada en el formato, aca le indicamos una nueva velocidad.
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

        public void fabricarMiniAsteroides(int cuantos, TamanioAsteroide tam, Vector3 pos, float radio)
        {
            Formato format = new Formato();
            format.tamanio = tam;
            Vector3 correccion;
            for (int i = 0; i < cuantos; i++)
            {
                correccion = Factory.VectorRandom(-500, 500);
                correccion.Normalize();
                correccion.Multiply(radio / 2);
                format.posicion = Vector3.Add(pos,correccion);
                activarAsteroide(format);
            }
        }

        public void fabricarMapaAsteroides(Vector3 pos_base, int cantidadAsteroides, int radioAdmisible)
        {
            foreach (var asteroide in controlados)  //No se realmente si conviene desactivar los que ya estaban activos. //Si, si conviene, no se quien sos, pero voy a encontrarte y voy a matarte. - Tomas(11/10)
            {                                       //Weon, me dijeron que vos no podes matar ni a una mosca!!  - Dante(?)(13/10) 
                desactivar(asteroide);
            }
            float max_x =  radioAdmisible + pos_base.X;
            float max_y =  radioAdmisible + pos_base.Y;
            float max_z =  radioAdmisible + pos_base.Z;
            float min_x = -radioAdmisible + pos_base.X;
            float min_y = -radioAdmisible + pos_base.Y;
            float min_z = -radioAdmisible + pos_base.Z;
            Formato setearFormato = new Formato();
            Random random = new Random();
            List<TamanioAsteroide> opciones = new List<TamanioAsteroide>(){TamanioAsteroide.CHICO, TamanioAsteroide.MEDIANO, TamanioAsteroide.GRANDE, TamanioAsteroide.MUYGRANDE};
            for (int i = 0; i < cantidadAsteroides; i++)
            {
                setearFormato.tamanio = Factory.elementoRandom<TamanioAsteroide>(opciones);
                setearFormato.posicion = new Vector3(random.Next((int)min_x, (int)max_x), random.Next((int)min_y, (int)max_y), random.Next((int)min_z, (int)max_z));
                //Pasaje de asteroides con formato
                activarAsteroide(setearFormato);
            }
            cantidadEnMapa = cantidadAsteroides;
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
                    //--------
                    int flagReintento = 0;
                    Dupla<Vector3> velocidades = Fisica.CalcularChoqueElastico(nave, asteroide);
                    Dupla <float> modulos = new Dupla<float>(asteroide.fisica.velocidadInstantanea * 0.9f, nave.fisica.velocidadInstantanea * 0.15f);
                    asteroide.impulsate(velocidades.fst, modulos.fst, 0.01f);
                    nave.impulsate(velocidades.snd, modulos.snd, 0.01f);
                    if (flagReintento == 0) EjemploAlumno.workspace().music.playAsteroideColision();
                    //--------
                    //nave.teChoque(asteroide, asteroide.velocidadActual());
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
                    EjemploAlumno.workspace().music.playAsteroideImpacto();
                    laser.desactivar();
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
                    Dupla<float> modulos;
                    if (controlados[pos].getColision().colisiono(((TgcBoundingSphere)controlados[i].getColision().getBoundingBox()))) 
                    {
                        int flagReintento = 0;
                        ((TgcBoundingSphere)controlados[pos].getColision().getBoundingBox()).setRenderColor(Color.DarkGreen);
                        ((TgcBoundingSphere)controlados[i].getColision().getBoundingBox()).setRenderColor(Color.DarkGreen);
                        velocidades = Fisica.CalcularChoqueElastico(controlados[i], controlados[pos]);
                        modulos = new Dupla<float>(controlados[pos].fisica.velocidadInstantanea * 0.9f, controlados[i].fisica.velocidadInstantanea * 0.9f);
                        controlados[pos].impulsate(velocidades.fst, modulos.fst, 0.01f);
                        controlados[i].impulsate(velocidades.snd, modulos.snd, 0.01f);
                        while (!distanciaSegura((Asteroide)controlados[i], (Asteroide)controlados[pos]))
                        {
                            controlados[pos].impulsate(Vector3.Multiply(controlados[pos].ultimaTraslacion, -1), modulos.fst, 0.5f);
                            controlados[i].impulsate(Vector3.Multiply(controlados[i].ultimaTraslacion, -1), modulos.snd, 0.5f);
                            if (flagReintento > 1000) { desactivar(controlados[i]); break; } //controlados[i].teChoque(controlados[pos], 10000); break; }   //Como reacomoda a lo pavote, puede vivir fallando.
                            flagReintento++;
                        }
                        if (flagReintento == 0) EjemploAlumno.workspace().music.playAsteroideColision();
                    }
                }
                if (pos < controlados.Count)colisionEntreAsteroides(++pos);
            }
        }

        private bool distanciaSegura(Asteroide colisionado, Asteroide colisionador)
        {
            float suma_radios = ((TgcBoundingSphere)colisionado.getColision().getBoundingBox()).Radius;
            suma_radios += ((TgcBoundingSphere)colisionador.getColision().getBoundingBox()).Radius;
            float distancia = Vector3.Subtract(colisionado.getPosicion(), colisionador.getPosicion()).Length();
            if (distancia > suma_radios) return true;
            else return false;
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
        List<int> opciones = new List<int>() { -1, 1 };

        public void actualizarAsteroide(Dibujable asteroide)
        {
            FormatoAsteroide formatoAUsar = Asteroide.elegirAsteroidePor(tamanio);

            asteroide.escalarSinBB(formatoAUsar.getVolumen());
            asteroide.setFisica(0, 0, 500, formatoAUsar.getMasa());     //IMPORTANTE: Aca se setea la velocidad maxima que puede alcanzar un asteroide.
            asteroide.velocidad = formatoAUsar.getVelocidad();
            ((Asteroide)asteroide).tamanioAnterior = formatoAUsar.tamanioAnterior();
            ((Asteroide)asteroide).Vida = formatoAUsar.vidaInicial();

            asteroide.traslacion = 1;
            asteroide.rotacion = Factory.elementoRandom<int>(opciones);
            asteroide.giro = Factory.elementoRandom<int>(opciones);
            asteroide.inclinacion = Factory.elementoRandom<int>(opciones);

            float radioMalla3DsMax = 10.633f;
            TgcBoundingSphere bounding = (TgcBoundingSphere) asteroide.getColision().getBoundingBox();
/*TAMAÑO*/  bounding.setValues(bounding.Center, radioMalla3DsMax * formatoAUsar.getVolumen().X);
            asteroide.ubicarEnUnaPosicion(posicion);
        }
    }
    #endregion
}

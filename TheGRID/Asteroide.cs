using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.DirectX;
using TgcViewer.Utils.TgcGeometry;
using AlumnoEjemplos.TheGRID.Explosiones;

namespace AlumnoEjemplos.TheGRID
{
    public class Asteroide : Dibujable
    {
        public Asteroide() : base() {}
        private float limite = 300f;    //QUE MIERDA ES LIMITE??
        public TamanioAsteroide tamanioAnterior;
        public ManagerAsteroide manager;
        private float vida;
        public float Vida { set { vida = value; } }

        public override bool soyAsteroide() { return true; }

        public override void teChoque(Dibujable colisionador, float moduloVelocidad)
        {
            //Verificacion de mierda por culpa del diseño de mieeeeeerda  --  Que onda gato!
            float masa = 0.01f;
            if (colisionador.fisica != null) masa = colisionador.fisica.Masa;
            
            daniate(masa, moduloVelocidad);
            if (vida <= 0) sinVida();
        }

        private void sinVida()
        {
            //float volumen = FastMath.PI * 2 * FastMath.Pow2(((TgcBoundingSphere)getColision().getBoundingBox()).Radius);
            if (limite < fisica.Masa) fraccionate();
            else
            {
                // Explosion.explosionAsteroide(this);
                //ManagerAsteroide manager = TheGrid.EjemploAlumno.workspace().AsteroidManager;
                manager.desactivar(this);
            }
        }

        private void daniate(float masa, float moduloVelocidad)
        {
            //Flaseada para bajar la vida
            vida -= (float) 5 * masa * moduloVelocidad;
        }

        private void fraccionate()
        {
            //ManagerAsteroide manager = TheGrid.EjemploAlumno.workspace().Escenario.asteroidManager;
            manager.desactivar(this);
            if (tamanioAnterior != TamanioAsteroide.NULO) 
                    manager.fabricarMiniAsteroides(3, tamanioAnterior, getPosicion(),((TgcBoundingSphere)colision.getBoundingBox()).Radius);
            EjemploAlumno.workspace().music.playAsteroideFragmentacion();
        }

        public static FormatoAsteroide elegirAsteroidePor(TamanioAsteroide tamanio)
        {
            switch (tamanio)
            {
                case TamanioAsteroide.MUYGRANDE: return new AsteroideMuyGrande();
                case TamanioAsteroide.GRANDE: return new AsteroideGrande();
                case TamanioAsteroide.MEDIANO: return new AsteroideMediano();
                case TamanioAsteroide.CHICO: return new AsteroideChico();
            }
            return null;
        }
    }

    public enum TamanioAsteroide { MUYGRANDE, GRANDE, MEDIANO, CHICO, NULO }

    public interface FormatoAsteroide
    {
        float getMasa(); //En toneladas
        Vector3 getVolumen(); //En realidad un factor de escalado
        float getVelocidad();
        TamanioAsteroide tamanioAnterior();
        float vidaInicial();
    }

    public class AsteroideMuyGrande : FormatoAsteroide
    {
        private float masa = 20000;
        private float longitud = 200;
        public float getMasa() { return masa; }
        public Vector3 getVolumen() { return new Vector3(longitud, longitud, longitud); }
        public float getVelocidad() { return 8; }
        public TamanioAsteroide tamanioAnterior() { return TamanioAsteroide.GRANDE; }
        public float vidaInicial() { return 10000; }
    }

    public class AsteroideGrande : FormatoAsteroide
    {
        private float masa = 8000;
        private float longitud = 100;
        public float getMasa() { return masa; }
        public Vector3 getVolumen() { return new Vector3(longitud, longitud, longitud); }
        public float getVelocidad() { return 13; }
        public TamanioAsteroide tamanioAnterior() { return TamanioAsteroide.MEDIANO; }
        public float vidaInicial() { return 5000; }
    }

    public class AsteroideMediano : FormatoAsteroide
    {
        private float masa = 5000;
        private float longitud = 50;
        public float getMasa() { return masa; }
        public Vector3 getVolumen() { return new Vector3(longitud, longitud, longitud); }
        public float getVelocidad() { return 18; }
        public TamanioAsteroide tamanioAnterior() { return TamanioAsteroide.CHICO; }
        public float vidaInicial() { return 2000; }
    }

    public class AsteroideChico : FormatoAsteroide
    {
        private float masa = 200;
        private float longitud = 25;
        public float getMasa() { return masa; }
        public Vector3 getVolumen() { return new Vector3(longitud, longitud, longitud); }
        public float getVelocidad() { return 24; }
        public float vidaInicial() { return 500; }
        public TamanioAsteroide tamanioAnterior() { return TamanioAsteroide.NULO; }
    }
}

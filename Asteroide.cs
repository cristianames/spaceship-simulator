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
        private float limite = 100f;
        private FormatoAsteroide formato;
        public FormatoAsteroide Formato { set { formato = value; } }

        public override void teChoque(Dibujable colisionador)
        { 
            float volumen = FastMath.PI * 2 * FastMath.Pow2(((TgcBoundingSphere)this.getColision().getBoundingBox()).Radius);
            if (limite < volumen) fraccionate(colisionador);
            else
            {
                // Explosion.explosionAsteroide(this);
            }
        }

        private void fraccionate(Dibujable colisionador)
        {
            ManagerAsteroide manager = TheGrid.EjemploAlumno.workspace().Escenario.asteroidManager;
            manager.fabricarMiniAsteroides(3, formato.tamanioAnterior(), getPosicion());
            manager.eliminarElemento(this);
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

    public enum TamanioAsteroide { MUYGRANDE, GRANDE, MEDIANO, CHICO }

    public interface FormatoAsteroide
    {
        float getMasa(); //En toneladas
        Vector3 getVolumen(); //En realidad un factor de escalado
        float getVelocidad();
        TamanioAsteroide tamanioAnterior();
    }

    public class AsteroideMuyGrande : FormatoAsteroide
    {
        private float masa = 1000;
        private float longitud = 8;
        public float getMasa() { return masa; }
        public Vector3 getVolumen() { return new Vector3(longitud, longitud, longitud); }
        public float getVelocidad() { return 20; }
        public TamanioAsteroide tamanioAnterior() { return TamanioAsteroide.GRANDE; }
    }

    public class AsteroideGrande : FormatoAsteroide
    {
        private float masa = 800;
        private float longitud = 6;
        public float getMasa() { return masa; }
        public Vector3 getVolumen() { return new Vector3(longitud, longitud, longitud); }
        public float getVelocidad() { return 25; }
        public TamanioAsteroide tamanioAnterior() { return TamanioAsteroide.MEDIANO; }
    }

    public class AsteroideMediano : FormatoAsteroide
    {
        private float masa = 500;
        private float longitud = 3;
        public float getMasa() { return masa; }
        public Vector3 getVolumen() { return new Vector3(longitud, longitud, longitud); }
        public float getVelocidad() { return 30; }
        public TamanioAsteroide tamanioAnterior() { return TamanioAsteroide.CHICO; }
    }

    public class AsteroideChico : FormatoAsteroide
    {
        private float masa = 200;
        private float longitud = 0.7f;
        public float getMasa() { return masa; }
        public Vector3 getVolumen() { return new Vector3(longitud, longitud, longitud); }
        public float getVelocidad() { return 40; }
        public TamanioAsteroide tamanioAnterior() { return TamanioAsteroide.CHICO; }
    }
}

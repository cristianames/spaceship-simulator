using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TgcViewer.Utils.TgcGeometry;
using AlumnoEjemplos.TheGRID.Explosiones;

namespace AlumnoEjemplos.TheGRID
{
    public class Asteroide : Dibujable
    {
        public Asteroide() : base() {}
        private float limite = 100f;

        public override void teChoque(Dibujable colisionador)
        { 
            float volumen = FastMath.PI * 2 * FastMath.Pow2(((TgcBoundingSphere)this.getColision().getBoundingBox()).Radius);
            if (limite < volumen)
            {
                fraccionate(colisionador);
            }
            else
            {
               // Explosion.explosionAsteroide(this);
            }
        }

        private void fraccionate(Dibujable colisionador)
        {
            //throw new NotImplementedException();
        }
    }
}

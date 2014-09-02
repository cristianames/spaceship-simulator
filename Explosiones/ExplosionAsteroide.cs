using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSceneLoader;

namespace AlumnoEjemplos.TheGRID.Explosiones
{
    class ExplosionAsteroide
    {
        private Dibujable asteroide;
        private float limite = 100f;

        void exiteChoqueEntre(Object colisionador, Object colisionado)
        {
            asteroide = (Dibujable)colisionado;
            //Por ahora calculo el volumen por aca
            float volumen = FastMath.PI * 2 * FastMath.Pow2(((TgcBoundingSphere)asteroide.getColision().getBoundingBox()).Radius);
            if (limite < volumen)
            {
                //Se divide el asteroide
            }
            else
            {
                //Explota el asteroide
            }
        }
    }
}

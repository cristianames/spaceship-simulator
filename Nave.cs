using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AlumnoEjemplos.TheGRID.Explosiones;

namespace AlumnoEjemplos.TheGRID
{
    public class Nave : Dibujable
    {
        public Nave() : base() {}
        private float vida;

        public override void teChoque(Dibujable colisionador)
        {
            //if (vida <= 0) Explosion.explosionNave(this);
            //else restaVida(colisionador);
        }

        private void restaVida(Dibujable colisionador)
        {
            //throw new NotImplementedException();
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlumnoEjemplos.TheGRID.Explosiones
{
    class ExplosionNave : Explosion
    {
        public ExplosionNave(Dibujable owner, float life, float shield)
        {
            duenio = owner;
            vida = life;
            escudo = shield;
        }
        public override void daniateEn(float danio)
        {
            if (escudo >= danio) escudo -= danio;
            else
            {
                danio -= escudo;
                escudo = 0;
                if (vida > danio) vida -= danio;
                else
                {
                    vida = 0;
                    duenio.morite();
                }
            }
        }
    }
}

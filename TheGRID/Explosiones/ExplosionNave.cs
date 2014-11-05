using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlumnoEjemplos.TheGRID.Explosiones
{
    //Modulo de explosion de la nave. El mismo no se usa en el estado actual del ejemplo
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
                    vida = 0; //Directamente se muere, para no tener negativos
                }
            }
        }
    }
}

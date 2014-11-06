using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AlumnoEjemplos.THE_GRID;

namespace AlumnoEjemplos.THE_GRID.Explosiones
{
    //Modulo de explosion de los asteroides que implementa la fragmentacion de los mismos.
    class ExplosionAsteroide : Explosion
    {
        List<int> fragmentos;

        ExplosionAsteroide(Dibujable owner, float life)
        {
            duenio = owner;
            vida = life;
            fragmentos = new List<int>();
            fragmentos.Add(1);
            fragmentos.Add(2);
            fragmentos.Add(3);
            fragmentos.Add(4);
        }
        public override void daniateEn(float danio)
        {
            if (vida > danio) vida -= danio;
            else
            {
                vida = 0;
                morite();
            }
        }

        private void morite()
        {
            int pedazos;
            pedazos = Factory.elementoRandom<int>(fragmentos);

        }
    }
}

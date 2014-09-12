using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlumnoEjemplos.TheGRID.Explosiones
{
    abstract class Explosion
    {
        internal float vida;
        abstract internal Dibujable duenio;

        abstract public void daniateEn(float danio);
    }
}

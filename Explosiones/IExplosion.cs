using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlumnoEjemplos.TheGRID.Explosiones
{
    interface IExplosion
    {
        void exiteChoqueEntre(Object colisionador, Object colisionado); //Se puede cambiar el nombre por uno mas expresivo.
    }
}

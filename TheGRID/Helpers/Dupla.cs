using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlumnoEjemplos.TheGRID.Helpers
{
    public class Dupla<T>
    {
        /*Clase definida para cuando es necesario retornar
         * dos objetos.
         * Utilizada mayormente en el sistema de deteccion de colisiones
         */
        private T item1;
        private T item2;
        
        public Dupla(T item1, T item2)
        {
            this.item1 = item1;
            this.item2 = item2;
        }

        public T fst { get { return item1; } set { item1 = value; } }
        public T snd { get { return item2; } set { item2 = value; } }
    }
}

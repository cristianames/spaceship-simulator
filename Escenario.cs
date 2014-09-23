using AlumnoEjemplos.TheGRID.Shaders;
using Microsoft.DirectX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TgcViewer.Utils.TgcGeometry;

namespace AlumnoEjemplos.TheGRID
{
    class Escenario
    {
        public ManagerLaser laserManager = new ManagerLaser(10);
        public ManagerAsteroide asteroidManager = new ManagerAsteroide(150); //Siempre debe ser mucho mayor que la cantidad de asteroides que queremos tener, pero no tanto sino colapsa
        public Dibujable principal;
        public TgcBoundingCylinder limite;
        private Boolean fuera_limite = false;
        enum TipoModo { THE_OPENIG, IMPULSE_DRIVE, WELCOME_HOME, VACUUM };
        private TipoModo escenarioActual = TipoModo.THE_OPENIG;

        

        public Escenario(Dibujable ppal) 
        {
            principal = ppal;
        }
        //-------------------------------------------------------------------------------------------CHAPTER-1
        public void loadChapter1()
        {
            disposeOld();
            asteroidManager = new ManagerAsteroide(150);
            asteroidManager.fabricarCinturonAsteroides(principal.getCentro(), 10, 100);
            limite = new TgcBoundingCylinder(principal.getCentro(), 10000, 100000);
        }
        //-------------------------------------------------------------------------------------------CHAPTER-2
        public void loadChapter2() 
        {
            disposeOld();
            asteroidManager.fabricarCinturonAsteroides(principal.getCentro(), 10, 100);
            limite = new TgcBoundingCylinder(principal.getCentro(), 10000, 100000);
            principal.setShader(new MotionBlur());

        }
        //-------------------------------------------------------------------------------------------CHAPTER-3
        public void loadChapter3() 
        {
            disposeOld();
        }
        //-------------------------------------------------------------------------------------------VACUUM
        public void loadVacuum() 
        {
            disposeOld();
            laserManager = new ManagerLaser(1);
            asteroidManager = new ManagerAsteroide(1);
        }

        private void disposeOld()
        {
            //Aca deberian borrarse todas las cosas de ser necesarias.
        }
        //-------------------------------------------------------------------------------------------
        internal void dispararLaser()
        {
            laserManager.fabricar(principal.getEjes(), principal.getPosicion());
        }
        internal void refrescar(float elapsedTime)
        {            
            laserManager.operar(elapsedTime);
            asteroidManager.operar(elapsedTime);

            //Chequeo de colision
            //Chequeo si la nave choco con algun asteroide

            //Eze: Codigo repetido, esto ya se realiza en asteroidManager.chocoNave(nave);

            /*bool naveColision = false;
            foreach (Dibujable asteroide in asteroidManager.lista())
            {

                if (nave.getColision().colisiono(((TgcBoundingSphere)asteroide.getColision().getBoundingBox())))
                {
                    ((TgcObb)nave.getColision().getBoundingBox()).setRenderColor(Color.Red);
                    ((TgcBoundingSphere)asteroide.getColision().getBoundingBox()).setRenderColor(Color.Red);
                    naveColision = true;
                }
                else
                {                  
                    ((TgcBoundingSphere)asteroide.getColision().getBoundingBox()).setRenderColor(Color.Yellow);
                }            
            }
            if (!naveColision) ((TgcObb)nave.getColision().getBoundingBox()).setRenderColor(Color.Yellow);
            */

            //no chequeo si los asteroides chocaron con la nave

            //Chequeo si la nave choco con algun asteroide
            asteroidManager.chocoNave(principal);
            //Chequeo si los lasers chocaron con algun asteroide

            //Invierto el flujo de este mensaje por cuestiones tecnicas
            //asteroidManager.chocoLasers(laserManager);
            laserManager.chocoAsteroide(); //no hace falta pasar el manager de asteroides, lo saca del singleton

            //no chequeo si algun laser choco con algun otro

            //Chequeo colision entre asteroides 
            asteroidManager.colisionEntreAsteroides(0); //hay que pasarle el 0 como parametro para que empieze a preguntar desde el asteoride 0, es una funcion recursiva
            
            if(TgcCollisionUtils.testPointCylinder(principal.getCentro(),limite)){
                fuera_limite = true;
                //Aca activaria el bombardeo de asteroides que te destrozarian la carroceria
            }
        }

        internal void chequearCambio(string opcion)
        {
            switch (opcion)
            {
                case "THE OPENIG":
                    if (escenarioActual != TipoModo.THE_OPENIG)
                        loadChapter1();
                    break;
                case "IMPULSE DRIVE":
                    if (escenarioActual != TipoModo.IMPULSE_DRIVE)
                        loadChapter2();
                    break;
                case "WELCOME HOME":
                    if (escenarioActual != TipoModo.WELCOME_HOME)
                        loadChapter3();
                    break;
                case "VACUUM":
                    if (escenarioActual != TipoModo.VACUUM)
                        loadVacuum();
                    break;
            }
        }
    }
}

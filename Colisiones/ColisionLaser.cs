﻿using Microsoft.DirectX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSceneLoader;

namespace AlumnoEjemplos.TheGRID.Colisiones
{
    class ColisionLaser: IColision
    {
        private TgcObb Obb;
        

        public IRenderObject getBoundingBox()
        {
            return this.Obb;
        }

        public void setBoundingBox(IRenderObject bb)
        {
            this.Obb = (TgcObb)bb;
        }

        public void transladar(Vector3 posicion)
        {
            this.Obb.move(posicion);
        }

        public void render()
        {
            Obb.render();
        }

        public void rotar() { ; }
        public void escalar(Vector3 tamanio)
        {
            ;
        }
    }
}
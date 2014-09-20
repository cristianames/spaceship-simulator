using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TgcViewer.Utils.TgcSceneLoader;

namespace AlumnoEjemplos.TheGRID.Shaders
{
    public interface ShaderInterface
    {
        void setShader(Dibujable dibujable);
        void shadear(TgcMesh mesh, float tiempo);
        void close();
    }
}

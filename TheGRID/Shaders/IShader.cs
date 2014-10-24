using Microsoft.DirectX.Direct3D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TgcViewer.Utils.TgcSceneLoader;

namespace AlumnoEjemplos.TheGRID.Shaders
{
    interface IShader
    {
        /* Los renderDefault y renderEffect devuelven la textura sobre DONDE renderizaron.
         * Haciendolo de esta manera, el renderDefault esta llamando a la textura de la capa inferior, sin conocer su
         * comportamiento; por lo que es posible anidar los distintos efectos. Es necesario definir un quienSoy();
         * para poder distinguir entre llamadas de los shaders y poder pasarle el anterior si el renderDefault, llegase
         * a llamar a la capa inferior. El modelo actual soporta: MotionBlur -> HDRL -> Dynamic Lights -> Bump Mapping
         * Siendo el renderDefault del BumpMapping, el render sin efectos, completamente al natural
         */
        Texture renderDefault(EstructuraRender parametros);
        Texture renderEffect(EstructuraRender parametros);

        void renderScene(Nave nave, String technique); //Para renderizar la nave
        void renderScene(Dibujable sol, String technique); //Para renderizar el sol
        void renderScene(List<Dibujable> dibujables, String technique); //Para renderizar asteroides y lasers
        void renderScene(List<IRenderObject> elementosRenderizables); //Para renderizar los objetos NO meshes

        SuperRender.tipo tipoShader();
        void close(); //Liberamos los Recursos
    }
}

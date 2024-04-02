using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/*
 * all known Model/Material information is the direct product of MattNL's work! check em out! 
 * https://github.com/MattN-L
*/
namespace OMI.Formats.Model
{
    public class ModelContainer
    {
        public int Version;

        public Dictionary<string, Model> Models = new Dictionary<string, Model>();

        /// <exception cref="ModelNotFoundException"></exception>
        Model GetModelByName(string name)
        {
            if (!Models.ContainsKey(name))
                throw new ModelNotFoundException(nameof(name));
            return Models[name];
        }
    }

    public class Model
    {
        public string Name;
        public Size TextureSize;
        public Dictionary<string, ModelPart> Parts = new Dictionary<string, ModelPart>();
    }

    public class ModelPart
    {
        public string Name;
        public string ParentName;
        public float UnknownFloat;
        public float TranslationX;
        public float TranslationY;
        public float TranslationZ;
        public float TextureOffsetX;
        public float TextureOffsetY;
        public float RotationX;
        public float RotationY;
        public float RotationZ;
        public List<ModelBox> Boxes = new List<ModelBox>();
    }

    public class ModelBox
    {
        public float PositionX;
        public float PositionY;
        public float PositionZ;
        public int Length;
        public int Width;
        public int Height;
        public float UvX;
        public float UvY;
        public float Scale;
        public bool Mirror;
    }
}

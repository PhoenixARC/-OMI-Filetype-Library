using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
/*
 * all known Model/Material information is the direct product of MattNL's work! check em out! 
 * https://github.com/MattN-L
*/
namespace OMI.Formats.Model
{
    public class ModelContainer : ICollection<Model>
    {
        public int Version;

        private Dictionary<string, Model> Models { get; } = new Dictionary<string, Model>();

        public int ModelCount => Models.Count;

        public int Count => Models.Count;

        public bool IsReadOnly => false;

        /// <exception cref="ModelNotFoundException"></exception>
        public Model GetModelByName(string name)
        {
            if (!Models.ContainsKey(name))
                throw new ModelNotFoundException(nameof(name));
            return Models[name];
        }

        public IEnumerable<string> GetModelNames() => Models.Keys;
        
        public IEnumerable<Model> GetModels() => Models.Values;

        public void Add(Model item) => Models.Add(item.Name, item);

        public void Clear() => Models.Clear();

        public bool ContainsModel(string name) => Models.ContainsKey(name);

        public bool Contains(Model item) => ContainsModel(item.Name);

        public void CopyTo(Model[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool Remove(Model item) => Models.Remove(item.Name);

        public IEnumerator<Model> GetEnumerator() => Models.Values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public class Model
    {
        public string Name { get; }
        public Size TextureSize { get; }
        public Dictionary<string, ModelPart> Parts { get; } = new Dictionary<string, ModelPart>();

        public Model(string name, Size textureSize)
        {
            Name = name;
            TextureSize = textureSize;
        }
    }

    public class ModelPart
    {
        public string Name;
        public string ParentName;
        public Vector3 Translation;
        public Vector3 Rotation;
        public Vector3 AdditionalRotation;
        public List<ModelBox> Boxes { get; } = new List<ModelBox>();

        public void AddBox(ModelBox modelBox) => Boxes.Add(modelBox);
    }

    public class ModelBox
    {
        public Vector3 Position;
        public Vector3 Size;
        public Vector2 Uv;
        public float Scale;
        public bool Mirror;
    }
}

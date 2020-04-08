
namespace Data.DI
{
    public class Diagram3D : Shape3D
    {
        public string documentation;
        public string name;
        public float resolution;

        public override string XmiType()
        {
            return "di:Diagram3D";
        }

    }
}

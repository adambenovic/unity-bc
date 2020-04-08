
namespace Data.DI
{
    public class Diagram2D : Shape2D
    {
        public string documentation;
        public string name;
        public float resolution;

        public override string XmiType()
        {
            return "di:Diagram2D";
        }

    }
}
